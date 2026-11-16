// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

using Istarnova.Extensions;

namespace Istarnova.Tests.Extensions;

public class DoubleExtensionsTests
{
    [Theory]
    [InlineData(50.55, "50.55")]
    [InlineData(6.99, "6.99")]
    [InlineData(14.3, "14.3")]
    [InlineData(0.0, "0")]
    [InlineData(-402.3, "-402.3")]
    [InlineData(0.00001, "0.00001")]
    public void ToDecimal_ShouldReturnTheNumberAsWritten_NotItsBinaryExpansion(double value, string expected)
    {
        // Act
        var result = value.ToDecimal();

        // Assert
        result.Should().Be(decimal.Parse(expected, System.Globalization.CultureInfo.InvariantCulture));
    }

    [Fact]
    public void ToDecimal_ShouldKeepAMidpointOnItsSide_WhenRounded()
    {
        // Act
        var rounded = Math.Round(50.55.ToDecimal(), 1);

        // Assert - the midpoint 50.55 rounds to even (50.6); its binary expansion 50.5499... would give 50.5.
        rounded.Should().Be(50.6m);
    }
}
