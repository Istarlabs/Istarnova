#region "Header"

// File: DialogTitle.razor.cs
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
///     The title of a dialog: an optional icon beside the heading.
/// </summary>
public sealed partial class DialogTitle
{
    #region "Properties"

    [Parameter]
    public Icon? Icon { get; set; }

    [Parameter]
    public string Title { get; set; } = string.Empty;

    #endregion
}
