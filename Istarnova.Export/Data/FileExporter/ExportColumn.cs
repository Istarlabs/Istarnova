#region "Header"

// File: ExportColumn.cs
// Created 2026/03/06
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

namespace Istarnova.Data.FileExporter;

public sealed record ExportColumn(string Name, Type ColumnType);
