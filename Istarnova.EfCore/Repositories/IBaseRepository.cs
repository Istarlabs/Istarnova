#region "Header"

// File: IBaseRepository.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

namespace Istarnova.EfCore.Repositories;

/// <summary>
///     The operations every repository has, whatever its key looks like.
/// </summary>
/// <typeparam name="T">The type of the entity that the repository manages.</typeparam>
public interface IBaseRepository<T>
{
    #region "Properties"

    /// <summary>
    ///     The total number of items in the repository.
    /// </summary>
    int Count { get; }

    #endregion

    #region "Methods"

    /// <summary>
    ///     Adds a new item.
    /// </summary>
    Task<bool> CreateAsync(T item, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes the item.
    /// </summary>
    Task<bool> DeleteAsync(T item, CancellationToken cancellationToken = default);

    /// <summary>
    ///     All items.
    /// </summary>
    Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Updates the item.
    /// </summary>
    Task<bool> UpdateAsync(T item, CancellationToken cancellationToken = default);

    #endregion
}
