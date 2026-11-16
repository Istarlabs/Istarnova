#region Header
// File: KeyboardKey.cs
// Created 2026/10/29
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later
#endregion

// Based on KeyboardKey.cs by Aurimas Niekis, MIT License
// https://gist.github.com/aurimasniekis/f4cfcd834c8563d16ffc6f0a2a50a742
// Copyright (c) 2021 Aurimas Niekis
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of
// the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#region Usings

using System.Collections.Frozen;

#endregion

namespace Istarnova.Input;

/// <summary>
/// A keyboard key that converts between the Win32, WPF, WinForms and JavaScript key codes.
/// A code this class does not know becomes <see cref="Unknown"/> and keeps the raw value.
/// </summary>
public sealed class KeyboardKey : IEquatable<KeyboardKey>
{
    #region Fields

    public static readonly KeyboardKey None = new(Keys.None);
    public static readonly KeyboardKey Cancel = new(Keys.Cancel);
    public static readonly KeyboardKey Backspace = new(Keys.Backspace);
    public static readonly KeyboardKey Tab = new(Keys.Tab);
    public static readonly KeyboardKey Clear = new(Keys.Clear);
    public static readonly KeyboardKey Enter = new(Keys.Enter);
    public static readonly KeyboardKey Pause = new(Keys.Pause);
    public static readonly KeyboardKey CapsLock = new(Keys.CapsLock);
    public static readonly KeyboardKey Escape = new(Keys.Escape);
    public static readonly KeyboardKey Space = new(Keys.Space);
    public static readonly KeyboardKey PageUp = new(Keys.PageUp);
    public static readonly KeyboardKey PageDown = new(Keys.PageDown);
    public static readonly KeyboardKey End = new(Keys.End);
    public static readonly KeyboardKey Home = new(Keys.Home);
    public static readonly KeyboardKey ArrowLeft = new(Keys.ArrowLeft);
    public static readonly KeyboardKey ArrowUp = new(Keys.ArrowUp);
    public static readonly KeyboardKey ArrowRight = new(Keys.ArrowRight);
    public static readonly KeyboardKey ArrowDown = new(Keys.ArrowDown);
    public static readonly KeyboardKey Select = new(Keys.Select);
    public static readonly KeyboardKey Print = new(Keys.Print);
    public static readonly KeyboardKey Execute = new(Keys.Execute);
    public static readonly KeyboardKey PrintScreen = new(Keys.PrintScreen);
    public static readonly KeyboardKey Insert = new(Keys.Insert);
    public static readonly KeyboardKey Delete = new(Keys.Delete);
    public static readonly KeyboardKey Help = new(Keys.Help);
    public static readonly KeyboardKey D0 = new(Keys.D0);
    public static readonly KeyboardKey D1 = new(Keys.D1);
    public static readonly KeyboardKey D2 = new(Keys.D2);
    public static readonly KeyboardKey D3 = new(Keys.D3);
    public static readonly KeyboardKey D4 = new(Keys.D4);
    public static readonly KeyboardKey D5 = new(Keys.D5);
    public static readonly KeyboardKey D6 = new(Keys.D6);
    public static readonly KeyboardKey D7 = new(Keys.D7);
    public static readonly KeyboardKey D8 = new(Keys.D8);
    public static readonly KeyboardKey D9 = new(Keys.D9);
    public static readonly KeyboardKey A = new(Keys.A);
    public static readonly KeyboardKey B = new(Keys.B);
    public static readonly KeyboardKey C = new(Keys.C);
    public static readonly KeyboardKey D = new(Keys.D);
    public static readonly KeyboardKey E = new(Keys.E);
    public static readonly KeyboardKey F = new(Keys.F);
    public static readonly KeyboardKey G = new(Keys.G);
    public static readonly KeyboardKey H = new(Keys.H);
    public static readonly KeyboardKey I = new(Keys.I);
    public static readonly KeyboardKey J = new(Keys.J);
    public static readonly KeyboardKey K = new(Keys.K);
    public static readonly KeyboardKey L = new(Keys.L);
    public static readonly KeyboardKey M = new(Keys.M);
    public static readonly KeyboardKey N = new(Keys.N);
    public static readonly KeyboardKey O = new(Keys.O);
    public static readonly KeyboardKey P = new(Keys.P);
    public static readonly KeyboardKey Q = new(Keys.Q);
    public static readonly KeyboardKey R = new(Keys.R);
    public static readonly KeyboardKey S = new(Keys.S);
    public static readonly KeyboardKey T = new(Keys.T);
    public static readonly KeyboardKey U = new(Keys.U);
    public static readonly KeyboardKey V = new(Keys.V);
    public static readonly KeyboardKey W = new(Keys.W);
    public static readonly KeyboardKey X = new(Keys.X);
    public static readonly KeyboardKey Y = new(Keys.Y);
    public static readonly KeyboardKey Z = new(Keys.Z);
    public static readonly KeyboardKey Meta = new(Keys.Meta);
    public static readonly KeyboardKey LeftMeta = new(Keys.LeftMeta);
    public static readonly KeyboardKey RightMeta = new(Keys.RightMeta);
    public static readonly KeyboardKey Apps = new(Keys.Apps);
    public static readonly KeyboardKey Sleep = new(Keys.Sleep);
    public static readonly KeyboardKey NumPad0 = new(Keys.NumPad0);
    public static readonly KeyboardKey NumPad1 = new(Keys.NumPad1);
    public static readonly KeyboardKey NumPad2 = new(Keys.NumPad2);
    public static readonly KeyboardKey NumPad3 = new(Keys.NumPad3);
    public static readonly KeyboardKey NumPad4 = new(Keys.NumPad4);
    public static readonly KeyboardKey NumPad5 = new(Keys.NumPad5);
    public static readonly KeyboardKey NumPad6 = new(Keys.NumPad6);
    public static readonly KeyboardKey NumPad7 = new(Keys.NumPad7);
    public static readonly KeyboardKey NumPad8 = new(Keys.NumPad8);
    public static readonly KeyboardKey NumPad9 = new(Keys.NumPad9);
    public static readonly KeyboardKey Multiply = new(Keys.Multiply);
    public static readonly KeyboardKey Add = new(Keys.Add);
    public static readonly KeyboardKey Separator = new(Keys.Separator);
    public static readonly KeyboardKey Subtract = new(Keys.Subtract);
    public static readonly KeyboardKey Decimal = new(Keys.Decimal);
    public static readonly KeyboardKey Divide = new(Keys.Divide);
    public static readonly KeyboardKey F1 = new(Keys.F1);
    public static readonly KeyboardKey F2 = new(Keys.F2);
    public static readonly KeyboardKey F3 = new(Keys.F3);
    public static readonly KeyboardKey F4 = new(Keys.F4);
    public static readonly KeyboardKey F5 = new(Keys.F5);
    public static readonly KeyboardKey F6 = new(Keys.F6);
    public static readonly KeyboardKey F7 = new(Keys.F7);
    public static readonly KeyboardKey F8 = new(Keys.F8);
    public static readonly KeyboardKey F9 = new(Keys.F9);
    public static readonly KeyboardKey F10 = new(Keys.F10);
    public static readonly KeyboardKey F11 = new(Keys.F11);
    public static readonly KeyboardKey F12 = new(Keys.F12);
    public static readonly KeyboardKey F13 = new(Keys.F13);
    public static readonly KeyboardKey F14 = new(Keys.F14);
    public static readonly KeyboardKey F15 = new(Keys.F15);
    public static readonly KeyboardKey F16 = new(Keys.F16);
    public static readonly KeyboardKey F17 = new(Keys.F17);
    public static readonly KeyboardKey F18 = new(Keys.F18);
    public static readonly KeyboardKey F19 = new(Keys.F19);
    public static readonly KeyboardKey F20 = new(Keys.F20);
    public static readonly KeyboardKey F21 = new(Keys.F21);
    public static readonly KeyboardKey F22 = new(Keys.F22);
    public static readonly KeyboardKey F23 = new(Keys.F23);
    public static readonly KeyboardKey F24 = new(Keys.F24);
    public static readonly KeyboardKey NumLock = new(Keys.NumLock);
    public static readonly KeyboardKey ScrollLock = new(Keys.ScrollLock);
    public static readonly KeyboardKey Shift = new(Keys.Shift);
    public static readonly KeyboardKey LeftShift = new(Keys.LeftShift);
    public static readonly KeyboardKey RightShift = new(Keys.RightShift);
    public static readonly KeyboardKey Control = new(Keys.Control);
    public static readonly KeyboardKey LeftControl = new(Keys.LeftControl);
    public static readonly KeyboardKey RightControl = new(Keys.RightControl);
    public static readonly KeyboardKey Alt = new(Keys.Alt);
    public static readonly KeyboardKey LeftAlt = new(Keys.LeftAlt);
    public static readonly KeyboardKey RightAlt = new(Keys.RightAlt);
    public static readonly KeyboardKey BrowserBack = new(Keys.BrowserBack);
    public static readonly KeyboardKey BrowserForward = new(Keys.BrowserForward);
    public static readonly KeyboardKey BrowserRefresh = new(Keys.BrowserRefresh);
    public static readonly KeyboardKey BrowserStop = new(Keys.BrowserStop);
    public static readonly KeyboardKey BrowserSearch = new(Keys.BrowserSearch);
    public static readonly KeyboardKey BrowserFavorites = new(Keys.BrowserFavorites);
    public static readonly KeyboardKey BrowserHome = new(Keys.BrowserHome);
    public static readonly KeyboardKey VolumeMute = new(Keys.VolumeMute);
    public static readonly KeyboardKey VolumeDown = new(Keys.VolumeDown);
    public static readonly KeyboardKey VolumeUp = new(Keys.VolumeUp);
    public static readonly KeyboardKey MediaNextTrack = new(Keys.MediaNextTrack);
    public static readonly KeyboardKey MediaPreviousTrack = new(Keys.MediaPreviousTrack);
    public static readonly KeyboardKey MediaStop = new(Keys.MediaStop);
    public static readonly KeyboardKey MediaPlayPause = new(Keys.MediaPlayPause);
    public static readonly KeyboardKey LaunchMail = new(Keys.LaunchMail);
    public static readonly KeyboardKey SelectMedia = new(Keys.SelectMedia);
    public static readonly KeyboardKey LaunchApplication1 = new(Keys.LaunchApplication1);
    public static readonly KeyboardKey LaunchApplication2 = new(Keys.LaunchApplication2);
    public static readonly KeyboardKey SemiColon = new(Keys.SemiColon);
    public static readonly KeyboardKey Plus = new(Keys.Plus);
    public static readonly KeyboardKey Comma = new(Keys.Comma);
    public static readonly KeyboardKey Minus = new(Keys.Minus);
    public static readonly KeyboardKey Period = new(Keys.Period);
    public static readonly KeyboardKey QuestionMark = new(Keys.QuestionMark);
    public static readonly KeyboardKey OemTilde = new(Keys.OemTilde);
    public static readonly KeyboardKey OpenBracket = new(Keys.OpenBracket);
    public static readonly KeyboardKey OemPipe = new(Keys.OemPipe);
    public static readonly KeyboardKey CloseBracket = new(Keys.CloseBracket);
    public static readonly KeyboardKey Quote = new(Keys.Quote);
    public static readonly KeyboardKey Backslash = new(Keys.Backslash);
    public static readonly KeyboardKey Play = new(Keys.Play);
    public static readonly KeyboardKey Zoom = new(Keys.Zoom);

    public static readonly KeyboardKey Unknown = new(Keys.Unknown);

    // Order is (WPF, WinForms, Win32). WPF has no generic modifier, so Shift, Control and Alt share the WPF value of
    // the left key; Meta has no generic virtual key and shares all three. Win32 and WinForms use VK_SHIFT, VK_CONTROL
    // and VK_MENU (0x10 to 0x12) for the generic ones.
    private static readonly Dictionary<Keys, (int WinInput, int WinForm, int Win32)> KeyToValues = new()
    {
        { Keys.None, (0, 0, 0x00) },
        { Keys.Cancel, (1, 3, 0x03) },
        { Keys.Backspace, (2, 8, 0x08) },
        { Keys.Tab, (3, 9, 0x09) },
        { Keys.Clear, (5, 12, 0x0C) },
        { Keys.Enter, (6, 13, 0x0D) },
        { Keys.Pause, (7, 19, 0x13) },
        { Keys.CapsLock, (8, 20, 0x14) },
        { Keys.Escape, (13, 27, 0x1B) },
        { Keys.Space, (18, 32, 0x20) },
        { Keys.PageUp, (19, 33, 0x21) },
        { Keys.PageDown, (20, 34, 0x22) },
        { Keys.End, (21, 35, 0x23) },
        { Keys.Home, (22, 36, 0x24) },
        { Keys.ArrowLeft, (23, 37, 0x25) },
        { Keys.ArrowUp, (24, 38, 0x26) },
        { Keys.ArrowRight, (25, 39, 0x27) },
        { Keys.ArrowDown, (26, 40, 0x28) },
        { Keys.Select, (27, 41, 0x29) },
        { Keys.Print, (28, 42, 0x2A) },
        { Keys.Execute, (29, 43, 0x2B) },
        { Keys.PrintScreen, (30, 44, 0x2C) },
        { Keys.Insert, (31, 45, 0x2D) },
        { Keys.Delete, (32, 46, 0x2E) },
        { Keys.Help, (33, 47, 0x2F) },
        { Keys.D0, (34, 48, 0x30) },
        { Keys.D1, (35, 49, 0x31) },
        { Keys.D2, (36, 50, 0x32) },
        { Keys.D3, (37, 51, 0x33) },
        { Keys.D4, (38, 52, 0x34) },
        { Keys.D5, (39, 53, 0x35) },
        { Keys.D6, (40, 54, 0x36) },
        { Keys.D7, (41, 55, 0x37) },
        { Keys.D8, (42, 56, 0x38) },
        { Keys.D9, (43, 57, 0x39) },
        { Keys.A, (44, 65, 0x41) },
        { Keys.B, (45, 66, 0x42) },
        { Keys.C, (46, 67, 0x43) },
        { Keys.D, (47, 68, 0x44) },
        { Keys.E, (48, 69, 0x45) },
        { Keys.F, (49, 70, 0x46) },
        { Keys.G, (50, 71, 0x47) },
        { Keys.H, (51, 72, 0x48) },
        { Keys.I, (52, 73, 0x49) },
        { Keys.J, (53, 74, 0x4A) },
        { Keys.K, (54, 75, 0x4B) },
        { Keys.L, (55, 76, 0x4C) },
        { Keys.M, (56, 77, 0x4D) },
        { Keys.N, (57, 78, 0x4E) },
        { Keys.O, (58, 79, 0x4F) },
        { Keys.P, (59, 80, 0x50) },
        { Keys.Q, (60, 81, 0x51) },
        { Keys.R, (61, 82, 0x52) },
        { Keys.S, (62, 83, 0x53) },
        { Keys.T, (63, 84, 0x54) },
        { Keys.U, (64, 85, 0x55) },
        { Keys.V, (65, 86, 0x56) },
        { Keys.W, (66, 87, 0x57) },
        { Keys.X, (67, 88, 0x58) },
        { Keys.Y, (68, 89, 0x59) },
        { Keys.Z, (69, 90, 0x5A) },
        { Keys.Meta, (70, 91, 0x5B) },
        { Keys.LeftMeta, (70, 91, 0x5B) },
        { Keys.RightMeta, (71, 92, 0x5C) },
        { Keys.Apps, (72, 93, 0x5D) },
        { Keys.Sleep, (73, 95, 0x5F) },
        { Keys.NumPad0, (74, 96, 0x60) },
        { Keys.NumPad1, (75, 97, 0x61) },
        { Keys.NumPad2, (76, 98, 0x62) },
        { Keys.NumPad3, (77, 99, 0x63) },
        { Keys.NumPad4, (78, 100, 0x64) },
        { Keys.NumPad5, (79, 101, 0x65) },
        { Keys.NumPad6, (80, 102, 0x66) },
        { Keys.NumPad7, (81, 103, 0x67) },
        { Keys.NumPad8, (82, 104, 0x68) },
        { Keys.NumPad9, (83, 105, 0x69) },
        { Keys.Multiply, (84, 106, 0x6A) },
        { Keys.Add, (85, 107, 0x6B) },
        { Keys.Separator, (86, 108, 0x6C) },
        { Keys.Subtract, (87, 109, 0x6D) },
        { Keys.Decimal, (88, 110, 0x6E) },
        { Keys.Divide, (89, 111, 0x6F) },
        { Keys.F1, (90, 112, 0x70) },
        { Keys.F2, (91, 113, 0x71) },
        { Keys.F3, (92, 114, 0x72) },
        { Keys.F4, (93, 115, 0x73) },
        { Keys.F5, (94, 116, 0x74) },
        { Keys.F6, (95, 117, 0x75) },
        { Keys.F7, (96, 118, 0x76) },
        { Keys.F8, (97, 119, 0x77) },
        { Keys.F9, (98, 120, 0x78) },
        { Keys.F10, (99, 121, 0x79) },
        { Keys.F11, (100, 122, 0x7A) },
        { Keys.F12, (101, 123, 0x7B) },
        { Keys.F13, (102, 124, 0x7C) },
        { Keys.F14, (103, 125, 0x7D) },
        { Keys.F15, (104, 126, 0x7E) },
        { Keys.F16, (105, 127, 0x7F) },
        { Keys.F17, (106, 128, 0x80) },
        { Keys.F18, (107, 129, 0x81) },
        { Keys.F19, (108, 130, 0x82) },
        { Keys.F20, (109, 131, 0x83) },
        { Keys.F21, (110, 132, 0x84) },
        { Keys.F22, (111, 133, 0x85) },
        { Keys.F23, (112, 134, 0x86) },
        { Keys.F24, (113, 135, 0x87) },
        { Keys.NumLock, (114, 144, 0x90) },
        { Keys.ScrollLock, (115, 145, 0x91) },
        { Keys.Shift, (116, 16, 0x10) },
        { Keys.LeftShift, (116, 160, 0xA0) },
        { Keys.RightShift, (117, 161, 0xA1) },
        { Keys.Control, (118, 17, 0x11) },
        { Keys.LeftControl, (118, 162, 0xA2) },
        { Keys.RightControl, (119, 163, 0xA3) },
        { Keys.Alt, (120, 18, 0x12) },
        { Keys.LeftAlt, (120, 164, 0xA4) },
        { Keys.RightAlt, (121, 165, 0xA5) },
        { Keys.BrowserBack, (122, 166, 0xA6) },
        { Keys.BrowserForward, (123, 167, 0xA7) },
        { Keys.BrowserRefresh, (124, 168, 0xA8) },
        { Keys.BrowserStop, (125, 169, 0xA9) },
        { Keys.BrowserSearch, (126, 170, 0xAA) },
        { Keys.BrowserFavorites, (127, 171, 0xAB) },
        { Keys.BrowserHome, (128, 172, 0xAC) },
        { Keys.VolumeMute, (129, 173, 0xAD) },
        { Keys.VolumeDown, (130, 174, 0xAE) },
        { Keys.VolumeUp, (131, 175, 0xAF) },
        { Keys.MediaNextTrack, (132, 176, 0xB0) },
        { Keys.MediaPreviousTrack, (133, 177, 0xB1) },
        { Keys.MediaStop, (134, 178, 0xB2) },
        { Keys.MediaPlayPause, (135, 179, 0xB3) },
        { Keys.LaunchMail, (136, 180, 0xB4) },
        { Keys.SelectMedia, (137, 181, 0xB5) },
        { Keys.LaunchApplication1, (138, 182, 0xB6) },
        { Keys.LaunchApplication2, (139, 183, 0xB7) },
        { Keys.SemiColon, (140, 186, 0xBA) },
        { Keys.Plus, (141, 187, 0xBB) },
        { Keys.Comma, (142, 188, 0xBC) },
        { Keys.Minus, (143, 189, 0xBD) },
        { Keys.Period, (144, 190, 0xBE) },
        { Keys.QuestionMark, (145, 191, 0xBF) },
        { Keys.OemTilde, (146, 192, 0xC0) },
        { Keys.OpenBracket, (149, 219, 0xDB) },
        { Keys.OemPipe, (150, 220, 0xDC) },
        { Keys.CloseBracket, (151, 221, 0xDD) },
        { Keys.Quote, (152, 222, 0xDE) },
        { Keys.Backslash, (154, 226, 0xE2) },
        { Keys.Play, (167, 250, 0xFA) },
        { Keys.Zoom, (168, 251, 0xFB) }
    };

    private static readonly Dictionary<Keys, string> KeyToJsValues = new()
    {
        { Keys.None, "Unidentified" },
        { Keys.Cancel, "Cancel" },
        { Keys.Backspace, "Backspace" },
        { Keys.Tab, "Tab" },
        { Keys.Clear, "Clear" },
        { Keys.Enter, "Enter" },
        { Keys.Pause, "Pause" },
        { Keys.CapsLock, "CapsLock" },
        { Keys.Escape, "Escape" },
        { Keys.Space, " " },
        { Keys.PageUp, "PageUp" },
        { Keys.PageDown, "PageDown" },
        { Keys.End, "End" },
        { Keys.Home, "Home" },
        { Keys.ArrowLeft, "ArrowLeft" },
        { Keys.ArrowUp, "ArrowUp" },
        { Keys.ArrowRight, "ArrowRight" },
        { Keys.ArrowDown, "ArrowDown" },
        { Keys.Select, "Select" },
        { Keys.Print, "PrintScreen" },
        { Keys.Execute, "Execute" },
        { Keys.PrintScreen, "PrintScreen" },
        { Keys.Insert, "Insert" },
        { Keys.Delete, "Delete" },
        { Keys.Help, "Help" },
        { Keys.D0, "0" },
        { Keys.D1, "1" },
        { Keys.D2, "2" },
        { Keys.D3, "3" },
        { Keys.D4, "4" },
        { Keys.D5, "5" },
        { Keys.D6, "6" },
        { Keys.D7, "7" },
        { Keys.D8, "8" },
        { Keys.D9, "9" },
        { Keys.A, "a" },
        { Keys.B, "b" },
        { Keys.C, "c" },
        { Keys.D, "d" },
        { Keys.E, "e" },
        { Keys.F, "f" },
        { Keys.G, "g" },
        { Keys.H, "h" },
        { Keys.I, "i" },
        { Keys.J, "j" },
        { Keys.K, "k" },
        { Keys.L, "l" },
        { Keys.M, "m" },
        { Keys.N, "n" },
        { Keys.O, "o" },
        { Keys.P, "p" },
        { Keys.Q, "q" },
        { Keys.R, "r" },
        { Keys.S, "s" },
        { Keys.T, "t" },
        { Keys.U, "u" },
        { Keys.V, "v" },
        { Keys.W, "w" },
        { Keys.X, "x" },
        { Keys.Y, "y" },
        { Keys.Z, "z" },
        { Keys.Meta, "Meta" },
        { Keys.LeftMeta, "Meta" },
        { Keys.RightMeta, "Meta" },
        { Keys.Apps, "AppSwitch" },
        { Keys.Sleep, "Standby" },
        { Keys.NumPad0, "0" },
        { Keys.NumPad1, "1" },
        { Keys.NumPad2, "2" },
        { Keys.NumPad3, "3" },
        { Keys.NumPad4, "4" },
        { Keys.NumPad5, "5" },
        { Keys.NumPad6, "6" },
        { Keys.NumPad7, "7" },
        { Keys.NumPad8, "8" },
        { Keys.NumPad9, "9" },
        { Keys.Multiply, "Multiply" },
        { Keys.Add, "Add" },
        { Keys.Separator, "Separator" },
        { Keys.Subtract, "Subtract" },
        { Keys.Decimal, "Decimal" },
        { Keys.Divide, "Divide" },
        { Keys.F1, "F1" },
        { Keys.F2, "F2" },
        { Keys.F3, "F3" },
        { Keys.F4, "F4" },
        { Keys.F5, "F5" },
        { Keys.F6, "F6" },
        { Keys.F7, "F7" },
        { Keys.F8, "F8" },
        { Keys.F9, "F9" },
        { Keys.F10, "F10" },
        { Keys.F11, "F11" },
        { Keys.F12, "F12" },
        { Keys.F13, "F13" },
        { Keys.F14, "F14" },
        { Keys.F15, "F15" },
        { Keys.F16, "F16" },
        { Keys.F17, "F17" },
        { Keys.F18, "F18" },
        { Keys.F19, "F19" },
        { Keys.F20, "F20" },
        { Keys.F21, "Unidentified" },
        { Keys.F22, "Unidentified" },
        { Keys.F23, "Unidentified" },
        { Keys.F24, "Unidentified" },
        { Keys.NumLock, "NumLock" },
        { Keys.ScrollLock, "ScrollLock" },
        { Keys.Shift, "Shift" },
        { Keys.LeftShift, "Shift" },
        { Keys.RightShift, "Shift" },
        { Keys.Control, "Control" },
        { Keys.LeftControl, "Control" },
        { Keys.RightControl, "Control" },
        { Keys.Alt, "Alt" },
        { Keys.LeftAlt, "Alt" },
        { Keys.RightAlt, "Alt" },
        { Keys.BrowserBack, "BrowserBack" },
        { Keys.BrowserForward, "BrowserForward" },
        { Keys.BrowserRefresh, "BrowserRefresh" },
        { Keys.BrowserStop, "BrowserStop" },
        { Keys.BrowserSearch, "BrowserSearch" },
        { Keys.BrowserFavorites, "BrowserFavorites" },
        { Keys.BrowserHome, "BrowserHome" },
        { Keys.VolumeMute, "AudioVolumeMute" },
        { Keys.VolumeDown, "AudioVolumeDown" },
        { Keys.VolumeUp, "AudioVolumeUp" },
        { Keys.MediaNextTrack, "MediaTrackNext" },
        { Keys.MediaPreviousTrack, "MediaTrackPrevious" },
        { Keys.MediaStop, "MediaStop" },
        { Keys.MediaPlayPause, "MediaPlayPause" },
        { Keys.LaunchMail, "LaunchMail" },
        { Keys.SelectMedia, "LaunchMediaPlayer" },
        { Keys.LaunchApplication1, "LaunchApplication1" },
        { Keys.LaunchApplication2, "LaunchApplication2" },
        { Keys.SemiColon, ";" },
        { Keys.Plus, "+" },
        { Keys.Comma, "," },
        { Keys.Minus, "-" },
        { Keys.Period, "." },
        { Keys.QuestionMark, "?" },
        { Keys.OemTilde, "~" },
        { Keys.OpenBracket, "[" },
        { Keys.OemPipe, "|" },
        { Keys.CloseBracket, "]" },
        { Keys.Quote, "'" },
        { Keys.Backslash, "\\" },
        { Keys.Play, "Play" },
        { Keys.Zoom, "ZoomToggle" }
    };

    private static readonly (int WinInput, int WinForm, int Win32)[] ValuesByKey = ToTable(KeyToValues);
    private static readonly string[] JsKeyByKey = ToTable(KeyToJsValues);

    // Later entries win, so the left key beats the generic modifier where they share a code (WPF, and Meta).
    // 171, 254 and 0xFE are the OEM clear key, which maps to Clear as well.
    private static readonly FrozenDictionary<int, Keys> WinInputToKey = BuildReverse(values => values.WinInput, 171);
    private static readonly FrozenDictionary<int, Keys> WinFormToKey = BuildReverse(values => values.WinForm, 254);
    private static readonly FrozenDictionary<int, Keys> Win32ToKey = BuildReverse(values => values.Win32, 0xFE);

    // "0" is D0 rather than NumPad0, "PrintScreen" is PrintScreen rather than Print and "Unidentified" is None.
    private static readonly FrozenDictionary<string, Keys> JsToKeys = BuildJsReverse();

    // Looks a key up straight from the characters of the event, without a string of its own.
    private static readonly FrozenDictionary<string, Keys>.AlternateLookup<ReadOnlySpan<char>> JsToKeysLookup =
        JsToKeys.GetAlternateLookup<ReadOnlySpan<char>>();

    #endregion

    #region Constructor

    /// <summary>Creates the key for a known <see cref="Keys"/> value.</summary>
    public KeyboardKey(Keys key)
    {
        Key = key;
    }

    #endregion

    #region Properties

    /// <summary>The Win32 virtual-key code, set only for an <see cref="Unknown"/> key read from Win32.</summary>
    public int Win32Key { get; private init; }

    /// <summary>The WPF key value, set only for an <see cref="Unknown"/> key read from WPF.</summary>
    public int WinInputKey { get; private init; }

    /// <summary>The WinForms key value, set only for an <see cref="Unknown"/> key read from WinForms.</summary>
    public int WinFormKey { get; private init; }

    /// <summary>The <c>KeyboardEvent.key</c> string, set only for an <see cref="Unknown"/> key read from JavaScript.</summary>
    public string? JsKey { get; private init; }

    /// <summary>The key.</summary>
    public Keys Key { get; }

    /// <summary>Where the raw code of an <see cref="Unknown"/> key came from.</summary>
    public KeySource KeySource { get; private init; } = KeySource.Internal;

    /// <summary>The name of <see cref="Key"/>.</summary>
    public string KeyName => Key.ToString();

    /// <summary>The numeric value of <see cref="Key"/>.</summary>
    public int KeyCode => (int)Key;

    #endregion

    #region Methods

    /// <summary>Creates a key from a <c>KeyboardEvent.key</c> string.</summary>
    public static KeyboardKey FromJs(string key)
    {
        ArgumentNullException.ThrowIfNull(key);

        return JsToKeysLookup.TryGetValue(key, out var value)
            ? new KeyboardKey(value)
            : new KeyboardKey(Keys.Unknown) { JsKey = key, KeySource = KeySource.JavaScript };
    }

    /// <summary>
    ///     Creates a key from a <c>KeyboardEvent.key</c> string held as characters. A string is only created for a key the
    ///     library does not know, to keep its raw value.
    /// </summary>
    public static KeyboardKey FromJs(ReadOnlySpan<char> key)
        => JsToKeysLookup.TryGetValue(key, out var value)
            ? new KeyboardKey(value)
            : new KeyboardKey(Keys.Unknown) { JsKey = key.ToString(), KeySource = KeySource.JavaScript };

    /// <summary>
    ///     Whether a <c>KeyboardEvent.key</c> string is this key, the same answer as comparing with
    ///     <see cref="FromJs(ReadOnlySpan{char})" /> but without creating a key.
    /// </summary>
    /// <param name="jsKey">The string the browser reported.</param>
    public bool IsJs(ReadOnlySpan<char> jsKey)
        => JsToKeysLookup.TryGetValue(jsKey, out var value)
            ? value == Key
            : Key == Keys.Unknown && JsKey is not null && jsKey.SequenceEqual(JsKey);

    /// <summary>Creates a key from a Win32 virtual-key code.</summary>
    public static KeyboardKey FromWin32(int key)
        => Win32ToKey.TryGetValue(key, out var value)
            ? new KeyboardKey(value)
            : new KeyboardKey(Keys.Unknown) { Win32Key = key, KeySource = KeySource.Win32 };

    /// <summary>Creates a key from a WPF <c>System.Windows.Input.Key</c> value.</summary>
    public static KeyboardKey FromWinInput(int key)
        => WinInputToKey.TryGetValue(key, out var value)
            ? new KeyboardKey(value)
            : new KeyboardKey(Keys.Unknown) { WinInputKey = key, KeySource = KeySource.WinInput };

    /// <summary>Creates a key from a <c>System.Windows.Forms.Keys</c> value.</summary>
    public static KeyboardKey FromWinForm(int key)
        => WinFormToKey.TryGetValue(key, out var value)
            ? new KeyboardKey(value)
            : new KeyboardKey(Keys.Unknown) { WinFormKey = key, KeySource = KeySource.WinForm };

    /// <summary>The Win32 virtual-key code; for an <see cref="Unknown"/> key the raw code it was created from.</summary>
    public int ToWin32Key()
        => TryGetValues(out var values) ? values.Win32 : Win32Key;

    /// <summary>The WPF key value; for an <see cref="Unknown"/> key the raw code it was created from.</summary>
    public int ToWinInputKey()
        => TryGetValues(out var values) ? values.WinInput : WinInputKey;

    /// <summary>The WinForms key value; for an <see cref="Unknown"/> key the raw code it was created from.</summary>
    public int ToWinFormKey()
        => TryGetValues(out var values) ? values.WinForm : WinFormKey;

    /// <summary>The <c>KeyboardEvent.key</c> string; for an <see cref="Unknown"/> key the raw string it was created from.</summary>
    public string ToJavaScriptKey()
        => IsTabulated ? JsKeyByKey[(int)Key] : JsKey ?? JsKeyByKey[(int)Keys.None];

    /// <inheritdoc />
    public override string ToString()
        => KeyName;

    /// <inheritdoc />
    public bool Equals(KeyboardKey? other)
        => other is not null
           && Key == other.Key
           && Win32Key == other.Win32Key
           && WinInputKey == other.WinInputKey
           && WinFormKey == other.WinFormKey
           && JsKey == other.JsKey;

    /// <inheritdoc />
    public override bool Equals(object? obj)
        => Equals(obj as KeyboardKey);

    /// <inheritdoc />
    public override int GetHashCode()
        => HashCode.Combine(Key, Win32Key, WinInputKey, WinFormKey, JsKey);

    /// <summary>Two keys are equal when they are the same key, or the same unrecognised raw code.</summary>
    public static bool operator ==(KeyboardKey? left, KeyboardKey? right)
        => Equals(left, right);

    /// <summary>The negation of the equality operator.</summary>
    public static bool operator !=(KeyboardKey? left, KeyboardKey? right)
        => !Equals(left, right);

    // Every key but Unknown, which is the last one, has a row. A plain index beats a hash lookup.
    private bool IsTabulated => (uint)Key < (uint)ValuesByKey.Length;

    private bool TryGetValues(out (int WinInput, int WinForm, int Win32) values)
    {
        if (IsTabulated)
        {
            values = ValuesByKey[(int)Key];

            return true;
        }

        values = default;

        return false;
    }

    private static T[] ToTable<T>(Dictionary<Keys, T> source)
    {
        var table = new T[(int)Keys.Unknown];
        foreach (var (key, value) in source)
        {
            table[(int)key] = value;
        }

        return table;
    }

    private static FrozenDictionary<int, Keys> BuildReverse(Func<(int WinInput, int WinForm, int Win32), int> select, int oemClear)
    {
        var map = new Dictionary<int, Keys>();
        foreach (var (key, values) in KeyToValues)
        {
            map[select(values)] = key;
        }

        map[oemClear] = Keys.Clear;

        return map.ToFrozenDictionary();
    }

    private static FrozenDictionary<string, Keys> BuildJsReverse()
    {
        var map = new Dictionary<string, Keys>();
        foreach (var (key, value) in KeyToJsValues)
        {
            if (key != Keys.Print)
            {
                map.TryAdd(value, key);
            }
        }

        return map.ToFrozenDictionary(StringComparer.Ordinal);
    }

    #endregion
}
