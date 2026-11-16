#region "Header"

// File: PageBreadcrumb.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

namespace Istarnova.Blazor.FluentUI.Scaffolding;

/// <summary>
///     A single entry in a <see cref="PageScaffold" /> breadcrumb trail.
/// </summary>
/// <param name="Text">The label shown to the user. Comes from a resource file, never a literal.</param>
/// <param name="Url">
///     The target of the entry, or <c>null</c> for the current page - the last entry is where the user already is, so it
///     must not be a link.
/// </param>
public sealed record PageBreadcrumb(string Text, string? Url = null);
