#region "Header"

// File: LinqExtensions.cs
// Created 2026/03/06
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

#region "Usings"

using System.Linq.Expressions;

#endregion

namespace Istarnova.Generics.Linq;

public static class LinqExtensions
{
    #region "Methods"

    /// <param name="queryable">An IQueryable&lt;T&gt; to filter.</param>
    /// <typeparam name="T">The type of the elements of source.</typeparam>
    extension<T>(IQueryable<T> queryable)
    {
        #region "Methods"

        /// <summary>
        ///     Filters a sequence of values based on any of the specified predicates using OR logic.
        /// </summary>
        /// <param name="predicates">The predicates to combine with OR logic.</param>
        /// <returns>An IQueryable&lt;T&gt; that contains elements from the input sequence that satisfy any of the predicates.</returns>
        /// <exception cref="ArgumentNullException">Thrown when queryable or predicates is null.</exception>
        /// <exception cref="ArgumentException">Thrown when no predicates are provided.</exception>
        public IQueryable<T> WhereAny(params ReadOnlySpan<Expression<Func<T, bool>>> predicates)
        {
            ArgumentNullException.ThrowIfNull(queryable);

            if (predicates.IsEmpty)
            {
                throw new ArgumentException("At least one predicate must be provided.", nameof(predicates));
            }

            if (predicates.Length == 1)
            {
                return queryable.Where(predicates[0]);
            }

            var parameter = Expression.Parameter(typeof(T));
            Expression? combinedExpression = null;

            foreach (var predicate in predicates)
            {
                ArgumentNullException.ThrowIfNull(predicate);

                var visitor = new ParameterSubstitutionVisitor(predicate.Parameters[0], parameter);
                var visitedBody = visitor.Visit(predicate.Body)
                                  ?? throw new InvalidOperationException(
                                      $"Failed to process predicate body for parameter '{predicate.Parameters[0].Name}'.");

                combinedExpression = combinedExpression is null
                    ? visitedBody
                    : Expression.OrElse(combinedExpression, visitedBody);
            }

            return queryable.Where(Expression.Lambda<Func<T, bool>>(combinedExpression!, parameter));
        }

        /// <summary>
        ///     Filters a sequence of values based on any of the specified predicates using OR logic.
        /// </summary>
        /// <param name="predicates">The predicates to combine with OR logic.</param>
        /// <returns>An IQueryable&lt;T&gt; that contains elements from the input sequence that satisfy any of the predicates.</returns>
        public IQueryable<T> WhereAny(params Expression<Func<T, bool>>[] predicates)
            =>
                queryable.WhereAny(predicates.AsSpan());

        #endregion
    }

    extension<TSource>(IEnumerable<TSource> source)
    {
        #region "Methods"

        public IEnumerable<IList<TSource>> Window(int size)
        {
            ArgumentNullException.ThrowIfNull(source);

            return size <= 0 ? throw new ArgumentOutOfRangeException(nameof(size)) : _();

            IEnumerable<IList<TSource>> _()
            {
                using var iter = source.GetEnumerator();

                // generate the first window of items
                var window = new TSource[size];
                int i;
                for (i = 0; i < size && iter.MoveNext(); i++)
                {
                    window[i] = iter.Current;
                }

                if (i < size)
                {
                    yield break;
                }

                while (iter.MoveNext())
                {
                    // generate the next window by shifting forward by one item
                    // and do that before exposing the data
                    var newWindow = new TSource[size];
                    Array.Copy(window, 1, newWindow, 0, size - 1);
                    newWindow[size - 1] = iter.Current;

                    yield return window;
                    window = newWindow;
                }

                // return the last window.
                yield return window;
            }
        }

        #endregion
    }

    #endregion
}
