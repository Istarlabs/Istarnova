#region "Header"

// File: FluentCrudDialog.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using Istarnova.EfCore.Repositories;
using Microsoft.AspNetCore.Components;

namespace Istarnova.Blazor.FluentUI.Components;

/// <summary>
///     <see cref="FluentDialogBaseComponent{TComponent,TDialogComponent,TRepository,TKey,TEntity}" /> for a record with an
///     <see cref="int" /> key.
/// </summary>
public abstract class FluentCrudDialog<TComponent, TDialogComponent, TRepository, TEntity>
    : FluentDialogBaseComponent<TComponent, TDialogComponent, TRepository, int, TEntity>
    where TComponent : IComponent
    where TDialogComponent : ComponentBase
    where TRepository : IRepository<TEntity, int>
    where TEntity : class, new()
{
}
