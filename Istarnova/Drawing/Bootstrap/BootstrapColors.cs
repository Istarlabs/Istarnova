#region "Header"

// File: BootstrapColors.cs
// Created 2026/03/06
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

#region "Usings"

using System.Drawing;
using Istarnova.Extensions;

#endregion

namespace Istarnova.Drawing.Bootstrap;

public static class BootstrapColors
{
    #region "Bootstrap Colors"

    /// <summary>
    ///     Primary Color of Bootstrap.
    /// </summary>
    public static Color BootstrapPrimary => Color.FromArgb(0x37, 0x5a, 0x7f);

    /// <summary>
    ///     Indigo Color of Bootstrap.
    /// </summary>
    public static Color BootstrapIndigo => Color.FromArgb(0x66, 0x10, 0xf2);

    /// <summary>
    ///     Secondary Color of Bootstrap.
    /// </summary>
    public static Color BootstrapSecondary => Color.FromArgb(0x44, 0x44, 0x44);

    /// <summary>
    ///     Purple Color of Bootstrap.
    /// </summary>
    public static Color BootstrapPurple => Color.FromArgb(0x6f, 0x42, 0xc1);

    /// <summary>
    ///     Pink Color of Bootstrap.
    /// </summary>
    public static Color BootstrapPink => Color.FromArgb(0xe8, 0x3e, 0x8c);

    /// <summary>
    ///     Danger Color of Bootstrap.
    /// </summary>
    public static Color BootstrapDanger => Color.FromArgb(0xe7, 0x4c, 0x3c);

    /// <summary>
    ///     Orange Color of Bootstrap.
    /// </summary>
    public static Color BootstrapOrange => Color.FromArgb(0xfd, 0x7e, 0x14);

    /// <summary>
    ///     Warning Color of Bootstrap.
    /// </summary>
    public static Color BootstrapWarning => Color.FromArgb(0xf3, 0x9c, 0x12);

    /// <summary>
    ///     Success Color of Bootstrap.
    /// </summary>
    public static Color BootstrapSuccess => Color.FromArgb(0x00, 0xbc, 0x8c);

    /// <summary>
    ///     Teal Color of Bootstrap.
    /// </summary>
    public static Color BootstrapTeal => Color.FromArgb(0x20, 0xc9, 0x97);

    /// <summary>
    ///     Dark Color of Bootstrap.
    /// </summary>
    public static Color BootstrapDark => Color.FromArgb(0x30, 0x30, 0x30);

    /// <summary>
    ///     Info Color of Bootstrap.
    /// </summary>
    public static Color BootstrapInfo => Color.FromArgb(0x34, 0x98, 0xdb);

    /// <summary>
    ///     Light Color of Bootstrap.
    /// </summary>
    public static Color BootstrapLight => Color.FromArgb(0xad, 0xb5, 0xbd);

    #endregion

    #region "Bootstrap Colors (HTML)"

    /// <summary>
    ///     Primary Color of Bootstrap.
    /// </summary>
    public static string BootstrapPrimaryHtml => BootstrapPrimary.ToHtml();

    /// <summary>
    ///     Indigo Color of Bootstrap.
    /// </summary>
    public static string BootstrapIndigoHtml => BootstrapIndigo.ToHtml();

    /// <summary>
    ///     Secondary Color of Bootstrap.
    /// </summary>
    public static string BootstrapSecondaryHtml => BootstrapSecondary.ToHtml();

    /// <summary>
    ///     Purple Color of Bootstrap.
    /// </summary>
    public static string BootstrapPurpleHtml => BootstrapPurple.ToHtml();

    /// <summary>
    ///     Pink Color of Bootstrap.
    /// </summary>
    public static string BootstrapPinkHtml => BootstrapPink.ToHtml();

    /// <summary>
    ///     Danger Color of Bootstrap.
    /// </summary>
    public static string BootstrapDangerHtml => BootstrapDanger.ToHtml();

    /// <summary>
    ///     Orange Color of Bootstrap.
    /// </summary>
    public static string BootstrapOrangeHtml => BootstrapOrange.ToHtml();

    /// <summary>
    ///     Warning Color of Bootstrap.
    /// </summary>
    public static string BootstrapWarningHtml => BootstrapWarning.ToHtml();

    /// <summary>
    ///     Success Color of Bootstrap.
    /// </summary>
    public static string BootstrapSuccessHtml => BootstrapSuccess.ToHtml();

    /// <summary>
    ///     Teal Color of Bootstrap.
    /// </summary>
    public static string BootstrapTealHtml => BootstrapTeal.ToHtml();

    /// <summary>
    ///     Dark Color of Bootstrap.
    /// </summary>
    public static string BootstrapDarkHtml => BootstrapDark.ToHtml();

    /// <summary>
    ///     Info Color of Bootstrap.
    /// </summary>
    public static string BootstrapInfoHtml => BootstrapInfo.ToHtml();

    /// <summary>
    ///     Light Color of Bootstrap.
    /// </summary>
    public static string BootstrapLightHtml => BootstrapLight.ToHtml();

    #endregion

    #region "Bootstrap Colors (CSS Variables)"
    public static class CssColors
    {
        public const string Blue = "var(--bs-blue)";
        public const string Indigo = "var(--bs-indigo)";
        public const string Purple = "var(--bs-purple)";
        public const string Pink = "var(--bs-pink)";
        public const string Red = "var(--bs-red)";
        public const string Orange = "var(--bs-orange)";
        public const string Yellow = "var(--bs-yellow)";
        public const string Green = "var(--bs-green)";
        public const string Teal = "var(--bs-teal)";
        public const string Cyan = "var(--bs-cyan)";
        public const string Black = "var(--bs-black)";
        public const string White = "var(--bs-white)";
        public const string Gray = "var(--bs-gray)";
        public const string GrayDark = "var(--bs-gray-dark)";
        public const string Gray100 = "var(--bs-gray-100)";
        public const string Gray200 = "var(--bs-gray-200)";
        public const string Gray300 = "var(--bs-gray-300)";
        public const string Gray400 = "var(--bs-gray-400)";
        public const string Gray500 = "var(--bs-gray-500)";
        public const string Gray600 = "var(--bs-gray-600)";
        public const string Gray700 = "var(--bs-gray-700)";
        public const string Gray800 = "var(--bs-gray-800)";
        public const string Gray900 = "var(--bs-gray-900)";
        public const string Primary = "var(--bs-primary)";
        public const string Secondary = "var(--bs-secondary)";
        public const string Success = "var(--bs-success)";
        public const string Info = "var(--bs-info)";
        public const string Warning = "var(--bs-warning)";
        public const string Danger = "var(--bs-danger)";
        public const string Light = "var(--bs-light)";
        public const string Dark = "var(--bs-dark)";
    }


    #endregion
}
