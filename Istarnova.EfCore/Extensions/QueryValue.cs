#region "Header"

// File: QueryValue.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using System.Linq.Expressions;

namespace Istarnova.EfCore.Extensions;

/// <summary>
///     Puts a value into a hand-built expression tree the way EF Core wants to see it.
/// </summary>
public static class QueryValue
{
    #region "Methods"

    /// <summary>
    ///     A value as something EF Core turns into a query parameter.
    /// </summary>
    public static MemberExpression Captured<TValue>(TValue value)
        => Expression.Property(Expression.Constant(new Box<TValue>(value)), nameof(Box<TValue>.Value));

    #endregion

    #region "Types"

    /// <summary>
    ///     Holds the value so the tree reads it off an object instead of carrying it - the shape the compiler produces
    ///     for a captured variable.
    /// </summary>
    private sealed record Box<TValue>(TValue Value);

    #endregion
}
