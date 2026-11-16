#region "Header"

// File: ServiceDefaultsTests.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using System.Net;
using Istarnova.AspNetCore.ServiceDefaults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Istarnova.Tests.AspNetCore;

public class ServiceDefaultsTests
{
    [Theory]
    [InlineData("/health")]
    [InlineData("/alive")]
    public async Task MapDefaultEndpoints_ShouldAnswerHealthy(string path)
    {
        var builder = TestHostApplication.CreateBuilder();
        builder.AddServiceDefaults();

        var (app, client) = await TestHostApplication.StartAsync(builder, application => application.MapDefaultEndpoints());
        await using var _ = app;

        var response = await client.GetAsync(path);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        (await response.Content.ReadAsStringAsync()).Should().Be("Healthy");
    }

    [Theory]
    [InlineData("/health")]
    [InlineData("/alive")]
    public async Task MapDefaultEndpoints_ShouldAnswerWithoutAUser_WhenAnonymousIsAllowedAndEveryoneElseNeedsOne(string path)
    {
        var builder = TestHostApplication.CreateBuilder();
        builder.AddServiceDefaults();
        builder.Services.AddAuthorizationBuilder()
            .SetFallbackPolicy(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());

        var (app, client) = await TestHostApplication.StartAsync(builder, application => application.MapDefaultEndpoints(allowAnonymous: true));
        await using var _ = app;

        var response = await client.GetAsync(path);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task AddServiceDefaults_ShouldPutResilienceOnEveryHttpClient()
    {
        var builder = TestHostApplication.CreateBuilder();
        builder.AddServiceDefaults();
        builder.Services.AddHttpClient("probe");

        var (app, _) = await TestHostApplication.StartAsync(builder, _ => { });
        await using var __ = app;

        var factory = app.Services.GetRequiredService<IHttpClientFactory>();

        factory.CreateClient("probe").Should().NotBeNull();
    }
}
