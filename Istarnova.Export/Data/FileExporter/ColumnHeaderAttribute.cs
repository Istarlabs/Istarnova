#region "Header"

// File: ColumnHeaderAttribute.cs
// Created 2026/03/06
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

namespace Istarnova.Data.FileExporter;

[AttributeUsage(AttributeTargets.Property)]
public sealed class ColumnHeaderAttribute : Attribute
{
    #region "Constructor"

    public ColumnHeaderAttribute(string? columnName = null)
    {
        ColumnName = columnName ?? string.Empty;

        // If ResourceType is specified, try to get the value from the resource
        if (ResourceType is null || string.IsNullOrEmpty(ColumnName))
        {
            return;
        }

        var property = ResourceType.GetProperty(ColumnName,
            System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
        if (property?.GetValue(null) is string resourceValue)
        {
            ColumnName = resourceValue;
        }
    }

    #endregion

    #region "Properties"

    public string ColumnName { get; set; }
    public Type? ResourceType { get; set; }

    #endregion
}
