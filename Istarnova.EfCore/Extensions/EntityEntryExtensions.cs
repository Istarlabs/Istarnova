#region "Header"

// File: EntityEntryExtensions.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using Istarnova.EfCore.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Istarnova.EfCore.Extensions;

public static class EntityEntryExtensions
{
    #region "Methods"

    /// <summary>
    ///     Copies an edited copy onto the tracked row it was read from. On a table that detects concurrent edits the save is
    ///     refused with a <see cref="Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException" /> if the row changed since;
    ///     a copy with row version 0 saves as before.
    /// </summary>
    public static void ApplyEdit<TEntity>(this EntityEntry<TEntity> entry, TEntity edited) where TEntity : BaseEntity
    {
        entry.CurrentValues.SetValues(edited);

        if (entry.Metadata.FindProperty(nameof(BaseEntity.RowVersion)) is null)
        {
            return;
        }

        var rowVersion = entry.Property(entity => entity.RowVersion);

        if (edited.RowVersion != 0)
        {
            rowVersion.OriginalValue = edited.RowVersion;
        }

        rowVersion.IsModified = false;
    }

    #endregion
}
