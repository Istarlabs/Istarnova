#region "Header"

// File: GridPageMemory.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

namespace Istarnova.Blazor.FluentUI.Grids;

/// <summary>
///     The page each data grid was left on, for as long as the circuit lives, so coming back from a detail page lands
///     where the list was left rather than on its first page.
/// </summary>
public sealed class GridPageMemory
{
    #region "Variables"

    private readonly Dictionary<string, int> _pages = [];

    #endregion

    #region "Methods"

    public void Remember(string key, int pageIndex)
        => _pages[key] = pageIndex;

    public int Recall(string key)
        => _pages.GetValueOrDefault(key);

    #endregion
}
