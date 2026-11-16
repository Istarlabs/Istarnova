#region Header
// File: KeySource.cs
// Created 2026/10/29
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later
#endregion

namespace Istarnova.Input;

/// <summary>
/// Where the raw code of an unrecognised <see cref="KeyboardKey"/> came from.
/// </summary>
public enum KeySource
{
    /// <summary>The key was created from <see cref="Keys"/> itself.</summary>
    Internal,

    /// <summary>A Win32 virtual-key code.</summary>
    Win32,

    /// <summary>A <c>System.Windows.Input.Key</c> value (WPF).</summary>
    WinInput,

    /// <summary>A <c>System.Windows.Forms.Keys</c> value.</summary>
    WinForm,

    /// <summary>A <c>KeyboardEvent.key</c> string.</summary>
    JavaScript,
}
