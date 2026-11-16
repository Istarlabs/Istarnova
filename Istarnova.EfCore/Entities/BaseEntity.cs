#region "Header"

// File: BaseEntity.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Istarnova.EfCore.Entities;

/// <summary>
///     Base entity for all database entities with audit fields. The fields are stamped on save, see
///     <see cref="Istarnova.EfCore.Auditing.AuditStamper" />.
/// </summary>
public abstract class BaseEntity
{
    #region "Properties"

    /// <summary>
    ///     Created by (the user's id; an ASP.NET Identity user id in the applications)
    /// </summary>
    [MaxLength(450)]
    [StringSyntax(StringSyntaxAttribute.GuidFormat)]
    public string? CreatedBy { get; set; }

    /// <summary>
    ///     Creation date (UTC)
    /// </summary>
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

    /// <summary>
    ///     Modified by (the user's id)
    /// </summary>
    [MaxLength(450)]
    [StringSyntax(StringSyntaxAttribute.GuidFormat)]
    public string? ModifiedBy { get; set; }

    /// <summary>
    ///     Last modification date (UTC)
    /// </summary>
    public DateTime ModifiedOn { get; set; } = DateTime.UtcNow;

    /// <summary>
    ///     PostgreSQL's <c>xmin</c> of the row as it was read, for tables that detect concurrent edits;
    ///     0 where the table does not, or when the caller did not carry it along.
    /// </summary>
    public uint RowVersion { get; set; }

    #endregion
}
