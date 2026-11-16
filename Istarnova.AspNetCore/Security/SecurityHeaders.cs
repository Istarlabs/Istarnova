#region "Header"

// File: SecurityHeaders.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using Microsoft.AspNetCore.Builder;

namespace Istarnova.AspNetCore.Security;

/// <summary>
///     The response headers a browser is told about an application, set in one place instead of in each proxy that might
///     sit in front of it.
/// </summary>
public static class SecurityHeaders
{
    #region "Variables"

    /// <summary>
    ///     Nothing on this origin may be put in a frame by another page. Enforced, unlike a report-only policy: it is the
    ///     header that stops clickjacking, and it breaks nothing an application does.
    /// </summary>
    private const string FrameAncestors = "frame-ancestors 'none'";

    /// <summary>
    ///     Features of the browser an application never uses, switched off so a script that got in could not use them.
    /// </summary>
    private const string PermissionsPolicy =
        "accelerometer=(), camera=(), geolocation=(), gyroscope=(), microphone=(), payment=(), usb=()";

    /// <summary>
    ///     What a Blazor Server application is expected to load, for use as the report-only policy until the console shows
    ///     nothing the application needs is missing.
    /// </summary>
    public const string WebApplicationPolicy =
        "default-src 'self'; base-uri 'self'; form-action 'self'; object-src 'none'; " +
        "connect-src 'self' wss: ws:; img-src 'self' data: blob:; font-src 'self' data:; " +
        "style-src 'self' 'unsafe-inline'; frame-src 'self' data: blob:";

    #endregion

    #region "Methods"

    /// <summary>
    ///     Sets <c>X-Content-Type-Options</c>, <c>Referrer-Policy</c>, <c>X-Frame-Options</c>, <c>frame-ancestors</c> and
    ///     <c>Permissions-Policy</c> on every response.
    /// </summary>
    /// <param name="app">The pipeline.</param>
    /// <param name="reportOnlyPolicy">A <c>Content-Security-Policy</c> to send as report-only, or <c>null</c> for none.</param>
    // The referrer is cut to the same origin: an address may carry an id, and every link out of the application would
    // otherwise hand it to the site it leads to.
    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app, string? reportOnlyPolicy = null)
        => app.Use((context, next) =>
        {
            var headers = context.Response.Headers;

            headers.XContentTypeOptions = "nosniff";
            headers.XFrameOptions = "DENY";
            headers["Referrer-Policy"] = "same-origin";
            headers["Permissions-Policy"] = PermissionsPolicy;
            headers.ContentSecurityPolicy = FrameAncestors;

            if (reportOnlyPolicy is not null)
            {
                headers.ContentSecurityPolicyReportOnly = reportOnlyPolicy;
            }

            return next(context);
        });

    #endregion
}
