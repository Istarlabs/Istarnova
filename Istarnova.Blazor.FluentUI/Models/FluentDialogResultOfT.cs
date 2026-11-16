#region "Header"

// File: FluentDialogResultOfT.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

namespace Istarnova.Blazor.FluentUI.Models;

/// <summary>
///     What a dialog hands back: how it ended, and the record it concerned.
/// </summary>
/// <typeparam name="TData">The record.</typeparam>
public sealed class FluentDialogResult<TData> where TData : class
{
    #region "Properties"

    public TData? Data { get; set; }

    public FluentDialogResult Result { get; set; }

    #endregion

    #region "Methods"

    public static FluentDialogResult<TData> Cancel() => new() { Result = FluentDialogResult.Cancel };

    public static FluentDialogResult<TData> CancelCreate() => new() { Result = FluentDialogResult.CancelCreate };

    public static FluentDialogResult<TData> CancelDelete() => new() { Result = FluentDialogResult.CancelDelete };

    public static FluentDialogResult<TData> CancelUpdate() => new() { Result = FluentDialogResult.CancelUpdate };

    public static FluentDialogResult<TData> ErrorCreate(TData data) => new() { Result = FluentDialogResult.ErrorCreate, Data = data };

    public static FluentDialogResult<TData> ErrorDelete(TData data) => new() { Result = FluentDialogResult.ErrorDelete, Data = data };

    public static FluentDialogResult<TData> ErrorUpdate(TData data) => new() { Result = FluentDialogResult.ErrorUpdate, Data = data };

    public static FluentDialogResult<TData> None() => new() { Result = FluentDialogResult.None };

    public static FluentDialogResult<TData> Ok(TData data) => new() { Result = FluentDialogResult.Ok, Data = data };

    public static FluentDialogResult<TData> OkCreate(TData data) => new() { Result = FluentDialogResult.OkCreate, Data = data };

    public static FluentDialogResult<TData> OkDelete(TData data) => new() { Result = FluentDialogResult.OkDelete, Data = data };

    public static FluentDialogResult<TData> OkUpdate(TData data) => new() { Result = FluentDialogResult.OkUpdate, Data = data };

    #endregion
}
