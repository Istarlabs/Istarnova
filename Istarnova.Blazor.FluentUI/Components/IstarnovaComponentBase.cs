#region "Header"

// File: IstarnovaComponentBase.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using System.Text;
using Istarnova.Blazor.Errors;
using Istarnova.Blazor.FluentUI.Extensions;
using Istarnova.Blazor.Logging;
using Istarnova.Blazor.State;
using Istarnova.Extensions;
using Istarnova.IO.Mime;
using Istarnova.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Istarnova.Blazor.FluentUI.Components;

/// <summary>
///     What every page, tab and dialog of an application needs from the framework: a logger of its own type, toasts, dialogs,
///     navigation and the subscription to states. An application derives its own base from this one for what is specific to it
///     (its user state, its scope) and has its components inherit that.
/// </summary>
/// <typeparam name="TComponent">The component itself, for the logger category.</typeparam>
public abstract class IstarnovaComponentBase<TComponent> : ComponentBase where TComponent : IComponent
{
    #region "Variables"

    private readonly List<IObservableState> _subscribedStates = [];

    #endregion

    #region "Properties"

    [Inject]
    protected IDialogService DialogService { get; set; } = null!;

    [Inject]
    protected IJSRuntime JsRuntime { get; set; } = null!;

    [Inject]
    protected ILogger<TComponent> Logger { get; set; } = null!;

    [Inject]
    protected NavigationManager NavigationManager { get; set; } = null!;

    [Inject]
    protected INotificationService NotificationService { get; set; } = null!;

    /// <summary>
    ///     The application's own exceptions and the text to show for each; <c>null</c> for those it does not know.
    ///     Overridden once in the application's base class.
    /// </summary>
    protected virtual Func<Exception, string?>? ApplicationErrorMessage => null;

    #endregion

    #region "Methods"

    /// <summary>
    ///     Asks before removing a record, removes it, and reloads what showed it.
    /// </summary>
    /// <param name="message">The question, already formatted with whatever names the record.</param>
    /// <param name="title">Dialog title, and the confirming button's text unless one is given.</param>
    /// <param name="failureMessage">Shown when the delete returns false or throws.</param>
    /// <param name="deleteAsync">Removes the record; false means it did not go.</param>
    /// <param name="onDeletedAsync">Reloads the list, once the record is gone.</param>
    /// <param name="confirmText">The confirming button's text, where it is not the title.</param>
    protected Task ConfirmDeleteAsync(
        string message,
        string title,
        string failureMessage,
        Func<Task<bool>> deleteAsync,
        Func<Task> onDeletedAsync,
        string? confirmText = null)
        => ShowNestedConfirmationAsync(
            message,
            confirmText ?? title,
            DialogResources.Cancel,
            title,
            async result =>
            {
                if (!result.Cancelled)
                {
                    await RemoveAsync(failureMessage, deleteAsync, onDeletedAsync);
                }
            });

    /// <summary>
    ///     Logs an exception and returns the localized text to display in place of it, for a component that renders the
    ///     message inline (a dialog's error bar) rather than as a toast.
    /// </summary>
    protected string LogAndDescribeError(Exception exception, string fallbackMessage)
    {
        Logger.LogComponentFailure(exception, typeof(TComponent).Name);

        return UserFacingError.Describe(exception, fallbackMessage, ApplicationErrorMessage);
    }

    /// <summary>
    ///     Logs an error and optionally shows it as a toast.
    /// </summary>
    protected void LogAndShowError(Exception exception, string userMessage, bool showToUser = true)
    {
        Logger.LogComponentFailure(exception, typeof(TComponent).Name);

        if (showToUser)
        {
            ShowError(userMessage);
        }
    }

    /// <summary>
    ///     Navigates back in the browser history.
    /// </summary>
    protected async Task NavigateBackAsync()
    {
        try
        {
            await JsRuntime.InvokeVoidAsync("history.back");
        }
        catch (JSDisconnectedException)
        {
        }
    }

    /// <summary>
    ///     Navigates to the specified URI.
    /// </summary>
    protected void NavigateTo(string uri, bool forceLoad = false)
    {
        NavigationManager.NavigateTo(uri, forceLoad);
    }

    /// <summary>
    ///     Removes a record that was already confirmed, in place or in a dialog, and reloads what showed it.
    /// </summary>
    /// <param name="failureMessage">Shown when the delete returns false or throws.</param>
    /// <param name="deleteAsync">Removes the record; false means it did not go.</param>
    /// <param name="onDeletedAsync">Reloads the list, once the record is gone.</param>
    protected async Task RemoveAsync(string failureMessage, Func<Task<bool>> deleteAsync, Func<Task> onDeletedAsync)
    {
        try
        {
            if (await deleteAsync())
            {
                await onDeletedAsync();
            }
            else
            {
                ShowError(failureMessage);
            }
        }
        catch (Exception exception)
        {
            LogAndShowError(exception, failureMessage);
        }
    }

    /// <summary>
    ///     Shows an error toast.
    /// </summary>
    protected void ShowError(string message, string? title = null)
        => ObserveToast(NotificationService.ShowErrorToastAsync(title ?? message, title is null ? null : message));

    /// <summary>
    ///     Shows an error toast with a title and a message that stays for <paramref name="lifetimeSeconds" />.
    /// </summary>
    protected void ShowError(string title, string message, int? lifetimeSeconds)
        => ObserveToast(NotificationService.ShowErrorToastAsync(title, message, lifetimeSeconds));

    /// <summary>
    ///     Shows an info toast.
    /// </summary>
    protected void ShowInfo(string message, string? title = null)
        => ObserveToast(NotificationService.ShowInfoToastAsync(title ?? message, title is null ? null : message));

    /// <summary>
    ///     Shows an info toast with a title and a message that stays for <paramref name="lifetimeSeconds" />.
    /// </summary>
    protected void ShowInfo(string title, string message, int? lifetimeSeconds)
        => ObserveToast(NotificationService.ShowInfoToastAsync(title, message, lifetimeSeconds));

    /// <summary>
    ///     Shows a success toast.
    /// </summary>
    protected void ShowSuccess(string message, string? title = null)
        => ObserveToast(NotificationService.ShowSuccessToastAsync(title ?? message, title is null ? null : message));

    /// <summary>
    ///     Shows a success toast with a title and a message that stays for <paramref name="lifetimeSeconds" />.
    /// </summary>
    protected void ShowSuccess(string title, string message, int? lifetimeSeconds)
        => ObserveToast(NotificationService.ShowSuccessToastAsync(title, message, lifetimeSeconds));

    /// <summary>
    ///     Shows a warning toast.
    /// </summary>
    protected void ShowWarning(string message, string? title = null)
        => ObserveToast(NotificationService.ShowWarningToastAsync(title ?? message, title is null ? null : message));

    /// <summary>
    ///     Shows a warning toast with a title and a message that stays for <paramref name="lifetimeSeconds" />.
    /// </summary>
    protected void ShowWarning(string title, string message, int? lifetimeSeconds)
        => ObserveToast(NotificationService.ShowWarningToastAsync(title, message, lifetimeSeconds));

    /// <summary>
    ///     Shows a confirmation dialog; the buttons and the title are yes, no and "Confirmation" in the current language
    ///     unless the caller says otherwise.
    /// </summary>
    protected Task<DialogResult> ShowConfirmationAsync(
        string message,
        string? primaryText = null,
        string? secondaryText = null,
        string? title = null)
        => DialogService.ShowConfirmationAsync(
            message,
            title ?? DialogResources.ConfirmationTitle,
            primaryText ?? DialogResources.Yes,
            secondaryText ?? DialogResources.No);

    /// <summary>
    ///     A confirmation prompt, answered before the caller continues.
    /// </summary>
    protected async Task<DialogResult> ShowNestedConfirmationAsync(
        string message,
        string primaryText,
        string secondaryText,
        string? title = null,
        Func<DialogResult, Task>? onResult = null)
    {
        var result = await DialogService.ShowConfirmationAsync(
            message,
            title ?? DialogResources.ConfirmationTitle,
            primaryText,
            secondaryText);

        if (onResult is { })
        {
            await onResult(result);
        }

        return result;
    }

    /// <summary>
    ///     Opens a dialog and waits for its result.
    /// </summary>
    /// <typeparam name="TDialog">The dialog component to open.</typeparam>
    /// <param name="content">The model handed to the dialog as its <c>Content</c> parameter.</param>
    /// <param name="options">Title, size and buttons.</param>
    /// <param name="onResult">What to do with the result before the caller continues.</param>
    /// <returns>The dialog's result, cancelled or not.</returns>
    protected async Task<DialogResult> ShowNestedDialogAsync<TDialog>(
        object content,
        DialogOptions options,
        Func<DialogResult, Task>? onResult = null)
        where TDialog : ComponentBase
    {
        var result = await DialogService.ShowDialogAsync<TDialog>(content, options);

        if (onResult is { })
        {
            await onResult(result);
        }

        StateHasChanged();

        return result;
    }

    /// <summary>
    ///     Re-renders when a state changes, and runs <paramref name="reload" /> first when one is given.
    /// </summary>
    protected void SubscribeToState(IObservableState state, Func<Task>? reload = null)
    {
        ArgumentNullException.ThrowIfNull(state);

        _subscribedStates.Add(state);

        state.Subscribe(this, () => InvokeAsync(async () =>
        {
            if (reload is { })
            {
                await reload();
            }

            StateHasChanged();
        }));
    }

    /// <summary>
    ///     Stops following every state this component subscribed to.
    /// </summary>
    protected void UnsubscribeFromStates()
    {
        foreach (var state in _subscribedStates)
        {
            state.Unsubscribe(this);
        }

        _subscribedStates.Clear();
    }

    /// <summary>
    ///     A toast is shown without the caller waiting for it to close, so the task is only watched for a failure.
    /// </summary>
    private void ObserveToast(Task toast)
        => _ = toast.ContinueWith(
            task => Logger.LogComponentFailure(task.Exception!, typeof(TComponent).Name),
            TaskContinuationOptions.OnlyOnFaulted);

    /// <summary>
    ///     Hands bytes to the browser as a download, through the application's <c>downloadFileFromUrl(fileName, dataUrl)</c>
    ///     script function.
    /// </summary>
    /// <param name="fileName">The name the browser offers to save the file under.</param>
    /// <param name="data">The file content.</param>
    /// <param name="mediaType">The media type of the content, for example <c>application/pdf</c>.</param>
    protected async Task DownloadFileFromUrlAsync(string fileName, byte[] data, string mediaType)
    {
        try
        {
            await JsRuntime.InvokeVoidAsync("downloadFileFromUrl", fileName, data.ToDataUrl(mediaType));
        }
        catch (JSDisconnectedException)
        {
        }
    }

    /// <summary>
    ///     Hands text, encoded as UTF-8, to the browser as a download. See the overload for bytes.
    /// </summary>
    /// <param name="fileName">The name the browser offers to save the file under.</param>
    /// <param name="data">The file content.</param>
    /// <param name="mediaType">The media type of the content, for example <c>text/csv</c>.</param>
    protected async Task DownloadFileFromUrlAsync(string fileName, string data, string mediaType)
    {
        var bytes = Encoding.UTF8.GetBytes(data);

        try
        {
            await JsRuntime.InvokeVoidAsync("downloadFileFromUrl", fileName, bytes.ToDataUrl(mediaType));
        }
        catch (JSDisconnectedException)
        {
        }
    }

    #endregion
}
