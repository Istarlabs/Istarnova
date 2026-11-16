#region "Header"

// File: AuditStamper.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using Istarnova.EfCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Istarnova.EfCore.Auditing;

/// <summary>
///     Stamps the audit fields of the entities a context is about to save, so no repository has to remember to.
///     Call it from <c>SaveChanges</c> and <c>SaveChangesAsync</c> of the context.
/// </summary>
public static class AuditStamper
{
    #region "Variables"

    /// <summary>
    ///     The audit columns that say when a record was created and by whom. They are written once, when the row is
    ///     inserted, and are never part of an update.
    /// </summary>
    private static readonly string[] CreationStampProperties =
        [nameof(BaseEntity.CreatedOn), nameof(BaseEntity.CreatedBy)];

    #endregion

    #region "Methods"

    /// <summary>
    ///     Stamps every added or modified <see cref="BaseEntity" /> the tracker holds.
    /// </summary>
    /// <param name="tracker">The change tracker of the context that is about to save.</param>
    /// <param name="userId">Who saves; <c>null</c> when only the timestamps are to be set.</param>
    /// <param name="now">The moment to stamp; the current UTC time when omitted.</param>
    public static void Apply(ChangeTracker tracker, string? userId, DateTime? now = null)
    {
        ArgumentNullException.ThrowIfNull(tracker);

        var moment = now ?? DateTime.UtcNow;

        foreach (var entry in tracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedOn = moment;
                    entry.Entity.CreatedBy = userId;
                    entry.Entity.ModifiedOn = moment;
                    entry.Entity.ModifiedBy = userId;

                    break;

                case EntityState.Modified:
                    entry.Entity.ModifiedOn = moment;
                    entry.Entity.ModifiedBy = userId;
                    KeepCreationStamp(entry);

                    break;
            }
        }
    }

    /// <summary>
    ///     Takes the creation stamp out of an update, so the row keeps what it was inserted with.
    /// </summary>
    private static void KeepCreationStamp(EntityEntry<BaseEntity> entry)
    {
        foreach (var name in CreationStampProperties)
        {
            var property = entry.Property(name);

            property.CurrentValue = property.OriginalValue;
            property.IsModified = false;
        }
    }

    #endregion
}
