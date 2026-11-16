#region "Header"

// File: AutoEquatableAttribute.cs
// Created 2026/02/17
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

namespace Istarnova.SourceGenerator.Attributes.Data.Equatable;

/// <summary>
///     Marks an entity for automatic IEquatable generation based on Primary Key.
///     The source generator will generate Equals, GetHashCode, and optionally == and != operators.
/// </summary>
/// <remarks>
///     By default, the generator uses the convention {ClassName}Id to find the key property.
///     For composite keys or non-standard naming, use the KeyProperties parameter.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class AutoEquatableAttribute : Attribute
{
    #region "Properties"

    /// <summary>
    ///     Whether to generate == and != operators. Default: true
    /// </summary>
    public bool GenerateOperators { get; set; } = true;

    /// <summary>
    ///     Optional: Explicit key property name(s), comma-separated for composite keys.
    ///     If not set, uses convention: {ClassName}Id
    /// </summary>
    /// <example>
    ///     Single key: KeyProperties = "CustomId"
    ///     Composite key: KeyProperties = "UserId, PatientId"
    /// </example>
    public string? KeyProperties { get; set; }

    #endregion
}
