#region "Header"

// File: CollectionExtensions.cs
// Created 2026/08/22
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

#region "Usings"

using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;
using Istarnova.Extensions;

#endregion

namespace Istarnova.Collections.Generics;

public static class CollectionExtensions
{
    #region "Methods"

    extension<T>(IEnumerable<T> source)
    {
        #region "Methods"

        public string JoinBy(string separator) => string.Join(separator, source);

        public string JoinBy(StringJoinBy jb) => source.JoinBy(jb.GetSeperatorCharacter());

        public string JoinByNewLine(string? newLine = null)
        {
            // if not specified we use the os's default new line
            // in most of the cases it'll be \n since a container will run under linux
            newLine ??= Environment.NewLine;

            return string.Join(newLine, source);
        }

        /// <summary>
        ///     Keeps the items where at least one of the selected strings contains the search term, ignoring case.
        /// </summary>
        /// <param name="searchTerm">The text to look for; blank returns every item.</param>
        /// <param name="propertySelectors">The strings to look in. Each is compiled once, not once per item.</param>
        public IEnumerable<T> SearchInProperties(string searchTerm,
            params ReadOnlySpan<Expression<Func<T, string?>>> propertySelectors)
        {
            ArgumentNullException.ThrowIfNull(source);

            if (string.IsNullOrWhiteSpace(searchTerm) || propertySelectors.Length == 0)
            {
                return source;
            }

            var compiledSelectors = new Func<T, string?>[propertySelectors.Length];
            for (var index = 0; index < compiledSelectors.Length; index++)
            {
                compiledSelectors[index] = propertySelectors[index].Compile();
            }

            return source.Where(item => ContainsSearchTerm(compiledSelectors, item, searchTerm));
        }

        /// <summary>
        ///     Keeps the items where at least one of the named string properties contains the search term, ignoring case.
        /// </summary>
        /// <param name="searchTerm">The text to look for; blank returns every item.</param>
        /// <param name="propertyNames">The public instance properties to look in; those that are not strings are skipped.</param>
        public IEnumerable<T> SearchInProperties(string searchTerm, params ReadOnlySpan<string> propertyNames)
        {
            ArgumentNullException.ThrowIfNull(source);

            if (string.IsNullOrWhiteSpace(searchTerm) || propertyNames.Length == 0)
            {
                return source;
            }

            var properties = new List<PropertyInfo>(propertyNames.Length);
            foreach (var name in propertyNames)
            {
                var property = typeof(T).GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
                if (property?.PropertyType == typeof(string))
                {
                    properties.Add(property);
                }
            }

            return properties.Count == 0
                ? source
                : source.Where(item => ContainsSearchTerm(properties, item, searchTerm));
        }

        #endregion
    }

    extension<TNumber>(List<TNumber> list) where TNumber : INumber<TNumber>
    {
        #region "Methods"

        public void SortAscending() => list.Sort((a, b) => a.CompareTo(b));
        public void SortDescending() => list.Sort((a, b) => b.CompareTo(a));

        #endregion
    }

    public static bool HasItems<T>(this ICollection<T>? collection) => collection is { Count: > 0 };

    public static bool HasItems<T>(this IEnumerable<T>? enumerable) => enumerable != null && enumerable.Any();

    private static bool ContainsSearchTerm<T>(Func<T, string?>[] selectors, T item, string searchTerm)
    {
        foreach (var selector in selectors)
        {
            if (selector(item).ContainsOrdinalIgnoreCase(searchTerm))
            {
                return true;
            }
        }

        return false;
    }

    private static bool ContainsSearchTerm<T>(List<PropertyInfo> properties, T item, string searchTerm)
    {
        foreach (var property in properties)
        {
            if ((property.GetValue(item) as string).ContainsOrdinalIgnoreCase(searchTerm))
            {
                return true;
            }
        }

        return false;
    }

    #endregion
}
