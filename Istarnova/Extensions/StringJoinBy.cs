#region "Header"

// File: StringJoinBy.cs
// Created 2026/08/22
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

#region "Usings"

using NetEscapades.EnumGenerators;

#endregion

namespace Istarnova.Extensions;

[EnumExtensions]
public enum StringJoinBy
{
    NewLine,
    Comma,
    CommaNoSpace,
    Semicolon,
    SemiColonNoSpace,
    Space,
    Tab,
}

public static partial class StringJoinByExtensions
{
    #region "Methods"

    extension(StringJoinBy joinBy)
    {
        #region "Methods"

        public string GetSeperatorCharacter() =>
            joinBy switch
            {
                StringJoinBy.NewLine => Environment.NewLine,
                StringJoinBy.Comma => ", ",
                StringJoinBy.CommaNoSpace => ",",
                StringJoinBy.Semicolon => "; ",
                StringJoinBy.SemiColonNoSpace => ";",
                StringJoinBy.Space => " ",
                StringJoinBy.Tab => "\t",
                var _ => throw new ArgumentOutOfRangeException(nameof(joinBy), joinBy, "Invalid StringJoinBy value"),
            };

        #endregion
    }

    #endregion
}
