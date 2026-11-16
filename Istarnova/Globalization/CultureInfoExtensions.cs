#region "Header"

// File: CultureInfoExtensions.cs
// Created 2026/10/27
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

#region "Usings"

using System.Diagnostics.CodeAnalysis;
using System.Globalization;

#endregion

namespace Istarnova.Globalization;

public static class CultureInfoExtensions
{
    #region "Methods"

    extension(CultureInfo cultureInfo)
    {
        /// <summary>
        ///     Whether the culture is a language, whatever the region: "de-AT" and "de-CH" both match "de".
        /// </summary>
        /// <param name="twoLetterCode">The ISO 639-1 code, in any case.</param>
        public bool CompareTwoLetter(string twoLetterCode)
            => cultureInfo.TwoLetterISOLanguageName.Equals(twoLetterCode, StringComparison.OrdinalIgnoreCase);

        /// <summary>
        ///     Whether both cultures are the same language, whatever the region.
        /// </summary>
        public bool HasSameLanguageAs(CultureInfo other)
            => cultureInfo.CompareTwoLetter(other.TwoLetterISOLanguageName);

        /// <summary>
        ///     The culture without its region: "de-AT" becomes "de". A culture that has none is returned as it is.
        /// </summary>
        public CultureInfo GetNeutralCulture()
            => cultureInfo.IsNeutralCulture || cultureInfo.Parent == CultureInfo.InvariantCulture
                ? cultureInfo
                : cultureInfo.Parent;

        /// <summary>
        ///     The short name of a weekday in this culture.
        /// </summary>
        public string GetAbbreviatedDayName(DayOfWeek day)
            => cultureInfo.DateTimeFormat.GetAbbreviatedDayName(day);

        /// <summary>
        ///     The short name of a month in this culture.
        /// </summary>
        /// <param name="month">The month, 1 to 12.</param>
        public string GetAbbreviatedMonthName(int month)
            => cultureInfo.DateTimeFormat.GetAbbreviatedMonthName(month);

        /// <summary>
        ///     The day a week starts on in this culture.
        /// </summary>
        public DayOfWeek FirstDayOfWeek
            => cultureInfo.DateTimeFormat.FirstDayOfWeek;
    }

    extension(CultureInfo)
    {
        /// <summary>
        ///     The culture for a name, or false where there is none - the name may come from a cookie or a stored setting.
        /// </summary>
        /// <param name="name">A culture name such as "de" or "pt-PT". Null or blank never matches.</param>
        /// <param name="cultureInfo">The culture, when there is one.</param>
        public static bool TryGet(string? name, [NotNullWhen(true)] out CultureInfo? cultureInfo)
        {
            cultureInfo = null;

            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            try
            {
                cultureInfo = CultureInfo.GetCultureInfo(name);

                return true;
            }
            catch (CultureNotFoundException)
            {
                return false;
            }
        }
    }

    #endregion
}
