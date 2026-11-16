#region "Header"

// File: IRepository.TwoKeys.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

namespace Istarnova.EfCore.Repositories;

/// <summary>
///     A repository whose entities are identified by a key of two parts.
/// </summary>
public interface IRepository<T, in TIdentity1, in TIdentity2> : IBaseRepository<T>
{
    #region "Properties"

    /// <summary>
    ///     The entity with the key, or <c>null</c> if there is none.
    /// </summary>
    T? this[TIdentity1 key1, TIdentity2 key2] { get; }

    #endregion

    #region "Methods"

    /// <summary>
    ///     Deletes the entity with the key; whether there was one.
    /// </summary>
    Task<bool> DeleteByIdAsync(TIdentity1 key1, TIdentity2 key2, CancellationToken cancellationToken = default);

    /// <summary>
    ///     The entity with the key, or <c>null</c> if there is none.
    /// </summary>
    Task<T?> GetAsync(TIdentity1 key1, TIdentity2 key2, CancellationToken cancellationToken = default);

    #endregion
}
