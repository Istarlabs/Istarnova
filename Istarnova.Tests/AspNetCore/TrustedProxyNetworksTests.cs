#region "Header"

// File: TrustedProxyNetworksTests.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using System.Net;
using Istarnova.AspNetCore.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Istarnova.Tests.AspNetCore;

public class TrustedProxyNetworksTests
{
    [Fact]
    public void Resolve_ShouldTrustThisMachineAndThePrivateRanges_WhenNothingIsConfigured()
    {
        var networks = TrustedProxyNetworks.Resolve(null);

        networks.Should().Contain(network => network.Contains(IPAddress.Parse("192.168.1.20")));
        networks.Should().Contain(network => network.Contains(IPAddress.Loopback));
        networks.Should().NotContain(network => network.Contains(IPAddress.Parse("203.0.113.9")));
    }

    [Fact]
    public void Resolve_ShouldTrustOnlyWhatIsConfigured_IgnoringBlankEntries()
    {
        var networks = TrustedProxyNetworks.Resolve(["10.89.0.0/24", " ", ""]);

        networks.Should().ContainSingle();
        networks[0].Contains(IPAddress.Parse("10.89.0.7")).Should().BeTrue();
        networks[0].Contains(IPAddress.Parse("192.168.1.20")).Should().BeFalse();
    }

    [Fact]
    public void Resolve_ShouldNameTheEntryThatIsNotANetwork()
    {
        var act = () => TrustedProxyNetworks.Resolve(["10.0.0.0/8", "not a network"]);

        act.Should().Throw<FormatException>().WithMessage("*not a network*");
    }

    [Theory]
    [InlineData("10.1.2.3", "203.0.113.9 https")]
    [InlineData("198.51.100.7", "198.51.100.7 http")]
    public async Task AddTrustedForwardedHeaders_ShouldBelieveTheProxyAndNobodyElse(string caller, string expected)
    {
        var builder = TestHostApplication.CreateBuilder(new() { [TrustedProxyNetworks.SectionName + ":0"] = "10.0.0.0/8" });
        builder.Services.AddTrustedForwardedHeaders(builder.Configuration);

        var (app, client) = await TestHostApplication.StartAsync(builder, application =>
        {
            application.Use((context, next) =>
            {
                context.Connection.RemoteIpAddress = IPAddress.Parse(caller);

                return next(context);
            });
            application.UseForwardedHeaders();
            application.MapGet("/", (HttpContext context) => $"{context.Connection.RemoteIpAddress} {context.Request.Scheme}");
        });
        await using var _ = app;

        using var request = new HttpRequestMessage(HttpMethod.Get, "/");
        request.Headers.Add("X-Forwarded-For", "203.0.113.9");
        request.Headers.Add("X-Forwarded-Proto", "https");

        var body = await (await client.SendAsync(request)).Content.ReadAsStringAsync();

        body.Should().Be(expected);
    }
}
