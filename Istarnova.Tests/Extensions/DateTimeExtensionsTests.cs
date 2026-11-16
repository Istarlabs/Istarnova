// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

using System.Globalization;
using Istarnova.Extensions;

namespace Istarnova.Tests.Extensions;

public class DateTimeExtensionsTests
{
    [Fact]
    public void SetDay_ShouldSetDayCorrectly_WhenValidDayProvided()
    {
        // Arrange
        var dateTime = new DateTime(2025, 2, 15, 10, 30, 45);
        var newDay = 25;

        // Act
        var result = dateTime.SetDay(newDay);

        // Assert
        result.Day.Should().Be(newDay);
        result.Year.Should().Be(dateTime.Year);
        result.Month.Should().Be(dateTime.Month);
        result.Hour.Should().Be(dateTime.Hour);
        result.Minute.Should().Be(dateTime.Minute);
        result.Second.Should().Be(dateTime.Second);
    }

    [Fact]
    public void SetDay_ShouldThrowArgumentOutOfRangeException_WhenDayIsInvalid()
    {
        // Arrange
        var dateTime = new DateTime(2025, 2, 15); // February has max 28 days in 2025

        // Act & Assert
        var action = () => dateTime.SetDay(30);
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void SetDay_Nullable_ShouldReturnNull_WhenDateTimeIsNull()
    {
        // Arrange
        DateTime? nullDateTime = null;

        // Act
        var result = nullDateTime.SetDay(15);

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [InlineData("de-DE", "8. Nov.")]
    [InlineData("en-US", "Nov 8")]
    public void AsShortMonthDayString_ShouldFollowTheOrderOfTheCulture(string culture, string expected)
    {
        // Arrange
        var provider = CultureInfo.GetCultureInfo(culture);

        // Act
        var fromDateOnly = new DateOnly(2026, 11, 8).AsShortMonthDayString(provider);
        var fromDateTime = new DateTime(2026, 11, 8, 14, 30, 0).AsShortMonthDayString(provider);

        // Assert
        fromDateOnly.Should().Be(expected);
        fromDateTime.Should().Be(expected);
    }

    [Fact]
    public void AsShortMonthDayString_ShouldNotCarryTheYear_WhenTheCultureIsInvariant()
    {
        // Act
        var result = new DateOnly(2026, 11, 8).AsShortMonthDayString(CultureInfo.InvariantCulture);

        // Assert
        result.Should().NotContain("2026");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(6)]
    [InlineData(12)]
    public void AsShortMonthString_ShouldReturnAbbreviatedMonthName(int month)
    {
        // Act
        var result = month.AsShortMonthString();

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Be(CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(month));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(6)]
    [InlineData(12)]
    public void AsMonthNameString_ShouldReturnFullMonthName(int month)
    {
        // Act
        var result = month.AsMonthNameString();

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Be(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month));
    }

    [Fact]
    public void GetNamedMonths_ShouldReturnTwelveMonths()
    {
        // Act
        var result = DateTimeExtensions.GetNamedMonths();

        // Assert
        result.Should().HaveCount(12);
        result.Should().AllSatisfy(month => month.Should().NotBeNullOrEmpty());
    }

    [Theory]
    [InlineData(2025, 8, 202508)]
    [InlineData(2024, 12, 202412)]
    [InlineData(2023, 1, 202301)]
    public void ToYearMonthString_Static_ShouldReturnCorrectFormat(int year, int month, int expected)
    {
        // Act
        var result = DateTimeExtensions.ToYearMonthString(year, month);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void ToYearMonthString_Extension_ShouldReturnCorrectFormat()
    {
        // Arrange
        var dateTime = new DateTime(2025, 8, 15);

        // Act
        var result = dateTime.ToYearMonthString();

        // Assert
        result.Should().Be(202508);
    }
}
