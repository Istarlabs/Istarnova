#region "Header"

// File: FormTextAreaField.razor.cs
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
///     A labelled multi-line text input in its own cell of the form grid.
/// </summary>
public sealed partial class FormTextAreaField
{
    #region "Properties"

    [Parameter]
    public string? Height { get; set; }

    [Parameter]
    public bool Immediate { get; set; }

    [Parameter]
    public int ImmediateDelay { get; set; } = 200;

    [Parameter]
    public int? MaxLength { get; set; }

    [Parameter]
    public string? Placeholder { get; set; }

    #endregion
}
