#region "Header"

// File: CrudPage.razor.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using Istarnova.Blazor.FluentUI.Scaffolding;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

namespace Istarnova.Blazor.FluentUI.Grids;

/// <summary>
///     The frame for an administration list page: the standard header with a "new entry" action, and the list below it.
/// </summary>
public sealed partial class CrudPage
{
    #region "Properties"

    /// <summary>
    ///     A replacement for the default action area. Wins over <see cref="OnCreate" />.
    /// </summary>
    [Parameter]
    public RenderFragment? Actions { get; set; }

    /// <summary>
    ///     The URL of an optional back link. See <see cref="PageScaffold.BackUrl" />.
    /// </summary>
    [Parameter]
    public string? BackUrl { get; set; }

    /// <summary>
    ///     An optional breadcrumb trail, which replaces the back link.
    /// </summary>
    [Parameter]
    public IReadOnlyList<PageBreadcrumb>? Breadcrumbs { get; set; }

    /// <summary>
    ///     The page content below the header.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    ///     The label of the default create button, e.g. "New country".
    /// </summary>
    [Parameter]
    public string? CreateText { get; set; }

    /// <summary>
    ///     The handler for the default create button. Leave unset on a page that has nothing to create - the button is then
    ///     not rendered.
    /// </summary>
    [Parameter]
    public EventCallback OnCreate { get; set; }

    /// <summary>
    ///     The page title.
    /// </summary>
    [Parameter]
    [EditorRequired]
    public string Title { get; set; } = null!;

    /// <summary>
    ///     An optional icon shown in front of the title.
    /// </summary>
    [Parameter]
    public Icon? TitleIcon { get; set; }

    #endregion
}
