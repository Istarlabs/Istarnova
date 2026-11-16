#region "Header"

// File: BaseEntityConfigurationTests.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Istarnova.Tests.EfCore;

public class BaseEntityConfigurationTests
{
    private static IEntityType Entity<T>()
    {
        using var context = new EfCoreTestContext();

        return context.DesignModel.FindEntityType(typeof(T))!;
    }

    [Fact]
    public void Configure_ShouldGiveTheAuditTimestampsADatabaseDefault_AndKeepEfSendingTheValue()
    {
        var entity = Entity<EfCoreTestContext.Plain>();

        foreach (var name in new[] { "CreatedOn", "ModifiedOn" })
        {
            var property = entity.FindProperty(name)!;
            property.GetDefaultValueSql().Should().Be("now()");
            property.ValueGenerated.Should().Be(ValueGenerated.Never);
        }
    }

    [Fact]
    public void Configure_ShouldIndexTheCreationDate_UnderTheEntitysName()
    {
        Entity<EfCoreTestContext.Plain>().GetIndexes()
            .Should().Contain(index => index.GetDatabaseName() == "IX_Plain_CreatedOn");
    }

    [Fact]
    public void Configure_ShouldLetAConfigurationNameTheCreationIndex()
    {
        Entity<EfCoreTestContext.Guarded>().GetIndexes()
            .Should().Contain(index => index.GetDatabaseName() == "IX_Guardeds_CreatedOn");
    }

    [Fact]
    public void Configure_ShouldIgnoreTheRowVersion_UnlessTheTableDetectsConcurrentEdits()
    {
        Entity<EfCoreTestContext.Plain>().FindProperty("RowVersion").Should().BeNull();
    }

    [Fact]
    public void Configure_ShouldMapTheRowVersionOntoXmin_WhenTheTableDetectsConcurrentEdits()
    {
        var property = Entity<EfCoreTestContext.Guarded>().FindProperty("RowVersion")!;

        property.IsConcurrencyToken.Should().BeTrue();
        property.GetColumnName().Should().Be("xmin");
    }

    [Fact]
    public void Configure_ShouldLimitTheAuditUserColumns()
    {
        var entity = Entity<EfCoreTestContext.Plain>();

        entity.FindProperty("CreatedBy")!.GetMaxLength().Should().Be(450);
        entity.FindProperty("ModifiedBy")!.GetMaxLength().Should().Be(450);
    }

    [Fact]
    public void SeedData_ShouldReachTheModel_WithTheFixedSeedDate()
    {
        var seeds = Entity<EfCoreTestContext.Seeded>().GetSeedData().ToList();

        seeds.Should().HaveCount(2);
        seeds.Select(seed => seed["CreatedOn"]).Should().OnlyContain(value => (DateTime)value! == Istarnova.EfCore.Configurations.SeedableBaseEntityConfiguration<EfCoreTestContext.Seeded>.DefaultSeedDate);
    }
}
