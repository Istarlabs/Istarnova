#region "Header"

// File: FormField.razor.cs
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
///     One cell of a <c>form-grid</c>: what is put in it, and the hint under it. FluentUI renders label and control as
///     siblings, so a labelled input has to sit in a cell of its own to keep the pair together.
/// </summary>
public sealed partial class FormField
{
    #region "Properties"

    /// <summary>Centres the content vertically, for a checkbox that sits beside a labelled input.</summary>
    [Parameter]
    public bool Center { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>Spans the whole row of the grid.</summary>
    [Parameter]
    public bool Full { get; set; }

    [Parameter]
    public string? Hint { get; set; }

    private string CssClass
        => (Full, Center) switch
        {
            (true, true) => "form-field form-field--full form-field--center",
            (true, false) => "form-field form-field--full",
            (false, true) => "form-field form-field--center",
            _ => "form-field",
        };

    #endregion
}
