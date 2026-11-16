#region "Header"

// File: ObservableState.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

namespace Istarnova.Blazor.State;

/// <summary>
///     Base for a state that notifies its followers. Derived states raise <see cref="NotifyChanged" />; the bookkeeping
///     lives here so no state has to write it again.
/// </summary>
public abstract class ObservableState : IObservableState, IDisposable
{
    #region "Variables"

    private readonly Dictionary<object, Action> _subscribers = [];

    #endregion

    #region "Methods"

    public void Dispose()
    {
        _subscribers.Clear();

        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    public void Subscribe(object owner, Action onChanged)
    {
        ArgumentNullException.ThrowIfNull(owner);
        ArgumentNullException.ThrowIfNull(onChanged);

        _subscribers[owner] = onChanged;
    }

    /// <inheritdoc />
    public void Unsubscribe(object owner)
    {
        ArgumentNullException.ThrowIfNull(owner);

        _subscribers.Remove(owner);
    }

    /// <summary>
    ///     Tells every subscriber that something they show has changed.
    /// </summary>
    protected void NotifyChanged()
    {
        // Copied first: a handler may unsubscribe its owner while being called.
        foreach (var onChanged in _subscribers.Values.ToArray())
        {
            onChanged();
        }
    }

    #endregion
}
