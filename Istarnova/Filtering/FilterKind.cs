#region "Header"

// File: FilterKind.cs
// Created 2026/10/28
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

namespace Istarnova.Filtering;

/// <summary>
///     What a filterable property holds, which decides the operators it offers and the editor for its value.
/// </summary>
public enum FilterKind
{
    /// <summary>
    ///     A string.
    /// </summary>
    Text,

    /// <summary>
    ///     Any numeric type, compared as <see cref="decimal" />.
    /// </summary>
    Number,

    /// <summary>
    ///     A <see cref="DateOnly" />.
    /// </summary>
    Date,

    /// <summary>
    ///     A <see cref="bool" />; <see langword="null" /> counts as <see langword="false" />.
    /// </summary>
    Boolean,
}
