#region "Header"

// File: LocalizedPaginator.razor.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

namespace Istarnova.Blazor.FluentUI.Grids;

/// <summary>
///     A paginator with the texts of the session's language.
/// </summary>
public sealed partial class LocalizedPaginator
{
    #region "Properties"

    [Parameter]
    public bool Disabled { get; set; }

    [Parameter]
    [EditorRequired]
    public PaginationState State { get; set; } = null!;

    #endregion
}
