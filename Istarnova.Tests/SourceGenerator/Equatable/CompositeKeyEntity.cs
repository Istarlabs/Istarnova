// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

using Istarnova.SourceGenerator.Attributes.Data.Equatable;

namespace Istarnova.Tests.SourceGenerator.Equatable;

[AutoEquatable(KeyProperties = $"{nameof(OwnerId)}, {nameof(ItemId)}")]
public partial class CompositeKeyEntity
{
    public int ItemId { get; set; }

    public int OwnerId { get; set; }
}
