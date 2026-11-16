#region "Header"

// File: NavMenuEntry.razor.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using Microsoft.AspNetCore.Components;

namespace Istarnova.Blazor.FluentUI.Navigation;

public sealed partial class NavMenuEntry
{
    #region "Properties"

    [Parameter]
    [EditorRequired]
    public NavigationMenuObject Item { get; set; } = null!;

    [CascadingParameter]
    private NavMenuState State { get; set; } = null!;

    #endregion
}
