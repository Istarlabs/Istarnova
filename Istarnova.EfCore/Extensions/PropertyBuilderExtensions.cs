#region "Header"

// File: PropertyBuilderExtensions.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Istarnova.EfCore.Extensions;

/// <summary>
///     Column shapes the applications use over and over
/// </summary>
public static class PropertyBuilderExtensions
{
    #region "Methods"

    extension<TProperty>(PropertyBuilder<TProperty> property) where TProperty : struct, Enum
    {
        #region "Methods"

        /// <summary>
        ///     Stores the enum as its member name. The rule of the applications: a disclosed or exported row must be readable
        ///     without the source code, and renumbering an enum must never reinterpret stored rows.
        /// </summary>
        public PropertyBuilder<TProperty> HasStringToEnumConversion()
            => property.HasConversion<string>();

        /// <summary>
        ///     Stores the enum as its number. Only for an enum whose number is the meaning (an external code).
        /// </summary>
        public PropertyBuilder<TProperty> HasEnumToIntegerConversion()
            => property.HasConversion<int>();

        #endregion
    }

    extension<TProperty>(PropertyBuilder<TProperty?> property) where TProperty : struct, Enum
    {
        #region "Methods"

        /// <inheritdoc cref="HasStringToEnumConversion{TProperty}(PropertyBuilder{TProperty})" />
        public PropertyBuilder<TProperty?> HasStringToEnumConversion()
            => property.HasConversion<string>();

        /// <inheritdoc cref="HasEnumToIntegerConversion{TProperty}(PropertyBuilder{TProperty})" />
        public PropertyBuilder<TProperty?> HasEnumToIntegerConversion()
            => property.HasConversion<int>();

        #endregion
    }

    extension<TProperty>(PropertyBuilder<TProperty> property)
    {
        #region "Methods"

        /// <summary>
        ///     Sets the property to a fixed-width character column; <c>text</c> (unbounded) when no length is given.
        /// </summary>
        /// <param name="length">The length, or <c>null</c> for unbounded text.</param>
        public PropertyBuilder<TProperty> HasCharColumn(int? length = null)
            => length is null
                ? property.HasColumnType("text")
                : property.HasColumnType($"char({length})").HasMaxLength(length.Value);

        /// <summary>
        ///     Sets the property to a decimal column; 18,4 is the standard for measured values.
        /// </summary>
        /// <param name="precision">Total number of digits, both sides of the decimal point.</param>
        /// <param name="scale">Digits right of the decimal point.</param>
        public PropertyBuilder<TProperty> HasDecimalColumn(int precision = 18, int scale = 4)
            => property.HasPrecision(precision, scale);

        /// <summary>
        ///     Sets the property to an unbounded text column.
        /// </summary>
        public PropertyBuilder<TProperty> HasUnboundedTextColumn()
            => property.HasColumnType("text");

        /// <summary>
        ///     Sets the property to a <c>jsonb</c> column. The property stays a <c>string</c>; the content has to read
        ///     without the application (self-describing keys, plain values).
        /// </summary>
        public PropertyBuilder<TProperty> HasJsonColumn()
            => property.HasColumnType("jsonb");

        #endregion
    }

    #endregion
}
