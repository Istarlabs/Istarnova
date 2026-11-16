#region "Header"

// File: DataFilter.razor.cs
// Created 2026/10/28
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
///     A button that opens a panel where conditions on <typeparamref name="TItem" /> are added and removed. It only edits
///     the <see cref="FilterSet" />; the caller applies it with <see cref="Istarnova.Filtering.FilterSet.Apply{TItem}" />
///     and tells the component how many items remain.
/// </summary>
/// <typeparam name="TItem">The type of item the conditions are about.</typeparam>
public sealed partial class DataFilter<TItem>
{
    #region "Constants"

    private const string AllMode = "All";
    private const string AnyMode = "Any";

    #endregion

    #region "Variables"

    private static readonly string[] MatchModes = [AllMode, AnyMode];

    private bool _open;

    #endregion

    #region "Properties"

    /// <summary>
    ///     The conditions; the component edits this instance.
    /// </summary>
    [Parameter]
    public FilterSet FilterSet { get; set; } = new();

    /// <summary>
    ///     Raised after every change of the conditions, so the caller applies them again.
    /// </summary>
    [Parameter]
    public EventCallback OnChanged { get; set; }

    /// <summary>
    ///     What can be filtered on. At least one entry.
    /// </summary>
    [Parameter]
    public IReadOnlyList<FilterProperty<TItem>> Properties { get; set; } = [];

    /// <summary>
    ///     How many items pass the conditions.
    /// </summary>
    [Parameter]
    public int ShownCount { get; set; }

    /// <summary>
    ///     How many items there are without any condition.
    /// </summary>
    [Parameter]
    public int TotalCount { get; set; }

    private string ShownText
        => string.Format(CultureInfo.CurrentCulture, FilterResources.Shown, ShownCount, TotalCount);

    #endregion

    #region "Methods"

    private static string MatchModeText(string mode)
        => mode == AnyMode ? FilterResources.MatchAny : FilterResources.MatchAll;

    private FilterProperty<TItem> PropertyOf(FilterRule rule)
        => Properties.FirstOrDefault(property => property.Name == rule.Property) ?? Properties[0];

    private void Toggle()
    {
        _open = !_open;

        if (_open && FilterSet.Rules.Count == 0 && Properties.Count > 0)
        {
            AddRule();
        }
    }

    private Task AddAsync()
    {
        AddRule();

        return OnChanged.InvokeAsync();
    }

    private void AddRule()
    {
        var property = Properties[0];

        FilterSet.Rules.Add(new() { Property = property.Name, Operator = FilterOperators.For(property.Kind)[0] });
    }

    private Task RemoveAsync(FilterRule rule)
    {
        FilterSet.Rules.Remove(rule);

        return OnChanged.InvokeAsync();
    }

    private Task ClearAsync()
    {
        FilterSet.Rules.Clear();
        _open = false;

        return OnChanged.InvokeAsync();
    }

    private Task OnPropertyChangedAsync(FilterRule rule, string? name)
    {
        var property = Properties.FirstOrDefault(entry => entry.Name == name);

        if (property is null)
        {
            return Task.CompletedTask;
        }

        rule.Property = property.Name;
        rule.Operator = FilterOperators.For(property.Kind)[0];
        rule.Text = null;
        rule.Number = null;
        rule.Date = null;

        return OnChanged.InvokeAsync();
    }

    private Task OnOperatorChangedAsync(FilterRule rule, string? name)
    {
        if (Enum.TryParse<FilterOperator>(name, out var filterOperator))
        {
            rule.Operator = filterOperator;
        }

        return OnChanged.InvokeAsync();
    }

    private Task OnMatchModeChangedAsync(string? mode)
    {
        FilterSet.MatchAny = mode == AnyMode;

        return OnChanged.InvokeAsync();
    }

    #endregion
}
