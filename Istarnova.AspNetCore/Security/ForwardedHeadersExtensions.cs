#region "Header"

// File: ForwardedHeadersExtensions.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Istarnova.AspNetCore.Security;

public static class ForwardedHeadersExtensions
{
    #region "Methods"

    extension(IServiceCollection services)
    {
        #region "Methods"

        /// <summary>
        ///     Behind a reverse proxy the application itself is reached over plain HTTP, so without this it would consider
        ///     every request insecure: the identity cookie loses its Secure flag and the audit trail records the proxy's
        ///     address instead of the caller's. The headers are believed only from the proxy's network
        ///     (<see cref="TrustedProxyNetworks.SectionName" />, this machine and the private ranges until it is set), so a
        ///     caller who reaches the port directly cannot write an address or a scheme of their own. Add
        ///     <c>app.UseForwardedHeaders()</c> first in the pipeline.
        /// </summary>
        public IServiceCollection AddTrustedForwardedHeaders(IConfiguration configuration)
            => services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                options.KnownIPNetworks.Clear();
                options.KnownProxies.Clear();

                foreach (var network in TrustedProxyNetworks.Resolve(configuration.GetSection(TrustedProxyNetworks.SectionName).Get<string[]>()))
                {
                    options.KnownIPNetworks.Add(network);
                }
            });

        #endregion
    }

    #endregion
}
