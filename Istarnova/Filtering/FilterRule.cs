#region "Header"

// File: FilterRule.cs
// Created 2026/10/28
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

namespace Istarnova.Filtering;

/// <summary>
///     One condition of a <see cref="FilterSet" />: a property, how to compare it, and the value to compare with.
/// </summary>
public sealed class FilterRule
{
    #region "Properties"

    /// <summary>
    ///     The <see cref="FilterProperty{TItem}.Name" /> of the property compared.
    /// </summary>
    public string Property { get; set; } = string.Empty;

    public FilterOperator Operator { get; set; }

    /// <summary>
    ///     The value of a <see cref="FilterKind.Text" /> property.
    /// </summary>
    public string? Text { get; set; }

    /// <summary>
    ///     The value of a <see cref="FilterKind.Number" /> property.
    /// </summary>
    public decimal? Number { get; set; }

    /// <summary>
    ///     The value of a <see cref="FilterKind.Date" /> property.
    /// </summary>
    public DateOnly? Date { get; set; }

    #endregion

    #region "Methods"

    /// <summary>
    ///     Whether the rule says something yet: an operator that needs a value has one. A rule that does not matches everything,
    ///     so a condition still being typed in does not empty the list.
    /// </summary>
    /// <param name="kind">What the property of the rule holds, which decides which value counts.</param>
    public bool IsComplete(FilterKind kind)
        => !FilterOperators.NeedsValue(Operator)
           || kind switch
           {
               FilterKind.Text => !string.IsNullOrWhiteSpace(Text),
               FilterKind.Number => Number is { },
               FilterKind.Date => Date is { },
               var _ => true,
           };

    #endregion
}
