#region "Header"

// File: LocalizedDescriptionAttribute.cs
// Created 2026/08/22
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

#region "Usings"

using System.ComponentModel;
using System.Resources;

#endregion

namespace Istarnova.SourceGenerator.Attributes.Common.Enums;

/// <summary>
///     A description that comes from a resource file instead of a literal, so an enum member can carry a
///     display name in every language the resources are translated into.
/// </summary>
/// <remarks>
///     <para>
///         Recognised by the <c>[EnumExtras]</c> generator: a member marked with it resolves through the
///         resource type's <see cref="ResourceManager" /> at runtime, so the name follows
///         <c>CultureInfo.CurrentUICulture</c> rather than being baked in at compile time. That is the whole
///         difference to <see cref="DescriptionAttribute" /> and <c>[Display]</c>, which the generator can
///         fold into a constant.
///     </para>
///     <para>
///         It also derives from <see cref="DescriptionAttribute" />, so anything that reads descriptions by
///         reflection - a property grid, a serializer, an older helper - sees the localized text too, without
///         knowing this type exists.
///     </para>
/// </remarks>
/// <example>
///     <code>
/// [EnumExtensions]
/// [EnumExtras]
/// public enum OrderState
/// {
///     [LocalizedDescription(nameof(OrderResources.Shipped), typeof(OrderResources))]
///     Shipped,
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.All)]
public sealed class LocalizedDescriptionAttribute : DescriptionAttribute
{
    #region "Variables"

    private readonly string _resourceKey;
    private readonly ResourceManager _resourceManager;

    #endregion

    #region "Constructor"

    /// <param name="resourceKey">Key of the string in the resource file.</param>
    /// <param name="resourceType">The generated resource class, e.g. <c>typeof(OrderResources)</c>.</param>
    public LocalizedDescriptionAttribute(string resourceKey, Type resourceType)
    {
        _resourceKey = resourceKey;
        _resourceManager = new(resourceType);
    }

    #endregion

    #region "Properties"

    /// <summary>
    ///     The translated text for the current UI culture, or <c>[[key]]</c> when the resource file has
    ///     no such key.
    /// </summary>
    /// <remarks>
    ///     The bracketed key is deliberate: a missing translation shows up as something obviously wrong
    ///     on screen, where falling back to the key alone or to an empty string would pass for a label
    ///     nobody wrote.
    /// </remarks>
    public override string Description
    {
        get
        {
            var displayName = _resourceManager.GetString(_resourceKey);

            return string.IsNullOrEmpty(displayName)
                ? $"[[{_resourceKey}]]"
                : displayName!;
        }
    }

    #endregion
}
