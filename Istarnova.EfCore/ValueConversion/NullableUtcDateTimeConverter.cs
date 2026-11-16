#region "Header"

// File: NullableUtcDateTimeConverter.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Istarnova.EfCore.ValueConversion;

/// <summary>
///     <see cref="UtcDateTimeConverter" /> for a nullable <see cref="DateTime" />
/// </summary>
public sealed class NullableUtcDateTimeConverter : ValueConverter<DateTime?, DateTime?>
{
    #region "Constructor"

    public NullableUtcDateTimeConverter()
        : base(
            value => value.HasValue ? UtcDateTimeConverter.NormalizeToUtc(value.Value) : null,
            value => value.HasValue ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc) : null)
    {
    }

    #endregion
}
