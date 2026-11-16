#region "Header"

// File: FilterValueEditor.razor.cs
// Created 2026/11/07
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

#region "Usings"

using System.Globalization;
using Istarnova.Filtering;
using Istarnova.Resources;
using Microsoft.AspNetCore.Components;

#endregion

namespace Istarnova.Blazor.FluentUI.Components;

/// <summary>
///     The input for the value of one <see cref="FilterRule" />, the kind of input following the <see cref="FilterKind" /> of
///     the property. Shared by <see cref="DataFilter{TItem}" /> and <see cref="ColumnFilter{TItem}" />.
/// </summary>
public sealed partial class FilterValueEditor
{
    #region "Properties"

    /// <summary>
    ///     What the property holds, which decides the input.
    /// </summary>
    [Parameter]
    public FilterKind Kind { get; set; }

    /// <summary>
    ///     Raised after the value of the rule changed.
    /// </summary>
    [Parameter]
    public EventCallback OnChanged { get; set; }

    /// <summary>
    ///     The rule whose value is edited; the component writes to this instance.
    /// </summary>
    [Parameter]
    public FilterRule Rule { get; set; } = new();

    private string NumberText
        => Rule.Number?.ToString(CultureInfo.CurrentCulture) ?? string.Empty;

    #endregion

    #region "Methods"

    private Task OnTextChangedAsync(string? text)
    {
        Rule.Text = text;

        return OnChanged.InvokeAsync();
    }

    private Task OnNumberChangedAsync(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            Rule.Number = null;
        }
        else if (decimal.TryParse(text, NumberStyles.Number, CultureInfo.CurrentCulture, out var number))
        {
            Rule.Number = number;
        }

        return OnChanged.InvokeAsync();
    }

    private Task OnDateChangedAsync(DateTime? date)
    {
        Rule.Date = date is { } picked ? DateOnly.FromDateTime(picked) : null;

        return OnChanged.InvokeAsync();
    }

    #endregion
}
