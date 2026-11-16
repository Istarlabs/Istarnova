#region "Header"

// File: DoubleExtensions.cs
// Created 2026/11/06
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

#region "Usings"

using System.Globalization;

#endregion

namespace Istarnova.Extensions;

/// <summary>
///     Conversions for numbers that a foreign format delivers as <see cref="double" />.
/// </summary>
public static class DoubleExtensions
{
    extension(double value)
    {
        #region "Methods"

        /// <summary>
        ///     The decimal the number was written as, so 50.55 stays 50.55. The cast <c>(decimal)value</c> keeps the binary
        ///     expansion instead (6.99 becomes 6.9900000000000002…), which moves a midpoint such as 50.55 to the wrong side
        ///     of a rounding.
        /// </summary>
        public decimal ToDecimal()
            => decimal.Parse(value.ToString(CultureInfo.InvariantCulture), NumberStyles.Float, CultureInfo.InvariantCulture);

        #endregion
    }
}
