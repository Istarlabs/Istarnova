#region "Header"

// File: SignInRateLimitingTests.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using System.Net;
using Istarnova.AspNetCore.Security;
using Microsoft.AspNetCore.Builder;

namespace Istarnova.Tests.AspNetCore;

public class SignInRateLimitingTests
{
    [Fact]
    public async Task AddSignInRateLimiting_ShouldAnswerTooManyRequests_AfterTheLimitOfOneMinute()
    {
        var builder = TestHostApplication.CreateBuilder();
        builder.Services.AddSignInRateLimiting();

        var (app, client) = await TestHostApplication.StartAsync(builder, application =>
        {
            application.UseRateLimiter();
            application.MapGet("/sign-in", () => "page").RequireRateLimiting(SignInRateLimiting.PolicyName);
            application.MapGet("/other", () => "page");
        });
        await using var _ = app;

        for (var request = 0; request < SignInRateLimiting.RequestsPerMinute; request++)
        {
            (await client.GetAsync("/sign-in")).StatusCode.Should().Be(HttpStatusCode.OK);
        }

        (await client.GetAsync("/sign-in")).StatusCode.Should().Be(HttpStatusCode.TooManyRequests);
        (await client.GetAsync("/other")).StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
