#region "Header"

// File: RequestLocalizationExtensions.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using Microsoft.AspNetCore.Builder;

namespace Istarnova.AspNetCore.Localization;

public static class RequestLocalizationExtensions
{
    #region "Methods"

    /// <summary>
    ///     The culture of a request: a cookie first, then what the browser asks for, then the default - instead of
    ///     whatever locale the server process happens to run in.
    /// </summary>
    /// <param name="app">The pipeline.</param>
    /// <param name="defaultCulture">The culture of someone whose browser asks for none of the supported ones.</param>
    /// <param name="supportedCultures">The cultures the application is translated into.</param>
    public static IApplicationBuilder UseSupportedCultures(this IApplicationBuilder app, string defaultCulture, params string[] supportedCultures)
        => app.UseRequestLocalization(new RequestLocalizationOptions()
            .SetDefaultCulture(defaultCulture)
            .AddSupportedCultures(supportedCultures)
            .AddSupportedUICultures(supportedCultures));

    #endregion
}
