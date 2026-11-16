#region "Header"

// File: FileDropZone.razor.cs
// Created 2026/10/28
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

#region "Usings"

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.JSInterop;

#endregion

namespace Istarnova.Blazor.FluentUI.Components;

/// <summary>
///     Takes one file: dropped onto it, chosen by clicking it and, when <see cref="AllowPaste" /> is set, pasted with Ctrl+V
///     anywhere on the page. The file is handed on as it came; checking its type and size and reading it is up to the caller.
/// </summary>
public sealed partial class FileDropZone : IAsyncDisposable
{
    #region "Variables"

    private InputFile? _input;
    private bool _isBusy;
    private IJSObjectReference? _module;
    private IJSObjectReference? _pasteListener;

    #endregion

    #region "Properties"

    /// <summary>
    ///     The value of the input's <c>accept</c> attribute: mime types, wildcards such as <c>image/*</c> and extensions.
    ///     A pasted file has to match it as well.
    /// </summary>
    [Parameter]
    public string? Accept { get; set; }

    /// <summary>
    ///     Whether a file can also be pasted with Ctrl+V anywhere on the page. Off by default, because a page with two
    ///     drop zones would both take the same paste.
    /// </summary>
    [Parameter]
    public bool AllowPaste { get; set; }

    /// <summary>
    ///     Switches the zone off, which also dims it.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    ///     The icon above the text; <see langword="null" /> shows none.
    /// </summary>
    [Parameter]
    public Icon? Icon { get; set; } = new Microsoft.FluentUI.AspNetCore.Components.Icons.Regular.Size24.DocumentArrowUp();

    [Inject]
    private IJSRuntime JsRuntime { get; init; } = null!;

    /// <summary>
    ///     Raised with the file chosen, dropped or pasted. The zone is busy until the handler is done.
    /// </summary>
    [Parameter]
    public EventCallback<IBrowserFile> OnFile { get; set; }

    /// <summary>
    ///     Whether a spinner shows and the zone refuses another file while <see cref="OnFile" /> runs.
    /// </summary>
    [Parameter]
    public bool ShowBusy { get; set; } = true;

    /// <summary>
    ///     The hint inside the zone.
    /// </summary>
    [Parameter]
    [EditorRequired]
    public string Text { get; set; } = string.Empty;

    private bool IsDisabled => Disabled || (ShowBusy && _isBusy);

    #endregion

    #region "Methods"

    public async ValueTask DisposeAsync()
    {
        try
        {
            if (_pasteListener is { })
            {
                await _pasteListener.InvokeVoidAsync("dispose");
                await _pasteListener.DisposeAsync();
            }

            if (_module is { })
            {
                await _module.DisposeAsync();
            }
        }
        catch (JSDisconnectedException)
        {
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender || !AllowPaste || _input?.Element is not { } element)
        {
            return;
        }

        try
        {
            _module = await JsRuntime.InvokeAsync<IJSObjectReference>("import",
                "./_content/Istarnova.Blazor.FluentUI/Components/FileDropZone.razor.js");
            _pasteListener = await _module.InvokeAsync<IJSObjectReference>("attachPaste", element, Accept);
        }
        catch (JSDisconnectedException)
        {
        }
    }

    private async Task OnChangeAsync(InputFileChangeEventArgs change)
    {
        _isBusy = true;

        try
        {
            await OnFile.InvokeAsync(change.File);
        }
        finally
        {
            _isBusy = false;
        }
    }

    #endregion
}
