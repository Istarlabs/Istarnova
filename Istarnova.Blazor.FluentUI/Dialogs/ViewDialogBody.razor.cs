#region "Header"

// File: ViewDialogBody.razor.cs
// Created 2026/10/30
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

#region "Usings"

using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

#endregion

namespace Istarnova.Blazor.FluentUI.Dialogs;

/// <summary>
///     The body of a dialog that only shows something: title with icon, the content and a Close button. It saves nothing,
///     so closing it is all the footer does.
/// </summary>
public sealed partial class ViewDialogBody
{
    #region "Properties"

    /// <summary>Buttons beside Close, in front of it.</summary>
    [Parameter]
    public RenderFragment? Actions { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [CascadingParameter]
    public IDialogInstance Dialog { get; set; } = null!;

    [Parameter]
    public Icon? Icon { get; set; }

    /// <summary>The header title the dialog was opened with, unless one is given here.</summary>
    [Parameter]
    public string? Title { get; set; }

    /// <summary>Replaces icon and title, for a heading with more than one line.</summary>
    [Parameter]
    public RenderFragment? TitleContent { get; set; }

    private string ResolvedTitle
        => Title ?? Dialog.Options.Header.Title ?? string.Empty;

    #endregion

    #region "Methods"

    private Task CloseAsync()
        => Dialog.CloseAsync();

    #endregion
}
