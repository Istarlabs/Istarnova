#region "Header"

// File: ISearchable.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

namespace Istarnova.EfCore.Repositories;

/// <summary>
///     Search that returns a finished list, for a picker.
/// </summary>
public interface ISearchable<TResult> where TResult : new()
{
    #region "Methods"

    Task<List<TResult>> SearchAsync(string term, CancellationToken cancellationToken = default);

    #endregion
}
