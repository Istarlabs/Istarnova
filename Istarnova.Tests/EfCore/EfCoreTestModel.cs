#region "Header"

// File: EfCoreTestModel.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using Istarnova.EfCore.Configurations;
using Istarnova.EfCore.Entities;
using Istarnova.EfCore.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Istarnova.Tests.EfCore;

/// <summary>
///     A small model to look at what the base configurations do. The Npgsql provider builds it without a connection.
/// </summary>
public sealed class EfCoreTestContext : DbContext
{
    #region "Constructor"

    public EfCoreTestContext()
        : base(new DbContextOptionsBuilder<EfCoreTestContext>().UseNpgsql("Host=localhost;Database=model_only").Options)
    {
    }

    #endregion

    #region "Methods"

    public IModel DesignModel
        => this.GetService<IDesignTimeModel>().Model;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new PlainConfiguration());
        modelBuilder.ApplyConfiguration(new GuardedConfiguration());
        modelBuilder.ApplyConfiguration(new SeededConfiguration());
        modelBuilder.ApplyConfiguration(new ShapedConfiguration());
    }

    #endregion

    #region "Types"

    public enum Colour
    {
        Red = 1,
        Blue = 2,
    }

    public sealed class Plain : BaseEntity
    {
        public int PlainId { get; set; }

        public string Name { get; set; } = string.Empty;
    }

    public sealed class Guarded : BaseEntity
    {
        public int GuardedId { get; set; }

        public string Name { get; set; } = string.Empty;
    }

    public sealed class Seeded : BaseEntity
    {
        public int SeededId { get; set; }

        public string Name { get; set; } = string.Empty;
    }

    public sealed class Shaped : BaseEntity
    {
        public decimal Amount { get; set; }

        public Colour Colour { get; set; }

        public Colour? OptionalColour { get; set; }

        public string Payload { get; set; } = string.Empty;

        public int ShapedId { get; set; }

        public string Code { get; set; } = string.Empty;
    }

    private sealed class PlainConfiguration : BaseEntityConfiguration<Plain>
    {
        protected override void Configure(EntityTypeBuilder<Plain> builder)
            => builder.HasKey(entity => entity.PlainId);
    }

    private sealed class GuardedConfiguration : BaseEntityConfiguration<Guarded>
    {
        protected override bool DetectsConcurrentEdits => true;

        protected override string CreatedOnIndexName => "IX_Guardeds_CreatedOn";

        protected override void Configure(EntityTypeBuilder<Guarded> builder)
            => builder.HasKey(entity => entity.GuardedId);
    }

    private sealed class SeededConfiguration : SeedableBaseEntityConfiguration<Seeded>
    {
        protected override IEnumerable<Seeded> GetSeedData()
            =>
            [
                new() { SeededId = 1, Name = "first", CreatedOn = SeedDate, ModifiedOn = SeedDate },
                new() { SeededId = 2, Name = "second", CreatedOn = SeedDate, ModifiedOn = SeedDate },
            ];

        protected override void Configure(EntityTypeBuilder<Seeded> builder)
            => builder.HasKey(entity => entity.SeededId);
    }

    private sealed class ShapedConfiguration : BaseEntityConfiguration<Shaped>
    {
        protected override void Configure(EntityTypeBuilder<Shaped> builder)
        {
            builder.HasKey(entity => entity.ShapedId);
            builder.Property(entity => entity.Amount).HasDecimalColumn();
            builder.Property(entity => entity.Colour).HasStringToEnumConversion();
            builder.Property(entity => entity.OptionalColour).HasStringToEnumConversion();
            builder.Property(entity => entity.Payload).HasJsonColumn();
            builder.Property(entity => entity.Code).HasCharColumn(3);
        }
    }

    #endregion
}
