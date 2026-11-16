#region "Header"

// File: BaseEntityConfiguration.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using System.Linq.Expressions;
using Istarnova.EfCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Istarnova.EfCore.Configurations;

/// <summary>
///     Base EF Core configuration for <see cref="BaseEntity" />: the audit columns, an index on the creation date, and the
///     opt-in hooks every table may need.
/// </summary>
/// <typeparam name="TEntity">The type of the entity being configured.</typeparam>
public abstract class BaseEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : BaseEntity
{
    #region "Properties"

    /// <summary>
    ///     A filter applied to every query of the entity, or <c>null</c> for none.
    /// </summary>
    protected virtual Expression<Func<TEntity, bool>>? QueryFilter => null;

    /// <summary>
    ///     The name of the <c>CreatedOn</c> index. Tables created by hand before their configuration
    ///     existed carry the table's plural instead of the entity's name, and keep it.
    /// </summary>
    protected virtual string CreatedOnIndexName
        => $"IX_{typeof(TEntity).Name}_CreatedOn";

    /// <summary>
    ///     Whether a save is refused when the row changed since it was read, via PostgreSQL's <c>xmin</c>.
    ///     Opt-in, because a table read through a <c>RETURNS TABLE</c> function has no <c>xmin</c>.
    /// </summary>
    protected virtual bool DetectsConcurrentEdits => false;

    #endregion

    #region "Methods"

    /// <inheritdoc cref="IEntityTypeConfiguration{TEntity}.Configure" />
    void IEntityTypeConfiguration<TEntity>.Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.Property(entity => entity.CreatedBy).HasMaxLength(450);
        builder.Property(entity => entity.ModifiedBy).HasMaxLength(450);

        // The database default serves SQL written by hand - backfills, seeds, psql. EF always sends the
        // value it has, as it did before the default existed.
        builder.Property(entity => entity.CreatedOn).HasDefaultValueSql("now()").ValueGeneratedNever();
        builder.Property(entity => entity.ModifiedOn).HasDefaultValueSql("now()").ValueGeneratedNever();

        if (DetectsConcurrentEdits)
        {
            // Npgsql maps a uint row version onto the xmin system column: no column of our own to add.
            builder.Property(entity => entity.RowVersion).IsRowVersion();
        }
        else
        {
            builder.Ignore(entity => entity.RowVersion);
        }

        builder.HasIndex(entity => entity.CreatedOn)
            .HasDatabaseName(CreatedOnIndexName);

        if (QueryFilter is not null)
        {
            builder.HasQueryFilter(QueryFilter);
        }

        Configure(builder);
        ConfigureSeedData(builder);
    }

    private protected virtual void ConfigureSeedData(EntityTypeBuilder<TEntity> builder)
    {
    }

    /// <summary>
    ///     Configures the entity
    /// </summary>
    /// <param name="builder">The builder to be used to configure the entity type.</param>
    protected abstract void Configure(EntityTypeBuilder<TEntity> builder);

    #endregion
}
