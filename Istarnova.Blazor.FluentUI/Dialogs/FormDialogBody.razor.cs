#region "Header"

// File: FormDialogBody.razor.cs
// Created 2026/10/30
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

#region "Usings"

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.FluentUI.AspNetCore.Components;
using Istarnova.Resources;

#endregion

namespace Istarnova.Blazor.FluentUI.Dialogs;

/// <summary>
///     The body of a dialog that edits one model: title with icon, the form, an error line and the save / cancel footer.
///     The dialog hands over its fields and what saving does; closing the dialog stays with it.
/// </summary>
public sealed partial class FormDialogBody
{
    #region "Variables"

    private EditContext? _editContext;
    private bool _isSaving;

    #endregion

    #region "Properties"

    /// <summary>Whether Save is on offer. A dialog that cannot save yet (a name still empty) says so here instead of letting the click do nothing.</summary>
    [Parameter]
    public bool CanSave { get; set; } = true;

    /// <summary>The fields, laid out inside the form grid.</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [CascadingParameter]
    public IDialogInstance Dialog { get; set; } = null!;

    /// <summary>Shown under the form, for a failure the model's own validation does not know.</summary>
    [Parameter]
    public string? ErrorMessage { get; set; }

    [Parameter]
    public Icon? Icon { get; set; }

    /// <summary>The model the form edits. A new instance builds a new <see cref="EditContext" />.</summary>
    [Parameter]
    [EditorRequired]
    public object Model { get; set; } = null!;

    /// <summary>Runs once the model validates. It closes the dialog itself; the footer is locked until it returns.</summary>
    [Parameter]
    [EditorRequired]
    public EventCallback OnSave { get; set; }

    [Parameter]
    public bool ShowValidationSummary { get; set; }

    /// <summary>The header title the dialog was opened with, unless one is given here.</summary>
    [Parameter]
    public string? Title { get; set; }

    private string ResolvedTitle
        => Title ?? Dialog.Options.Header.Title ?? string.Empty;

    #endregion

    #region "Methods"

    protected override void OnParametersSet()
    {
        if (_editContext?.Model != Model)
        {
            _editContext = new(Model);
        }
    }

    private Task CancelAsync()
        => Dialog.CancelAsync();

    private async Task SaveAsync()
    {
        if (_editContext is null || _isSaving)
        {
            return;
        }

        _isSaving = true;

        try
        {
            if (!await _editContext.ValidateAsync())
            {
                return;
            }

            await OnSave.InvokeAsync();
        }
        finally
        {
            _isSaving = false;
        }
    }

    #endregion
}
