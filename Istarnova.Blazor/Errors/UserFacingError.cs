#region "Header"

// File: UserFacingError.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using Istarnova.Resources;
using Microsoft.EntityFrameworkCore;

namespace Istarnova.Blazor.Errors;

/// <summary>
///     Turns an exception into text that is worth putting in front of a user.
/// </summary>
public static class UserFacingError
{
    #region "Methods"

    /// <summary>
    ///     The localized message to show for an exception.
    /// </summary>
    /// <param name="exception">The exception that was caught.</param>
    /// <param name="fallbackMessage">Localized text describing what failed, used unless the exception has a more specific message.</param>
    /// <param name="applicationMessage">
    ///     The application's own exceptions: returns the text for those it knows and <c>null</c> for the rest.
    /// </param>
    /// <returns>Text safe to display to the user.</returns>
    public static string Describe(Exception exception, string fallbackMessage, Func<Exception, string?>? applicationMessage = null)
        => applicationMessage?.Invoke(exception) ?? exception switch
        {
            DbUpdateConcurrencyException => BlazorResources.ChangedInTheMeantime,
            _ => fallbackMessage,
        };

    #endregion
}
