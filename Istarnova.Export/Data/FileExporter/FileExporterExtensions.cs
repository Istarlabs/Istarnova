#region "Header"

// File: FileExporterExtensions.cs
// Created 2026/03/06
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

#region "Usings"

using System.Data;
using System.Reflection;

#endregion

namespace Istarnova.Data.FileExporter;

/// <summary>
///     Contains extension methods for exporting data to files.
/// </summary>
public static class FileExporterExtensions
{
    #region "Methods"

    /// <summary>
    ///     Returns the column name for a given PropertyInfo object.
    /// </summary>
    /// <param name="property">The PropertyInfo object</param>
    /// <returns>The column name</returns>
    public static string GetColumnName(this PropertyInfo property)
    {
        var attribute = property.GetCustomAttribute<ColumnHeaderAttribute>();
        return attribute?.ColumnName ?? property.Name;
    }

    /// <summary>
    ///     Returns a list of columns for a given type, where each column contains
    ///     the name of a property and its corresponding type.
    /// </summary>
    /// <param name="type">The type of object to get a list of columns for.</param>
    /// <returns>A list of ExportColumn objects.</returns>
    public static List<ExportColumn> GetList(this Type type)
    {
        return
        [
            .. type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead)
                .Select(p => new ExportColumn(p.GetColumnName(), p.PropertyType)),
        ];
    }

    /// <summary>
    ///     Returns an array of property values for a given object.
    /// </summary>
    /// <typeparam name="T">The type of object being passed in.</typeparam>
    /// <param name="obj">The object to retrieve values for.</param>
    /// <returns>An array of property values for the given object.</returns>
    public static object?[] GetValues<T>(object obj) =>
    [
        .. typeof(T).PropertyList()
            .Select(property => property.GetValue(obj)),
    ];

    /// <summary>
    ///     Returns a list of properties of a given type which are decorated with the ColumnHeader attribute.
    /// </summary>
    /// <param name="type">The type of object to get property names for.</param>
    /// <returns>A list of properties.</returns>
    public static IReadOnlyList<PropertyInfo> PropertyList(this Type type) =>
    [
        .. type.GetProperties()
            .Where(property => property.IsDefined(typeof(ColumnHeaderAttribute))),
    ];

    /// <summary>
    ///     Converts a list of ExportColumns and data to a DataTable object.
    /// </summary>
    /// <typeparam name="T">The type of object being passed in.</typeparam>
    /// <param name="columns">A list of ExportColumns to map to a DataTable.</param>
    /// <param name="data">A list of objects to map to a DataTable.</param>
    /// <returns>A DataTable object with columns and data.</returns>
    public static DataTable ToDataTable<T>(this List<ExportColumn> columns, IReadOnlyList<T> data)
    {
        var table = new DataTable();
        var type = typeof(T);
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead)
            .ToArray();

        // Add columns
        foreach (var column in columns)
        {
            var columnType = Nullable.GetUnderlyingType(column.ColumnType) ?? column.ColumnType;
            table.Columns.Add(column.Name, columnType);
        }

        // Add rows
        foreach (var item in data)
        {
            var values = new object?[columns.Count];

            for (var i = 0; i < columns.Count; i++)
            {
                var property = properties.FirstOrDefault(p => p.GetColumnName() == columns[i].Name);
                if (property != null)
                {
                    var value = property.GetValue(item);
                    values[i] = value ?? DBNull.Value;
                }
                else
                {
                    values[i] = DBNull.Value;
                }
            }

            table.Rows.Add(values);
        }

        return table;
    }

    #endregion
}
