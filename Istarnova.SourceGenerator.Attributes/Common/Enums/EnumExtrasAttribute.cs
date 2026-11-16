#region "Header"

// File: EnumExtrasAttribute.cs
// Created 2026/08/09
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

namespace Istarnova.SourceGenerator.Attributes.Common.Enums;

/// <summary>
///     Opt-in for the Istarnova enum convenience layer generated on top of
///     NetEscapades.EnumGenerators. Adds a second <c>partial</c> part to the
///     <c>{Enum}Extensions</c> class produced by NetEscapades, providing display-name resolution
///     (including runtime-localized <c>[LocalizedDescription]</c>), <c>EnumKeyValue</c> list helpers,
///     circular navigation and (for <c>[Flags]</c> enums) flag helpers.
/// </summary>
/// <remarks>
///     Requires <c>[EnumExtensions]</c> (NetEscapades) on the same enum: the generated code calls the
///     NetEscapades siblings (<c>GetValues()</c>, <c>ToStringFast()</c>) directly.
/// </remarks>
[AttributeUsage(AttributeTargets.Enum)]
public sealed class EnumExtrasAttribute : Attribute
{
    #region "Properties"

    /// <summary>
    ///     Generate display-name helpers (<c>GetDisplayName</c>, <c>GetDisplayList</c>,
    ///     <c>TryParseByDisplayName</c>) from <c>[Display]</c>/<c>[Description]</c>/<c>[LocalizedDescription]</c>.
    ///     Default: <c>true</c>.
    /// </summary>
    public bool GenerateDisplayNames { get; set; } = true;

    /// <summary>
    ///     Generate <c>CombineFlags</c>/<c>GetFlags</c> helpers. Only honored when the enum is
    ///     decorated with <c>[Flags]</c>. Default: <c>true</c>.
    /// </summary>
    public bool GenerateFlagHelpers { get; set; } = true;

    /// <summary>
    ///     Generate circular <c>GetNext</c>/<c>GetPrevious</c> navigation helpers. Default: <c>true</c>.
    /// </summary>
    public bool GenerateNavigation { get; set; } = true;

    #endregion
}
