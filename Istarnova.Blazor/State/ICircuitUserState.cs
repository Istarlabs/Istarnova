#region "Header"

// File: ICircuitUserState.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using System.Security.Claims;

namespace Istarnova.Blazor.State;

/// <summary>
///     The state of one circuit that depends on who is signed in. It is initialised when the connection comes up and
///     again whenever the authentication changes.
/// </summary>
public interface ICircuitUserState
{
    #region "Methods"

    /// <summary>
    ///     Loads whatever the state keeps about the user (profile, roles, settings) for the claims of the principal.
    /// </summary>
    Task InitializeAsync(ClaimsPrincipal principal);

    #endregion
}
