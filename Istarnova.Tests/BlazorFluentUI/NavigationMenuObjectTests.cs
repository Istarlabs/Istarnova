#region "Header"

// File: NavigationMenuObjectTests.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using Istarnova.Blazor.FluentUI.Navigation;
using Microsoft.AspNetCore.Components.Routing;

namespace Istarnova.Tests.BlazorFluentUI;

public class NavigationMenuObjectTests
{
    [Fact]
    public void UrlWithQueryParameters_ShouldAppendTheParameters()
    {
        var item = new NavigationMenuObject { Title = "x", Url = "/list" };
        item.AddQueryParameter("kind", "a");
        item.AddQueryParameter("page", 2);

        item.UrlWithQueryParameters.Should().Be("/list?kind=a&page=2");
        item.HasQueryParameters.Should().BeTrue();
    }

    [Fact]
    public void UrlWithQueryParameters_ShouldBeTheUrl_WithoutParameters()
        => new NavigationMenuObject { Title = "x", Url = "/list" }.UrlWithQueryParameters.Should().Be("/list");

    [Fact]
    public void CombinedRoles_ShouldJoinTheRoles_OrBeNull()
    {
        new NavigationMenuObject { Title = "x" }.CombinedRoles.Should().BeNull();
        new NavigationMenuObject { Title = "x", Roles = ["Admin", "Editor"] }.CombinedRoles.Should().Be("Admin,Editor");
    }

    [Theory]
    [InlineData("/", "", true)]
    [InlineData("/", "/anything", false)]
    [InlineData("/patients", "/patients", true)]
    [InlineData("/patients", "/patients/42/edit", true)]
    [InlineData("/patients", "/patientsfoo", false)]
    [InlineData("/patients/", "/PATIENTS", true)]
    public void IsCurrentPage_ShouldMatchTheAddressWithoutQueryAndOnSegmentBorders(string url, string current, bool expected)
    {
        var item = new NavigationMenuObject { Title = "x", Url = url };

        NavMenu.IsCurrentPage(item, current).Should().Be(expected);
    }

    [Fact]
    public void IsCurrentPage_ShouldOnlyMatchTheExactAddress_ForAnAllMatch()
    {
        var item = new NavigationMenuObject { Title = "x", Url = "/patients", Match = NavLinkMatch.All };

        NavMenu.IsCurrentPage(item, "/patients").Should().BeTrue();
        NavMenu.IsCurrentPage(item, "/patients/42").Should().BeFalse();
    }

    [Fact]
    public void IsCurrentPage_ShouldNotMatchAnEntryWithoutAnUrl()
        => NavMenu.IsCurrentPage(new NavigationMenuObject { Title = "group" }, "/any").Should().BeFalse();
}
