#region "Header"

// File: GenerateApiControllerAttribute.cs
// Created 2026/02/13
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

namespace Istarnova.SourceGenerator.Attributes.Controller;

[AttributeUsage(AttributeTargets.Interface)]
public sealed class GenerateApiControllerAttribute : Attribute
{
    #region "Properties"

    public bool AuthRequired { get; set; } = true;
    public string? ParentRoute { get; set; }
    public string? Route { get; set; }

    #endregion
}
