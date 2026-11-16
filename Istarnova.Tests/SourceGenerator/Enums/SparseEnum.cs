#region "Header"

// File: SparseEnum.cs
// Created 2026/09/16
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

#region "Usings"

using Istarnova.SourceGenerator.Attributes.Common.Enums;
using NetEscapades.EnumGenerators;

#endregion

namespace Istarnova.Tests.SourceGenerator.Enums;

[EnumExtensions]
[EnumExtras]
public enum SparseEnum
{
    Negative = -5,
    One = 1,
    Two = 2,
    AliasOfTwo = 2,
    Ninety = 90,
}
