// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later


using Istarnova.Extensions;

namespace Istarnova.Tests.Extensions;

public class StringExtensionsTests
{
    [Fact]
    public void AsUri_ShouldCreateUri_WhenValidStringProvided()
    {
        // Arrange
        var uriString = "https://example.com";

        // Act
        var result = uriString.AsUri();

        // Assert
        result.Should().NotBeNull();
        result.Host.Should().Be("example.com");
        result.Scheme.Should().Be("https");
    }

    [Theory]
    [InlineData("This is a test string", 10, "This is a  ...")]
    [InlineData("Short", 10, "Short")]
    [InlineData("Exactly ten", 10, "Exactly te ...")]
    public void DisplayShortText_ShouldTruncateCorrectly(string input, int maxLength, string expected)
    {
        // Act
        var result = input.DisplayShortText(maxLength);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void DisplayShortText_ShouldThrowArgumentNullException_WhenStringIsNull()
    {
        // Arrange
        string? nullString = null;

        // Act & Assert
        var action = () => nullString!.DisplayShortText();
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void DisplayShortText_ShouldThrowArgumentOutOfRangeException_WhenMaxLengthIsZero()
    {
        // Arrange
        var testString = "Test";

        // Act & Assert
        var action = () => testString.DisplayShortText(0);
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData("https://example.com", "example.com")]
    [InlineData("https://sub.example.com:8080/path", "sub.example.com")]
    [InlineData("http://localhost", "localhost")]
    [InlineData("", null)]
    [InlineData(null, null)]
    [InlineData("not-a-uri", null)]
    public void GetHostFromUri_ShouldExtractHostCorrectly(string? input, string? expected)
    {
        // Act
        var result = input.GetHostFromUri();

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void ToBytes_ShouldConvertDataUrlToBytes()
    {
        // Arrange
        var base64Data = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("Hello World"));
        var dataUrl = $"data:text/plain;base64,{base64Data}";

        // Act
        var result = dataUrl.ToBytes();

        // Assert
        var resultString = System.Text.Encoding.UTF8.GetString(result);
        resultString.Should().Be("Hello World");
    }

    [Fact]
    public void ToBytes_ShouldReturnEmptyArray_WhenNoCommaFound()
    {
        // Arrange
        var invalidDataUrl = "data:text/plain;base64";

        // Act
        var result = invalidDataUrl.ToBytes();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void ToBytes_ShouldThrowArgumentNullException_WhenDataUrlIsNull()
    {
        // Arrange
        string? nullDataUrl = null;

        // Act & Assert
        var action = () => nullDataUrl!.ToBytes();
        action.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData("routine check-up", "RoutineCheckUp")]
    [InlineData("ROUTINE CHECK-UP", "RoutineCheckUp")]
    [InlineData("Vorsorge", "Vorsorge")]
    [InlineData("Facharzt-Überweisung", "FacharztÜberweisung")]
    [InlineData("  leading and trailing  ", "LeadingAndTrailing")]
    [InlineData("mrt/ct 2 scan", "MrtCt2Scan")]
    [InlineData("---", "")]
    [InlineData("", "")]
    [InlineData(null, "")]
    public void ToPascalCase_ShouldJoinWordsWithCapitals(string? input, string expected)
    {
        // Act
        var result = input.ToPascalCase();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("routine check-up", "routineCheckUp")]
    [InlineData("ROUTINE CHECK-UP", "routineCheckUp")]
    [InlineData(null, "")]
    public void ToCamelCase_ShouldStartLowerCase(string? input, string expected)
    {
        // Act
        var result = input.ToCamelCase();

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void ToPascalCase_ShouldStartEveryLineTheSameWay()
    {
        // Arrange
        var input = "routine check-up\nsick leave";

        // Act
        var result = input.ToPascalCase();

        // Assert
        result.Should().Be("RoutineCheckUp\nSickLeave");
    }

    [Theory]
    [InlineData("routine check-up", "routine_check_up")]
    [InlineData("  ROUTINE  check-up  ", "routine_check_up")]
    [InlineData("---", "")]
    [InlineData(null, "")]
    public void ToSnakeCase_ShouldDelimitWordsWithUnderscores(string? input, string expected)
    {
        // Act
        var result = input.ToSnakeCase();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("routine check-up", "ROUTINE_CHECK_UP")]
    [InlineData(null, "")]
    public void ToConstantCase_ShouldUpperCaseAndDelimit(string? input, string expected)
    {
        // Act
        var result = input.ToConstantCase();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("routine check-up", "routine-check-up")]
    [InlineData("Routine  Check Up", "routine-check-up")]
    [InlineData(null, "")]
    public void ToKebabCase_ShouldDelimitWordsWithHyphens(string? input, string expected)
    {
        // Act
        var result = input.ToKebabCase();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("routine check-up", "ROUTINE-CHECK-UP")]
    [InlineData(null, "")]
    public void ToCobolCase_ShouldUpperCaseAndHyphenate(string? input, string expected)
    {
        // Act
        var result = input.ToCobolCase();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("routine check-up", "Routine-Check-Up")]
    [InlineData("  ROUTINE check up", "Routine-Check-Up")]
    [InlineData(null, "")]
    public void ToTrainCase_ShouldCapitaliseEveryWord(string? input, string expected)
    {
        // Act
        var result = input.ToTrainCase();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("hello there. HOW are you?", "Hello there. How are you?")]
    [InlineData("erste zeile\nZWEITE zeile", "Erste zeile\nZweite zeile")]
    [InlineData(null, "")]
    public void ToSentenceCase_ShouldCapitaliseEverySentence(string? input, string expected)
    {
        // Act
        var result = input.ToSentenceCase();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("the QUICK fox", "The Quick Fox")]
    [InlineData("mrt/ct scan", "Mrt/Ct Scan")]
    [InlineData(null, "")]
    public void ToTitleCase_ShouldCapitaliseEveryWordAndKeepSeparators(string? input, string expected)
    {
        // Act
        var result = input.ToTitleCase();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("hello", "hElLo")]
    [InlineData(null, "")]
    public void ToAlternatingCase_ShouldStartLowerCase(string? input, string expected)
    {
        // Act
        var result = input.ToAlternatingCase();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("hello", "HeLlO")]
    [InlineData(null, "")]
    public void ToReverseAlternatingCase_ShouldStartUpperCase(string? input, string expected)
    {
        // Act
        var result = input.ToReverseAlternatingCase();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("abc", "cba")]
    [InlineData("Grüße", "eßürG")]
    [InlineData(null, "")]
    public void ReverseText_ShouldTurnTheTextAround(string? input, string expected)
    {
        // Act
        var result = input.ReverseText();

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void ReverseText_ShouldKeepAGraphemeInOnePiece()
    {
        // Arrange - an emoji made of a surrogate pair, and an "e" with a combining acute accent.
        var input = "ab🙂é";

        // Act
        var result = input.ReverseText();

        // Assert
        result.Should().Be("é🙂ba");
    }
}
