#region "Header"

// File: FileExporterCsvTests.cs
// Created 2026/09/16
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

#region "Usings"

using System.Text;
using Istarnova.Data.FileExporter;

#endregion

namespace Istarnova.Tests.Data.FileExporter;

public class FileExporterCsvTests
{
    private static async Task<string> ExportAsync(CsvExportRow row, CsvSeparator separator = CsvSeparator.Comma)
        => Encoding.UTF8.GetString(await Istarnova.Data.FileExporter.FileExporter.ExportToCsvAsync([row], separator));

    [Fact]
    public async Task ExportToCsvAsync_ShouldQuoteAndEscape_WhenFieldContainsDelimiterQuoteOrLineBreak()
    {
        var csv = await ExportAsync(new() { Name = "Doe, \"John\"\nJr.", Change = 3 });

        csv.Should().Be("Name,Change\r\n\"Doe, \"\"John\"\"\nJr.\",3\r\n");
    }

    [Fact]
    public async Task ExportToCsvAsync_ShouldUseSeparator_WhenSemicolonIsChosen()
    {
        var csv = await ExportAsync(new() { Name = "a,b", Change = 1 }, CsvSeparator.Semicolon);

        csv.Should().Be("Name;Change\r\na,b;1\r\n");
    }

    [Theory]
    [InlineData("=HYPERLINK(\"http://x\")")]
    [InlineData("+1+1")]
    [InlineData("-2+3")]
    [InlineData("@SUM(A1)")]
    public async Task ExportToCsvAsync_ShouldEscapeFormula_WhenTextColumnStartsWithInjectionCharacter(string name)
    {
        var csv = await ExportAsync(new() { Name = name, Change = 0 });

        csv.Split("\r\n")[1].Should().StartWith("\"'");
    }

    [Fact]
    public async Task ExportToCsvAsync_ShouldKeepNegativeNumber_WhenColumnIsNumeric()
    {
        var csv = await ExportAsync(new() { Name = "Weight", Change = -5 });

        csv.Should().Be("Name,Change\r\nWeight,-5\r\n");
    }

    [Theory]
    [InlineData("report", "report.csv")]
    [InlineData("report.txt", "report.csv")]
    [InlineData("report.CSV", "report.CSV")]
    public async Task SaveToFileAsync_ShouldKeepDirectory_WhenExtensionIsAdded(string fileName, string expected)
    {
        var directory = Directory.CreateTempSubdirectory();

        try
        {
            await Istarnova.Data.FileExporter.FileExporter.SaveToFileAsync<CsvExportRow>(
                [new() { Name = "a", Change = 1 }], Path.Combine(directory.FullName, fileName), CsvSeparator.Comma);

            directory.GetFiles().Select(file => file.Name).Should().Equal(expected);
        }
        finally
        {
            directory.Delete(true);
        }
    }
}
