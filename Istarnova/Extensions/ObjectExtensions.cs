#region "Header"

// File: ObjectExtensions.cs
// Created 2026/03/06
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

#region "Usings"

using System.Text.Json;

#endregion

namespace Istarnova.Extensions;

public static class ObjectExtensions
{
    #region "Methods"

    extension(object? obj)
    {
        #region "Methods"

        public T? As<T>() where T : class
        {
            if (obj is T t)
            {
                return t;
            }

            return null;
        }

        public T AsOrDefault<T>(T defaultValue) where T : class
        {
            if (obj is T t)
            {
                return t;
            }

            return defaultValue;
        }

        #endregion
    }

    extension<T>(T val) where T : IComparable
    {
        #region "Methods"

        public bool IsWithin(T min, T max) =>
            val.CompareTo(min) >= 0 && val.CompareTo(max) <= 0;

        #endregion
    }

    extension<T>(T obj)
    {
        #region "Methods"

        public T? DeepCopy()
        {
            var json = JsonSerializer.Serialize(obj);
            return JsonSerializer.Deserialize<T>(json);
        }

        public T GetValueOrDefault(T defaultValue = default!)
        {
            return obj switch
            {
                string str when str.IsEmptyOrWhiteSpace() => defaultValue,
                null when typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(Nullable<>) =>
                    defaultValue,
                null when !typeof(T).IsValueType => defaultValue,
                var _ when typeof(T).IsValueType && EqualityComparer<T>.Default.Equals(obj, default) => defaultValue,
                var _ => obj,
            };
        }

        #endregion
    }

    extension(object obj)
    {
        #region "Methods"

        public object? GetPropertyValue(string propertyName)
        {
            ArgumentNullException.ThrowIfNull(obj);
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw new ArgumentException("Property name cannot be null or empty.", nameof(propertyName));
            }

            var propertyInfo = obj.GetType().GetProperty(propertyName);
            if (propertyInfo == null)
            {
                throw new ArgumentException($"Property '{propertyName}' not found on type '{obj.GetType().FullName}'.",
                    nameof(propertyName));
            }

            return propertyInfo.GetValue(obj);
        }

        public T? GetPropertyValue<T>(string propertyName)
        {
            var value = obj.GetPropertyValue(propertyName);
            if (value is T t)
            {
                return t;
            }

            return default;
        }

        #endregion
    }

    #endregion
}
