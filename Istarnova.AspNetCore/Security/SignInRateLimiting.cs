#region "Header"

// File: SignInRateLimiting.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;

namespace Istarnova.AspNetCore.Security;

/// <summary>
///     A limit on how often one address may knock on the sign-in and registration pages. The lockout of an account protects
///     one account; this is what slows down trying one password across many.
/// </summary>
public static class SignInRateLimiting
{
    #region "Variables"

    /// <summary>
    ///     The name the endpoints refer to: <c>[EnableRateLimiting(SignInRateLimiting.PolicyName)]</c> on a page,
    ///     <c>RequireRateLimiting(SignInRateLimiting.PolicyName)</c> on a route group.
    /// </summary>
    public const string PolicyName = "sign-in";

    /// <summary>
    ///     Enough for a person who mistypes and for a household behind one address; not for a script.
    /// </summary>
    public const int RequestsPerMinute = 20;

    #endregion

    #region "Methods"

    /// <summary>
    ///     Registers the policy. Add <c>app.UseRateLimiter()</c> to the pipeline.
    /// </summary>
    public static IServiceCollection AddSignInRateLimiting(this IServiceCollection services)
        => services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.AddPolicy(PolicyName, context => RateLimitPartition.GetFixedWindowLimiter(
                context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                _ => new()
                {
                    PermitLimit = RequestsPerMinute,
                    Window = TimeSpan.FromMinutes(1),
                    QueueLimit = 0,
                }));
        });

    #endregion
}
