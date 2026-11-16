#region "Header"

// File: SeedableBaseEntityConfiguration.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using Istarnova.EfCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Istarnova.EfCore.Configurations;

/// <summary>
///     A configuration whose table ships with rows. They reach a database only through the model, so a row missing here
///     is missing on every fresh install.
/// </summary>
/// <typeparam name="TEntity">The type of the entity being configured.</typeparam>
public abstract class SeedableBaseEntityConfiguration<TEntity> : BaseEntityConfiguration<TEntity>
    where TEntity : BaseEntity
{
    #region "Variables"

    /// <summary>
    ///     The audit timestamp of seeded rows unless the configuration says otherwise
    /// </summary>
    public static readonly DateTime DefaultSeedDate = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    #endregion

    #region "Properties"

    /// <summary>
    ///     The audit timestamp of every seeded row. Fixed, because a changing value makes EF see a model change on
    ///     every build. An application that already has seeded rows keeps its date by overriding this.
    /// </summary>
    protected virtual DateTime SeedDate => DefaultSeedDate;

    #endregion

    #region "Methods"

    /// <summary>
    ///     The rows every instance ships with
    /// </summary>
    protected abstract IEnumerable<TEntity> GetSeedData();

    private protected sealed override void ConfigureSeedData(EntityTypeBuilder<TEntity> builder)
        => builder.HasData(GetSeedData());

    #endregion
}
