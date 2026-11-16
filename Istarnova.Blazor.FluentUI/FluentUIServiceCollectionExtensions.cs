#region "Header"

// File: FluentUIServiceCollectionExtensions.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using Istarnova.Blazor.FluentUI.Grids;
using Istarnova.Blazor.FluentUI.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.FluentUI.AspNetCore.Components;

namespace Istarnova.Blazor.FluentUI;

public static class FluentUIServiceCollectionExtensions
{
    #region "Methods"

    extension(IServiceCollection services)
    {
        #region "Methods"

        /// <summary>
        ///     Registers FluentUI the way the applications use it: the library's localizer for the texts FluentUI draws itself,
        ///     class-name validation off, toasts in the top corner, and the memory of the page each grid was left on.
        /// </summary>
        /// <param name="configure">Everything else, applied last.</param>
        /// <param name="localizer">A localizer with the application's own languages; the library's when omitted.</param>
        public IServiceCollection AddIstarnovaFluentUI(Action<LibraryConfiguration>? configure = null, IFluentLocalizer? localizer = null)
        {
            services.AddFluentUIComponents(options =>
            {
                options.ValidateClassNames = false;
                options.Localizer = localizer ?? new IstarnovaFluentLocalizer();
                options.Toast.Position = ToastPosition.TopEnd;

                configure?.Invoke(options);
            });

            services.TryAddScoped<GridPageMemory>();

            return services;
        }

        #endregion
    }

    #endregion
}
