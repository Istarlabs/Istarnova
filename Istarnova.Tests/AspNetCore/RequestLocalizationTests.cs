#region "Header"

// File: RequestLocalizationTests.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using System.Globalization;
using Istarnova.AspNetCore.Localization;
using Microsoft.AspNetCore.Builder;

namespace Istarnova.Tests.AspNetCore;

public class RequestLocalizationTests
{
    [Theory]
    [InlineData("de-DE,de;q=0.9", "de")]
    [InlineData("ja", "ja")]
    [InlineData("fr-FR", "en")]
    [InlineData("", "en")]
    public async Task UseSupportedCultures_ShouldFollowTheBrowser_AndFallBackToTheDefault(string acceptLanguage, string expected)
    {
        var (app, client) = await TestHostApplication.StartAsync(TestHostApplication.CreateBuilder(), application =>
        {
            application.UseSupportedCultures("en", "en", "de", "ja");
            application.MapGet("/", () => CultureInfo.CurrentUICulture.Name);
        });
        await using var _ = app;

        using var request = new HttpRequestMessage(HttpMethod.Get, "/");

        if (acceptLanguage.Length > 0)
        {
            request.Headers.Add("Accept-Language", acceptLanguage);
        }

        var culture = await (await client.SendAsync(request)).Content.ReadAsStringAsync();

        culture.Should().StartWith(expected);
    }

    [Fact]
    public async Task UseSupportedCultures_ShouldPreferTheCultureCookie()
    {
        var (app, client) = await TestHostApplication.StartAsync(TestHostApplication.CreateBuilder(), application =>
        {
            application.UseSupportedCultures("en", "en", "de", "ja");
            application.MapGet("/", () => CultureInfo.CurrentUICulture.Name);
        });
        await using var _ = app;

        using var request = new HttpRequestMessage(HttpMethod.Get, "/");
        request.Headers.Add("Accept-Language", "de");
        request.Headers.Add("Cookie", ".AspNetCore.Culture=c%3Dja%7Cuic%3Dja");

        (await (await client.SendAsync(request)).Content.ReadAsStringAsync()).Should().StartWith("ja");
    }
}
