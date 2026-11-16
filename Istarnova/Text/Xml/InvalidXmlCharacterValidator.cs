#region "Header"

// File: InvalidXmlCharacterValidator.cs
// Created 2026/03/06
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

#region "Usings"

using System.Buffers;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

#endregion

namespace Istarnova.Text.Xml;

/// <summary>
///     XML character validation and cleaning utilities.
///     Provides high-performance methods to sanitize XML content.
/// </summary>
public static partial class InvalidXmlCharacterValidator
{
    #region "Methods"

    /// <param name="content">The string to clean</param>
    extension(string? content)
    {
        #region "Methods"

        /// <summary>
        ///     Attempts to fix common XML encoding issues.
        /// </summary>
        /// <returns>Fixed content</returns>
        public string? FixXmlEncoding()
        {
            if (string.IsNullOrEmpty(content))
            {
                return content;
            }

            var result = ReplaceInOnePass(content, EncodingFixes, EncodingFixLeadingCharacters, StringComparison.Ordinal);

            return result.SanitizeForXml();
        }

        /// <summary>
        ///     Fast validation check to see if a string contains invalid XML characters.
        /// </summary>
        /// <returns>True if the string is valid for XML</returns>
        public bool IsValidXmlContent()
        {
            if (string.IsNullOrEmpty(content))
            {
                return true;
            }

            var span = content.AsSpan();

            foreach (var t in span)
            {
                if (!IsValidXmlChar(t))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        ///     Removes invalid XML control characters using modern Span-based processing.
        /// </summary>
        /// <returns>String without invalid control characters</returns>
        public string? RemoveInvalidControlChars()
        {
            if (string.IsNullOrEmpty(content))
            {
                return content;
            }

            // Fast path: check if any invalid chars exist
            var span = content.AsSpan();
            var hasInvalidChars = false;

            foreach (var t in span)
            {
                if (IsValidXmlChar(t))
                {
                    continue;
                }

                hasInvalidChars = true;
                break;
            }

            if (!hasInvalidChars)
            {
                return content;
            }

            // Use StringBuilder only when needed
            var stringBuilder = new StringBuilder(content.Length);

            foreach (var c in span)
            {
                if (IsValidXmlChar(c))
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        ///     Decodes HTML entities and removes invalid XML characters in one pass.
        ///     Uses modern .NET HTML decoding instead of manual dictionary lookups.
        /// </summary>
        /// <returns>Clean XML-compatible string</returns>
        public string? RemoveInvalidXmlChars()
        {
            if (string.IsNullOrEmpty(content))
            {
                return content;
            }

            // Step 1: Decode HTML entities using modern .NET WebUtility
            var decoded = WebUtility.HtmlDecode(content);

            // Step 2: Handle special cases that WebUtility doesn't cover
            decoded = ReplaceInOnePass(decoded, LineBreakTags, LineBreakLeadingCharacters, StringComparison.OrdinalIgnoreCase);

            // Step 3: Remove invalid XML control characters
            return decoded.RemoveInvalidControlChars();
        }

        /// <summary>
        ///     Sanitizes XML content by removing invalid characters and decoding entities.
        ///     This is the main method you should use for XML sanitization.
        /// </summary>
        /// <returns>XML-safe content</returns>
        public string? SanitizeForXml() =>
            string.IsNullOrEmpty(content)
                ? content
                :
                // Use the modern approach
                content.RemoveInvalidXmlChars();

        #endregion
    }

    #endregion

    #region "Static/Constants"

    // Generated at build time, so RegexOptions.Compiled would do nothing here.
    [GeneratedRegex(@"[\x00-\x08\x0B\x0C\x0E-\x1F]")]
    private static partial Regex InvalidControlCharsRegex();

    [GeneratedRegex(@"&[a-zA-Z][a-zA-Z0-9]*;|&#[0-9]+;|&#x[0-9a-fA-F]+;")]
    private static partial Regex HtmlEntityRegex();

    // Text that went through the wrong encoding, with the character it stands for.
    private static readonly (string Needle, string Replacement)[] EncodingFixes =
    [
        ("â\u0080\u0099", "'"), // Smart quote (')
        ("â\u0080\u009C", "\""), // Smart quote (")
        ("â\u0080\u009D", "\""), // Smart quote (")
        ("â\u0080\u0093", "–"), // En dash
        ("â\u0080\u0094", "—"), // Em dash
        ("Â ", " "), // Non-breaking space issue
    ];

    private static readonly SearchValues<char> EncodingFixLeadingCharacters = SearchValues.Create("âÂ");

    private static readonly (string Needle, string Replacement)[] LineBreakTags =
    [
        ("&lt;br /&gt;", "\r\n"),
        ("<br />", "\r\n"),
        ("<br>", "\r\n"),
    ];

    private static readonly SearchValues<char> LineBreakLeadingCharacters = SearchValues.Create("&<");

    /// <summary>
    ///     Replaces every needle in a single pass. The string is returned as it is, without a copy, when none of them
    ///     occurs; otherwise one builder collects the result instead of one string per replacement.
    /// </summary>
    /// <param name="text">The text to search.</param>
    /// <param name="replacements">The needles and what each becomes. None of the replacements may contain a needle.</param>
    /// <param name="firstCharacters">Every character a needle can start with, for the fast search.</param>
    /// <param name="comparison">How a needle is compared.</param>
    private static string ReplaceInOnePass(
        string text,
        ReadOnlySpan<(string Needle, string Replacement)> replacements,
        SearchValues<char> firstCharacters,
        StringComparison comparison)
    {
        var remaining = text.AsSpan();
        var index = remaining.IndexOfAny(firstCharacters);
        if (index < 0)
        {
            return text;
        }

        StringBuilder? builder = null;
        while (index >= 0)
        {
            builder ??= new StringBuilder(text.Length);
            builder.Append(remaining[..index]);
            remaining = remaining[index..];

            var matched = false;
            foreach (var (needle, replacement) in replacements)
            {
                if (remaining.StartsWith(needle, comparison))
                {
                    builder.Append(replacement);
                    remaining = remaining[needle.Length..];
                    matched = true;
                    break;
                }
            }

            if (!matched)
            {
                builder.Append(remaining[0]);
                remaining = remaining[1..];
            }

            index = remaining.IndexOfAny(firstCharacters);
        }

        return builder!.Append(remaining).ToString();
    }

    /// <summary>
    ///     Set of valid XML characters according to XML 1.0 specification.
    /// </summary>
    private static bool IsValidXmlChar(char c) =>
        c == 0x09 || c == 0x0A || c == 0x0D ||
        (c >= 0x20 && c <= 0xD7FF) ||
        (c >= 0xE000 && c <= 0xFFFD);

    #endregion
}
