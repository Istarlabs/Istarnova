#region "Header"

// File: IRefreshableList.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

namespace Istarnova.Blazor.Components;

/// <summary>
///     A list that fetches its own pages and can be told to fetch the current one again.
/// </summary>
public interface IRefreshableList
{
    #region "Methods"

    /// <summary>
    ///     Loads the page currently shown again, after something outside changed the rows.
    /// </summary>
    Task RefreshAsync();

    #endregion
}
