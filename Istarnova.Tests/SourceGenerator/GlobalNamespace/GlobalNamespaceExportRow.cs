// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

using Istarnova.Data.FileExporter;
using Istarnova.SourceGenerator.Attributes.Data.FileExporter;

// Deliberately without a namespace: the generators used to emit "namespace <global namespace>;".
[FileExportable]
public class GlobalNamespaceExportRow
{
    [ColumnHeader("Full name")]
    public string Name { get; set; } = string.Empty;
}
