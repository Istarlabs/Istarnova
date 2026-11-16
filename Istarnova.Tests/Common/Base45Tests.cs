#region "Header"

// File: Base45Tests.cs
// Created 2026/03/06
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using Istarnova.Common;

namespace Istarnova.Tests.Common;

public class Base45Tests
{
    #region "Encode Tests"

    [Theory]
    [InlineData(new byte[] { }, "")]
    [InlineData(new byte[] { 0x00 }, "00")]
    [InlineData(new byte[] { 0x01 }, "10")]
    [InlineData(new byte[] { 0x2C }, ":0")]
    [InlineData(new byte[] { 0xFF }, "U5")]
    [InlineData(new byte[] { 0x00, 0x00 }, "000")]
    [InlineData(new byte[] { 0x48, 0x65 }, "%69")]
    [InlineData(new byte[] { 0x48, 0x65, 0x6C }, "%69I2")]
    [InlineData(new byte[] { 0x48, 0x65, 0x6C, 0x6C }, "%69 VD")]
    [InlineData(new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F }, "%69 VDL2")]
    public void Encode_ShouldReturnExpectedString_WhenGivenBytes(byte[] input, string expected)
    {
        // Act
        var result = Base45.Encode(input);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void Encode_ShouldHandleLargeInput_WhenInputExceeds512Bytes()
    {
        // Arrange
        var input = new byte[1024];
        for (var i = 0; i < input.Length; i++)
        {
            input[i] = (byte)(i % 256);
        }

        // Act
        var result = Base45.Encode(input);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Length.Should().Be(1024 / 2 * 3); // Expected length for 1024 bytes
    }

    [Fact]
    public void Encode_ShouldUseOnlyValidCharacters_WhenEncoding()
    {
        // Arrange
        var input = new byte[] { 0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0 };
        const string validChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ $%*+-./:";

        // Act
        var result = Base45.Encode(input);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.All(c => validChars.Contains(c)).Should().BeTrue();
    }

    #endregion

    #region "Decode Tests"

    [Theory]
    [InlineData("", new byte[] { })]
    [InlineData("00", new byte[] { 0x00 })]
    [InlineData("10", new byte[] { 0x01 })]
    [InlineData(":0", new byte[] { 0x2C })]
    [InlineData("U5", new byte[] { 0xFF })]
    [InlineData("000", new byte[] { 0x00, 0x00 })]
    [InlineData("%69", new byte[] { 0x48, 0x65 })]
    [InlineData("%69I2", new byte[] { 0x48, 0x65, 0x6C })]
    [InlineData("%69 VD", new byte[] { 0x48, 0x65, 0x6C, 0x6C })]
    [InlineData("%69 VDL2", new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F })]
    public void Decode_ShouldReturnExpectedBytes_WhenGivenValidString(string input, byte[] expected)
    {
        // Act
        var result = Base45.Decode(input);

        // Assert
        result.Should().Equal(expected);
    }

    [Theory]
    [InlineData("0@")]
    [InlineData("!0")]
    [InlineData("AB@")]
    [InlineData("###")]
    [InlineData("~00")]
    [InlineData("00~")]
    public void Decode_ShouldThrowFormatException_WhenInputContainsInvalidCharacters(string input)
    {
        // Act
        var act = () => Base45.Decode(input);

        // Assert
        act.Should().Throw<FormatException>()
            .WithMessage("Invalid Base45 character at position*");
    }

    [Fact]
    public void Decode_ShouldHandleLargeInput_WhenInputIsLong()
    {
        // Arrange
        var originalBytes = new byte[1024];
        for (var i = 0; i < originalBytes.Length; i++)
        {
            originalBytes[i] = (byte)(i % 256);
        }
        var encoded = Base45.Encode(originalBytes);

        // Act
        var result = Base45.Decode(encoded);

        // Assert
        result.Should().Equal(originalBytes);
    }

    #endregion

    #region "Roundtrip Tests"

    [Theory]
    [InlineData(new byte[] { })]
    [InlineData(new byte[] { 0x00 })]
    [InlineData(new byte[] { 0xFF })]
    [InlineData(new byte[] { 0x12, 0x34 })]
    [InlineData(new byte[] { 0x12, 0x34, 0x56 })]
    [InlineData(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05 })]
    public void EncodeAndDecode_ShouldReturnOriginalBytes_WhenRoundtripping(byte[] original)
    {
        // Act
        var encoded = Base45.Encode(original);
        var decoded = Base45.Decode(encoded);

        // Assert
        decoded.Should().Equal(original);
    }

    [Fact]
    public void EncodeAndDecode_ShouldHandleAllByteValues_WhenRoundtripping()
    {
        // Arrange
        var original = new byte[256];
        for (var i = 0; i < 256; i++)
        {
            original[i] = (byte)i;
        }

        // Act
        var encoded = Base45.Encode(original);
        var decoded = Base45.Decode(encoded);

        // Assert
        decoded.Should().Equal(original);
    }

    [Theory]
    [InlineData("Hello, World!")]
    [InlineData("The quick brown fox jumps over the lazy dog")]
    [InlineData("Base45 encoding test 123!@#")]
    [InlineData("ÄÖÜäöüß")] // German characters
    public void EncodeAndDecode_ShouldHandleTextData_WhenRoundtripping(string text)
    {
        // Arrange
        var original = System.Text.Encoding.UTF8.GetBytes(text);

        // Act
        var encoded = Base45.Encode(original);
        var decoded = Base45.Decode(encoded);

        // Assert
        decoded.Should().Equal(original);
        System.Text.Encoding.UTF8.GetString(decoded).Should().Be(text);
    }

    #endregion

    #region "Edge Cases"

    [Fact]
    public void Encode_ShouldHandleOddByteCount_WhenInputLengthIsOdd()
    {
        // Arrange
        var input = new byte[] { 0x12, 0x34, 0x56 };

        // Act
        var result = Base45.Encode(input);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Length.Should().Be(5); // 2 bytes -> 3 chars, 1 byte -> 2 chars
    }

    [Fact]
    public void Decode_ShouldHandleTwoCharacterInput_WhenInputLengthIsTwo()
    {
        // Arrange
        var input = "AB";

        // Act
        var result = Base45.Decode(input);

        // Assert
        result.Should().HaveCount(1);
    }

    [Fact]
    public void Encode_ShouldHandleMaxByteValues_WhenInputContainsMaxValues()
    {
        // Arrange
        var input = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF };

        // Act
        var result = Base45.Encode(input);
        var decoded = Base45.Decode(result);

        // Assert
        decoded.Should().Equal(input);
    }

    #endregion
}