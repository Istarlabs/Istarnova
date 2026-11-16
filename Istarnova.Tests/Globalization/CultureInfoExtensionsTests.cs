// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

using System.Globalization;
using Istarnova.Globalization;

namespace Istarnova.Tests.Globalization;

public class CultureInfoExtensionsTests
{
    [Theory]
    [InlineData("de-AT", "DE", true)]
    [InlineData("de", "de", true)]
    [InlineData("pt-PT", "de", false)]
    public void CompareTwoLetter_ShouldIgnoreRegionAndCase(string culture, string code, bool expected)
        => CultureInfo.GetCultureInfo(culture).CompareTwoLetter(code).Should().Be(expected);

    [Fact]
    public void HasSameLanguageAs_ShouldBeTrue_ForTwoRegionsOfOneLanguage()
        => CultureInfo.GetCultureInfo("de-AT").HasSameLanguageAs(CultureInfo.GetCultureInfo("de-CH")).Should()
            .BeTrue();

    [Theory]
    [InlineData("de-AT", "de")]
    [InlineData("de", "de")]
    public void GetNeutralCulture_ShouldDropTheRegion(string culture, string expected)
        => CultureInfo.GetCultureInfo(culture).GetNeutralCulture().Name.Should().Be(expected);

    [Theory]
    [InlineData("de", true)]
    [InlineData("pt-PT", true)]
    [InlineData("", false)]
    [InlineData(null, false)]
    [InlineData("not a culture", false)]
    public void TryGet_ShouldReturnFalse_ForNamesThatAreNoCulture(string? name, bool expected)
    {
        CultureInfo.TryGet(name, out var culture).Should().Be(expected);
        (culture is not null).Should().Be(expected);
    }
}
