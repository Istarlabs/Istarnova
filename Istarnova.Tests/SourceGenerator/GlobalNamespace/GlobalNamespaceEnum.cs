// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

using System.ComponentModel.DataAnnotations;
using Istarnova.SourceGenerator.Attributes.Common.Enums;
using NetEscapades.EnumGenerators;

// Deliberately without a namespace: the generators used to emit "namespace <global namespace>;".
[EnumExtensions]
[EnumExtras]
public enum GlobalNamespaceEnum
{
    [Display(Name = "Waiting")]
    Pending,
    Shipped,
}
