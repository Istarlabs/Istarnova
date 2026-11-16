#region "Header"

// File: FluentDialogResult.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

namespace Istarnova.Blazor.FluentUI.Models;

/// <summary>
///     How a dialog of a CRUD flow ended.
/// </summary>
public enum FluentDialogResult
{
    Ok,
    Cancel,
    OkCreate,
    ErrorCreate,
    OkUpdate,
    ErrorUpdate,
    OkDelete,
    ErrorDelete,
    CancelCreate,
    CancelUpdate,
    CancelDelete,
    None,
}
