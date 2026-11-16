// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

using System.Globalization;
using Istarnova.Extensions;

namespace Istarnova.Tests.Extensions;

public class NumberExtensionsTests
{
    [Fact]
    public void MoneyTwoDigitLocalized_ShouldGroupAsTheReaderReads_ButKeepTheAmountsOwnCurrency()
    {
        // Arrange
        var previous = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = new CultureInfo("de-DE");

        try
        {
            // Act
            var euros = 1234.5m.MoneyTwoDigitLocalized("EUR");
            var rupees = 4500m.MoneyTwoDigitLocalized("INR");

            // Assert - German separators in both, and the money someone paid abroad stays that money.
            euros.Should().Be("1.234,50 EUR");
            rupees.Should().Be("4.500,00 INR");
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }

    [Fact]
    public void Round_ShouldRoundHalvesToTheEvenNeighbour_ForDecimalAndDouble()
    {
        // Assert
        50.55m.Round(1).Should().Be(50.6m);
        50.45m.Round(1).Should().Be(50.4m);
        2.675.Round(2).Should().Be(2.67);
        98.7654m.Round(0).Should().Be(99m);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void MoneyTwoDigitLocalized_ShouldClaimNoCurrency_WhenNoneWasRecorded(string? currencyCode)
    {
        // Arrange
        var previous = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = new CultureInfo("de-DE");

        try
        {
            // Act
            var result = 12m.MoneyTwoDigitLocalized(currencyCode);

            // Assert - the reader's culture must not lend its own currency to an amount that never
            // stated one.
            result.Should().Be("12,00");
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }

    [Fact]
    public void MoneyTwoDigitLocalized_ShouldNormaliseTheCode_WhenItIsWrittenLoosely()
    {
        // Act
        var result = 1m.MoneyTwoDigitLocalized(" chf ");

        // Assert
        result.Should().Contain("CHF");
    }

    [Fact]
    public void MoneyTwoDigitLocalized_ShouldReturnThePlaceholder_WhenThereIsNoAmount()
    {
        // Arrange
        decimal? amount = null;

        // Act
        var result = amount.MoneyTwoDigitLocalized("EUR", "—");

        // Assert
        result.Should().Be("—");
    }

    [Fact]
    public void MoneyTwoDigitLocalized_ShouldFormatTheAmount_WhenThereIsOne()
    {
        // Arrange
        var previous = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = new CultureInfo("de-DE");
        decimal? amount = 5m;

        try
        {
            // Act
            var result = amount.MoneyTwoDigitLocalized("EUR", "—");

            // Assert
            result.Should().Be("5,00 EUR");
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }

    [Theory]
    [InlineData(123.456, "123.46")]
    [InlineData(0, "0.00")]
    [InlineData(-45.7, "-45.70")]
    [InlineData(1000.1234, "1,000.12")]
    public void TwoDigitFormatted_ShouldFormatCorrectly(double input, string expected)
    {
        // Act
        var result = input.TwoDigitFormatted();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(123.456)]
    [InlineData(0)]
    [InlineData(-45.7)]
    [InlineData(1000.1234)]
    public void MoneyTwoDigitFormatted_ShouldFormatAsCurrency(double input)
    {
        // Act
        var result = input.MoneyTwoDigitFormatted();

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Contain("."); // Should contain decimal point
        // Don't test exact currency format since it depends on system culture
    }

    [Theory]
    [InlineData(123.456, "123.456")]
    [InlineData(0, "0")]
    [InlineData(-45.7, "-45.7")]
    public void ToInvariantFloatingPointString_ShouldFormatInvariant(double input, string expected)
    {
        // Act
        var result = input.ToInvariantFloatingPointString();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(2.5, 2.5, 0.0, true)]
    [InlineData(2.5, 2.5000001, 0.0, false)]
    [InlineData(2.5, 2.5000001, 0.001, true)]
    [InlineData(2.5, 2.6, 0.001, false)]
    [InlineData(double.PositiveInfinity, double.PositiveInfinity, 0.0, true)]
    [InlineData(double.NaN, double.NaN, 0.1, false)]
    public void EqualsTo_ShouldCompareWithinTheTolerance(double left, double right, double tolerance, bool expected)
    {
        // Act
        var result = left.EqualsTo(right, tolerance);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void EqualsTo_ShouldAcceptDecimal()
    {
        // Act
        var result = 72.4m.EqualsTo(72.45m, 0.05m);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void EqualsTo_ShouldThrow_WhenTheToleranceIsNegative()
    {
        // Act
        var act = () => 1.0.EqualsTo(1.0, -0.1);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(null, null, true)]
    [InlineData(null, 0.0, false)]
    [InlineData(0.0, null, false)]
    [InlineData(1.5, 1.5, true)]
    public void EqualsTo_ShouldTreatMissingAsItsOwnValue_WhenNullable(double? left, double? right, bool expected)
    {
        // Act
        var result = left.EqualsTo(right);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(5, -5)]
    [InlineData(-10, 10)]
    [InlineData(0, 0)]
    public void Negate_ShouldNegateNumber(int input, int expected)
    {
        // Act
        var result = input.Negate();

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void NegateOrDefault_ShouldNegateValue_WhenNotNull()
    {
        // Arrange
        int? number = 5;

        // Act
        var result = number.NegateOrDefault();

        // Assert
        result.Should().Be(-5);
    }

    [Fact]
    public void NegateOrDefault_ShouldReturnZero_WhenNull()
    {
        // Arrange
        int? number = null;

        // Act
        var result = number.NegateOrDefault();

        // Assert
        result.Should().Be(0);
    }
}
