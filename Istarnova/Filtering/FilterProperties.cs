#region "Header"

// File: FilterProperties.cs
// Created 2026/11/07
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

#region "Usings"

using System.Linq.Expressions;

#endregion

namespace Istarnova.Filtering;

/// <summary>
///     Makes a <see cref="FilterProperty{TItem}" /> from one expression, so the same property filters a list in memory and a
///     query on the database.
/// </summary>
public static class FilterProperties
{
    #region "Methods"

    /// <summary>
    ///     A text property.
    /// </summary>
    public static FilterProperty<TItem> Text<TItem>(string name, string title, Expression<Func<TItem, string?>> selector)
        => Create(name, title, FilterKind.Text, selector);

    /// <summary>
    ///     A number: any numeric type, nullable or not.
    /// </summary>
    public static FilterProperty<TItem> Number<TItem, TNumber>(string name, string title, Expression<Func<TItem, TNumber>> selector)
        => Create(name, title, FilterKind.Number, selector);

    /// <summary>
    ///     A day: <see cref="DateOnly" />, nullable or not.
    /// </summary>
    public static FilterProperty<TItem> Date<TItem, TDate>(string name, string title, Expression<Func<TItem, TDate>> selector)
        => Create(name, title, FilterKind.Date, selector);

    /// <summary>
    ///     A flag: <see cref="bool" />, nullable or not; null counts as false.
    /// </summary>
    public static FilterProperty<TItem> Boolean<TItem, TFlag>(string name, string title, Expression<Func<TItem, TFlag>> selector)
        => Create(name, title, FilterKind.Boolean, selector);

    /// <summary>
    ///     The value of an item whose navigation to the property is missing is null, not an exception: a row without an address
    ///     has no city, and it must neither fail the list nor drop out of it silently.
    /// </summary>
    private static FilterProperty<TItem> Create<TItem, TValue>(string name, string title, FilterKind kind, Expression<Func<TItem, TValue>> selector)
    {
        var read = selector.Compile();

        return new(name, title, kind, item => ReadOrNull(read, item), selector);
    }

    private static object? ReadOrNull<TItem, TValue>(Func<TItem, TValue> read, TItem item)
    {
        try
        {
            return read(item);
        }
        catch (NullReferenceException)
        {
            return null;
        }
    }

    #endregion
}
