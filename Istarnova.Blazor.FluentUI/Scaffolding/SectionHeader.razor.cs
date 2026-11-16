#region "Header"

// File: SectionHeader.razor.cs
// Created 2026/10/24
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

namespace Istarnova.Blazor.FluentUI.Scaffolding;

/// <summary>
///     The title of a card or a list: an optional icon and the title on one centred line, with the actions that belong to it
///     (an edit or add button) on the trailing side. The title is exposed as a heading without a heading element inside the
///     text, because that element's own margin would push the text off the icon's centre line.
/// </summary>
public sealed partial class SectionHeader
{
    #region "Properties"

    /// <summary>
    ///     The actions rendered on the trailing side.
    /// </summary>
    [Parameter]
    public RenderFragment? Actions { get; set; }

    /// <summary>
    ///     An optional icon shown in front of the title.
    /// </summary>
    [Parameter]
    public Icon? Icon { get; set; }

    /// <summary>
    ///     The colour of the icon.
    /// </summary>
    [Parameter]
    public Color IconColor { get; set; } = Color.Primary;

    /// <summary>
    ///     The heading level announced to assistive technology; it does not change how the title looks.
    /// </summary>
    [Parameter]
    public int Level { get; set; } = 3;

    /// <summary>
    ///     The title.
    /// </summary>
    [Parameter]
    [EditorRequired]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    ///     The size of the title text.
    /// </summary>
    [Parameter]
    public TextSize TitleSize { get; set; } = TextSize.Size400;

    #endregion
}
