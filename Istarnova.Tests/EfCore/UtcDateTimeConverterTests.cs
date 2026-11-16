#region "Header"

// File: UtcDateTimeConverterTests.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using Istarnova.EfCore.ValueConversion;

namespace Istarnova.Tests.EfCore;

public class UtcDateTimeConverterTests
{
    private static readonly DateTime Moment = new(2026, 10, 23, 12, 30, 0);

    [Fact]
    public void ConvertToProvider_ShouldKeepUtc()
    {
        var utc = DateTime.SpecifyKind(Moment, DateTimeKind.Utc);

        var result = (DateTime)new UtcDateTimeConverter().ConvertToProvider(utc)!;

        result.Should().Be(utc);
        result.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Fact]
    public void ConvertToProvider_ShouldTakeAnUnspecifiedKindAsUtc()
    {
        var result = (DateTime)new UtcDateTimeConverter().ConvertToProvider(Moment)!;

        result.Kind.Should().Be(DateTimeKind.Utc);
        result.Ticks.Should().Be(Moment.Ticks);
    }

    [Fact]
    public void ConvertToProvider_ShouldConvertLocalTimeToUtc()
    {
        var local = DateTime.SpecifyKind(Moment, DateTimeKind.Local);

        var result = (DateTime)new UtcDateTimeConverter().ConvertToProvider(local)!;

        result.Kind.Should().Be(DateTimeKind.Utc);
        result.Should().Be(local.ToUniversalTime());
    }

    [Fact]
    public void ConvertFromProvider_ShouldMarkTheValueAsUtc()
    {
        var result = (DateTime)new UtcDateTimeConverter().ConvertFromProvider(Moment)!;

        result.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Fact]
    public void Nullable_ShouldPassNullThrough_AndNormalizeAValue()
    {
        var converter = new NullableUtcDateTimeConverter();

        converter.ConvertToProvider(null).Should().BeNull();
        ((DateTime?)converter.ConvertToProvider(Moment))!.Value.Kind.Should().Be(DateTimeKind.Utc);
        ((DateTime?)converter.ConvertFromProvider(Moment))!.Value.Kind.Should().Be(DateTimeKind.Utc);
    }
}
