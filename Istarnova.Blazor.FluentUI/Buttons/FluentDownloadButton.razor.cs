#region "Header"

// File: FluentDownloadButton.razor.cs
// Created 2026/10/30
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

#region "Usings"

using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using RegularIcons = Microsoft.FluentUI.AspNetCore.Components.Icons.Regular;

#endregion

namespace Istarnova.Blazor.FluentUI.Buttons;

/// <summary>
///     A button that downloads what the server answers at <see cref="Href" />. It is a <see cref="FluentAnchorButton" />
///     that always loads the address: left to itself, the anchor button hands a same-origin address to the client-side
///     router, which finds no page for a file and shows "Not found" without the server being asked.
/// </summary>
public sealed partial class FluentDownloadButton : ComponentBase
{
    #region "Properties"

    [Parameter]
    public ButtonAppearance Appearance { get; set; } = ButtonAppearance.Transparent;

    /// <summary>The text beside the icon. Left out, the button is icon-only and <see cref="Title" /> is its name.</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>The address of the file. The response should carry <c>Content-Disposition: attachment</c>.</summary>
    [Parameter]
    [EditorRequired]
    public string Href { get; set; } = string.Empty;

    [Parameter]
    public Icon Icon { get; set; } = new RegularIcons.Size20.ArrowDownload();

    [Parameter]
    public ButtonSize? Size { get; set; }

    /// <summary>The accessible name, and the hover text. Required: an icon-only button without one is read as "link".</summary>
    [Parameter]
    [EditorRequired]
    public string Title { get; set; } = string.Empty;

    #endregion
}
