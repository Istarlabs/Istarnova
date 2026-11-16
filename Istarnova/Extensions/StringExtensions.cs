#region "Header"

// File: StringExtensions.cs
// Created 2026/08/22
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

#region "Usings"

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

#endregion

namespace Istarnova.Extensions;

public static class StringExtensions
{
    #region "Constants"

    private const int MaxStackAlloc = 1024;

    private static readonly char[] Separator = ['.', '!', '?'];

    #endregion

    #region "Methods"

    /// <param name="str">The string to shorten.</param>
    extension(string str)
    {
        #region "Methods"

        public Uri AsUri() => new(str, UriKind.RelativeOrAbsolute);

        /// <summary>
        ///     Displays a shortened version of the string if it exceeds the maximum allowed characters.
        /// </summary>
        /// <param name="maxAllowedCharacters">The maximum number of characters allowed. Default is 100.</param>
        /// <returns>Returns the original string or a shortened version with "..." appended.</returns>
        public string DisplayShortText(int maxAllowedCharacters = 100)
        {
            ArgumentNullException.ThrowIfNull(str);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxAllowedCharacters);

            return str.Length > maxAllowedCharacters
                ? string.Concat(str.AsSpan(0, maxAllowedCharacters), " ...")
                : str;
        }

        public bool ContainsInvariantIgnoreCase(string value)
            => str.NotEmptyOrWhiteSpace() && value.NotEmptyOrWhiteSpace()
                                          && str.Contains(value, StringComparison.InvariantCultureIgnoreCase);

        /// <summary>
        ///     Whether the string contains the value, compared ordinally. False when either of them is null or empty.
        /// </summary>
        /// <param name="value">The text to look for.</param>
        public bool ContainsOrdinal(string value)
            => str.NotEmpty() && value.NotEmpty()
                              && str.Contains(value, StringComparison.Ordinal);

        /// <summary>
        ///     Whether the string ends with the value, compared case-insensitively with the invariant culture.
        ///     False when either of them is null or empty.
        /// </summary>
        /// <param name="value">The text the string should end with.</param>
        public bool EndsWithInvariantIgnoreCase(string value)
            => str.NotEmpty() && value.NotEmpty()
                              && str.EndsWith(value, StringComparison.InvariantCultureIgnoreCase);

        /// <summary>
        ///     Whether the string ends with the value, compared ordinally. False when either of them is null or empty.
        /// </summary>
        /// <param name="value">The text the string should end with.</param>
        public bool EndsWithOrdinal(string value)
            => str.NotEmpty() && value.NotEmpty()
                              && str.EndsWith(value, StringComparison.Ordinal);

        /// <summary>
        ///     Cuts string to given maximal character length by also taking care to not cut a word.
        /// </summary>
        /// <param name="count">The amount of characters the cut string should have. Default is 120.</param>
        /// <returns>Returns a new string having only the maximal length of the specified count.</returns>
        public string LimitOnWordBoundary(int count = 120)
        {
            ArgumentNullException.ThrowIfNull(str);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(count);

            if (str.Length <= count - 3)
            {
                return str;
            }

            var span = str.AsSpan();
            var searchLength = count - 3;

            var lastSpaceIndex = span[..searchLength].LastIndexOf(' ');
            if (lastSpaceIndex > 0 && lastSpaceIndex > count - 20)
            {
                // limits the backward search to a max of 20 chars
                return span[..lastSpaceIndex].SpanConcat("...");
            }

            // No space in the last 20 chars, so get all the string minus 3
            return span[..searchLength].SpanConcat("...");
        }

        /// <summary>
        ///     Whether the string starts with the value, compared case-insensitively with the invariant culture.
        ///     False when either of them is null or empty.
        /// </summary>
        /// <param name="value">The text the string should start with.</param>
        public bool StartsWithInvariantIgnoreCase(string value)
            => str.NotEmpty() && value.NotEmpty()
                              && str.StartsWith(value, StringComparison.InvariantCultureIgnoreCase);

        /// <summary>
        ///     Whether the string starts with the value, compared ordinally. False when either of them is null or empty.
        /// </summary>
        /// <param name="value">The text the string should start with.</param>
        public bool StartsWithOrdinal(string value)
            => str.NotEmpty() && value.NotEmpty()
                              && str.StartsWith(value, StringComparison.Ordinal);

        /// <summary>
        ///     Converts a data URL string to a byte array.
        /// </summary>
        /// <returns>Returns a byte array of the decoded data, or an empty array if invalid.</returns>
        public byte[] ToBytes()
        {
            ArgumentNullException.ThrowIfNull(str);

            var span = str.AsSpan();
            var pos = span.IndexOf(',');

            if (pos < 0)
            {
                return [];
            }

            var base64Span = span[(pos + 1)..];

            // Estimate buffer size for base64 decoding
            var maxDecodedLength = (base64Span.Length * 3 + 3) / 4;
            var buffer = maxDecodedLength <= MaxStackAlloc
                ? stackalloc byte[maxDecodedLength]
                : new byte[maxDecodedLength];

            return Convert.TryFromBase64Chars(base64Span, buffer, out var bytesWritten)
                ? [.. buffer[..bytesWritten]]
                : [];
        }

        public bool EqualsIgnoreCase(string cmp) =>
            string.Equals(str, cmp, StringComparison.InvariantCultureIgnoreCase);

        public bool EqualsOrdinal(string cmp) => string.Equals(str, cmp, StringComparison.Ordinal);
        public bool EqualsOrdinalIgnoreCase(string cmp) => string.Equals(str, cmp, StringComparison.OrdinalIgnoreCase);

        /// <summary>
        ///     Splits the string into blocks at the blank lines between them - the shape of a text input made of
        ///     paragraphs. The lines inside a block stay together.
        /// </summary>
        /// <param name="options">Empty blocks are dropped by default.</param>
        public string[] SplitByDoubleNewLine(StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries)
        {
            ArgumentNullException.ThrowIfNull(str);

            return str.Split(Environment.NewLine + Environment.NewLine, options);
        }

        /// <summary>
        ///     Splits the string at its line breaks.
        /// </summary>
        /// <param name="options">Empty lines are dropped by default.</param>
        public string[] SplitByNewLine(StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries)
            => str.SplitBy(StringSplitBy.NewLine, options);

        /// <summary>
        ///     Splits the string at its spaces.
        /// </summary>
        /// <param name="options">Empty parts are dropped by default.</param>
        public string[] SplitBySpace(StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries)
            => str.SplitBy(StringSplitBy.Space, options);

        /// <summary>
        ///     Turns a number of days, as chosen in a filter, into the point in time that many days ago.
        /// </summary>
        /// <returns>The UTC time <c>days</c> days before now, or <c>null</c> when the string is not a whole number.</returns>
        public DateTime? SelectedSince()
            => int.TryParse(str, CultureInfo.InvariantCulture, out var days)
                ? DateTime.UtcNow.AddDays(-days)
                : null;

        #endregion
    }

    extension([NotNullWhen(true)] string? text)
    {
        #region "Methods"

        /// <summary>
        ///     Whether the string holds at least one character. A <c>true</c> answer tells the compiler the string is not null.
        /// </summary>
        public bool NotEmpty()
            => !string.IsNullOrEmpty(text);

        /// <summary>
        ///     Whether the string holds at least one character that is not white space.
        /// </summary>
        public bool NotEmptyOrWhiteSpace()
            => !string.IsNullOrWhiteSpace(text);

        #endregion
    }

    extension([NotNullWhen(false)] string? text)
    {
        #region "Methods"

        /// <summary>
        ///     Whether the string is null or has no characters.
        /// </summary>
        public bool IsEmpty()
            => string.IsNullOrEmpty(text);

        /// <summary>
        ///     Whether the string is null, has no characters, or only white space.
        /// </summary>
        public bool IsEmptyOrWhiteSpace()
            => string.IsNullOrWhiteSpace(text);

        #endregion
    }

    extension(ICollection<string> list)
    {
        #region "Methods"

        public string JoinBy(string separator)
            => list.Count == 0 ? string.Empty : string.Join(separator, list);

        public string JoinBy(StringJoinBy joinBy)
            => list.Count == 0 ? string.Empty : string.Join(joinBy.GetSeperatorCharacter(), list);

        public string JoinByNewLine(string? newLine = null)
            => string.Join(newLine ?? Environment.NewLine, list);

        #endregion
    }

    extension(string? text)
    {
        #region "Methods"

        /// <summary>
        ///     Whether the text contains the value, compared ordinally and ignoring case. False when either of them is null or empty.
        /// </summary>
        /// <param name="value">The text to look for.</param>
        public bool ContainsOrdinalIgnoreCase(string? value)
            => text.NotEmpty() && value.NotEmpty()
                               && text.Contains(value, StringComparison.OrdinalIgnoreCase);

        /// <summary>
        ///     Whether the text starts with the value, compared ordinally and ignoring case. False when either of them is null or empty.
        /// </summary>
        /// <param name="value">The text the string should start with.</param>
        public bool StartsWithOrdinalIgnoreCase(string? value)
            => text.NotEmpty() && value.NotEmpty()
                               && text.StartsWith(value, StringComparison.OrdinalIgnoreCase);

        /// <summary>
        ///     Whether the text ends with the value, compared ordinally and ignoring case. False when either of them is null or empty.
        /// </summary>
        /// <param name="value">The text the string should end with.</param>
        public bool EndsWithOrdinalIgnoreCase(string? value)
            => text.NotEmpty() && value.NotEmpty()
                               && text.EndsWith(value, StringComparison.OrdinalIgnoreCase);

        /// <summary>
        ///     The string, or null when it has no characters - for a value that is optional where "" would mean something else.
        /// </summary>
        public string? NullIfEmpty()
            => string.IsNullOrEmpty(text) ? null : text;

        /// <summary>
        ///     The string, or null when it has no characters or only white space.
        /// </summary>
        public string? NullIfWhiteSpace()
            => string.IsNullOrWhiteSpace(text) ? null : text;

        /// <summary>
        ///     Turns the text back to front, by what a reader sees rather than by char: an emoji made of
        ///     two chars, or a letter followed by a combining accent, stays in one piece.
        /// </summary>
        /// <returns>Returns the reversed text, or an empty string when there is nothing to reverse.</returns>
        public string ReverseText()
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            // Turning the text around keeps its length, so the result is written into its own memory in
            // one pass: take the next text element from the front, copy it to the back.
            return string.Create(text.Length, text, static (destination, source) =>
            {
                var remaining = source.AsSpan();
                var written = destination.Length;

                while (!remaining.IsEmpty)
                {
                    var length = StringInfo.GetNextTextElementLength(remaining);

                    remaining[..length].CopyTo(destination[(written -= length)..]);
                    remaining = remaining[length..];
                }
            });
        }

        /// <summary>
        ///     Alternates lower and upper case over the whole text, starting lower: "hello" becomes
        ///     "hElLo".
        /// </summary>
        /// <returns>Returns the alternated text, or an empty string when there is nothing to convert.</returns>
        public string ToAlternatingCase()
            => ConvertToAlternatingCase(text, true);

        /// <summary>
        ///     Joins the words of the text into one, each but the first starting with a capital:
        ///     "routine check-up" becomes "routineCheckUp". Anything that is not a letter or a digit
        ///     separates two words and does not survive.
        /// </summary>
        /// <returns>Returns the text in camelCase, or an empty string when there is nothing to convert.</returns>
        public string ToCamelCase()
            => ConvertToCamelCase(text, false);

        /// <summary>
        ///     Joins the words of the text with hyphens, all upper case: "routine check-up" becomes
        ///     "ROUTINE-CHECK-UP".
        /// </summary>
        /// <returns>Returns the text in COBOL-CASE, or an empty string when there is nothing to convert.</returns>
        public string ToCobolCase()
            => ConvertToDelimited(text, '-', true);

        /// <summary>
        ///     Joins the words of the text with underscores, all upper case: "routine check-up" becomes
        ///     "ROUTINE_CHECK_UP".
        /// </summary>
        /// <returns>Returns the text in CONSTANT_CASE, or an empty string when there is nothing to convert.</returns>
        public string ToConstantCase()
            => ConvertToDelimited(text, '_', true);

        /// <summary>
        ///     Joins the words of the text with hyphens, all lower case: "routine check-up" becomes
        ///     "routine-check-up".
        /// </summary>
        /// <returns>Returns the text in kebab-case, or an empty string when there is nothing to convert.</returns>
        public string ToKebabCase()
            => ConvertToDelimited(text, '-', false);

        /// <summary>
        ///     Joins the words of the text into one, each starting with a capital: "routine check-up"
        ///     becomes "RoutineCheckUp". Anything that is not a letter or a digit separates two words and
        ///     does not survive.
        /// </summary>
        /// <remarks>
        ///     Letters keep their alphabet: "Facharztüberweisung" stays as it is, umlauts and all. Where
        ///     the result has to be ASCII - an identifier, a column that stores one - transliterate
        ///     before converting.
        /// </remarks>
        /// <returns>Returns the text in PascalCase, or an empty string when there is nothing to convert.</returns>
        public string ToPascalCase()
            => ConvertToCamelCase(text, true);

        /// <summary>
        ///     Alternates upper and lower case over the whole text, starting upper: "hello" becomes
        ///     "HeLlO".
        /// </summary>
        /// <returns>Returns the alternated text, or an empty string when there is nothing to convert.</returns>
        public string ToReverseAlternatingCase()
            => ConvertToAlternatingCase(text, false);

        /// <summary>
        ///     Upper-cases the first letter of every sentence and lower-cases the rest, keeping the text
        ///     as text: "hello there. HOW are you?" becomes "Hello there. How are you?".
        /// </summary>
        /// <returns>Returns the text in sentence case, or an empty string when there is nothing to convert.</returns>
        public string ToSentenceCase()
            => ConvertToSentenceCase(text);

        /// <summary>
        ///     Joins the words of the text with underscores, all lower case: "routine check-up" becomes
        ///     "routine_check_up".
        /// </summary>
        /// <returns>Returns the text in snake_case, or an empty string when there is nothing to convert.</returns>
        public string ToSnakeCase()
            => ConvertToDelimited(text, '_', false);

        /// <summary>
        ///     Upper-cases the first letter of every word and lower-cases the rest, keeping the
        ///     separators: "the QUICK fox" becomes "The Quick Fox".
        /// </summary>
        /// <returns>Returns the text in title case, or an empty string when there is nothing to convert.</returns>
        public string ToTitleCase()
            => ConvertToTitleCase(text);

        /// <summary>
        ///     Joins the words of the text with hyphens, each starting on a capital: "routine check-up"
        ///     becomes "Routine-Check-Up".
        /// </summary>
        /// <returns>Returns the text in Train-Case, or an empty string when there is nothing to convert.</returns>
        public string ToTrainCase()
            => ConvertToTrainCase(text);

        #endregion
    }

    extension(string text)
    {
        #region "Methods"

        public string[] SplitByLength(int maxLength)
        {
            var result = new List<string>();
            var idx = 0;

            while (idx < text.Length)
            {
                var length = Math.Min(maxLength, text.Length - idx);
                result.Add(text[idx..length]);
                idx += length;
            }

            return [.. result];
        }

        public string[] SplitText(int maxLength)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return [];
            }

            var sentences = text.Split(Separator, StringSplitOptions.RemoveEmptyEntries);

            if (sentences.Length == 0)
            {
                return text.SplitByLength(maxLength);
            }

            var sb = new StringBuilder();
            var result = new List<string>();

            foreach (var sentence in sentences)
            {
                if (sb.Length + sentence.Length + 1 <= maxLength)
                {
                    if (sb.Length > 0)
                    {
                        sb.Append(' ');
                    }
                }
                else
                {
                    result.Add(sb.ToString());
                    sb.Clear();
                }

                sb.Append(sentence);
            }

            if (sb.Length > 0)
            {
                result.Add(sb.ToString());
            }

            return [.. result];
        }

        #endregion
    }

    /// <summary>
    ///     Extracts the host from a URI string.
    /// </summary>
    /// <param name="uri">The URI string to extract the host from.</param>
    /// <returns>Returns the host part of the URI, or null if the URI is invalid or empty.</returns>
    public static string? GetHostFromUri(this string? uri)
    {
        if (string.IsNullOrWhiteSpace(uri))
        {
            return null;
        }

        return Uri.TryCreate(uri, UriKind.Absolute, out var result) ? result.Host : null;
    }

    public static string SpanConcat(this ReadOnlySpan<char> span, ReadOnlySpan<char> suffix)
    {
        if (span.IsEmpty)
        {
            return suffix.IsEmpty ? string.Empty : new(suffix);
        }

        if (suffix.IsEmpty)
        {
            return new(span);
        }

        var totalLength = span.Length + suffix.Length;

        var result = totalLength <= MaxStackAlloc
            ? stackalloc char[totalLength]
            : new char[totalLength];

        span.CopyTo(result);
        suffix.CopyTo(result[span.Length..]);

        return new(result);
    }

    /// <summary>
    ///     Alternates upper and lower case over the whole text, separators counting as their own step -
    ///     which is what keeps the pattern going across a space instead of restarting on every word.
    /// </summary>
    /// <param name="text">The text to convert.</param>
    /// <param name="startLower">Whether the first character is lower case.</param>
    private static string ConvertToAlternatingCase(string? text, bool startLower)
    {
        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }

        return string.Create(text.Length, (text, startLower), static (destination, state) =>
        {
            var (source, lower) = state;

            for (var index = 0; index < source.Length; index++)
            {
                destination[index] = lower
                    ? char.ToLowerInvariant(source[index])
                    : char.ToUpperInvariant(source[index]);
                lower = !lower;
            }
        });
    }

    /// <summary>
    ///     Walks the text once and rebuilds it without its separators: every letter or digit is taken
    ///     over, everything else is dropped and marks the next letter as the start of a word.
    /// </summary>
    /// <param name="text">The text to convert.</param>
    /// <param name="upperFirst">Whether the first letter of the result is upper case.</param>
    private static string ConvertToCamelCase(string? text, bool upperFirst)
    {
        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }

        // Separators are dropped and nothing is ever added, so the result cannot be longer than the
        // input: one buffer of that size always holds it, and the string is built once, at the end.
        var buffer = text.Length <= MaxStackAlloc ? stackalloc char[text.Length] : new char[text.Length];

        return new(buffer[..WriteCamelCase(text, buffer, upperFirst)]);
    }

    /// <summary>
    ///     Walks the text once and puts a delimiter between its words: every letter or digit is taken
    ///     over in one case, and a run of anything else becomes a single delimiter - but only where a
    ///     word follows, so the result neither starts nor ends with one.
    /// </summary>
    /// <param name="text">The text to convert.</param>
    /// <param name="delimiter">The character that separates two words.</param>
    /// <param name="upperCase">Whether the letters are upper case.</param>
    private static string ConvertToDelimited(string? text, char delimiter, bool upperCase)
    {
        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }

        // A delimiter only ever stands where at least one character was dropped, so the result stays
        // within the input's length.
        var buffer = text.Length <= MaxStackAlloc ? stackalloc char[text.Length] : new char[text.Length];

        return new(buffer[..WriteDelimited(text, buffer, delimiter, upperCase)]);
    }

    /// <summary>
    ///     Upper-cases the first letter of every sentence and lower-cases the rest. A sentence ends on
    ///     '.', '?', '!' or a line break; everything that is not a letter or a digit is kept as it is,
    ///     so the text stays readable rather than becoming one word.
    /// </summary>
    /// <param name="text">The text to convert.</param>
    private static string ConvertToSentenceCase(string? text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }

        // Every character is kept and only its case changes, so the length of the result is known up
        // front: it can be written straight into the string's own memory.
        return string.Create(text.Length, text, static (destination, source) =>
        {
            var newSentence = true;

            for (var index = 0; index < source.Length; index++)
            {
                var character = source[index];

                if (character is '.' or '?' or '!' or '\n')
                {
                    destination[index] = character;
                    newSentence = true;

                    continue;
                }

                if (!char.IsLetterOrDigit(character))
                {
                    destination[index] = character;

                    continue;
                }

                destination[index] = newSentence
                    ? char.ToUpperInvariant(character)
                    : char.ToLowerInvariant(character);
                newSentence = false;
            }
        });
    }

    /// <summary>
    ///     Upper-cases the first letter of every word and lower-cases the rest, keeping the separators:
    ///     "the QUICK fox" becomes "The Quick Fox".
    /// </summary>
    /// <remarks>
    ///     Deliberately not <c>TextInfo.ToTitleCase</c>, which leaves a word that is already all upper
    ///     case alone ("QUICK" stays "QUICK") and follows the culture's rules about small words.
    /// </remarks>
    /// <param name="text">The text to convert.</param>
    private static string ConvertToTitleCase(string? text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }

        return string.Create(text.Length, text, static (destination, source) =>
        {
            var startOfWord = true;

            for (var index = 0; index < source.Length; index++)
            {
                var character = source[index];

                if (!char.IsLetterOrDigit(character))
                {
                    destination[index] = character;
                    startOfWord = true;

                    continue;
                }

                destination[index] = startOfWord
                    ? char.ToUpperInvariant(character)
                    : char.ToLowerInvariant(character);
                startOfWord = false;
            }
        });
    }

    /// <summary>
    ///     Like <see cref="ConvertToDelimited" />, with every word starting on a capital: "Train-Case".
    /// </summary>
    /// <param name="text">The text to convert.</param>
    private static string ConvertToTrainCase(string? text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }

        var buffer = text.Length <= MaxStackAlloc ? stackalloc char[text.Length] : new char[text.Length];

        return new(buffer[..WriteTrainCase(text, buffer)]);
    }

    private static int WriteCamelCase(ReadOnlySpan<char> text, Span<char> destination, bool upperFirst)
    {
        var written = 0;
        var startOfWord = upperFirst;

        foreach (var character in text)
        {
            if (char.IsLetterOrDigit(character))
            {
                destination[written++] = startOfWord
                    ? char.ToUpperInvariant(character)
                    : char.ToLowerInvariant(character);
                startOfWord = false;

                continue;
            }

            // A line is its own text: it starts the way the whole text started, so a converted list
            // reads the same on every line.
            if (character == '\n')
            {
                destination[written++] = character;
                startOfWord = upperFirst;

                continue;
            }

            startOfWord = true;
        }

        return written;
    }

    private static int WriteDelimited(ReadOnlySpan<char> text, Span<char> destination, char delimiter, bool upperCase)
    {
        var written = 0;
        var pendingDelimiter = false;
        var wordStarted = false;

        foreach (var character in text)
        {
            if (char.IsLetterOrDigit(character))
            {
                if (pendingDelimiter && wordStarted)
                {
                    destination[written++] = delimiter;
                }

                destination[written++] = upperCase
                    ? char.ToUpperInvariant(character)
                    : char.ToLowerInvariant(character);
                pendingDelimiter = false;
                wordStarted = true;

                continue;
            }

            if (character == '\n')
            {
                destination[written++] = character;
                pendingDelimiter = false;
                wordStarted = false;

                continue;
            }

            pendingDelimiter = true;
        }

        return written;
    }

    private static int WriteTrainCase(ReadOnlySpan<char> text, Span<char> destination)
    {
        var written = 0;
        var pendingDelimiter = false;
        var startOfWord = true;

        foreach (var character in text)
        {
            if (char.IsLetterOrDigit(character))
            {
                if (pendingDelimiter && !startOfWord)
                {
                    destination[written++] = '-';
                    startOfWord = true;
                }

                destination[written++] = startOfWord
                    ? char.ToUpperInvariant(character)
                    : char.ToLowerInvariant(character);
                pendingDelimiter = false;
                startOfWord = false;

                continue;
            }

            if (character == '\n')
            {
                destination[written++] = character;
                pendingDelimiter = false;
                startOfWord = true;

                continue;
            }

            pendingDelimiter = true;
        }

        return written;
    }

    #endregion
}
