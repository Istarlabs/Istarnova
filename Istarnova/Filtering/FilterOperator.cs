#region "Header"

// File: FilterOperator.cs
// Created 2026/10/28
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

#region "Usings"

using Istarnova.Resources;
using Istarnova.SourceGenerator.Attributes.Common.Enums;
using NetEscapades.EnumGenerators;

#endregion

namespace Istarnova.Filtering;

/// <summary>
///     How a <see cref="FilterRule" /> compares a property with its value.
/// </summary>
[EnumExtensions]
[EnumExtras]
public enum FilterOperator
{
    [LocalizedDescription(nameof(FilterResources.Contains), typeof(FilterResources))]
    Contains,

    [LocalizedDescription(nameof(FilterResources.IsEqualTo), typeof(FilterResources))]
    IsEqualTo,

    [LocalizedDescription(nameof(FilterResources.IsNotEqualTo), typeof(FilterResources))]
    IsNotEqualTo,

    [LocalizedDescription(nameof(FilterResources.GreaterThan), typeof(FilterResources))]
    GreaterThan,

    [LocalizedDescription(nameof(FilterResources.GreaterThanOrEqualTo), typeof(FilterResources))]
    GreaterThanOrEqualTo,

    [LocalizedDescription(nameof(FilterResources.LessThan), typeof(FilterResources))]
    LessThan,

    [LocalizedDescription(nameof(FilterResources.LessThanOrEqualTo), typeof(FilterResources))]
    LessThanOrEqualTo,

    [LocalizedDescription(nameof(FilterResources.IsEmpty), typeof(FilterResources))]
    IsEmpty,

    [LocalizedDescription(nameof(FilterResources.IsNotEmpty), typeof(FilterResources))]
    IsNotEmpty,

    [LocalizedDescription(nameof(FilterResources.IsTrue), typeof(FilterResources))]
    IsTrue,

    [LocalizedDescription(nameof(FilterResources.IsFalse), typeof(FilterResources))]
    IsFalse,

    [LocalizedDescription(nameof(FilterResources.StartsWith), typeof(FilterResources))]
    StartsWith,

    [LocalizedDescription(nameof(FilterResources.EndsWith), typeof(FilterResources))]
    EndsWith,

    [LocalizedDescription(nameof(FilterResources.NotContains), typeof(FilterResources))]
    NotContains,
}
