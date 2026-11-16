#region "Header"

// File: FormDateField.razor.cs
// Created 2026/10/30
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

#region "Usings"

using Microsoft.AspNetCore.Components;

#endregion

namespace Istarnova.Blazor.FluentUI.Fields;

/// <summary>
///     A labelled date picker in its own cell of the form grid.
/// </summary>
/// <typeparam name="TValue">
///     <see cref="DateTime" /> or <see cref="Nullable{T}" /> of it.
/// </typeparam>
public sealed partial class FormDateField<TValue>
{
    #region "Variables"

    private readonly Dictionary<string, object> _bounds = [];

    #endregion

    #region "Properties"

    [Parameter]
    public TValue? MaxDate { get; set; }

    [Parameter]
    public TValue? MinDate { get; set; }

    [Parameter]
    public string? Placeholder { get; set; }

    #endregion

    #region "Methods"

    public override Task SetParametersAsync(ParameterView parameters)
    {
        CollectGiven<TValue>(parameters, _bounds, nameof(MinDate), nameof(MaxDate));

        return base.SetParametersAsync(parameters);
    }

    #endregion
}
