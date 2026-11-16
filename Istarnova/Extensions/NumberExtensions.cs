#region "Header"

// File: NumberExtensions.cs
// Created 2026/08/20
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

#region "Usings"

using System.Numerics;

#endregion

namespace Istarnova.Extensions;

public static class NumberExtensions
{
    #region "Methods"

    extension<TNumber>(TNumber number) where TNumber : struct, IFloatingPoint<TNumber>
    {
        /// <summary>
        ///     Whether both numbers lie within <paramref name="tolerance" /> of each other. The default of zero is
        ///     an exact comparison, which is right for stored values; a tolerance only belongs where a value was
        ///     calculated, and only the caller knows how coarse its unit is.
        /// </summary>
        public bool EqualsTo(TNumber other, TNumber tolerance = default)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(tolerance);

            return number == other || TNumber.Abs(number - other) <= tolerance;
        }

        /// <summary>
        ///     The number rounded to <paramref name="decimals" /> places; a half goes to the even neighbour, as with
        ///     <see cref="Math.Round(decimal, int)" />.
        /// </summary>
        public TNumber Round(int decimals)
            => TNumber.Round(number, decimals);
    }

    extension<TNumber>(TNumber? number) where TNumber : struct, IFloatingPoint<TNumber>
    {
        /// <summary>
        ///     As the value overload; two missing numbers are equal, and a missing number never equals zero.
        /// </summary>
        public bool EqualsTo(TNumber? other, TNumber tolerance = default)
            => (number, other) switch
            {
                (null, null) => true,
                ({ } left, { } right) => left.EqualsTo(right, tolerance),
                var _ => false,
            };
    }

    extension<T>(T number) where T : INumber<T>
    {
        #region "Methods"

        public string MoneyTwoDigitFormatted() =>
            number.ToString("C2", System.Globalization.CultureInfo.InvariantCulture);

        /// <summary>
        ///     An amount written the way the reader's culture writes numbers, in the currency the amount
        ///     is actually in.
        /// </summary>
        /// <param name="currencyCode">
        ///     The ISO 4217 code the amount is in, or null when nothing recorded which currency it was.
        ///     A null code prints the bare number: claiming a currency nobody stated is worse than
        ///     leaving the question open.
        /// </param>
        /// <remarks>
        ///     <para>
        ///         The culture decides the separators and where the currency goes. It must never decide
        ///         <b>which</b> currency: someone treated abroad paid in the currency of that country, and
        ///         rendering 4500 INR as "4.500,00 €" because the reader sits in Germany turns a record
        ///         into a false statement about money.
        ///     </para>
        ///     <para>
        ///         The code is printed as-is rather than swapped for a symbol. "$" alone does not say
        ///         which dollar, and a record that has to be understood years later by someone else is
        ///         exactly where that ambiguity must not be introduced.
        ///     </para>
        /// </remarks>
        public string MoneyTwoDigitLocalized(string? currencyCode)
        {
            var culture = System.Globalization.CultureInfo.CurrentCulture;

            if (string.IsNullOrWhiteSpace(currencyCode))
            {
                return number.ToString("#,0.00", culture);
            }

            var format = (System.Globalization.NumberFormatInfo)culture.NumberFormat.Clone();
            format.CurrencySymbol = currencyCode.Trim().ToUpperInvariant();

            return number.ToString("C2", format);
        }

        public string ToInvariantFloatingPointString() =>
            number.ToString(null, System.Globalization.CultureInfo.InvariantCulture);

        public string TwoDigitFormatted() =>
            number.ToString("#,0.00", System.Globalization.CultureInfo.InvariantCulture);

        #endregion
    }

    extension<TNumber>(TNumber number) where TNumber : IUnaryNegationOperators<TNumber, TNumber>
    {
        #region "Methods"

        public TNumber Negate() => -number;

        #endregion
    }

    extension<TNumber>(TNumber? number)
        where TNumber : struct, INumber<TNumber>, IUnaryNegationOperators<TNumber, TNumber>
    {
        #region "Methods"

        public TNumber NegateOrDefault() => number?.Negate() ?? TNumber.Zero;

        #endregion
    }

    extension<TNumber>(TNumber? number) where TNumber : struct, INumber<TNumber>
    {
        #region "Methods"

        /// <summary>
        ///     An amount of money as <see cref="NumberExtensions" />' value overload writes it, or
        ///     <paramref name="placeholder" /> where there is no amount.
        /// </summary>
        /// <remarks>
        ///     A missing amount is not zero, and printing 0.00 for one states something the record never
        ///     said. The placeholder is the caller's to choose because it is a matter of house style -
        ///     an em dash on a printed record, an empty cell in a table, "not stated" in a sentence.
        /// </remarks>
        public string MoneyTwoDigitLocalized(string? currencyCode, string placeholder) =>
            number?.MoneyTwoDigitLocalized(currencyCode) ?? placeholder;

        #endregion
    }

    extension<TNumber>(TNumber number)
        where TNumber : INumber<TNumber>, IParsable<TNumber>, IComparisonOperators<TNumber, TNumber, bool>
    {
        #region "Methods"

        public bool IsInRange(string strVal, IFormatProvider? formatProvider = null)
        {
            var num = TNumber.Parse(strVal, formatProvider);

            return number >= num && number <= num;
        }

        #endregion
    }

    extension<TNumber1, TNumber2>(TNumber1 number)
        where TNumber1 : INumber<TNumber1>, IComparisonOperators<TNumber1, TNumber2, bool>
        where TNumber2 : INumber<TNumber2>
    {
        #region "Methods"

        public bool IsBetween(TNumber2 min, TNumber2 max) => number >= min && number <= max;

        #endregion
    }

    extension<TNumber>(TNumber number)
        where TNumber : IFloatingPoint<TNumber>,
        IModulusOperators<TNumber, TNumber, TNumber>,
        ISubtractionOperators<TNumber, TNumber, TNumber>
    {
        #region "Methods"

        public string ToPercentageFormatted() => number % TNumber.One == TNumber.Zero
            ? number.ToString("N0", null) + '%'
            : number.ToString("#,0.00", null) + '%';

        #endregion
    }

    #endregion
}
