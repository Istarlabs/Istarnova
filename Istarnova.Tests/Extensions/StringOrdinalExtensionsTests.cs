// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

using Istarnova.Extensions;

namespace Istarnova.Tests.Extensions;

public class StringOrdinalExtensionsTests
{
    [Theory]
    [InlineData("Hello World", "World", true)]
    [InlineData("Hello World", "world", false)]
    [InlineData("Hello World", "", false)]
    [InlineData("", "World", false)]
    public void ContainsOrdinal_ShouldCompareCaseSensitive_WhenBothHaveContent(string text, string value, bool expected)
        => text.ContainsOrdinal(value).Should().Be(expected);

    [Theory]
    [InlineData("Hello World", "world", true)]
    [InlineData("Hello World", "WORLD", true)]
    [InlineData("Hello World", "Mars", false)]
    [InlineData("Hello World", "", false)]
    [InlineData("", "World", false)]
    [InlineData(null, "World", false)]
    public void ContainsOrdinalIgnoreCase_ShouldIgnoreCase_WhenBothHaveContent(string? text, string value, bool expected)
        => text.ContainsOrdinalIgnoreCase(value).Should().Be(expected);

    [Theory]
    [InlineData("Hello World", "HELLO", true)]
    [InlineData("Hello World", "World", false)]
    [InlineData("Hello World", "", false)]
    [InlineData("", "Hello", false)]
    [InlineData(null, "Hello", false)]
    public void StartsWithOrdinalIgnoreCase_ShouldIgnoreCase_WhenBothHaveContent(string? text, string value, bool expected)
        => text.StartsWithOrdinalIgnoreCase(value).Should().Be(expected);

    [Theory]
    [InlineData("Hello World", "WORLD", true)]
    [InlineData("Hello World", "Hello", false)]
    [InlineData("Hello World", "", false)]
    [InlineData("", "World", false)]
    [InlineData(null, "World", false)]
    public void EndsWithOrdinalIgnoreCase_ShouldIgnoreCase_WhenBothHaveContent(string? text, string value, bool expected)
        => text.EndsWithOrdinalIgnoreCase(value).Should().Be(expected);

    [Theory]
    [InlineData("Hello World", "Hello", true)]
    [InlineData("Hello World", "hello", false)]
    [InlineData("Hello World", "", false)]
    [InlineData("", "Hello", false)]
    public void StartsWithOrdinal_ShouldCompareCaseSensitive_WhenBothHaveContent(string text, string value, bool expected)
        => text.StartsWithOrdinal(value).Should().Be(expected);

    [Theory]
    [InlineData("Hello World", "World", true)]
    [InlineData("Hello World", "world", false)]
    [InlineData("Hello World", "", false)]
    [InlineData("", "World", false)]
    public void EndsWithOrdinal_ShouldCompareCaseSensitive_WhenBothHaveContent(string text, string value, bool expected)
        => text.EndsWithOrdinal(value).Should().Be(expected);

    [Fact]
    public void ContainsOrdinal_ShouldFindWhiteSpace_WhenTheValueIsOnlyWhiteSpace()
        => "a b".ContainsOrdinal(" ").Should().BeTrue();

    [Theory]
    [InlineData("30")]
    [InlineData("0")]
    public void SelectedSince_ShouldSubtractTheDays_WhenStringIsANumber(string days)
    {
        var before = DateTime.UtcNow.AddDays(-int.Parse(days));

        var result = days.SelectedSince();

        result.Should().BeOnOrAfter(before).And.BeOnOrBefore(DateTime.UtcNow.AddDays(-int.Parse(days)));
    }

    [Theory]
    [InlineData("")]
    [InlineData("abc")]
    [InlineData("1.5")]
    public void SelectedSince_ShouldReturnNull_WhenStringIsNotAWholeNumber(string days)
        => days.SelectedSince().Should().BeNull();
}
