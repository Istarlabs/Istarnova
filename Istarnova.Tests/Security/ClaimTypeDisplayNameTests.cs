// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

using System.Security.Claims;
using Istarnova.Security.Claims;
using Istarnova.Tests.BlazorFluentUI;

namespace Istarnova.Tests.Security;

public class ClaimTypeDisplayNameTests
{
    [Fact]
    public void GetClaimTypeDisplayName_ShouldReturnTheClaimType_WhenItHasNoText()
        => ClaimsPrincipalExtensions.GetClaimTypeDisplayName("custom").Should().Be("custom");

    [Fact]
    public void GetClaimTypeDisplayName_ShouldFollowTheCurrentCulture_NotTheOneOfTheFirstCall()
    {
        string english;
        string german;

        using (new CultureScope("en"))
        {
            english = ClaimsPrincipalExtensions.GetClaimTypeDisplayName(ClaimTypes.Role);
        }

        using (new CultureScope("de"))
        {
            german = ClaimsPrincipalExtensions.GetClaimTypeDisplayName(ClaimTypes.Role);
        }

        english.Should().NotBe(ClaimTypes.Role);
        german.Should().NotBe(ClaimTypes.Role).And.NotBe(english);
    }

    [Fact]
    public void GetClaimTypeDisplayName_ShouldKeepTheAioClaimType_AsItIs()
        => ClaimsPrincipalExtensions.GetClaimTypeDisplayName("aio").Should().Be("aio");
}
