// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

using Istarnova.Extensions;

namespace Istarnova.Tests.Extensions;

public class StringEmptinessExtensionsTests
{
    [Theory]
    [InlineData(null, false, false)]
    [InlineData("", false, false)]
    [InlineData(" ", true, false)]
    [InlineData("a", true, true)]
    public void NotEmpty_ShouldMatchFramework_ForNullEmptyAndWhiteSpace(string? input, bool notEmpty,
        bool notEmptyOrWhiteSpace)
    {
        input.NotEmpty().Should().Be(notEmpty);
        input.NotEmptyOrWhiteSpace().Should().Be(notEmptyOrWhiteSpace);
        input.IsEmpty().Should().Be(!notEmpty);
        input.IsEmptyOrWhiteSpace().Should().Be(!notEmptyOrWhiteSpace);
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData("", null)]
    [InlineData(" ", " ")]
    [InlineData("a", "a")]
    public void NullIfEmpty_ShouldReturnNull_OnlyForNullOrEmpty(string? input, string? expected)
        => input.NullIfEmpty().Should().Be(expected);

    [Theory]
    [InlineData(null, null)]
    [InlineData("", null)]
    [InlineData(" ", null)]
    [InlineData("a", "a")]
    public void NullIfWhiteSpace_ShouldReturnNull_ForBlankText(string? input, string? expected)
        => input.NullIfWhiteSpace().Should().Be(expected);
}
