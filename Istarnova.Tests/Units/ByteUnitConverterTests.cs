// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

using System.Globalization;

using Istarnova.Units;

namespace Istarnova.Tests.Units;

public class ByteUnitConverterTests
{
    [Theory]
    [InlineData(1024, 1.0)]
    [InlineData(2048, 2.0)]
    [InlineData(512, 0.5)]
    public void FromBytes_ShouldConvertToKibibytes(long inputBytes, double expectedKiB)
    {
        // Arrange & Act
        var converter = ByteUnitConverter.FromBytes(inputBytes);

        // Assert
        converter.TotalBytes.Should().Be(inputBytes);
        var kibibytes = (double)converter.TotalBytes / 1024;
        kibibytes.Should().BeApproximately(expectedKiB, 0.001);
    }

    [Fact]
    public void FromKilobytes_ShouldCreateCorrectByteCount()
    {
        // Arrange & Act
        var converter = ByteUnitConverter.FromKilobytes(1);

        // Assert
        converter.TotalBytes.Should().Be(1000);
    }

    [Fact]
    public void FromMegabytes_ShouldCreateCorrectByteCount()
    {
        // Arrange & Act
        var converter = ByteUnitConverter.FromMegabytes(1);

        // Assert
        converter.TotalBytes.Should().Be(1_000_000);
    }

    [Fact]
    public void ByteUnitConverter_ShouldImplementIComparable()
    {
        // Arrange
        var smaller = ByteUnitConverter.FromBytes(1024);
        var larger = ByteUnitConverter.FromMegabytes(1);

        // Act & Assert
        smaller.CompareTo(larger).Should().BeLessThan(0);
        larger.CompareTo(smaller).Should().BeGreaterThan(0);
    }

    [Fact]
    public void ByteUnitConverter_ShouldImplementIEquatable()
    {
        // Arrange
        var converter1 = ByteUnitConverter.FromBytes(1024);
        var converter2 = ByteUnitConverter.FromKibibytes(1);

        // Act & Assert
        converter1.Equals(converter2).Should().BeTrue();
    }

    [Fact]
    public void Constructor_ShouldThrowForNegativeBytes()
    {
        // Act & Assert
        var action = () => new ByteUnitConverter(-1);
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData("1024", 1024)]
    [InlineData("10MB", 10_000_000)]
    [InlineData("10mb", 10_000_000)]
    [InlineData("10 MB", 10_000_000)]
    [InlineData("10mib", 10_485_760)]
    [InlineData("10MiB", 10_485_760)]
    [InlineData("  512 kb  ", 512_000)]
    [InlineData("1.5GB", 1_500_000_000)]
    [InlineData("2K", 2000)]
    [InlineData("42 bytes", 42)]
    [InlineData("0B", 0)]
    public void TryParse_ShouldParseValueWithUnit(string input, long expectedBytes)
    {
        // Act
        var success = ByteUnitConverter.TryParse(input, CultureInfo.InvariantCulture, out var result);

        // Assert
        success.Should().BeTrue();
        result.TotalBytes.Should().Be(expectedBytes);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("MB")]
    [InlineData("10XB")]
    [InlineData("10 MB extra")]
    [InlineData("-5MB")]
    [InlineData("1000EB")]
    public void TryParse_ShouldFailForInvalidInput(string? input)
    {
        // Act
        var success = ByteUnitConverter.TryParse(input, CultureInfo.InvariantCulture, out var result);

        // Assert
        success.Should().BeFalse();
        result.TotalBytes.Should().Be(0);
    }

    [Fact]
    public void Parse_ShouldThrowForInvalidInput()
    {
        // Act & Assert
        var action = () => ByteUnitConverter.Parse("nonsense");
        action.Should().Throw<FormatException>();
    }

    [Fact]
    public void Parse_ShouldUseFormatProviderForDecimalSeparator()
    {
        // Act
        var result = ByteUnitConverter.Parse("1,5 GB", new CultureInfo("de-DE"));

        // Assert
        result.TotalBytes.Should().Be(1_500_000_000);
    }

    [Theory]
    [InlineData("1024", 1024)]
    [InlineData("50MB", 50_000_000)]
    [InlineData("1.5GB", 1_500_000_000)]
    [InlineData("  512 kb  ", 512_000)]
    public void TryParseInvariant_ShouldParseValueWithUnit(string input, long expectedBytes)
    {
        // Act
        var success = ByteUnitConverter.TryParseInvariant(input, out var result);

        // Assert
        success.Should().BeTrue();
        result.TotalBytes.Should().Be(expectedBytes);
    }

    [Fact]
    public void TryParseInvariant_ShouldIgnoreCurrentCulture_WhenSeparatorMeansSomethingElseThere()
    {
        // Arrange - under de-DE the dot is a thousands separator, so the plain overload reads
        // "1.5GB" as 15 GB. A size written in a configuration file must not change meaning with
        // the machine it is read on.
        var previousCulture = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = new CultureInfo("de-DE");

        try
        {
            // Act
            var invariant = ByteUnitConverter.TryParseInvariant("1.5GB", out var invariantResult);
            var currentCulture = ByteUnitConverter.TryParse("1.5GB", out var currentCultureResult);

            // Assert
            invariant.Should().BeTrue();
            invariantResult.TotalBytes.Should().Be(1_500_000_000);

            currentCulture.Should().BeTrue();
            currentCultureResult.TotalBytes.Should().NotBe(invariantResult.TotalBytes);
        }
        finally
        {
            CultureInfo.CurrentCulture = previousCulture;
        }
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("MB")]
    [InlineData("10XB")]
    public void TryParseInvariant_ShouldFailForInvalidInput(string? input)
    {
        // Act
        var success = ByteUnitConverter.TryParseInvariant(input, out var result);

        // Assert
        success.Should().BeFalse();
        result.TotalBytes.Should().Be(0);
    }
}
