#region "Header"

// File: FileExporter.cs
// Created 2026/03/06
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

#region "Usings"

using System.Data;
using System.Globalization;
using System.Text;
using ClosedXML.Excel;
using CsvHelper.Configuration;

#endregion

namespace Istarnova.Data.FileExporter;

/// <summary>
///     Modern file exporter using ClosedXML for Excel and CsvHelper for CSV.
///     Supports Excel (.xlsx) and CSV export formats.
/// </summary>
public static class FileExporter
{
    #region "Methods"

    /// <summary>
    ///     Exports data to Excel (.xlsx) or CSV format based on separator parameter.
    /// </summary>
    /// <typeparam name="T">Type of objects to export</typeparam>
    /// <param name="data">Data to export</param>
    /// <param name="csvSeparator">CSV separator (null = Excel format)</param>
    /// <param name="worksheetName">Name of the Excel worksheet (default: "Data")</param>
    /// <returns>File content as byte array</returns>
    public static async Task<byte[]> ExportAsync<T>(IReadOnlyList<T> data, CsvSeparator? csvSeparator = null,
        string worksheetName = "Data")
    {
        ArgumentNullException.ThrowIfNull(data);

        var type = typeof(T);
        var columns = type.GetList();
        var dataTable = columns.ToDataTable(data);

        return csvSeparator.HasValue
            ? await ExportToCsvAsync(dataTable, csvSeparator.Value.ToSeparatorString())
            : await ExportToExcelAsync(dataTable, worksheetName);
    }

    /// <summary>
    ///     Exports data to Excel (.xlsx) or CSV format based on separator parameter.
    ///     Uses compile-time generated methods for better performance when T has FileExportableAttribute.
    /// </summary>
    /// <typeparam name="T">Type of objects to export (must have FileExportableAttribute for optimization)</typeparam>
    /// <param name="data">Data to export</param>
    /// <param name="csvSeparator">CSV separator (null = Excel format)</param>
    /// <param name="worksheetName">Name of the Excel worksheet (default: "Data")</param>
    /// <returns>File content as byte array</returns>
    public static async Task<byte[]> ExportOptimizedAsync<T>(IReadOnlyList<T> data, CsvSeparator? csvSeparator = null,
        string worksheetName = "Data")
        where T : class
    {
        ArgumentNullException.ThrowIfNull(data);

        // Use reflection to check if the generated exporter exists
        var typeName = typeof(T).Name + "FileExporter";
        var exporterType = typeof(T).Assembly.GetType($"{typeof(T).Namespace}.{typeName}");

        DataTable dataTable;

        if (exporterType != null)
        {
            // Use generated method for better performance
            var toDataTableMethod = exporterType.GetMethod("ToDataTable", [typeof(IReadOnlyList<T>)]);
            if (toDataTableMethod != null)
            {
                dataTable = (DataTable)toDataTableMethod.Invoke(null, [data])!;
            }
            else
            {
                // Fallback to reflection
                var type = typeof(T);
                var columns = type.GetList();
                dataTable = columns.ToDataTable(data);
            }
        }
        else
        {
            // Fallback to reflection
            var type = typeof(T);
            var columns = type.GetList();
            dataTable = columns.ToDataTable(data);
        }

        return csvSeparator.HasValue
            ? await ExportToCsvAsync(dataTable, csvSeparator.Value.ToSeparatorString())
            : await ExportToExcelAsync(dataTable, worksheetName);
    }

    /// <summary>
    ///     Exports data directly to CSV format.
    /// </summary>
    /// <typeparam name="T">Type of objects to export</typeparam>
    /// <param name="data">Data to export</param>
    /// <param name="separator">CSV separator type</param>
    /// <returns>CSV file content as byte array</returns>
    public static Task<byte[]> ExportToCsvAsync<T>(IReadOnlyList<T> data, CsvSeparator separator = CsvSeparator.Comma)
    {
        ArgumentNullException.ThrowIfNull(data);

        var type = typeof(T);
        var columns = type.GetList();
        var dataTable = columns.ToDataTable(data);

        return ExportToCsvAsync(dataTable, separator.ToSeparatorString());
    }

    /// <summary>
    ///     Exports data directly to Excel format.
    /// </summary>
    /// <typeparam name="T">Type of objects to export</typeparam>
    /// <param name="data">Data to export</param>
    /// <param name="worksheetName">Name of the Excel worksheet</param>
    /// <returns>Excel file content as byte array</returns>
    public static Task<byte[]> ExportToExcelAsync<T>(IReadOnlyList<T> data, string worksheetName = "Data")
    {
        ArgumentNullException.ThrowIfNull(data);

        var type = typeof(T);
        var columns = type.GetList();
        var dataTable = columns.ToDataTable(data);

        return ExportToExcelAsync(dataTable, worksheetName);
    }

    /// <summary>
    ///     Saves exported data directly to a file with manual path control.
    /// </summary>
    /// <typeparam name="T">Type of objects to export</typeparam>
    /// <param name="data">Data to export</param>
    /// <param name="filePath">Exact file path (no extension modification)</param>
    /// <param name="csvSeparator">CSV separator (null = Excel format)</param>
    public static async Task SaveToExactFileAsync<T>(IReadOnlyList<T> data, string filePath,
        CsvSeparator? csvSeparator = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        var fileData = await ExportAsync(data, csvSeparator);
        await File.WriteAllBytesAsync(filePath, fileData);
    }

    /// <summary>
    ///     Saves exported data directly to a file with automatic file extension.
    /// </summary>
    /// <typeparam name="T">Type of objects to export</typeparam>
    /// <param name="data">Data to export</param>
    /// <param name="filePath">Path where to save the file (extension will be added automatically)</param>
    /// <param name="csvSeparator">CSV separator (null = Excel format)</param>
    public static async Task SaveToFileAsync<T>(IReadOnlyList<T> data, string filePath,
        CsvSeparator? csvSeparator = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        // Auto-add correct extension if not present
        var finalFilePath = csvSeparator.HasValue
            ? EnsureFileExtension(filePath, csvSeparator.Value.GetFileExtension())
            : EnsureFileExtension(filePath, "xlsx");

        var fileData = await ExportAsync(data, csvSeparator);
        await File.WriteAllBytesAsync(finalFilePath, fileData);
    }

    /// <summary>
    ///     Ensures the file path has the correct extension.
    /// </summary>
    /// <param name="filePath">Original file path</param>
    /// <param name="extension">Required extension (without dot)</param>
    /// <returns>File path with correct extension</returns>
    private static string EnsureFileExtension(string filePath, string extension)
    {
        var currentExtension = Path.GetExtension(filePath).TrimStart('.');

        return string.Equals(currentExtension, extension, StringComparison.OrdinalIgnoreCase)
            ? filePath
            : Path.ChangeExtension(filePath, extension);
    }

    /// <summary>
    ///     Exports a DataTable to CSV format using CsvHelper.
    /// </summary>
    /// <param name="dataTable">DataTable to export</param>
    /// <param name="separator">CSV separator character</param>
    /// <returns>CSV file content as byte array, UTF-8 without BOM</returns>
    private static Task<byte[]> ExportToCsvAsync(DataTable dataTable, string separator)
    {
        return Task.Run(() =>
        {
            var configuration = new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = separator };
            using var stream = new MemoryStream();

            using (var writer = new StreamWriter(stream, new UTF8Encoding(false)))
            using (var csv = new FormulaSafeCsvWriter(writer, configuration, dataTable))
            {
                foreach (DataColumn column in dataTable.Columns)
                {
                    csv.WriteField(column.ColumnName);
                }

                csv.NextRecord();

                foreach (DataRow row in dataTable.Rows)
                {
                    foreach (var field in row.ItemArray)
                    {
                        csv.WriteField(field?.ToString() ?? string.Empty);
                    }

                    csv.NextRecord();
                }
            }

            return stream.ToArray();
        });
    }

    /// <summary>
    ///     Exports a DataTable to Excel format using ClosedXML.
    /// </summary>
    /// <param name="dataTable">DataTable to export</param>
    /// <param name="worksheetName">Name of the Excel worksheet</param>
    /// <returns>Excel file content as byte array</returns>
    private static async Task<byte[]> ExportToExcelAsync(DataTable dataTable, string worksheetName)
    {
        return await Task.Run(() =>
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add(worksheetName);

            // Insert data table with headers
            worksheet.Cell(1, 1).InsertTable(dataTable);

            // Format headers
            var headerRange = worksheet.Range(1, 1, 1, dataTable.Columns.Count);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

            // Auto-fit columns
            worksheet.Columns().AdjustToContents();

            // Return as byte array
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        });
    }

    #endregion
}
