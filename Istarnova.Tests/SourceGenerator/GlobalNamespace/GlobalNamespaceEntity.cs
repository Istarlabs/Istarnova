// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

using Istarnova.SourceGenerator.Attributes.Data.Equatable;

// Deliberately without a namespace: the generators used to emit "namespace <global namespace>;".
[AutoEquatable]
public partial class GlobalNamespaceEntity
{
    public long GlobalNamespaceEntityId { get; set; }
}
