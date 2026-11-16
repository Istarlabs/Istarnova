#region "Header"

// File: FluentDialogBaseComponent.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using System.Linq.Expressions;
using System.Reflection;
using Istarnova.Blazor.FluentUI.Models;
using Istarnova.Blazor.Logging;
using Istarnova.EfCore.Extensions;
using Istarnova.EfCore.Repositories;
using Istarnova.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.FluentUI.AspNetCore.Components;

namespace Istarnova.Blazor.FluentUI.Components;

/// <summary>
///     Base class for a page that creates, edits and deletes records through dialogs and the record's repository. The dialog
///     returns a <see cref="FluentDialogResult{T}" />; this class persists it and reports the outcome.
/// </summary>
/// <typeparam name="TComponent">The page itself, for the logger category.</typeparam>
/// <typeparam name="TDialogComponent">The dialog that edits one record.</typeparam>
/// <typeparam name="TRepository">The repository to inject.</typeparam>
/// <typeparam name="TKey">The record's key type.</typeparam>
/// <typeparam name="TEntity">The record.</typeparam>
public abstract class FluentDialogBaseComponent<TComponent, TDialogComponent, TRepository, TKey, TEntity>
    : IstarnovaComponentBase<TComponent>
    where TComponent : IComponent
    where TDialogComponent : ComponentBase
    where TRepository : IRepository<TEntity, TKey>
    where TEntity : class, new()
{
    #region "Fields"

    /// <summary>
    ///     The property holding the record's key, by the convention <c>{TypeName}Id</c> and then <c>Id</c>, or <c>null</c>
    ///     when it follows neither.
    /// </summary>
    private static readonly PropertyInfo? KeyProperty =
        typeof(TEntity).GetProperty($"{typeof(TEntity).Name}Id") ?? typeof(TEntity).GetProperty("Id");

    #endregion

    #region "Properties"

    /// <summary>
    ///     Height of the dialog, or <c>null</c> for the FluentUI default. FluentUI applies this as a fixed height, so a form
    ///     that can grow needs a scrolling body rather than a taller value.
    /// </summary>
    protected virtual string? DialogHeight => null;

    protected FluentDialogMessageDictionary DialogMessages { get; set; } = new(typeof(TEntity).Name);

    /// <summary>
    ///     Width of the dialog, or <c>null</c> for the FluentUI default.
    /// </summary>
    protected virtual string? DialogWidth => null;

    protected Type EntityType => typeof(TEntity);

    /// <summary>
    ///     Called after a successful create, update or delete, to refresh what showed the record.
    /// </summary>
    protected EventCallback<FluentDialogResult<TEntity>> OnActionExecuted { get; set; }

    [Inject]
    protected TRepository Repository { get; set; } = default!;

    #endregion

    #region "Methods"

    /// <summary>
    ///     Shows a confirmation dialog and deletes the record if confirmed.
    /// </summary>
    /// <param name="id">The record's key.</param>
    /// <param name="additionalWarning">What else goes with it, appended to the confirmation; none by default.</param>
    public async Task ConfirmDeleteAsync(TKey id, string? additionalWarning = null)
    {
        var result = await ShowNestedConfirmationAsync(
            string.IsNullOrEmpty(additionalWarning)
                ? DialogMessages.DeleteConfirmMessage
                : $"{DialogMessages.DeleteConfirmMessage} {additionalWarning}",
            DialogResources.Delete,
            DialogResources.No,
            DialogMessages.DeleteTitle);

        if (!result.Cancelled)
        {
            if (!await Repository.DeleteByIdAsync(id))
            {
                ShowError(DialogMessages.DeleteErrorMessage);

                return;
            }

            ShowSuccess(DialogMessages.DeleteSuccessMessage);
            await RefreshAsync(new()
            {
                Result = FluentDialogResult.OkDelete,
            });
        }
        else
        {
            ShowWarning(DialogMessages.DeleteCancelledMessage);
        }
    }

    public async Task OpenAddNewDialogAsync()
    {
        var result = await ShowNestedDialogAsync<TDialogComponent>(
            new TEntity(),
            CreateDialogOptions(DialogMessages.AddNewTitle, DialogResources.Add));

        if (result is { Cancelled: false, Value: FluentDialogResult<TEntity> { Data: { } data } dialogResult })
        {
            if (dialogResult.Result.IsOkCreate())
            {
                await Repository.CreateAsync(data);
                await OnEntitySavedAsync(data);
                ShowSuccess(DialogMessages.AddNewSuccessMessage);
                await RefreshAsync(dialogResult);
            }
            else if (dialogResult.Result.IsErrorCreate())
            {
                ShowError(DialogMessages.AddNewErrorMessage);
            }
        }
        else if (result.Cancelled)
        {
            ShowInfo(DialogMessages.AddNewCancelledMessage);
        }
    }

    /// <summary>
    ///     Opens a dialog to update an existing record.
    /// </summary>
    /// <param name="id">The record's key.</param>
    /// <param name="configureQuery">Adds the <c>Include</c>s the dialog needs, when the repository can be queried.</param>
    public async Task OpenUpdateDialogAsync(TKey id, Func<IQueryable<TEntity>, IQueryable<TEntity>>? configureQuery = null)
    {
        TEntity? entity = null;

        if (configureQuery is { } && Repository is IQueryableRepository<TEntity> queryableRepository)
        {
            var (context, query) = queryableRepository.CreateQuery();

            try
            {
                query = configureQuery(query);

                if (KeyProperty is null)
                {
                    Logger.LogComponentIdUnreadable(typeof(TEntity).Name);
                    ShowError(DialogMessages.ItemNotFoundMessage);

                    return;
                }

                var parameter = Expression.Parameter(typeof(TEntity), "x");
                var equal = Expression.Equal(Expression.Property(parameter, KeyProperty), QueryValue.Captured(id));
                var predicate = Expression.Lambda<Func<TEntity, bool>>(equal, parameter);

                entity = await query.Where(predicate).FirstOrDefaultAsync();
            }
            finally
            {
                context.Dispose();
            }
        }
        else
        {
            entity = await Repository.GetAsync(id);
        }

        if (entity is null)
        {
            ShowError(DialogMessages.ItemNotFoundMessage);

            return;
        }

        var result = await ShowNestedDialogAsync<TDialogComponent>(
            entity,
            CreateDialogOptions(DialogMessages.EditTitle, DialogResources.Save));

        if (result is { Cancelled: false, Value: FluentDialogResult<TEntity> { Data: { } data } dialogResult })
        {
            if (dialogResult.Result.IsOkUpdate())
            {
                await Repository.UpdateAsync(data);
                await OnEntitySavedAsync(data);
                ShowSuccess(DialogMessages.EditSuccessMessage);
                await RefreshAsync(dialogResult);
            }
            else if (dialogResult.Result.IsOkCreate())
            {
                await Repository.CreateAsync(data);
                await OnEntitySavedAsync(data);
                ShowSuccess(DialogMessages.AddNewSuccessMessage);
                await RefreshAsync(dialogResult);
            }
            else if (dialogResult.Result.IsErrorUpdate())
            {
                ShowError(DialogMessages.EditErrorMessage);
            }
        }
        else if (result.Cancelled)
        {
            ShowInfo(DialogMessages.EditCancelledMessage);
        }
    }

    /// <summary>
    ///     Runs once the repository has created or updated the record, before the success message. An application overrides
    ///     it for what has to be written with the record's key (a related set the dialog collected).
    /// </summary>
    protected virtual Task OnEntitySavedAsync(TEntity entity)
        => Task.CompletedTask;

    /// <summary>
    ///     Reads the record's primary key.
    /// </summary>
    /// <returns>
    ///     False when the record carries neither <c>{TypeName}Id</c> nor <c>Id</c>, or the property is not of
    ///     <typeparamref name="TKey" />. Callers report that rather than acting on a default, which for a numeric key would
    ///     be a valid-looking 0.
    /// </returns>
    protected static bool TryGetKey(TEntity entity, out TKey key)
    {
        if (KeyProperty?.GetValue(entity) is TKey found)
        {
            key = found;

            return true;
        }

        key = default!;

        return false;
    }

    private DialogOptions CreateDialogOptions(string title, string primaryLabel)
        => new()
        {
            Header = { Title = title },
            Footer =
            {
                PrimaryAction = { Label = primaryLabel },
                SecondaryAction = { Label = DialogResources.Cancel },
            },
            Width = DialogWidth,
            Height = DialogHeight,
        };

    private Task RefreshAsync(FluentDialogResult<TEntity>? data)
        => OnActionExecuted.HasDelegate && data is { } ? OnActionExecuted.InvokeAsync(data) : Task.CompletedTask;

    #endregion
}
