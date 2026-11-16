#region "Header"

// File: StringSplitBy.cs
// Created 2026/10/29
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

#region "Usings"

using NetEscapades.EnumGenerators;

#endregion

namespace Istarnova.Extensions;

/// <summary>
///     What a text is split at. The counterpart of <see cref="StringJoinBy" />, except that the separators
///     carry no trailing space: <c>"a, b"</c> splits at <see cref="Comma" /> into <c>"a"</c> and <c>" b"</c>.
/// </summary>
[EnumExtensions]
public enum StringSplitBy
{
    NewLine,
    Comma,
    Semicolon,
    Space,
    Tab,
}

public static partial class StringSplitByExtensions
{
    #region "Methods"

    extension(StringSplitBy splitBy)
    {
        #region "Methods"

        /// <summary>
        ///     The text the value stands for.
        /// </summary>
        public string GetSeparator() =>
            splitBy switch
            {
                StringSplitBy.NewLine => Environment.NewLine,
                StringSplitBy.Comma => ",",
                StringSplitBy.Semicolon => ";",
                StringSplitBy.Space => " ",
                StringSplitBy.Tab => "\t",
                var _ => throw new ArgumentOutOfRangeException(nameof(splitBy), splitBy, "Invalid StringSplitBy value"),
            };

        #endregion
    }

    extension(string text)
    {
        #region "Methods"

        /// <summary>
        ///     Splits the string at the separator the value stands for.
        /// </summary>
        /// <param name="splitBy">What to split at.</param>
        /// <param name="options">Empty entries are dropped by default.</param>
        public string[] SplitBy(StringSplitBy splitBy, StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries)
        {
            ArgumentNullException.ThrowIfNull(text);

            return text.Split(splitBy.GetSeparator(), options);
        }

        #endregion
    }

    extension(ReadOnlySpan<char> text)
    {
        #region "Methods"

        /// <summary>
        ///     Walks over the parts of the text between the separators without allocating. Empty parts are
        ///     included, so a caller that does not want them skips them.
        /// </summary>
        /// <param name="splitBy">What to split at.</param>
        /// <returns>An enumerator of <see cref="Range" />, to be applied to the same text.</returns>
        public MemoryExtensions.SpanSplitEnumerator<char> SplitBy(StringSplitBy splitBy)
            => text.Split(splitBy.GetSeparator());

        #endregion
    }

    #endregion
}
