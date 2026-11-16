#region "Header"

// File: IQueryableRepository.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

namespace Istarnova.EfCore.Repositories;

/// <summary>
///     A repository that hands out a query for a list that pages and filters further.
/// </summary>
public interface IQueryableRepository<T> where T : class
{
    #region "Properties"

    /// <summary>
    ///     Creates a new context and returns a query on it. The caller disposes the context.
    /// </summary>
    Func<(IDisposable Context, IQueryable<T> Query)> CreateQuery { get; }

    #endregion
}
