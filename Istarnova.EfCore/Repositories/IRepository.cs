#region "Header"

// File: IRepository.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

namespace Istarnova.EfCore.Repositories;

/// <summary>
///     A repository whose entities are identified by one key.
/// </summary>
/// <typeparam name="T">The type of the entity managed by the repository.</typeparam>
/// <typeparam name="TIdentity">The type of the key.</typeparam>
public interface IRepository<T, in TIdentity> : IBaseRepository<T>
{
    #region "Properties"

    /// <summary>
    ///     The entity with the key, or <c>null</c> if there is none.
    /// </summary>
    T? this[TIdentity id] { get; }

    #endregion

    #region "Methods"

    /// <summary>
    ///     Deletes the entity with the key; whether there was one.
    /// </summary>
    Task<bool> DeleteByIdAsync(TIdentity id, CancellationToken cancellationToken = default);

    /// <summary>
    ///     The entity with the key, or <c>null</c> if there is none.
    /// </summary>
    Task<T?> GetAsync(TIdentity id, CancellationToken cancellationToken = default);

    #endregion
}
