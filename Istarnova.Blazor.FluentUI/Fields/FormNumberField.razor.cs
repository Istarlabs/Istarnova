#region "Header"

// File: FormNumberField.razor.cs
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
///     A labelled number input in its own cell of the form grid.
/// </summary>
/// <typeparam name="TValue">The number type, nullable or not.</typeparam>
public sealed partial class FormNumberField<TValue>
{
    #region "Variables"

    private readonly Dictionary<string, object> _bounds = [];

    #endregion

    #region "Properties"

    [Parameter]
    public bool Immediate { get; set; }

    [Parameter]
    public int ImmediateDelay { get; set; } = 200;

    [Parameter]
    public TValue? Max { get; set; }

    [Parameter]
    public TValue? Min { get; set; }

    [Parameter]
    public string? Placeholder { get; set; }

    [Parameter]
    public TValue? Step { get; set; }

    #endregion

    #region "Methods"

    /// <summary>
    ///     Hands <c>Min</c>, <c>Max</c> and <c>Step</c> on only when the caller gave them: an unset <c>Min</c> forwarded as
    ///     <c>0</c> would forbid every negative number.
    /// </summary>
    public override Task SetParametersAsync(ParameterView parameters)
    {
        CollectGiven<TValue>(parameters, _bounds, nameof(Min), nameof(Max), nameof(Step));

        return base.SetParametersAsync(parameters);
    }

    #endregion
}
