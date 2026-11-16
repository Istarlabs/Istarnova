#region "Header"

// File: FormSelectField.razor.cs
// Created 2026/10/30
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

#region "Usings"

using Microsoft.AspNetCore.Components;

#endregion

namespace Istarnova.Blazor.FluentUI.Fields;

/// <summary>
///     A labelled select over a short, known list in its own cell of the form grid. A list that can grow is a
///     <see cref="FormRecordField{TItem,TRepository}" />.
/// </summary>
/// <typeparam name="TItem">The option, which is also the bound value; a string works as well as a record.</typeparam>
public sealed partial class FormSelectField<TItem>
{
    #region "Properties"

    [Parameter]
    [EditorRequired]
    public IEnumerable<TItem> Items { get; set; } = [];

    [Parameter]
    [EditorRequired]
    public Func<TItem, string> OptionText { get; set; } = null!;

    [Parameter]
    public string? Placeholder { get; set; }

    #endregion
}
