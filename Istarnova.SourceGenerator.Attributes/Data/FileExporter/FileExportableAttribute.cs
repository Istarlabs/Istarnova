#region "Header"

// File: FileExportableAttribute.cs
// Created 2026/03/06
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

namespace Istarnova.SourceGenerator.Attributes.Data.FileExporter;

/// <summary>
///     Marks a class as exportable and generates export metadata at compile time.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class FileExportableAttribute : Attribute
{
    #region "Constructor"

    /// <summary>
    ///     Initializes a new instance of the FileExportableAttribute.
    /// </summary>
    public FileExportableAttribute()
    {
    }

    #endregion
}
