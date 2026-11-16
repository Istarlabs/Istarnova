#region "Header"

// File: FormTextField.razor.cs
// Created 2026/10/30
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

#region "Usings"

using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

#endregion

namespace Istarnova.Blazor.FluentUI.Fields;

/// <summary>
///     A labelled single-line text input in its own cell of the form grid.
/// </summary>
public sealed partial class FormTextField
{
    #region "Properties"

    /// <summary>The browser's autofill hint ("off", "new-password", "email"); left out, none is sent.</summary>
    [Parameter]
    public string? AutoComplete { get; set; }

    [Parameter]
    public bool Immediate { get; set; }

    [Parameter]
    public int ImmediateDelay { get; set; } = 200;

    [Parameter]
    public int? MaxLength { get; set; }

    [Parameter]
    public string? Placeholder { get; set; }

    [Parameter]
    public TextInputType? TextInputType { get; set; }

    #endregion
}
