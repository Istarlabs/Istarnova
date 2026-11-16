#region "Header"

// File: AuthenticationCircuitHandler.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.Extensions.Logging;

namespace Istarnova.Blazor.State;

/// <summary>
///     Keeps the user state of a circuit in step with the authentication: it is initialised when the connection comes up
///     and every time the authentication state changes. An application that has more to do at those moments derives from
///     this class and overrides <see cref="OnAuthenticationAsync" />.
/// </summary>
/// <typeparam name="TState">The state of the circuit.</typeparam>
public partial class AuthenticationCircuitHandler<TState> : CircuitHandler, IDisposable
    where TState : class, ICircuitUserState
{
    #region "Variables"

    private readonly AuthenticationStateProvider _authenticationStateProvider;

    private readonly ILogger _logger;

    #endregion

    #region "Constructor"

    public AuthenticationCircuitHandler(AuthenticationStateProvider authenticationStateProvider, TState state,
        ILogger<AuthenticationCircuitHandler<TState>> logger)
    {
        _authenticationStateProvider = authenticationStateProvider;
        State = state;
        _logger = logger;
    }

    #endregion

    #region "Properties"

    /// <summary>
    ///     The state of the circuit this handler serves.
    /// </summary>
    protected TState State { get; }

    #endregion

    #region "Methods"

    public void Dispose()
    {
        _authenticationStateProvider.AuthenticationStateChanged -= AuthenticationChanged;

        GC.SuppressFinalize(this);
    }

    public override Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        _authenticationStateProvider.AuthenticationStateChanged += AuthenticationChanged;

        return base.OnCircuitOpenedAsync(circuit, cancellationToken);
    }

    public override async Task OnConnectionUpAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        try
        {
            var state = await _authenticationStateProvider.GetAuthenticationStateAsync();
            await OnAuthenticationAsync(state);
        }
        catch (Exception exception)
        {
            LogInitializeFailed(exception);
        }
    }

    /// <summary>
    ///     Brings the state up to date for the authentication state. Derived handlers call the base first.
    /// </summary>
    protected virtual Task OnAuthenticationAsync(AuthenticationState state)
        => State.InitializeAsync(state.User);

    private void AuthenticationChanged(Task<AuthenticationState> task)
        => _ = UpdateAsync(task);

    private async Task UpdateAsync(Task<AuthenticationState> task)
    {
        try
        {
            await OnAuthenticationAsync(await task);
        }
        catch (Exception exception)
        {
            LogAuthenticationChangeFailed(exception);
        }
    }

    #endregion
}
