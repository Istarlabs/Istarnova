#region "Header"

// File: NavigationMenuObject.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using Microsoft.AspNetCore.Components.Routing;
using Microsoft.FluentUI.AspNetCore.Components;

namespace Istarnova.Blazor.FluentUI.Navigation;

/// <summary>
///     A navigation menu entry with support for groups, roles and a policy.
/// </summary>
public sealed class NavigationMenuObject
{
    #region "Properties"

    /// <summary>
    ///     The roles as a comma-separated string for <c>AuthorizeView</c>, or <c>null</c> for none.
    /// </summary>
    public string? CombinedRoles
        => Roles.Count > 0 ? string.Join(",", Roles) : null;

    /// <summary>
    ///     A custom color as a hex value, where the icon is not to use <see cref="IconColor" />.
    /// </summary>
    public string? CustomColor { get; init; }

    public bool HasQueryParameters
        => QueryParameters.Count > 0;

    public bool HasSubItems
        => SubItems.Count > 0;

    /// <summary>
    ///     The icon of the entry.
    /// </summary>
    public Icon? Icon { get; init; }

    /// <summary>
    ///     The FluentUI color of the icon.
    /// </summary>
    public Color IconColor { get; init; } = Color.Primary;

    /// <summary>
    ///     How the address has to match for the entry to count as the current page.
    /// </summary>
    public NavLinkMatch Match { get; init; } = NavLinkMatch.Prefix;

    /// <summary>
    ///     The authorization policy the user has to satisfy for the entry to be shown. <c>null</c> means everyone who may
    ///     see the menu.
    /// </summary>
    public string? Policy { get; init; }

    public Dictionary<string, object> QueryParameters { get; init; } = [];

    /// <summary>
    ///     The roles that may see the entry.
    /// </summary>
    public List<string> Roles { get; init; } = [];

    /// <summary>
    ///     The entries of a group.
    /// </summary>
    public List<NavigationMenuObject> SubItems { get; init; } = [];

    /// <summary>
    ///     The display title. Comes from a resource file, never a literal.
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    ///     The address the entry navigates to. A group may carry one so it stays open on pages that have no entry of their own.
    /// </summary>
    public string? Url { get; init; }

    public string UrlWithQueryParameters
        => Url + BuildQueryParameters();

    #endregion

    #region "Methods"

    public void AddQueryParameter(string key, object value)
        => QueryParameters.Add(key, value);

    private string BuildQueryParameters()
        => QueryParameters.Count == 0
            ? string.Empty
            : "?" + string.Join("&", QueryParameters.Select(parameter => $"{parameter.Key}={parameter.Value}"));

    #endregion
}
