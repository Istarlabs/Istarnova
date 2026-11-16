#region "Header"

// File: NotificationServiceExtensions.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using Microsoft.FluentUI.AspNetCore.Components;

namespace Istarnova.Blazor.FluentUI.Extensions;

/// <summary>
///     Toasts for the components that do not derive from a component base class, shown without waiting for them to close.
/// </summary>
public static class NotificationServiceExtensions
{
    #region "Methods"

    public static void ShowError(this INotificationService service, string message)
        => _ = service.ShowErrorToastAsync(message, null);

    public static void ShowInfo(this INotificationService service, string message)
        => _ = service.ShowInfoToastAsync(message, null);

    public static void ShowSuccess(this INotificationService service, string message)
        => _ = service.ShowSuccessToastAsync(message, null);

    public static void ShowWarning(this INotificationService service, string message)
        => _ = service.ShowWarningToastAsync(message, null);

    #endregion
}
