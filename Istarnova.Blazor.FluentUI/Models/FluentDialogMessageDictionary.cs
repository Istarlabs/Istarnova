#region "Header"

// File: FluentDialogMessageDictionary.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using Istarnova.Resources;

namespace Istarnova.Blazor.FluentUI.Models;

/// <summary>
///     The texts of a create, edit and delete flow for one kind of record, in the current language. Those that name the record
///     carry its display name; a page overrides a single text by assigning the property.
/// </summary>
public sealed class FluentDialogMessageDictionary
{
    #region "Constructor"

    /// <param name="entity">What the record is called to a person ("Doctor", "Wish banner").</param>
    public FluentDialogMessageDictionary(string entity)
    {
        AddNewTitle = string.Format(DialogResources.AddNewTitle, entity);
        AddNewSuccessMessage = string.Format(DialogResources.AddNewSuccessMessage, entity);
        AddNewErrorMessage = string.Format(DialogResources.AddNewErrorMessage, entity);
        AddNewCancelledMessage = DialogResources.AddNewCancelledMessage;

        EditTitle = string.Format(DialogResources.EditTitle, entity);
        EditSuccessMessage = string.Format(DialogResources.EditSuccessMessage, entity);
        EditErrorMessage = string.Format(DialogResources.EditErrorMessage, entity);
        EditCancelledMessage = DialogResources.EditCancelledMessage;

        DeleteTitle = string.Format(DialogResources.DeleteTitle, entity);
        DeleteSuccessMessage = string.Format(DialogResources.DeleteSuccessMessage, entity);
        DeleteErrorMessage = string.Format(DialogResources.DeleteErrorMessage, entity);
        DeleteCancelledMessage = DialogResources.DeleteCancelledMessage;
        DeleteConfirmMessage = string.Format(DialogResources.DeleteConfirmMessage, entity);

        ItemNotFoundMessage = DialogResources.ItemNotFoundMessage;
    }

    #endregion

    #region "Properties"

    public string AddNewCancelledMessage { get; set; }

    public string AddNewErrorMessage { get; set; }

    public string AddNewSuccessMessage { get; set; }

    public string AddNewTitle { get; set; }

    public string DeleteCancelledMessage { get; set; }

    public string DeleteConfirmMessage { get; set; }

    public string DeleteErrorMessage { get; set; }

    public string DeleteSuccessMessage { get; set; }

    public string DeleteTitle { get; set; }

    public string EditCancelledMessage { get; set; }

    public string EditErrorMessage { get; set; }

    public string EditSuccessMessage { get; set; }

    public string EditTitle { get; set; }

    public string ItemNotFoundMessage { get; set; }

    #endregion
}
