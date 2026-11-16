#region "Header"

// File: NavMenuNode.razor.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using Microsoft.AspNetCore.Components;

namespace Istarnova.Blazor.FluentUI.Navigation;

/// <summary>
///     One menu entry at any depth, behind the authorization it asks for.
/// </summary>
public sealed partial class NavMenuNode
{
    #region "Properties"

    [Parameter]
    [EditorRequired]
    public NavigationMenuObject Item { get; set; } = null!;

    private bool IsRestricted
        => !string.IsNullOrEmpty(Item.Policy) || !string.IsNullOrEmpty(Item.CombinedRoles);

    #endregion
}
