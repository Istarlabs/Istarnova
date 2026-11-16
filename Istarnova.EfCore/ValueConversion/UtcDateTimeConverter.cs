#region "Header"

// File: UtcDateTimeConverter.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Istarnova.EfCore.ValueConversion;

/// <summary>
///     Normalizes <see cref="DateTime" /> values to UTC, which Npgsql requires for <c>timestamp with time zone</c> columns.
///     An unspecified kind is taken to be UTC already.
/// </summary>
public sealed class UtcDateTimeConverter : ValueConverter<DateTime, DateTime>
{
    #region "Constructor"

    public UtcDateTimeConverter()
        : base(
            value => NormalizeToUtc(value),
            value => DateTime.SpecifyKind(value, DateTimeKind.Utc))
    {
    }

    #endregion

    #region "Methods"

    internal static DateTime NormalizeToUtc(DateTime value)
        => value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc),
        };

    #endregion
}
