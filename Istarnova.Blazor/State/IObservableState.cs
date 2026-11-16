#region "Header"

// File: IObservableState.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

namespace Istarnova.Blazor.State;

/// <summary>
///     A state components can follow: it tells them when what they are showing has changed.
/// </summary>
public interface IObservableState
{
    #region "Methods"

    /// <summary>
    ///     Registers <paramref name="onChanged" /> for <paramref name="owner" />, replacing whatever that owner had
    ///     registered before.
    /// </summary>
    void Subscribe(object owner, Action onChanged);

    /// <summary>
    ///     Removes the owner's registration. Doing this for an owner that never subscribed is fine.
    /// </summary>
    void Unsubscribe(object owner);

    #endregion
}
