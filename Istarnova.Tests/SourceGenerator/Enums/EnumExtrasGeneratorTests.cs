#region "Header"

// File: EnumExtrasGeneratorTests.cs
// Created 2026/09/16
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

namespace Istarnova.Tests.SourceGenerator.Enums;

public class EnumExtrasGeneratorTests
{
    [Fact]
    public void GetValuesDescending_ShouldListTheHighestValueFirst_AndAnAliasOnce()
    {
        // Act
        var values = SparseEnumExtensions.GetValuesDescending();

        // Assert
        values.Should().Equal(SparseEnum.Ninety, SparseEnum.Two, SparseEnum.One, SparseEnum.Negative);
    }

    [Theory]
    [InlineData(1, SparseEnum.One)]
    [InlineData(2, SparseEnum.Two)]
    [InlineData(90, SparseEnum.Ninety)]
    [InlineData(-5, SparseEnum.Negative)]
    public void TryGetByValue_ShouldReturnTheMember_WhenTheValueIsDefined(int value, SparseEnum expected)
    {
        // Act
        var found = SparseEnumExtensions.TryGetByValue(value, out var result);

        // Assert
        found.Should().BeTrue();
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    [InlineData(6)]
    public void TryGetByValue_ShouldReturnFalse_WhenNoMemberHasTheValue(int value)
    {
        // Act
        var found = SparseEnumExtensions.TryGetByValue(value, out _);

        // Assert
        found.Should().BeFalse();
    }

    [Fact]
    public void TryGetByValue_ShouldReturnFalse_WhenTheValueIsMissing()
    {
        // Arrange
        int? missingInt = null;
        long? missingLong = null;

        // Act & Assert
        SparseEnumExtensions.TryGetByValue(missingInt, out _).Should().BeFalse();
        SparseEnumExtensions.TryGetByValue(missingLong, out _).Should().BeFalse();
    }

    [Fact]
    public void TryGetByValue_ShouldAcceptLongAndNullable()
    {
        // Arrange
        int? nullableInt = 90;
        long? nullableLong = 1L;

        // Act & Assert
        SparseEnumExtensions.TryGetByValue(90L, out var fromLong).Should().BeTrue();
        fromLong.Should().Be(SparseEnum.Ninety);
        SparseEnumExtensions.TryGetByValue(nullableInt, out var fromNullableInt).Should().BeTrue();
        fromNullableInt.Should().Be(SparseEnum.Ninety);
        SparseEnumExtensions.TryGetByValue(nullableLong, out var fromNullableLong).Should().BeTrue();
        fromNullableLong.Should().Be(SparseEnum.One);
    }
}
