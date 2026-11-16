#region "Header"

// File: PropertyBuilderExtensionsTests.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Istarnova.Tests.EfCore;

public class PropertyBuilderExtensionsTests
{
    private static IProperty Property(string name)
    {
        using var context = new EfCoreTestContext();

        return context.DesignModel.FindEntityType(typeof(EfCoreTestContext.Shaped))!.FindProperty(name)!;
    }

    [Theory]
    [InlineData("Colour")]
    [InlineData("OptionalColour")]
    public void HasStringToEnumConversion_ShouldStoreTheMemberName(string name)
    {
        var property = Property(name);

        property.GetProviderClrType().Should().Be(typeof(string));
        property.GetTypeMapping().Converter!.ConvertToProvider(EfCoreTestContext.Colour.Blue).Should().Be("Blue");
    }

    [Fact]
    public void HasDecimalColumn_ShouldDefaultToEighteenAndFour()
    {
        var property = Property("Amount");

        property.GetPrecision().Should().Be(18);
        property.GetScale().Should().Be(4);
    }

    [Fact]
    public void HasJsonColumn_ShouldUseJsonb()
        => Property("Payload").GetColumnType().Should().Be("jsonb");

    [Fact]
    public void HasCharColumn_ShouldUseAFixedWidth()
    {
        var property = Property("Code");

        property.GetColumnType().Should().Be("char(3)");
        property.GetMaxLength().Should().Be(3);
    }
}
