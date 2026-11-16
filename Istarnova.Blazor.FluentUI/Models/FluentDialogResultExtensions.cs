#region "Header"

// File: FluentDialogResultExtensions.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

namespace Istarnova.Blazor.FluentUI.Models;

public static class FluentDialogResultExtensions
{
    #region "Methods"

    extension(FluentDialogResult result)
    {
        #region "Methods"

        public bool IsCancel() => result == FluentDialogResult.Cancel;

        public bool IsCancelCreate() => result == FluentDialogResult.CancelCreate;

        public bool IsCancelDelete() => result == FluentDialogResult.CancelDelete;

        public bool IsCancelUpdate() => result == FluentDialogResult.CancelUpdate;

        public bool IsErrorCreate() => result == FluentDialogResult.ErrorCreate;

        public bool IsErrorDelete() => result == FluentDialogResult.ErrorDelete;

        public bool IsErrorUpdate() => result == FluentDialogResult.ErrorUpdate;

        public bool IsNone() => result == FluentDialogResult.None;

        public bool IsOk() => result == FluentDialogResult.Ok;

        public bool IsOkCreate() => result == FluentDialogResult.OkCreate;

        public bool IsOkDelete() => result == FluentDialogResult.OkDelete;

        public bool IsOkUpdate() => result == FluentDialogResult.OkUpdate;

        #endregion
    }

    #endregion
}
