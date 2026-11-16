#region "Header"

// File: CultureScope.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using System.Globalization;

namespace Istarnova.Tests.BlazorFluentUI;

/// <summary>
///     Runs a test in the language of a session and puts the previous one back afterwards.
/// </summary>
public sealed class CultureScope : IDisposable
{
    #region "Variables"

    private readonly CultureInfo _previous = CultureInfo.CurrentUICulture;

    private readonly CultureInfo _previousCulture = CultureInfo.CurrentCulture;

    #endregion

    #region "Constructor"

    public CultureScope(string language)
    {
        CultureInfo.CurrentUICulture = new CultureInfo(language);
        CultureInfo.CurrentCulture = new CultureInfo(language);
    }

    #endregion

    #region "Methods"

    public void Dispose()
    {
        CultureInfo.CurrentUICulture = _previous;
        CultureInfo.CurrentCulture = _previousCulture;
    }

    #endregion
}
