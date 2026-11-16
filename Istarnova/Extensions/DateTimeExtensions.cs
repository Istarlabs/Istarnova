#region "Header"

// File: DateTimeExtensions.cs
// Created 2026/03/06
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

#region "Usings"

using System.Globalization;

#endregion

namespace Istarnova.Extensions;

/// <summary>
///     Modern DateTime extension methods with precise calculations and comprehensive error handling.
/// </summary>
public static class DateTimeExtensions
{
    #region "Enums"

    public enum DateComparison
    {
        GreaterThanEqual,
        LessThanEqual,
        Equal,
        NotEqual,
        GreaterThan,
        LessThan,
    }

    #endregion

    #region "Methods"

    /// <param name="dateTime">Date of birth</param>
    extension(DateTime dateTime)
    {
        #region "Methods"

        public bool CompareDateOnly(DateTime? dt2, DateComparison comparison)
        {
            if (!dt2.HasValue)
            {
                return false;
            }

            return comparison switch
            {
                DateComparison.GreaterThanEqual => dateTime.Date >= dt2.Value.Date,
                DateComparison.LessThanEqual => dateTime.Date <= dt2.Value.Date,
                DateComparison.Equal => dateTime.Date == dt2.Value.Date,
                DateComparison.NotEqual => dateTime.Date != dt2.Value.Date,
                DateComparison.GreaterThan => dateTime.Date > dt2.Value.Date,
                DateComparison.LessThan => dateTime.Date < dt2.Value.Date,
                var _ => throw new ArgumentOutOfRangeException(nameof(comparison), "Invalid comparison type."),
            };
        }

        /// <summary>
        ///     Calculates the precise age in years, considering actual calendar dates.
        /// </summary>
        /// <param name="referenceDate">Reference date for calculation (defaults to today)</param>
        /// <returns>Age in completed years</returns>
        /// <exception cref="ArgumentException">Thrown when date of birth is in the future</exception>
        public int GetAge(DateTime? referenceDate = null)
        {
            var refDate = referenceDate ?? DateTime.Today; // Use Today instead of Now for date calculations

            if (dateTime.Date > refDate.Date)
            {
                throw new ArgumentException("Date of birth cannot be in the future", nameof(dateTime));
            }

            // Calculate age by comparing actual dates, not just days
            var age = refDate.Year - dateTime.Year;

            // Subtract one year if birthday hasn't occurred yet this year
            if (refDate.Month < dateTime.Month ||
                (refDate.Month == dateTime.Month && refDate.Day < dateTime.Day))
            {
                age--;
            }

            return age;
        }

        /// <summary>
        ///     Calculates age in months with precision.
        /// </summary>
        /// <param name="referenceDate">Reference date for calculation (defaults to today)</param>
        /// <exception cref="ArgumentException"></exception>
        /// <returns>Age in completed months</returns>
        public int GetAgeInMonths(DateTime? referenceDate = null)
        {
            var refDate = referenceDate ?? DateTime.Today;

            if (dateTime.Date > refDate.Date)
            {
                throw new ArgumentException("Date of birth cannot be in the future", nameof(dateTime));
            }

            var months = (refDate.Year - dateTime.Year) * 12 + refDate.Month - dateTime.Month;

            // Subtract one month if the day of the month hasn't been reached yet
            if (refDate.Day < dateTime.Day)
            {
                months--;
            }

            return Math.Max(0, months);
        }

        /// <summary>
        ///     Gets age with detailed information including days until next birthday.
        /// </summary>
        /// <param name="referenceDate">Reference date for calculation (defaults to today)</param>
        /// <returns>Detailed age information</returns>
        public AgeInfo GetDetailedAge(DateTime? referenceDate = null)
        {
            var refDate = referenceDate ?? DateTime.Today;

            if (dateTime.Date > refDate.Date)
            {
                throw new ArgumentException("Date of birth cannot be in the future", nameof(dateTime));
            }

            var age = dateTime.GetAge(refDate);

            // Calculate next birthday
            var nextBirthday = new DateTime(refDate.Year, dateTime.Month, dateTime.Day);

            // Handle February 29th for non-leap years
            if (dateTime is { Month: 2, Day: 29 } && !DateTime.IsLeapYear(refDate.Year))
            {
                nextBirthday = new(refDate.Year, 2, 28);
            }

            // If birthday already passed this year, use next year
            if (nextBirthday < refDate)
            {
                var nextYear = refDate.Year + 1;
                nextBirthday = new(nextYear, dateTime.Month, dateTime.Day);

                // Handle February 29th for next year
                if (dateTime is { Month: 2, Day: 29 } && !DateTime.IsLeapYear(nextYear))
                {
                    nextBirthday = new(nextYear, 2, 28);
                }
            }

            var daysUntilBirthday = (nextBirthday - refDate).Days;
            var totalDaysLived = (refDate - dateTime.Date).Days;

            return new(age, daysUntilBirthday, totalDaysLived, nextBirthday.Date);
        }

        /// <summary>
        ///     Calculates precise age including fractional years.
        /// </summary>
        /// <param name="referenceDate">Reference date for calculation (defaults to today)</param>
        /// <returns>Age as decimal with fractional part</returns>
        public decimal GetPreciseAge(DateTime? referenceDate = null)
        {
            var refDate = referenceDate ?? DateTime.Today;

            if (dateTime.Date > refDate.Date)
            {
                throw new ArgumentException("Date of birth cannot be in the future", nameof(dateTime));
            }

            var age = dateTime.GetAge(refDate);

            // Calculate the fractional part based on days since last birthday
            var lastBirthday = new DateTime(refDate.Year, dateTime.Month, dateTime.Day);

            // Handle February 29th
            if (dateTime is { Month: 2, Day: 29 } && !DateTime.IsLeapYear(refDate.Year))
            {
                lastBirthday = new(refDate.Year, 2, 28);
            }

            // If birthday is in the future this year, use last year's birthday
            if (lastBirthday > refDate)
            {
                var lastYear = refDate.Year - 1;
                lastBirthday = new(lastYear, dateTime.Month, dateTime.Day);

                if (dateTime is { Month: 2, Day: 29 } && !DateTime.IsLeapYear(lastYear))
                {
                    lastBirthday = new(lastYear, 2, 28);
                }
            }

            var daysSinceLastBirthday = (refDate - lastBirthday).Days;
            var daysInCurrentYear = DateTime.IsLeapYear(refDate.Year) ? 366 : 365;
            var fractionalPart = (decimal)daysSinceLastBirthday / daysInCurrentYear;

            return age + fractionalPart;
        }

        /// <summary>
        ///     Determines if a person is a specific age or older.
        /// </summary>
        /// <param name="minimumAge">Minimum age to check</param>
        /// <param name="referenceDate">Reference date for calculation (defaults to today)</param>
        /// <returns>True if person is at least the minimum age</returns>
        public bool IsAtLeast(int minimumAge, DateTime? referenceDate = null) =>
            dateTime.GetAge(referenceDate) >= minimumAge;

        /// <summary>
        ///     Determines if today is the person's birthday.
        /// </summary>
        /// <param name="referenceDate">Reference date to check (defaults to today)</param>
        /// <returns>True if it's the person's birthday</returns>
        public bool IsBirthday(DateTime? referenceDate = null)
        {
            var refDate = referenceDate ?? DateTime.Today;

            // Handle February 29th birthdays on non-leap years
            if (dateTime is { Month: 2, Day: 29 } && !DateTime.IsLeapYear(refDate.Year))
            {
                return refDate is { Month: 2, Day: 28 };
            }

            return refDate.Month == dateTime.Month && refDate.Day == dateTime.Day;
        }

        public DateTime SetDay(int day)
        {
            if (day < 1 || day > DateTime.DaysInMonth(dateTime.Year, dateTime.Month))
            {
                throw new ArgumentOutOfRangeException(nameof(day),
                    "Day must be between 1 and the number of days in the month.");
            }

            return new(dateTime.Year, dateTime.Month, day, dateTime.Hour, dateTime.Minute, dateTime.Second,
                dateTime.Kind);
        }

        public int ToYearMonthString() => dateTime.Year * 100 + dateTime.Month;

        #endregion
    }

    /// <param name="dt1">Nullable date of birth</param>
    extension(DateTime? dt1)
    {
        #region "Methods"

        public bool CompareDateOnly(DateTime? dt2, DateComparison comparison)
            => dt1.HasValue && dt1.Value.CompareDateOnly(dt2, comparison);

        /// <summary>
        ///     Calculates the age for nullable DateTime, returning 0 if null.
        /// </summary>
        /// <param name="referenceDate">Reference date for calculation (defaults to today)</param>
        /// <returns>Age in completed years, or 0 if dateOfBirth is null</returns>
        public int GetAge(DateTime? referenceDate = null)
            => dt1?.GetAge(referenceDate) ?? 0;

        public DateTime? SetDay(int day) => dt1?.SetDay(day);

        #endregion
    }

    extension(DateOnly date)
    {
        #region "Methods"

        /// <summary>
        ///     The day and the month without the year, in the order of the culture and with the month cut to three letters:
        ///     "8. Nov." in German, "Nov 8" in English. No standard format specifier does this: "m" spells the month out.
        /// </summary>
        /// <param name="provider">The culture; the current one when null.</param>
        public string AsShortMonthDayString(IFormatProvider? provider = null)
            => date.ToString(ShortMonthDayPattern(provider), provider ?? CultureInfo.CurrentCulture);

        #endregion
    }

    extension(DateTime dateTime)
    {
        #region "Methods"

        /// <inheritdoc cref="AsShortMonthDayString(DateOnly, IFormatProvider)" />
        public string AsShortMonthDayString(IFormatProvider? provider = null)
            => dateTime.ToString(ShortMonthDayPattern(provider), provider ?? CultureInfo.CurrentCulture);

        #endregion
    }

    extension(int month)
    {
        #region "Methods"

        public string AsMonthNameString()
            => CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);

        public string AsShortMonthString()
            => CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(month);

        #endregion
    }

    private static string ShortMonthDayPattern(IFormatProvider? provider)
        => DateTimeFormatInfo.GetInstance(provider ?? CultureInfo.CurrentCulture).MonthDayPattern
            .Replace("MMMM", "MMM", StringComparison.Ordinal);

    public static List<string> GetNamedMonths() =>
        [.. Enumerable.Range(1, 12).Select(x => x.AsMonthNameString())];

    public static int ToYearMonthString(int year, int month) => year * 100 + month;

    #endregion
}

/// <summary>
///     Detailed age information with additional metrics.
/// </summary>
/// <param name="Years">Age in completed years</param>
/// <param name="DaysUntilNextBirthday">Days until next birthday</param>
/// <param name="TotalDaysLived">Total days lived since birth</param>
/// <param name="NextBirthday">Date of next birthday</param>
public readonly record struct AgeInfo(
    int Years,
    int DaysUntilNextBirthday,
    int TotalDaysLived,
    DateTime NextBirthday)
{
    #region "Methods"

    /// <summary>
    ///     Returns a formatted string representation of the age information.
    /// </summary>
    /// <returns>Formatted age string</returns>
    public override string ToString() =>
        $"{Years} years old, {DaysUntilNextBirthday} days until birthday ({NextBirthday:MMM dd})";

    #endregion
}
