// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

using Istarnova.Numerics;

namespace Istarnova.Tests.Numerics;

public class MathCompareTests
{
    [Theory]
    [InlineData(12, 8, 4)]
    [InlineData(48, 18, 6)]
    [InlineData(7, 5, 1)]
    [InlineData(0, 5, 5)]
    [InlineData(5, 0, 5)]
    public void GreatestCommonDivisor_ShouldReturnCorrectGCD(int a, int b, int expected)
    {
        // Act
        var result = MathCompare.GreatestCommonDivisor(a, b);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(4, 6, 12)]
    [InlineData(12, 8, 24)]
    [InlineData(7, 5, 35)]
    [InlineData(3, 9, 9)]
    public void LowestCommonMultiple_ShouldReturnCorrectLCM(int a, int b, int expected)
    {
        // Act
        var result = MathCompare.LowestCommonMultiple(a, b);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void GreatestCommonDivisor_ShouldHandleSameNumbers()
    {
        // Arrange
        var number = 42;

        // Act
        var result = MathCompare.GreatestCommonDivisor(number, number);

        // Assert
        result.Should().Be(number);
    }

    [Fact]
    public void LowestCommonMultiple_ShouldHandleSameNumbers()
    {
        // Arrange
        var number = 42;

        // Act
        var result = MathCompare.LowestCommonMultiple(number, number);

        // Assert
        result.Should().Be(number);
    }
}
