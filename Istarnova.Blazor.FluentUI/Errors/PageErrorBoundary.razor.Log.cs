#region "Header"

// File: PageErrorBoundary.razor.Log.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using Microsoft.Extensions.Logging;

namespace Istarnova.Blazor.FluentUI.Errors;

public sealed partial class PageErrorBoundary
{
    #region "Methods"

    [LoggerMessage(EventId = 6030, Level = LogLevel.Error, Message = "Unhandled error {errorReference} on {path}")]
    private static partial void LogUnhandledComponentError(ILogger logger, Exception exception, string errorReference, string path);

    #endregion
}
