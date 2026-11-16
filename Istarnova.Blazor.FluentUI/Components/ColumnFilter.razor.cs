#region "Header"

// File: ColumnFilter.razor.cs
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
///     A filter button for the header of one column: an operator and a value in a popover. It edits one rule of the shared
///     <see cref="FilterSet" />, so what is set here also shows in a <see cref="DataFilter{TItem}" /> on the same set, and the
///     other way round. Where the set holds several rules for the column, it edits the first and leaves the others alone.
/// </summary>
/// <typeparam name="TItem">The type of item the column belongs to.</typeparam>
public sealed partial class ColumnFilter<TItem>
{
    #region "Variables"

    private readonly string _anchorId = $"column-filter-{Guid.NewGuid():N}";

    private bool _open;

    #endregion

    #region "Properties"

    /// <summary>
    ///     The conditions; the component edits this instance.
    /// </summary>
    [Parameter]
    public FilterSet FilterSet { get; set; } = new();

    /// <summary>
    ///     Raised after every change of the rule, so the caller applies the set again.
    /// </summary>
    [Parameter]
    public EventCallback OnChanged { get; set; }

    /// <summary>
    ///     The column: its name in the rules, its title and what it holds.
    /// </summary>
    [Parameter]
    [EditorRequired]
    public FilterProperty<TItem> Property { get; set; } = null!;

    private string ButtonTitle
        => string.Format(CultureInfo.CurrentCulture, FilterResources.FilterColumn, Property.Title);

    /// <summary>
    ///     Whether the column filters anything yet: the button says so, as an operator without its value does not.
    /// </summary>
    private bool IsActive
        => Rule is { } rule && rule.IsComplete(Property.Kind);

    private FilterRule? Rule
        => FilterSet.Rules.Find(rule => rule.Property == Property.Name);

    #endregion

    #region "Methods"

    /// <summary>
    ///     Opening a column without a rule starts one, as the data filter does, so the operator it shows is the one it will use.
    /// </summary>
    private void Open()
    {
        if (Rule is null)
        {
            FilterSet.Rules.Add(new() { Property = Property.Name, Operator = FilterOperators.For(Property.Kind)[0] });
        }
    }

    private Task ToggleAsync()
    {
        _open = !_open;

        if (_open)
        {
            Open();
        }

        return Task.CompletedTask;
    }

    private Task ClearAsync()
    {
        FilterSet.Rules.RemoveAll(rule => rule.Property == Property.Name);
        _open = false;

        return OnChanged.InvokeAsync();
    }

    private Task OnOperatorChangedAsync(string? name)
    {
        if (Rule is { } rule && Enum.TryParse<FilterOperator>(name, out var filterOperator))
        {
            rule.Operator = filterOperator;
        }

        return OnChanged.InvokeAsync();
    }

    #endregion
}
