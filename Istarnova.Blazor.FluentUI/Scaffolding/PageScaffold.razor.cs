#region "Header"

// File: PageScaffold.razor.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using Istarnova.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

namespace Istarnova.Blazor.FluentUI.Scaffolding;

/// <summary>
///     The standard frame for a content page: an optional way back (breadcrumb trail or a single back link), the page
///     title, the page-level actions, and the content below them.
/// </summary>
public sealed partial class PageScaffold
{
    #region "Properties"

    /// <summary>
    ///     The page-level actions rendered on the trailing side of the title row.
    /// </summary>
    [Parameter]
    public RenderFragment? Actions { get; set; }

    /// <summary>
    ///     The label of the back link. Defaults to the library's "Back to overview" text.
    /// </summary>
    [Parameter]
    public string? BackLinkText { get; set; }

    /// <summary>
    ///     The URL the back link navigates to.
    /// </summary>
    [Parameter]
    public string? BackUrl { get; set; }

    /// <summary>
    ///     The breadcrumb trail. When set, it replaces the back link - the second-to-last entry already is the way up, and
    ///     offering both means two controls doing the same thing.
    /// </summary>
    [Parameter]
    public IReadOnlyList<PageBreadcrumb>? Breadcrumbs { get; set; }

    /// <summary>
    ///     The page content rendered below the header.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    ///     The page title. Optional: a page with a richer heading of its own leaves it unset and uses the scaffold only for
    ///     the way back and the spacing.
    /// </summary>
    [Parameter]
    public string? Title { get; set; }

    /// <summary>
    ///     An optional icon shown in front of the title.
    /// </summary>
    [Parameter]
    public Icon? TitleIcon { get; set; }

    /// <summary>
    ///     Whatever belongs next to the title (a help button, a badge).
    /// </summary>
    [Parameter]
    public RenderFragment? TitleSuffix { get; set; }

    /// <summary>
    ///     The effective back-link label. Computed rather than defaulted into <see cref="BackLinkText" /> so the parameter
    ///     itself is never written to.
    /// </summary>
    private string BackText
        => string.IsNullOrWhiteSpace(BackLinkText) ? ComponentResources.BackToOverview : BackLinkText;

    private bool HasBackLink
        => !string.IsNullOrWhiteSpace(BackUrl);

    private bool HasBreadcrumbs
        => Breadcrumbs is { Count: > 0 };

    private bool HasHeaderRow
        => !string.IsNullOrWhiteSpace(Title) || Actions is { };

    [Inject]
    private NavigationManager NavigationManager { get; set; } = null!;

    #endregion

    #region "Methods"

    private void NavigateBack()
        => NavigationManager.NavigateTo(BackUrl!);

    #endregion
}
