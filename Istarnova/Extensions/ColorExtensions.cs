#region "Header"

// File: ColorExtensions.cs
// Created 2026/03/06
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

#region "Usings"

using System.Drawing;

#endregion

namespace Istarnova.Extensions;

public static class ColorExtensions
{
    #region "Methods"

    extension(Color color)
    {
        #region "Methods"

        public string ToHtml() => ColorTranslator.ToHtml(color);

        #endregion
    }

    #endregion
}
