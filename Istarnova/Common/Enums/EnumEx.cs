#region "Header"

// File: EnumEx.cs
// Created 2026/08/22
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

#region "Usings"

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;

#endregion

namespace Istarnova.Common.Enums;

/// <summary>
///     Modern enum extension utilities with flexible resource management.
///     Provides type-safe enum operations with caching and custom display name resolution.
/// </summary>
/// <typeparam name="T">The enum type (must be a struct enum)</typeparam>
public static class EnumEx<T> where T : struct, Enum
{
    #region "Constants"

    private static readonly Lazy<string[]> LazyNames = new(Enum.GetNames<T>);
    private static readonly Lazy<T[]> LazyValues = new(Enum.GetValues<T>);
    private static readonly Lazy<EnumKeyValue<T>[]> LazyDisplayList = new(BuildDisplayList);
    private static readonly bool IsFlagsEnum = typeof(T).GetCustomAttribute<FlagsAttribute>() != null;


    private static readonly TypeCode UnderlyingType = Type.GetTypeCode(Enum.GetUnderlyingType(typeof(T)));

    #endregion

    #region "Properties"

    /// <summary>
    ///     Gets the number of enum values.
    /// </summary>
    public static int Count => LazyValues.Value.Length;

    /// <summary>
    ///     Gets enum key-value pairs with display names resolved from various sources.
    ///     Uses caching for optimal performance.
    /// </summary>
    public static ReadOnlySpan<EnumKeyValue<T>> DisplayList => LazyDisplayList.Value;

    /// <summary>
    ///     Checks if the enum has the [Flags] attribute.
    /// </summary>
    public static bool IsFlags => IsFlagsEnum;

    /// <summary>
    ///     Gets all enum names as a readonly span.
    /// </summary>
    public static ReadOnlySpan<string> Names => LazyNames.Value;

    /// <summary>
    ///     Gets all enum values as a readonly span for maximum performance.
    /// </summary>
    public static ReadOnlySpan<T> Values => LazyValues.Value;

    #endregion

    #region "Methods"

    /// <summary>
    ///     Combines multiple enum values into a single flags value.
    ///     Only works with [Flags] enums.
    /// </summary>
    /// <param name="values">Values to combine</param>
    /// <returns>Combined flags value</returns>
    public static T CombineFlags(params ReadOnlySpan<T> values)
    {
        if (!IsFlags)
        {
            throw new InvalidOperationException($"Enum {typeof(T).Name} is not decorated with [Flags] attribute.");
        }

        long combined = 0;
        foreach (var value in values)
        {
            combined |= ToInt64(value);
        }

        return Unsafe.As<long, T>(ref combined);
    }

    /// <summary>
    ///     Creates a lookup dictionary from enum values to display names.
    /// </summary>
    /// <param name="resourceManager">Optional resource manager</param>
    /// <returns>Dictionary for fast lookup</returns>
    public static Dictionary<T, string> CreateDisplayLookup(ResourceManager? resourceManager = null)
    {
        return GetDisplayList(resourceManager).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

    /// <summary>
    ///     Creates a reverse lookup dictionary from display names to enum values.
    /// </summary>
    /// <param name="resourceManager">Optional resource manager</param>
    /// <param name="ignoreCase">Whether to use case-insensitive lookup</param>
    /// <returns>Dictionary for reverse lookup</returns>
    public static Dictionary<string, T> CreateReverseLookup(ResourceManager? resourceManager = null,
        bool ignoreCase = true)
    {
        var comparer = ignoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;
        return GetDisplayList(resourceManager).ToDictionary(kvp => kvp.Value, kvp => kvp.Key, comparer);
    }

    /// <summary>
    ///     Performs fuzzy search on display names.
    /// </summary>
    /// <param name="searchTerm">Term to search for</param>
    /// <param name="resourceManager">Optional resource manager</param>
    /// <param name="minScore">Minimum similarity score (0-1)</param>
    /// <returns>Matching enum values with similarity scores</returns>
    public static (T Value, string DisplayName, double Score)[] FuzzySearch(
        string searchTerm,
        ResourceManager? resourceManager = null,
        double minScore = 0.3)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return [];
        }

        return
        [
            .. GetDisplayList(resourceManager)
                .Select(kvp => new
                {
                    kvp.Key,
                    kvp.Value,
                    Score = CalculateLevenshteinSimilarity(searchTerm.ToLowerInvariant(), kvp.Value.ToLowerInvariant()),
                })
                .Where(x => x.Score >= minScore)
                .OrderByDescending(x => x.Score)
                .Select(x => (x.Key, x.Value, x.Score)),
        ];
    }

    /// <summary>
    ///     Gets enum key-value pairs with custom description resolver.
    /// </summary>
    /// <param name="descriptionResolver">Custom function to resolve descriptions</param>
    /// <returns>Array of enum key-value pairs with descriptions</returns>
    public static EnumKeyValue<T>[] GetDescriptionList(Func<T, string?> descriptionResolver)
    {
        ArgumentNullException.ThrowIfNull(descriptionResolver);

        var values = LazyValues.Value;
        var result = new EnumKeyValue<T>[values.Length];
        for (var i = 0; i < values.Length; i++)
        {
            result[i] = new(values[i], descriptionResolver(values[i]) ?? values[i].ToString());
        }

        return result;
    }

    /// <summary>
    ///     Gets enum key-value pairs with resource manager for description localization.
    /// </summary>
    /// <param name="resourceManager">Resource manager for localized descriptions</param>
    /// <param name="fallbackToEnumName">If true, falls back to enum name when resource not found</param>
    /// <returns>Array of enum key-value pairs with descriptions</returns>
    public static EnumKeyValue<T>[] GetDescriptionList(ResourceManager? resourceManager, bool fallbackToEnumName = true)
    {
        return GetDescriptionList(value =>
            ResolveByAttribute<T, DescriptionAttribute>(value, resourceManager, fallbackToEnumName, "Description"));
    }

    /// <summary>
    ///     Gets enum key-value pairs with custom display name resolver.
    /// </summary>
    /// <param name="displayNameResolver">Custom function to resolve display names</param>
    /// <returns>Array of enum key-value pairs</returns>
    public static EnumKeyValue<T>[] GetDisplayList(Func<T, string?> displayNameResolver)
    {
        ArgumentNullException.ThrowIfNull(displayNameResolver);

        var values = LazyValues.Value;
        var result = new EnumKeyValue<T>[values.Length];
        for (var i = 0; i < values.Length; i++)
        {
            result[i] = new(values[i], displayNameResolver(values[i]) ?? values[i].ToString());
        }

        return result;
    }

    /// <summary>
    ///     Gets enum key-value pairs with resource manager for localization.
    /// </summary>
    /// <param name="resourceManager">Resource manager for localized strings</param>
    /// <param name="fallbackToEnumName">If true, falls back to enum name when resource not found</param>
    /// <returns>Array of enum key-value pairs</returns>
    public static EnumKeyValue<T>[] GetDisplayList(ResourceManager? resourceManager, bool fallbackToEnumName = true)
    {
        return GetDisplayList(value => ResolveDisplayName(value, resourceManager, fallbackToEnumName));
    }

    /// <summary>
    ///     Gets the display name for an enum value using various resolution strategies.
    /// </summary>
    /// <param name="value">The enum value</param>
    /// <param name="resourceManager">Optional resource manager for localization</param>
    /// <param name="fallbackToEnumName">Whether to fall back to enum name if no display name found</param>
    /// <returns>Display name or enum name</returns>
    public static string GetDisplayName(T value, ResourceManager? resourceManager = null,
        bool fallbackToEnumName = true) =>
        ResolveDisplayName(value, resourceManager, fallbackToEnumName) ?? value.ToString();

    /// <summary>
    ///     Gets individual flag values from a combined flags enum value.
    ///     Only works with [Flags] enums.
    /// </summary>
    /// <param name="combinedValue">The combined flags value</param>
    /// <returns>Individual flag values that make up the combined value</returns>
    public static T[] GetFlags(T combinedValue)
    {
        if (!IsFlags)
        {
            throw new InvalidOperationException($"Enum {typeof(T).Name} is not decorated with [Flags] attribute.");
        }

        var result = new List<T>();
        var combinedInt = ToInt64(combinedValue);

        foreach (var value in Values)
        {
            var valueInt = ToInt64(value);
            if (valueInt != 0 && (combinedInt & valueInt) == valueInt)
            {
                result.Add(value);
            }
        }

        return [.. result];
    }

    /// <summary>
    ///     Gets the next enum value in sequence (circular).
    /// </summary>
    /// <param name="current">Current enum value</param>
    /// <returns>Next enum value, or first if current is last</returns>
    public static T GetNext(T current)
    {
        var values = LazyValues.Value;
        var currentIndex = Array.IndexOf(values, current);
        return currentIndex == -1 ? default : values[(currentIndex + 1) % values.Length];
    }

    /// <summary>
    ///     Gets the previous enum value in sequence (circular).
    /// </summary>
    /// <param name="current">Current enum value</param>
    /// <returns>Previous enum value, or last if current is first</returns>
    public static T GetPrevious(T current)
    {
        var values = LazyValues.Value;
        var currentIndex = Array.IndexOf(values, current);

        return currentIndex == -1
            ? default
            : values[currentIndex == 0 ? values.Length - 1 : currentIndex - 1];
    }

    /// <summary>
    ///     Gets a random enum value.
    /// </summary>
    /// <param name="random">Optional random instance (uses Random.Shared if null)</param>
    /// <returns>Random enum value</returns>
    public static T GetRandom(Random? random = null)
    {
        var rng = random ?? Random.Shared;
        var values = LazyValues.Value;
        return values[rng.Next(values.Length)];
    }

    /// <summary>
    ///     Gets enum values that have a specific attribute.
    /// </summary>
    /// <typeparam name="TAttribute">The attribute type to search for</typeparam>
    /// <returns>Enum values that have the specified attribute</returns>
    public static T[] GetValuesWithAttribute<TAttribute>() where TAttribute : Attribute
    {
        var values = LazyValues.Value;
        var result = new List<T>(values.Length);

        foreach (var value in values)
        {
            if (typeof(T).GetField(value.ToString())?.GetCustomAttribute<TAttribute>() != null)
            {
                result.Add(value);
            }
        }

        return [.. result];
    }

    /// <summary>
    ///     Gets enum values with their attributes in a dictionary.
    /// </summary>
    /// <typeparam name="TAttribute">The attribute type</typeparam>
    /// <returns>Dictionary mapping enum values to their attributes</returns>
    public static Dictionary<T, TAttribute> GetValuesWithAttributeMap<TAttribute>() where TAttribute : Attribute
    {
        var result = new Dictionary<T, TAttribute>();
        foreach (var value in Values)
        {
            var attribute = typeof(T).GetField(value.ToString())?.GetCustomAttribute<TAttribute>();
            if (attribute != null)
            {
                result[value] = attribute;
            }
        }

        return result;
    }

    /// <summary>
    ///     Checks if a value is defined in the enum.
    /// </summary>
    /// <param name="value">Value to check</param>
    /// <returns>True if the value is defined</returns>
    public static bool IsDefined(T value) => Enum.IsDefined(value);

    /// <summary>
    ///     Parses a string value to the enum type or returns default value.
    /// </summary>
    /// <param name="value">String value to parse</param>
    /// <param name="defaultValue">Default value to return if parsing fails</param>
    /// <param name="ignoreCase">Whether to ignore case when parsing</param>
    /// <returns>Parsed enum value or default value</returns>
    public static T ParseOrDefault(string? value, T defaultValue = default, bool ignoreCase = false) =>
        TryParse(value, ignoreCase, out var result) ? result : defaultValue;

    /// <summary>
    ///     Gets the hexadecimal string representation of the enum's underlying value.
    /// </summary>
    /// <param name="value">The enum value</param>
    /// <returns>Hexadecimal string (uppercase, no prefix)</returns>
    public static string ToHexString(T value) => ToInt64(value).ToString("X");

    /// <summary>
    ///     Gets the underlying integer value of an enum.
    /// </summary>
    /// <param name="value">The enum value</param>
    /// <returns>Integer representation</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ToInt32(T value)
    {
        if (!IsInInt32Range())
        {
            return 0;
        }

        // Create a local copy to ensure we have a valid reference for Unsafe.As
        var local = value;

        return UnderlyingType switch
        {
            TypeCode.Byte => Unsafe.As<T, byte>(ref local),
            TypeCode.SByte => Unsafe.As<T, sbyte>(ref local),
            TypeCode.Int16 => Unsafe.As<T, short>(ref local),
            TypeCode.UInt16 => Unsafe.As<T, ushort>(ref local),
            TypeCode.Int32 or TypeCode.UInt32 => Unsafe.As<T, int>(ref local),
            var _ => Convert.ToInt32(value),
        };
    }

    /// <summary>
    ///     Gets the underlying long value of an enum.
    /// </summary>
    /// <param name="value">The enum value</param>
    /// <returns>Long representation</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long ToInt64(T value) =>
        // Use Convert for safe conversion regardless of underlying type
        Convert.ToInt64(value);

    /// <summary>
    ///     Tries to parse a string value to the enum type.
    /// </summary>
    /// <param name="value">String value to parse</param>
    /// <param name="ignoreCase">Whether to ignore case when parsing</param>
    /// <param name="result">The parsed enum value if successful</param>
    /// <returns>True if parsing was successful</returns>
    public static bool TryParse(string? value, bool ignoreCase, out T result)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            return Enum.TryParse(value, ignoreCase, out result);
        }

        result = default;
        return false;
    }

    /// <summary>
    ///     Tries to parse a string value to the enum type (case-sensitive).
    /// </summary>
    /// <param name="value">String value to parse</param>
    /// <param name="result">The parsed enum value if successful</param>
    /// <returns>True if parsing was successful</returns>
    public static bool TryParse(string? value, out T result) => TryParse(value, false, out result);

    /// <summary>
    ///     Tries to parse by a specific attribute's string representation.
    /// </summary>
    /// <typeparam name="TAttribute">The attribute type to search for</typeparam>
    /// <param name="attributeValue">The string value of the attribute to match</param>
    /// <param name="result">The matching enum value if found</param>
    /// <returns>True if a matching enum was found</returns>
    public static bool TryParseByAttribute<TAttribute>(string? attributeValue, out T result)
        where TAttribute : Attribute
    {
        result = default;
        if (string.IsNullOrWhiteSpace(attributeValue))
        {
            return false;
        }

        foreach (var value in Values)
        {
            var attribute = typeof(T).GetField(value.ToString())?.GetCustomAttribute<TAttribute>();

            var attributeString = attribute?.ToString();
            if (attributeString == null || attributeString != attributeValue)
            {
                continue;
            }

            result = value;
            return true;
        }

        return false;
    }

    /// <summary>
    ///     Tries to parse by display name instead of enum name.
    /// </summary>
    /// <param name="displayName">Display name to search for</param>
    /// <param name="resourceManager">Optional resource manager for localization</param>
    /// <param name="result">The matching enum value if found</param>
    /// <returns>True if a matching enum was found</returns>
    public static bool TryParseByDisplayName(string? displayName, out T result, ResourceManager? resourceManager = null)
    {
        result = default;
        if (string.IsNullOrWhiteSpace(displayName))
        {
            return false;
        }

        foreach (var kvp in GetDisplayList(resourceManager))
        {
            if (!string.Equals(kvp.Value, displayName, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            result = kvp.Key;
            return true;
        }

        return false;
    }

    private static EnumKeyValue<T>[] BuildDisplayList()
    {
        return
        [
            .. Values.ToArray()
                .Select(value => new EnumKeyValue<T>(value, ResolveDisplayName(value, null, true) ?? value.ToString())),
        ];
    }

    private static int CalculateLevenshteinDistance(string source, string target)
    {
        // Optimize memory usage by using two rows instead of full matrix
        // Memory: O(min(n,m)) instead of O(n*m)

        if (source.Length > target.Length)
        {
            // Swap to ensure source is the shorter string
            (source, target) = (target, source);
        }

        var previousRow = new int[source.Length + 1];
        var currentRow = new int[source.Length + 1];

        // Initialize first row
        for (var i = 0; i <= source.Length; i++)
        {
            previousRow[i] = i;
        }

        for (var j = 1; j <= target.Length; j++)
        {
            currentRow[0] = j;

            for (var i = 1; i <= source.Length; i++)
            {
                var cost = source[i - 1] == target[j - 1] ? 0 : 1;
                currentRow[i] = Math.Min(
                    Math.Min(currentRow[i - 1] + 1, previousRow[i] + 1),
                    previousRow[i - 1] + cost);
            }

            // Swap rows for next iteration
            (previousRow, currentRow) = (currentRow, previousRow);
        }

        return previousRow[source.Length];
    }

    private static double CalculateLevenshteinSimilarity(string source, string target)
    {
        if (source == target)
        {
            return 1.0;
        }

        if (source.Length == 0 || target.Length == 0)
        {
            return 0.0;
        }

        var distance = CalculateLevenshteinDistance(source, target);
        var maxLength = Math.Max(source.Length, target.Length);
        return 1.0 - (double)distance / maxLength;
    }

    private static bool IsInInt32Range() => UnderlyingType is TypeCode.Byte or TypeCode.SByte or TypeCode.Int16
        or TypeCode.UInt16 or TypeCode.Int32 or TypeCode.UInt32;

    /// <summary>
    ///     Resolves a string value from an attribute using reflection to access properties dynamically.
    ///     Supports attributes with ResourceType/Name pattern (like DisplayAttribute, DescriptionAttribute).
    /// </summary>
    /// <typeparam name="TValue">The enum type</typeparam>
    /// <typeparam name="TAttribute">The attribute type to resolve from</typeparam>
    /// <param name="value">The enum value</param>
    /// <param name="resourceManager">Optional resource manager for localization</param>
    /// <param name="fallbackToEnumName">Whether to fall back to enum name if no value found</param>
    /// <param name="textPropertyName">Name of the property containing the text/name (e.g., "Name", "Description")</param>
    /// <returns>Resolved string value or null</returns>
    private static string? ResolveByAttribute<TValue, TAttribute>(
        TValue value,
        ResourceManager? resourceManager,
        bool fallbackToEnumName,
        string textPropertyName = "Name")
        where TValue : struct, Enum
        where TAttribute : Attribute
    {
        var field = typeof(TValue).GetField(value.ToString());
        if (field == null)
        {
            return fallbackToEnumName ? value.ToString() : null;
        }

        var attribute = field.GetCustomAttribute<TAttribute>();
        if (attribute == null)
        {
            return fallbackToEnumName ? field.Name : null;
        }

        var attributeType = typeof(TAttribute);

        // Try to get ResourceType property
        var resourceTypeProperty =
            attributeType.GetProperty("ResourceType", BindingFlags.Public | BindingFlags.Instance);
        var textProperty = attributeType.GetProperty(textPropertyName, BindingFlags.Public | BindingFlags.Instance);

        // If ResourceType property exists and is set, try to resolve from resources
        if (resourceTypeProperty != null && textProperty != null)
        {
            var resourceType = resourceTypeProperty.GetValue(attribute) as Type;
            var textValue = textProperty.GetValue(attribute) as string;

            if (resourceType != null && !string.IsNullOrEmpty(textValue))
            {
                var property = resourceType.GetProperty(textValue, BindingFlags.Static | BindingFlags.Public);
                if (property?.GetValue(null) is string resourceValue)
                {
                    return resourceValue;
                }
            }

            // Use text value directly if ResourceType is not set
            if (!string.IsNullOrEmpty(textValue))
            {
                return textValue;
            }
        }

        // Try provided resource manager
        if (resourceManager != null && textProperty != null)
        {
            var resourceKey = textProperty.GetValue(attribute) as string ?? field.Name;
            try
            {
                var resourceValue = resourceManager.GetString(resourceKey);
                if (!string.IsNullOrEmpty(resourceValue))
                {
                    return resourceValue;
                }
            }
            catch (MissingManifestResourceException)
            {
                // Resource not found - continue to fallback
            }
            catch (InvalidOperationException)
            {
                // Resource manager not initialized properly - continue to fallback
            }
        }

        // Try to get the text value directly from the property
        if (textProperty != null)
        {
            var directValue = textProperty.GetValue(attribute) as string;
            if (!string.IsNullOrEmpty(directValue))
            {
                return directValue;
            }
        }

        // Fallback to enum name if requested
        return fallbackToEnumName ? field.Name : null;
    }

    private static string? ResolveDisplayName(T value, ResourceManager? resourceManager, bool fallbackToEnumName) =>
        ResolveByAttribute<T, DisplayAttribute>(value, resourceManager, fallbackToEnumName);

    #endregion


    // Cache for better performance - avoid reflection on every call
}
