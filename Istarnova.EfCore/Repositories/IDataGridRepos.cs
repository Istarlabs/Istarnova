#region "Header"

// File: IDataGridRepos.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

namespace Istarnova.EfCore.Repositories;

/// <summary>
///     A repository that delivers the rows of a grid as a projection instead of entities.
/// </summary>
public interface IDataGridRepos<T> where T : new()
{
    #region "Methods"

    Task<List<T>> GetAllAsDtoAsync();

    #endregion
}
