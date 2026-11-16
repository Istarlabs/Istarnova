#region "Header"

// File: FilterProperty.cs
// Created 2026/10/28
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

#region "Usings"

using System.Linq.Expressions;

#endregion

namespace Istarnova.Filtering;

/// <summary>
///     A property of <typeparamref name="TItem" /> the user may filter on.
/// </summary>
/// <param name="Name">Identifies the property in a <see cref="FilterRule" />.</param>
/// <param name="Title">What the user sees.</param>
/// <param name="Kind">Decides operators and editor.</param>
/// <param name="Value">
///     Reads the property: text as <see cref="string" />, numbers as any numeric type, dates as <see cref="DateOnly" />,
///     flags as <see cref="bool" />; <see langword="null" /> when the item has none.
/// </param>
/// <param name="Selector">
///     The same property as an expression, which is what a filter on a query needs: the database cannot run
///     <paramref name="Value" />. Null for a property that only filters a list in memory. Made by
///     <see cref="FilterProperties" />.
/// </param>
public sealed record FilterProperty<TItem>(
    string Name,
    string Title,
    FilterKind Kind,
    Func<TItem, object?> Value,
    LambdaExpression? Selector = null);
