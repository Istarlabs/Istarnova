#region "Header"

// File: EnumKeyValue.cs
// Created 2026/03/06
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

namespace Istarnova.Common.Enums;

/// <summary>
///     Immutable record representing an enum key-value pair with display name.
///     Provides efficient value equality and pattern matching support.
/// </summary>
/// <typeparam name="T">The enum type</typeparam>
/// <param name="Key">The enum value</param>
/// <param name="Value">The display name or description</param>
public readonly record struct EnumKeyValue<T>(T Key, string Value) where T : struct, Enum
{
    #region "Properties"

    /// <summary>
    ///     Gets the underlying integer value of the enum key.
    /// </summary>
    public int IntValue => EnumEx<T>.ToInt32(Key);

    /// <summary>
    ///     Gets the underlying long value of the enum key.
    /// </summary>
    public long LongValue => EnumEx<T>.ToInt64(Key);

    #endregion

    #region "Methods"

    /// <summary>
    ///     Deconstructs the key-value pair for pattern matching and tuple assignments.
    /// </summary>
    /// <param name="key">The enum key</param>
    /// <param name="value">The display value</param>
    public void Deconstruct(out T key, out string value)
    {
        key = Key;
        value = Value;
    }

    /// <summary>
    ///     Deconstructs the key-value pair with integer value.
    /// </summary>
    /// <param name="key">The enum key</param>
    /// <param name="value">The display value</param>
    /// <param name="intValue">The integer representation</param>
    public void Deconstruct(out T key, out string value, out int intValue)
    {
        key = Key;
        value = Value;
        intValue = IntValue;
    }

    /// <summary>
    ///     Returns the display value as string representation.
    /// </summary>
    /// <returns>The display value</returns>
    public override string ToString() => Value;

    /// <summary>
    ///     Implicitly converts to the enum value for convenience.
    /// </summary>
    /// <param name="keyValue">The enum key-value pair</param>
    public static implicit operator T(EnumKeyValue<T> keyValue) => keyValue.Key;

    /// <summary>
    ///     Implicitly converts to the display string for convenience.
    /// </summary>
    /// <param name="keyValue">The enum key-value pair</param>
    public static implicit operator string(EnumKeyValue<T> keyValue) => keyValue.Value;

    #endregion
}
