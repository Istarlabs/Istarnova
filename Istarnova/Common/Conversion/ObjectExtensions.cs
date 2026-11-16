#region "Header"

// File: ObjectExtensions.cs
// Created 2026/03/06
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

#region "Usings"

using System.ComponentModel;
using System.Reflection;
using System.Text.Json;

#endregion

namespace Istarnova.Common.Conversion;

/// <summary>
///     Object extension methods.
/// </summary>
public static class ObjectExtensions
{
    #region "Methods"

    /// <param name="obj">Object to convert</param>
    extension(object? obj)
    {
        #region "Methods"

        /// <summary>
        ///     Safely converts an object to the specified type using modern .NET conversion.
        /// </summary>
        /// <typeparam name="T">Target type</typeparam>
        /// <returns>Converted value or default</returns>
        public T? To<T>()
        {
            if (obj is null)
            {
                return default;
            }

            if (obj is T directCast)
            {
                return directCast;
            }

            try
            {
                var targetType = typeof(T);
                var nullableType = Nullable.GetUnderlyingType(targetType);

                if (nullableType is { })
                {
                    targetType = nullableType;
                }

                if (targetType.IsEnum)
                {
                    return (T)Enum.Parse(targetType, obj.ToString()!);
                }

                var converter = TypeDescriptor.GetConverter(targetType);
                return converter.CanConvertFrom(obj.GetType())
                    ? (T)converter.ConvertFrom(obj)!
                    : (T)Convert.ChangeType(obj, targetType);
            }
            catch
            {
                return default;
            }
        }

        /// <summary>
        ///     Attempts to convert an object to the specified type.
        /// </summary>
        /// <typeparam name="T">Target type</typeparam>
        /// <param name="result">Converted value</param>
        /// <returns>True if conversion succeeded</returns>
        public bool TryTo<T>(out T? result)
        {
            try
            {
                result = obj.To<T>();
                return true;
            }
            catch
            {
                result = default;
                return false;
            }
        }

        #endregion
    }

    /// <summary>
    ///     Deep clone an object using System.Text.Json serialization.
    /// </summary>
    /// <typeparam name="T">Type of object to clone</typeparam>
    /// <param name="obj">Object to clone</param>
    /// <returns>Deep cloned object</returns>
    public static T? DeepClone<T>(this T obj) where T : class
    {
        ArgumentNullException.ThrowIfNull(obj);

        var json = JsonSerializer.Serialize(obj);
        return JsonSerializer.Deserialize<T>(json);
    }

    /// <summary>
    ///     Checks if an object is null or default value.
    /// </summary>
    /// <typeparam name="T">Type of object</typeparam>
    /// <param name="obj">Object to check</param>
    /// <returns>True if null or default</returns>
    public static bool IsNullOrDefault<T>(this T? obj) => EqualityComparer<T>.Default.Equals(obj, default);

    /// <summary>
    ///     Converts an anonymous object to a Dictionary for modern scenarios.
    /// </summary>
    /// <param name="obj">The object to convert</param>
    /// <returns>Dictionary representation of the object properties</returns>
    public static Dictionary<string, object?> ToDictionary(this object obj)
    {
        ArgumentNullException.ThrowIfNull(obj);

        return obj.GetType()
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .ToDictionary(prop => prop.Name, prop => prop.GetValue(obj));
    }

    #endregion
}
