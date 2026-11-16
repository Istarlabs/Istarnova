#region "Header"

// File: ISearchQueryableRepository.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

namespace Istarnova.EfCore.Repositories;

/// <summary>
///     Search that stays a query, for a list that pages and filters further.
/// </summary>
public interface ISearchQueryableRepository<T> where T : class
{
    #region "Properties"

    /// <summary>
    ///     The records matching a term, as a query on a fresh context the caller has to dispose - the same contract as
    ///     <see cref="IQueryableRepository{T}.CreateQuery" />, with a term.
    /// </summary>
    Func<string, (IDisposable Context, IQueryable<T> Query)> CreateSearchQuery { get; }

    #endregion
}
