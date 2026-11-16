#region "Header"

// File: NLogExtensions.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using NLog.Web;

namespace Istarnova.AspNetCore.Logging;

public static class NLogExtensions
{
    #region "Methods"

    extension(WebApplicationBuilder builder)
    {
        #region "Methods"

        /// <summary>
        ///     NLog as the logger of the application, configured from <c>appsettings.json</c> (the <c>NLog</c> section).
        ///     The configuration is registered as a source of <c>${configsetting:...}</c> first, and that is not optional:
        ///     the database target takes its connection string from it, and without it the renderer resolves to an empty
        ///     string. NLog does not fail on that - it opens a connection with no connection string, logs its own complaint
        ///     to a place nobody reads, and the operational log stays empty while looking configured.
        /// </summary>
        public WebApplicationBuilder AddIstarnovaNLog()
        {
            LogManager.Setup().SetupExtensions(extensions => extensions.RegisterConfigSettings(builder.Configuration));

            builder.Logging.AddNLog();
            builder.Host.UseNLog();

            return builder;
        }

        #endregion
    }

    #endregion
}
