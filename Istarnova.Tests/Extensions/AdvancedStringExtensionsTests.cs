// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

using Istarnova.Extensions;

namespace Istarnova.Tests.Extensions;

public class AdvancedStringExtensionsTests
{
    // [Fact] - Specific test for complex scenario
    [Fact]
    public void ToBytes_ShouldHandleComplexDataUrl_WithMultipleParameters()
    {
        // Arrange - Complex data structure
        var jsonData = "{\"name\":\"test\",\"value\":123}";
        var base64Data = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(jsonData));
        var dataUrl = $"data:application/json;charset=utf-8;base64,{base64Data}";

        // Act
        var result = dataUrl.ToBytes();

        // Assert
        var resultString = System.Text.Encoding.UTF8.GetString(result);
        resultString.Should().Be(jsonData);
    }

    // [Theory] - Multiple edge cases for URI extraction
    [Theory]
    [InlineData("https://example.com", "example.com")]
    [InlineData("https://sub.example.com:8080/path?param=value", "sub.example.com")]
    [InlineData("http://localhost:3000", "localhost")]
    [InlineData("ftp://files.example.com", "files.example.com")]
    [InlineData("", null)] // Edge Case: Empty string
    [InlineData("   ", null)] // Edge Case: Whitespace
    [InlineData("not-a-valid-uri", null)] // Edge Case: Invalid URI
    [InlineData("://invalid", null)] // Edge Case: Malformed URI
    public void GetHostFromUri_ShouldHandleVariousUriFormats(string? input, string? expected)
    {
        // Act
        var result = input.GetHostFromUri();

        // Assert
        result.Should().Be(expected);
    }

    // [Theory] - Boundary testing for text truncation
    [Theory]
    [InlineData("", 10, "")] // Edge Case: Empty string
    [InlineData("a", 1, "a")] // Edge Case: Minimal length
    [InlineData("ab", 1, "a ...")] // Edge Case: Truncation needed
    [InlineData("Exactly 10", 10, "Exactly 10")] // Edge Case: Exact length
    [InlineData("This is longer than 10", 10, "This is lo ...")] // Normal truncation
    [InlineData("Unicode: 🚀🌟💫", 8, "Unicode: ...")] // Edge Case: Unicode characters (corrected)
    public void DisplayShortText_ShouldHandleBoundaryCases(string input, int maxLength, string expected)
    {
        // Act
        var result = input.DisplayShortText(maxLength);

        // Assert
        result.Should().Be(expected);
    }

    // [Fact] - Specific test for performance-critical code
    [Fact]
    public void ToBytes_ShouldHandleLargeDataUrl_WithinReasonableTime()
    {
        // Arrange - Large dataset
        var largeData = new string('A', 10000); // 10KB String
        var base64Data = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(largeData));
        var dataUrl = $"data:text/plain;base64,{base64Data}";

        // Act & Assert - Performance test
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = dataUrl.ToBytes();
        stopwatch.Stop();

        // Verify correctness
        var resultString = System.Text.Encoding.UTF8.GetString(result);
        resultString.Should().Be(largeData);

        // Verify performance (under 100ms for 10KB)
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(100);
    }
}
