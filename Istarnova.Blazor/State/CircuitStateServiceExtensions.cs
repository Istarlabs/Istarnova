#region "Header"

// File: CircuitStateServiceExtensions.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Istarnova.Blazor.State;

public static class CircuitStateServiceExtensions
{
    #region "Methods"

    extension(IServiceCollection services)
    {
        #region "Methods"

        /// <summary>
        ///     Registers the user state of a circuit (one per circuit) and the handler that keeps it in step with the
        ///     authentication.
        /// </summary>
        /// <typeparam name="TState">The state contract the components ask for.</typeparam>
        /// <typeparam name="TImplementation">The state.</typeparam>
        public IServiceCollection AddCircuitUserState<TState, TImplementation>()
            where TState : class, ICircuitUserState
            where TImplementation : class, TState
        {
            services.TryAddScoped<TState, TImplementation>();
            services.TryAddEnumerable(ServiceDescriptor.Scoped<CircuitHandler, AuthenticationCircuitHandler<TState>>());

            return services;
        }

        #endregion
    }

    #endregion
}
