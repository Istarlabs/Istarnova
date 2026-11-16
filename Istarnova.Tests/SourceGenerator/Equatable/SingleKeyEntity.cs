// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

using Istarnova.SourceGenerator.Attributes.Data.Equatable;

namespace Istarnova.Tests.SourceGenerator.Equatable;

[AutoEquatable]
public partial class SingleKeyEntity
{
    public long SingleKeyEntityId { get; set; }

    public string? Name { get; set; }
}
