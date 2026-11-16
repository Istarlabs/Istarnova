#region "Header"

// File: CsvExportRow.cs
// Created 2026/09/16
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

namespace Istarnova.Tests.Data.FileExporter;

public class CsvExportRow
{
    public string Name { get; set; } = string.Empty;

    public int Change { get; set; }
}
