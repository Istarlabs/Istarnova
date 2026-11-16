#region "Header"

// File: CsvSeparator.cs
// Created 2026/03/06
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

#region "Usings"

using NetEscapades.EnumGenerators;

#endregion

namespace Istarnova.Data.FileExporter;

/// <summary>
///     Defines the available CSV separator characters for export.
/// </summary>
[EnumExtensions]
public enum CsvSeparator
{
    /// <summary>
    ///     Comma separator (,) - Standard for US/International CSV
    /// </summary>
    Comma,

    /// <summary>
    ///     Semicolon separator (;) - Standard for European CSV (Excel)
    /// </summary>
    Semicolon,

    /// <summary>
    ///     Tab separator (\t) - For TSV (Tab-Separated Values) files
    /// </summary>
    Tab,

    /// <summary>
    ///     Pipe separator (|) - Alternative separator
    /// </summary>
    Pipe,
}

/// <summary>
///     Extension methods for CsvSeparator enum.
/// </summary>
public static partial class CsvSeparatorExtensions
{
    #region "Methods"

    /// <param name="separator">The CSV separator enum value</param>
    extension(CsvSeparator separator)
    {
        #region "Methods"

        /// <summary>
        ///     Gets the recommended file extension for the separator type.
        /// </summary>
        /// <returns>File extension (without dot)</returns>
        public string GetFileExtension() => separator switch
        {
            CsvSeparator.Tab => "tsv",
            var _ => "csv",
        };

        /// <summary>
        ///     Gets the actual string character for the CSV separator.
        /// </summary>
        /// <returns>The separator string</returns>
        public string ToSeparatorString() => separator switch
        {
            CsvSeparator.Comma => ",",
            CsvSeparator.Semicolon => ";",
            CsvSeparator.Tab => "\t",
            CsvSeparator.Pipe => "|",
            var _ => throw new ArgumentOutOfRangeException(nameof(separator), separator, "Invalid CSV separator"),
        };

        #endregion
    }

    #endregion
}
