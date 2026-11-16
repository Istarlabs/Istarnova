// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

using Istarnova.Extensions;

namespace Istarnova.Tests.Extensions;

public class StringSplitByTests
{
    [Theory]
    [InlineData(StringSplitBy.Comma, ",")]
    [InlineData(StringSplitBy.Semicolon, ";")]
    [InlineData(StringSplitBy.Space, " ")]
    [InlineData(StringSplitBy.Tab, "\t")]
    public void GetSeparator_ShouldReturnTheText_WhenTheValueIsDefined(StringSplitBy splitBy, string expected)
        => splitBy.GetSeparator().Should().Be(expected);

    [Fact]
    public void GetSeparator_ShouldUseTheNewLineOfThePlatform_WhenSplittingByNewLine()
        => StringSplitBy.NewLine.GetSeparator().Should().Be(Environment.NewLine);

    [Fact]
    public void GetSeparator_ShouldThrow_WhenTheValueIsUndefined()
        => FluentActions.Invoking(() => ((StringSplitBy)99).GetSeparator())
            .Should().Throw<ArgumentOutOfRangeException>();

    [Fact]
    public void SplitBy_ShouldDropEmptyEntries_WhenUsingTheDefaultOptions()
        => "a  b ".SplitBy(StringSplitBy.Space).Should().Equal("a", "b");

    [Fact]
    public void SplitBy_ShouldKeepEmptyEntries_WhenAskedTo()
        => "a;;b".SplitBy(StringSplitBy.Semicolon, StringSplitOptions.None).Should().Equal("a", "", "b");

    [Fact]
    public void SplitBy_ShouldThrow_WhenTheStringIsNull()
        => FluentActions.Invoking(() => ((string)null!).SplitBy(StringSplitBy.Space))
            .Should().Throw<ArgumentNullException>();

    [Fact]
    public void SplitBySpace_ShouldSplitLikeSplitBy_WhenCalledOnAString()
        => " a  b ".SplitBySpace().Should().Equal("a", "b");

    [Fact]
    public void SplitByNewLine_ShouldSplitLikeSplitBy_WhenCalledOnAString()
        => $"a{Environment.NewLine}{Environment.NewLine}b".SplitByNewLine().Should().Equal("a", "b");

    [Fact]
    public void SplitByDoubleNewLine_ShouldSplitAtBlankLinesOnly_WhenBlocksHaveSeveralLines()
    {
        var newLine = Environment.NewLine;
        var input = $"1{newLine}2{newLine}{newLine}3{newLine}{newLine}{newLine}{newLine}4{newLine}";

        input.SplitByDoubleNewLine().Should().Equal($"1{newLine}2", "3", $"4{newLine}");
    }

    [Fact]
    public void SplitByHelpers_ShouldKeepEmptyParts_WhenOptionsAreNone()
    {
        var newLine = Environment.NewLine;

        "a  b".SplitBySpace(StringSplitOptions.None).Should().Equal("a", "", "b");
        $"a{newLine}{newLine}b".SplitByNewLine(StringSplitOptions.None).Should().Equal("a", "", "b");
        $"{newLine}{newLine}a{newLine}{newLine}".SplitByDoubleNewLine(StringSplitOptions.None).Should().Equal("", "a", "");
    }

    [Theory]
    [InlineData(StringJoinBy.CommaNoSpace, "a,b")]
    [InlineData(StringJoinBy.SemiColonNoSpace, "a;b")]
    [InlineData(StringJoinBy.Comma, "a, b")]
    public void JoinBy_ShouldUseTheSeparatorOfTheValue_WhenJoiningACollection(StringJoinBy joinBy, string expected)
        => new List<string> { "a", "b" }.JoinBy(joinBy).Should().Be(expected);

    [Fact]
    public void SplitBy_ShouldYieldRangesIntoTheText_WhenSplittingASpan()
    {
        var text = "a,,bc".AsSpan();
        var parts = new List<string>();

        foreach (var range in text.SplitBy(StringSplitBy.Comma))
        {
            parts.Add(text[range].ToString());
        }

        parts.Should().Equal("a", "", "bc");
    }
}
