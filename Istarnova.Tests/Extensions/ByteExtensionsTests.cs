#region "Header"

// File: ByteExtensionsTests.cs
// Created 2026/03/06
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using Istarnova.Extensions;

namespace Istarnova.Tests.Extensions;

public class ByteExtensionsTests
{
    #region "ToBase45 Tests"

    [Theory]
    [InlineData(new byte[] { }, "")]
    [InlineData(new byte[] { 0x00 }, "00")]
    [InlineData(new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F }, "%69 VDL2")]
    [InlineData(new byte[] { 0xFF, 0xFF }, "FGW")]
    public void ToBase45_ShouldEncodeCorrectly_WhenGivenBytes(byte[] input, string expected)
    {
        // Act
        var result = input.ToBase45();

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void ToBase45_ShouldHandleEmptyArray_WhenArrayIsEmpty()
    {
        // Arrange
        var emptyArray = Array.Empty<byte>();

        // Act
        var result = emptyArray.ToBase45();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void ToBase45_ShouldUseOnlyValidCharacters_WhenEncoding()
    {
        // Arrange
        var input = new byte[] { 0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0 };
        const string validChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ $%*+-./:";

        // Act
        var result = input.ToBase45();

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.All(c => validChars.Contains(c)).Should().BeTrue();
    }

    #endregion

    #region "ToDataUrl Tests"

    [Theory]
    [InlineData(new byte[] { 0x89, 0x50, 0x4E, 0x47 }, "image/png")]
    [InlineData(new byte[] { 0xFF, 0xD8, 0xFF }, "image/jpeg")]
    [InlineData(new byte[] { 0x47, 0x49, 0x46, 0x38 }, "image/gif")]
    [InlineData(new byte[] { 0x42, 0x4D }, "image/bmp")]
    [InlineData(new byte[] { 0x49, 0x49, 0x2A, 0x00 }, "image/tiff")]
    [InlineData(new byte[] { 0x4D, 0x4D, 0x00, 0x2A }, "image/tiff")]
    public void ToDataUrl_ShouldNameTheMediaType_WhenTheBytesIdentifyThemselves(byte[] input, string expected)
    {
        // Act
        var result = input.ToDataUrl();

        // Assert
        result.Should().StartWith($"data:{expected};base64,");
    }

    [Fact]
    public void ToDataUrl_ShouldFallBackToOctetStream_WhenTheBytesAreNotAKnownImage()
    {
        // Arrange - "not a picture" in bytes, matching no magic number.
        var input = "not a picture"u8.ToArray();

        // Act
        var result = input.ToDataUrl();

        // Assert
        result.Should().StartWith("data:application/octet-stream;base64,");
    }

    [Fact]
    public void ToDataUrl_ShouldBelieveTheCaller_WhenTheMediaTypeIsGiven()
    {
        // Arrange - PNG magic number, declared as something else on purpose: the overload exists so a
        // caller that knows better than the first four bytes can say so.
        var input = new byte[] { 0x89, 0x50, 0x4E, 0x47 };

        // Act
        var result = input.ToDataUrl("application/pdf");

        // Assert
        result.Should().StartWith("data:application/pdf;base64,");
    }

    [Fact]
    public void ToDataUrl_ShouldRoundTrip_WhenReadBackWithToBytes()
    {
        // Arrange
        var input = new byte[] { 0xFF, 0xD8, 0xFF, 0x01, 0x02, 0x03 };

        // Act
        var restored = input.ToDataUrl().ToBytes();

        // Assert
        restored.Should().Equal(input);
    }

    [Fact]
    public void ToDataUrl_ShouldReturnEmpty_WhenArrayIsEmpty()
    {
        // Act
        var result = Array.Empty<byte>().ToDataUrl();

        // Assert - nothing to point at, so no URL rather than one promising no content.
        result.Should().BeEmpty();
    }

    #endregion
}
