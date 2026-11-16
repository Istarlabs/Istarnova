#region "Header"

// File: AlphanumComparator.cs
// Created 2026/03/06
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

#region "Usings"

using System.Globalization;
using System.Numerics;

#endregion

namespace Istarnova.Generics.Comparer;

/// <summary>
///     Alphanumeric string comparer that handles numbers naturally (e.g., "file10" comes after "file2").
///     Based on Dave Koelle's Alphanum algorithm (https://www.davekoelle.com/alphanum.html), span-based.
/// </summary>
public sealed class AlphanumComparator : IComparer<string>
{
    #region "Constants"

    private static readonly AlphanumComparator SInstance = new();

    #endregion

    #region "Properties"

    /// <summary>
    ///     Gets the singleton instance of the AlphanumComparator.
    /// </summary>
    public static AlphanumComparator Instance => SInstance;

    #endregion

    #region "Methods"

    /// <summary>
    ///     Compares two strings alphanumerically.
    /// </summary>
    /// <param name="s1">First string to compare</param>
    /// <param name="s2">Second string to compare</param>
    /// <returns>A signed integer that indicates the relative values of s1 and s2</returns>
    public int Compare(string? s1, string? s2)
    {
        // Handle null cases using modern pattern matching
        return (s1, s2) switch
        {
            (null, null) => 0,
            (null, var _) => -1,
            (var _, null) => 1,
            var _ => CompareCore(s1.AsSpan(), s2.AsSpan()),
        };
    }

    private static int CompareChunks(ReadOnlySpan<char> chunk1, ReadOnlySpan<char> chunk2)
    {
        var first1 = chunk1[0];
        var first2 = chunk2[0];

        // Both chunks are numeric
        if ((char.IsAsciiDigit(first1) || IsFullWidthDigit(first1)) &&
            (char.IsAsciiDigit(first2) || IsFullWidthDigit(first2)))
        {
            return CompareNumericChunks(chunk1, chunk2);
        }

        // Both chunks are alphabetic - use ordinal comparison for performance
        return chunk1.CompareTo(chunk2, StringComparison.Ordinal);
    }

    private static int CompareCore(ReadOnlySpan<char> s1, ReadOnlySpan<char> s2)
    {
        int thisMarker = 0, thatMarker = 0;

        while (thisMarker < s1.Length || thatMarker < s2.Length)
        {
            if (thisMarker >= s1.Length)
            {
                return -1;
            }

            if (thatMarker >= s2.Length)
            {
                return 1;
            }

            // Extract chunks using spans for better performance
            var thisChunk = ExtractChunk(s1, ref thisMarker);
            var thatChunk = ExtractChunk(s2, ref thatMarker);

            // Compare chunks
            var result = CompareChunks(thisChunk, thatChunk);
            if (result != 0)
            {
                return result;
            }
        }

        return 0;
    }

    private static int CompareNumericChunks(ReadOnlySpan<char> chunk1, ReadOnlySpan<char> chunk2)
    {
        // Convert full-width digits and parse as BigInteger to avoid overflow
        var normalizedChunk1 = NormalizeNumericChunk(chunk1);
        var normalizedChunk2 = NormalizeNumericChunk(chunk2);

        // Use BigInteger to handle arbitrarily large numbers - solves the TODO!
        if (BigInteger.TryParse(normalizedChunk1, NumberStyles.None, CultureInfo.InvariantCulture, out var num1) &&
            BigInteger.TryParse(normalizedChunk2, NumberStyles.None, CultureInfo.InvariantCulture, out var num2))
        {
            return num1.CompareTo(num2);
        }

        // Fallback to string comparison if parsing fails
        return string.Compare(normalizedChunk1, normalizedChunk2, StringComparison.Ordinal);
    }

    private static ReadOnlySpan<char> ExtractChunk(ReadOnlySpan<char> source, ref int marker)
    {
        var start = marker;
        var firstChar = source[marker];
        var isNumeric = char.IsAsciiDigit(firstChar) || IsFullWidthDigit(firstChar);

        marker++;

        // Continue while characters are of the same type (numeric or alphabetic)
        while (marker < source.Length)
        {
            var currentChar = source[marker];
            var currentIsNumeric = char.IsAsciiDigit(currentChar) || IsFullWidthDigit(currentChar);

            if (isNumeric != currentIsNumeric)
            {
                break;
            }

            marker++;
        }

        return source[start..marker];
    }

    /// <summary>
    ///     Checks if a character is a full-width digit (U+FF10 to U+FF19).
    /// </summary>
    /// <param name="c">Character to check</param>
    /// <returns>True if the character is a full-width digit</returns>
    private static bool IsFullWidthDigit(char c) => c is >= '\uFF10' and <= '\uFF19';

    private static string NormalizeNumericChunk(ReadOnlySpan<char> chunk)
    {
        // Check if any full-width digits exist
        var hasFullWidth = false;
        foreach (var c in chunk)
        {
            if (!IsFullWidthDigit(c))
            {
                continue;
            }

            hasFullWidth = true;
            break;
        }

        if (!hasFullWidth)
        {
            return chunk.ToString();
        }

        // Convert full-width digits to regular digits using modern string creation
        return string.Create(chunk.Length, chunk, static (buffer, source) =>
        {
            for (var i = 0; i < source.Length; i++)
            {
                var c = source[i];
                buffer[i] = IsFullWidthDigit(c) ? (char)('0' + (c - '\uFF10')) : c;
            }
        });
    }

    #endregion
}
