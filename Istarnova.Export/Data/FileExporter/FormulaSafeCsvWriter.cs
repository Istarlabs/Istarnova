#region "Header"

// File: FormulaSafeCsvWriter.cs
// Created 2026/09/16
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

#region "Usings"

using System.Data;
using CsvHelper;
using CsvHelper.Configuration;

#endregion

namespace Istarnova.Data.FileExporter;

/// <summary>
///     A <see cref="CsvWriter" /> that escapes formula injection only in the header and in columns whose type can
///     carry arbitrary text, so a negative number stays <c>-5</c> instead of becoming <c>'-5</c>.
/// </summary>
internal sealed class FormulaSafeCsvWriter : CsvWriter
{
    #region "Fields"

    private readonly bool[] _isTextColumn;

    #endregion

    #region "Constructor"

    public FormulaSafeCsvWriter(TextWriter writer, CsvConfiguration configuration, DataTable dataTable)
        : base(writer, configuration with { InjectionOptions = InjectionOptions.Escape })
    {
        _isTextColumn =
        [
            .. dataTable.Columns.Cast<DataColumn>()
                .Select(column => !CannotCarryFormula(column.DataType)),
        ];
    }

    #endregion

    #region "Methods"

    protected override string? SanitizeForInjection(string? field)
        => Row == 1 || _isTextColumn[Index]
            ? base.SanitizeForInjection(field)
            : field;

    private static bool CannotCarryFormula(Type type)
        => type.IsEnum
           || (type.IsPrimitive && type != typeof(char))
           || type == typeof(decimal)
           || type == typeof(DateTime)
           || type == typeof(DateTimeOffset)
           || type == typeof(DateOnly)
           || type == typeof(TimeOnly)
           || type == typeof(TimeSpan)
           || type == typeof(Guid);

    #endregion
}
