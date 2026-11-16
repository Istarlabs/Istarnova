#region "Header"

// File: MathCompare.cs
// Created 2026/03/06
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

#region "Usings"

using System.Numerics;

#endregion

namespace Istarnova.Numerics;

public sealed class MathCompare
{
    #region "Methods"

    public static T GreatestCommonDivisor<T>(T a, T b)
        where T : struct, IComparable, IEquatable<T>, INumberBase<T>, IModulusOperators<T, T, T>
    {
        while (b != T.Zero)
        {
            var temp = b;
            b = a % b;
            a = temp;
        }

        return a;
    }

    public static T LowestCommonMultiple<T>(T a, T b)
        where T : struct, IComparable, IEquatable<T>, INumberBase<T>, IModulusOperators<T, T, T>
        => a * b / GreatestCommonDivisor(a, b);

    #endregion
}
