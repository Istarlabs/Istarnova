#region "Header"

// File: FilterOperators.cs
// Created 2026/10/28
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

namespace Istarnova.Filtering;

/// <summary>
///     The operators each <see cref="FilterKind" /> offers.
/// </summary>
public static class FilterOperators
{
    #region "Methods"

    /// <summary>
    ///     The operators of a kind, the first one being what a new rule starts with.
    /// </summary>
    public static IReadOnlyList<FilterOperator> For(FilterKind kind)
        => kind switch
        {
            FilterKind.Text =>
            [
                FilterOperator.Contains, FilterOperator.NotContains, FilterOperator.StartsWith, FilterOperator.EndsWith,
                FilterOperator.IsEqualTo, FilterOperator.IsNotEqualTo,
                FilterOperator.IsEmpty, FilterOperator.IsNotEmpty,
            ],
            FilterKind.Boolean => [FilterOperator.IsTrue, FilterOperator.IsFalse],
            var _ =>
            [
                FilterOperator.IsEqualTo, FilterOperator.IsNotEqualTo, FilterOperator.GreaterThan,
                FilterOperator.GreaterThanOrEqualTo, FilterOperator.LessThan, FilterOperator.LessThanOrEqualTo,
                FilterOperator.IsEmpty, FilterOperator.IsNotEmpty,
            ],
        };

    /// <summary>
    ///     Whether the operator compares with a value the user enters.
    /// </summary>
    public static bool NeedsValue(FilterOperator filterOperator)
        => filterOperator is not (FilterOperator.IsEmpty or FilterOperator.IsNotEmpty or FilterOperator.IsTrue or FilterOperator.IsFalse);

    #endregion
}
