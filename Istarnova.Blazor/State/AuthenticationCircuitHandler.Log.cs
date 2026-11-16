#region "Header"

// File: AuthenticationCircuitHandler.Log.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using Microsoft.Extensions.Logging;

namespace Istarnova.Blazor.State;

public partial class AuthenticationCircuitHandler<TState>
{
    #region "Methods"

    [LoggerMessage(EventId = 6040, Level = LogLevel.Error, Message = "The user state could not be initialised when the circuit connected")]
    private partial void LogInitializeFailed(Exception exception);

    [LoggerMessage(EventId = 6041, Level = LogLevel.Error, Message = "The user state could not be updated after the authentication changed")]
    private partial void LogAuthenticationChangeFailed(Exception exception);

    #endregion
}
