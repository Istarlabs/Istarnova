#region "Header"

// File: DialogServiceExtensions.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

namespace Istarnova.Blazor.FluentUI.Extensions;

/// <summary>
///     Opens a dialog the way every dialog is written: one model, handed over as its <c>Content</c> parameter.
/// </summary>
public static class DialogServiceExtensions
{
    #region "Methods"

    public static Task<DialogResult> ShowDialogAsync<TDialog>(this IDialogService service, object content, DialogOptions options)
        where TDialog : ComponentBase
    {
        options.Parameters["Content"] = content;

        // A modal dialog is positioned between its top and bottom insets: "auto" stretches it over the whole viewport,
        // and only fit-content sizes it to its content.
        if (options.Height == "auto")
        {
            options.Height = "fit-content";
        }

        return service.ShowDialogAsync<TDialog>(options);
    }

    #endregion
}
