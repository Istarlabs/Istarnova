// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

using Istarnova.Text.Xml;

namespace Istarnova.Tests.Text.Xml;

public class InvalidXmlCharacterValidatorTests
{
    [Theory]
    [InlineData("plain text", "plain text")]
    [InlineData("itâ\u0080\u0099s", "it's")]
    [InlineData("â\u0080\u009Cquotedâ\u0080\u009D", "\"quoted\"")]
    [InlineData("aâ\u0080\u0093bâ\u0080\u0094c", "a–b—c")]
    [InlineData("aÂ b", "a b")]
    [InlineData("â stays Â", "â stays Â")]
    [InlineData("â\u0080\u0099â\u0080\u0099", "''")]
    public void FixXmlEncoding_ShouldReplaceTheGarbledSequences_WhenTheyOccur(string input, string expected)
        => input.FixXmlEncoding().Should().Be(expected);

    [Theory]
    [InlineData("a<br />b", "a\r\nb")]
    [InlineData("a<BR>b<br>c", "a\r\nb\r\nc")]
    [InlineData("a&lt;br /&gt;b", "a\r\nb")]
    [InlineData("1 < 2 & 3", "1 < 2 & 3")]
    [InlineData("tom &amp; jerry", "tom & jerry")]
    public void RemoveInvalidXmlChars_ShouldTurnLineBreakTagsIntoNewLines_WhenTheyOccur(string input, string expected)
        => input.RemoveInvalidXmlChars().Should().Be(expected);

    [Fact]
    public void RemoveInvalidXmlChars_ShouldReturnTheSameString_WhenNothingNeedsToChange()
    {
        const string text = "nothing to do";

        text.RemoveInvalidXmlChars().Should().BeSameAs(text);
        text.FixXmlEncoding().Should().BeSameAs(text);
    }

    [Fact]
    public void RemoveInvalidXmlChars_ShouldDropControlCharacters_WhenTheyOccur()
        => "a\u0001b\u0008c<br>d".RemoveInvalidXmlChars().Should().Be("abc\r\nd");
}
