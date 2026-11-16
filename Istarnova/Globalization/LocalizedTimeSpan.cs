#region "Header"

// File: LocalizedTimeSpan.cs
// Created 2026/03/06
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

#region "Usings"

using Istarnova.Resources;

#endregion

namespace Istarnova.Globalization;

public sealed class LocalizedTimeSpan
{
    #region "Constructor"

    public LocalizedTimeSpan(DateTime? startDate, DateTime? endDate)
    {
        if (!startDate.HasValue)
        {
            throw new ArgumentNullException(nameof(startDate));
        }

        endDate ??= DateTime.Now;

        var s = startDate.Value;
        var e = endDate.Value;

        if (s > e)
        {
            (s, e) = (e, s);
        }

        var startYear = s.Year;
        var startMonth = s.Month;
        var startDay = s.Day;

        var endYear = e.Year;
        var endMonth = e.Month;
        var endDay = e.Day;

        if (endDay < startDay)
        {
            var previousMonth = e.AddMonths(-1);

            endDay += DateTime.DaysInMonth(previousMonth.Year, previousMonth.Month);
            endMonth -= 1;

            if (endMonth < 1)
            {
                endMonth += 12;
                endYear -= 1;
            }
        }

        if (endMonth < startMonth)
        {
            endMonth += 12;
            endYear -= 1;
        }

        Years = endYear - startYear;
        Months = endMonth - startMonth;
        Days = endDay - startDay;
    }

    #endregion

    #region "Properties"

    public int Days { get; }

    public int Months { get; }

    public int Years { get; }

    #endregion

    #region "Methods"

    public override string ToString()
    {
        if (Years == 0 && Months == 0 && Days == 0)
        {
            return string.Empty;
        }

        var hasPlural = DateResources.TimeSpanPlural != "no";
        var yearString = Years == 0
            ? string.Empty
            : $"{Years} {DateResources.Year}{(hasPlural && Years > 1 ? DateResources.TimeSpanPlural : string.Empty)} ";
        var monthString = Months == 0
            ? string.Empty
            : $"{Months} {DateResources.Month}{(hasPlural && Months > 1 ? DateResources.TimeSpanPlural : string.Empty)} ";
        var dayString =
            $"{Days} {DateResources.Day}{(hasPlural && Days != 1 ? DateResources.TimeSpanPlural : string.Empty)} ";

        return $"{yearString}{monthString}{dayString}";
    }

    #endregion
}
