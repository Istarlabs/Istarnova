#region "Header"

// File: CrudComponentBase.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using Istarnova.Blazor.Components;
using Istarnova.Blazor.FluentUI.Models;
using Istarnova.Blazor.Logging;
using Istarnova.EfCore.Repositories;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

namespace Istarnova.Blazor.FluentUI.Components;

/// <summary>
///     Base class for a list page with create, edit and delete: loads the list from the repository, and reloads it after the
///     dialog has changed something.
/// </summary>
/// <typeparam name="TComponent">The page itself, for the logger category.</typeparam>
/// <typeparam name="TEntity">The record being managed.</typeparam>
/// <typeparam name="TKey">The record's key type.</typeparam>
/// <typeparam name="TDialog">The dialog that creates and edits one record.</typeparam>
public abstract class CrudComponentBase<TComponent, TEntity, TKey, TDialog> :
    FluentDialogBaseComponent<TComponent, TDialog, IRepository<TEntity, TKey>, TKey, TEntity>, IDisposable
    where TComponent : IComponent
    where TEntity : class, new()
    where TDialog : ComponentBase
{
    #region "Variables"

    /// <summary>
    ///     Releases the context the current query reads from.
    /// </summary>
    private IDisposable? _queryContext;

    #endregion

    #region "Properties"

    /// <summary>
    ///     What the record is called to a person ("Doctor"); it names the record in the dialogs and toasts.
    /// </summary>
    protected abstract string EntityDisplayName { get; }

    /// <summary>
    ///     The grid, on a page that lets it page and search itself. Set from the markup with <c>@ref</c>; null on a page that
    ///     still binds <see cref="Items" />.
    /// </summary>
    protected IRefreshableList? Grid { get; set; }

    /// <summary>
    ///     The rows the data grid shows.
    /// </summary>
    protected IQueryable<TEntity>? Items { get; set; }

    /// <summary>
    ///     Pagination state for a grid that does not page itself.
    /// </summary>
    protected PaginationState Pagination { get; set; } = new() { ItemsPerPage = 10 };

    /// <summary>
    ///     The repository as something a grid can page and search itself.
    /// </summary>
    /// <exception cref="InvalidCastException">
    ///     The repository cannot be queried. Every CRUD page's can - the list would have nothing to render otherwise - so this
    ///     is a wiring mistake and not a state to handle.
    /// </exception>
    protected IQueryableRepository<TEntity> QueryableRepository => (IQueryableRepository<TEntity>)Repository;

    /// <summary>
    ///     What the page's search box holds, or null on a page without one.
    /// </summary>
    protected virtual string? SearchTerm => null;

    #endregion

    #region "Methods"

    public void Dispose()
    {
        _queryContext?.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Orders the query. Override to customize.
    /// </summary>
    protected virtual IQueryable<TEntity> ApplyDefaultOrdering(IQueryable<TEntity> query)
        => query;

    /// <summary>
    ///     Deletes a record after a confirmation.
    /// </summary>
    protected virtual Task DeleteEntityAsync(TEntity entity)
        => DeleteEntityAsync(entity, null);

    /// <summary>
    ///     Deletes a record after a confirmation, naming <paramref name="additionalWarning" /> in it when one is given.
    /// </summary>
    protected Task DeleteEntityAsync(TEntity entity, string? additionalWarning)
    {
        OnActionExecuted = EventCallback.Factory.Create<FluentDialogResult<TEntity>>(this, OnDialogActionCompletedAsync);

        if (TryGetKey(entity, out var id))
        {
            return ConfirmDeleteAsync(id, additionalWarning);
        }

        Logger.LogComponentIdUnreadable(typeof(TEntity).Name);

        return Task.CompletedTask;
    }

    /// <summary>
    ///     The display name of one record, for a message that names it.
    /// </summary>
    protected abstract string GetEntityName(TEntity entity);

    /// <summary>
    ///     Loads the records from the repository. Override to customize.
    /// </summary>
    protected virtual void LoadEntities()
    {
        // A page that offers a search overrides SearchTerm; what a term matches is the repository's business, so the page
        // never spells it out.
        if (!string.IsNullOrWhiteSpace(SearchTerm) && Repository is ISearchQueryableRepository<TEntity> searchableRepository)
        {
            var (searchContext, searchQuery) = searchableRepository.CreateSearchQuery(SearchTerm);
            _queryContext?.Dispose();
            _queryContext = searchContext;
            Items = ApplyDefaultOrdering(searchQuery);

            return;
        }

        if (Repository is IQueryableRepository<TEntity> queryableRepository)
        {
            var (context, query) = queryableRepository.CreateQuery();
            _queryContext?.Dispose();
            _queryContext = context;
            Items = ApplyDefaultOrdering(query);
        }
        else
        {
            Logger.LogComponentRepositoryNotQueryable(typeof(TEntity).Name);
        }
    }

    /// <summary>
    ///     Called when a dialog action is completed. A page whose grid fetches its own pages overrides this and refreshes that
    ///     grid instead.
    /// </summary>
    protected virtual Task OnDialogActionCompletedAsync(FluentDialogResult<TEntity> result)
        => RefreshListAsync();

    protected override void OnInitialized()
    {
        base.OnInitialized();

        OnActionExecuted = EventCallback.Factory.Create<FluentDialogResult<TEntity>>(this, OnDialogActionCompletedAsync);
        DialogMessages = new(EntityDisplayName);

        LoadEntities();
    }

    /// <summary>
    ///     Opens the create dialog.
    /// </summary>
    protected virtual Task OpenCreateDialogAsync()
    {
        OnActionExecuted = EventCallback.Factory.Create<FluentDialogResult<TEntity>>(this, OnDialogActionCompletedAsync);

        return OpenAddNewDialogAsync();
    }

    /// <summary>
    ///     Opens the edit dialog.
    /// </summary>
    protected virtual Task OpenEditDialogAsync(TEntity entity)
        => OpenEditDialogAsync(entity, null);

    /// <summary>
    ///     Opens the edit dialog, with an optional query configurator that adds the <c>Include</c>s the dialog needs:
    ///     <c>query => query.Include(x => x.Relation).ThenInclude(...)</c>.
    /// </summary>
    /// <param name="entity">The record (its key is read from it).</param>
    /// <param name="configureQuery">Optional query customization to include related records.</param>
    protected virtual Task OpenEditDialogAsync(TEntity entity, Func<IQueryable<TEntity>, IQueryable<TEntity>>? configureQuery)
    {
        OnActionExecuted = EventCallback.Factory.Create<FluentDialogResult<TEntity>>(this, OnDialogActionCompletedAsync);

        if (TryGetKey(entity, out var id))
        {
            return OpenUpdateDialogAsync(id, configureQuery);
        }

        Logger.LogComponentIdUnreadable(typeof(TEntity).Name);

        return Task.CompletedTask;
    }

    /// <summary>
    ///     Reads the list again, after a change this page made outside the create/edit dialog.
    /// </summary>
    protected Task RefreshListAsync()
    {
        if (Grid is { })
        {
            return Grid.RefreshAsync();
        }

        LoadEntities();
        StateHasChanged();

        return Task.CompletedTask;
    }

    /// <summary>
    ///     Rebuilds the query and redraws; what a filter or search box binds to with <c>@bind-Value:after</c>.
    /// </summary>
    protected Task ReloadAsync()
    {
        LoadEntities();
        StateHasChanged();

        return Task.CompletedTask;
    }

    #endregion
}
