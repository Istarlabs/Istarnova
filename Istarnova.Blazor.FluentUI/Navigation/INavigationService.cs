#region "Header"

// File: INavigationService.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

namespace Istarnova.Blazor.FluentUI.Navigation;

/// <summary>
///     Builds the menu of an application. The library renders it, the application decides what is in it.
/// </summary>
public interface INavigationService
{
    #region "Methods"

    /// <summary>
    ///     The top-level entries of the main menu.
    /// </summary>
    IEnumerable<NavigationMenuObject> BuildMainMenu();

    #endregion
}
