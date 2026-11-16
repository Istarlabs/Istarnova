#region "Header"

// File: SecurityHeadersTests.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using Istarnova.AspNetCore.Security;
using Microsoft.AspNetCore.Builder;

namespace Istarnova.Tests.AspNetCore;

public class SecurityHeadersTests
{
    [Fact]
    public async Task UseSecurityHeaders_ShouldSetTheHeadersOnEveryResponse()
    {
        var (app, client) = await TestHostApplication.StartAsync(TestHostApplication.CreateBuilder(), application =>
        {
            application.UseSecurityHeaders();
            application.MapGet("/", () => "ok");
        });
        await using var _ = app;

        var response = await client.GetAsync("/");

        response.Headers.GetValues("X-Content-Type-Options").Should().Equal("nosniff");
        response.Headers.GetValues("X-Frame-Options").Should().Equal("DENY");
        response.Headers.GetValues("Referrer-Policy").Should().Equal("same-origin");
        response.Headers.GetValues("Permissions-Policy").Single().Should().Contain("camera=()");
        response.Headers.GetValues("Content-Security-Policy").Should().Equal("frame-ancestors 'none'");
        response.Headers.Contains("Content-Security-Policy-Report-Only").Should().BeFalse();
    }

    [Fact]
    public async Task UseSecurityHeaders_ShouldSendTheGivenPolicyAsReportOnly()
    {
        var (app, client) = await TestHostApplication.StartAsync(TestHostApplication.CreateBuilder(), application =>
        {
            application.UseSecurityHeaders(SecurityHeaders.WebApplicationPolicy);
            application.MapGet("/", () => "ok");
        });
        await using var _ = app;

        var response = await client.GetAsync("/");

        response.Headers.GetValues("Content-Security-Policy-Report-Only").Single().Should().StartWith("default-src 'self'");
    }
}
