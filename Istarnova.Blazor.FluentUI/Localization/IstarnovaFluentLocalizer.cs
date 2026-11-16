#region "Header"

// File: IstarnovaFluentLocalizer.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using System.Globalization;
using System.Resources;
using Istarnova.Resources;
using Microsoft.FluentUI.AspNetCore.Components;

namespace Istarnova.Blazor.FluentUI.Localization;

/// <summary>
///     The texts FluentUI draws itself (grid menus, paginator, message box buttons) in the session's language. English,
///     German and Japanese come with the library; an application with more languages passes its own resources, which are
///     asked first. A key no resource has falls back to the library's English.
/// </summary>
public sealed class IstarnovaFluentLocalizer : IFluentLocalizer
{
    #region "Variables"

    private readonly ResourceManager? _applicationResources;

    #endregion

    #region "Constructor"

    /// <param name="applicationResources">
    ///     Resources with FluentUI's keys (<c>Paginator_GoFirstPage</c> and so on) in the application's languages, or
    ///     <c>null</c> to use the library's.
    /// </param>
    public IstarnovaFluentLocalizer(ResourceManager? applicationResources = null)
    {
        _applicationResources = applicationResources;
    }

    #endregion

    #region "Properties"

    public string this[string key, params object[] arguments]
    {
        get
        {
            var culture = CultureInfo.CurrentUICulture;

            var text = _applicationResources?.GetString(key, culture)
                       ?? FluentLibraryResources.ResourceManager.GetString(key, culture)
                       ?? IFluentLocalizer.GetDefault(key);

            return arguments.Length > 0
                ? string.Format(CultureInfo.CurrentCulture, text, arguments)
                : text;
        }
    }

    #endregion
}
