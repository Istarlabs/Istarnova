#region "Header"

// File: KeyboardEventArgsExtensions.cs
// Created 2026/10/29
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using Istarnova.Input;
using Microsoft.AspNetCore.Components.Web;

namespace Istarnova.Blazor.Input;

/// <summary>
///     Reads the key of a Blazor keyboard event as a <see cref="KeyboardKey" />, so a page compares keys and not strings.
/// </summary>
public static class KeyboardEventArgsExtensions
{
    #region "Methods"

    extension(KeyboardEventArgs args)
    {
        /// <summary>
        ///     The key of the event. One the library does not know becomes <see cref="KeyboardKey.Unknown" /> with the raw string.
        /// </summary>
        public KeyboardKey ToKeyboardKey()
        {
            ArgumentNullException.ThrowIfNull(args);

            return KeyboardKey.FromJs(args.Key.AsSpan());
        }

        /// <summary>
        ///     Whether the event is for the given key. The browser reports a digit as <c>"0"</c> whether it came from the number row
        ///     or the numeric keypad, so that string is always <see cref="KeyboardKey.D0" />, never <see cref="KeyboardKey.NumPad0" />.
        /// </summary>
        /// <param name="key">The key to compare with.</param>
        public bool IsKey(KeyboardKey key)
        {
            ArgumentNullException.ThrowIfNull(args);
            ArgumentNullException.ThrowIfNull(key);

            return key.IsJs(args.Key);
        }

        /// <summary>
        ///     Whether the event is for the Enter key.
        /// </summary>
        public bool IsEnter()
            => args.IsKey(KeyboardKey.Enter);

        /// <summary>
        ///     Whether the event is for the Escape key.
        /// </summary>
        public bool IsEscape()
            => args.IsKey(KeyboardKey.Escape);
    }

    #endregion
}
