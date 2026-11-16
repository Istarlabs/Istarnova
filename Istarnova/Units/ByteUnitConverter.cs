#region "Header"

// File: ByteUnitConverter.cs
// Created 2026/08/22
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

#region "Usings"

using System.Collections.Frozen;
using System.Globalization;

#endregion

namespace Istarnova.Units;

public enum ByteUnitSizeType
{
    Byte,
    Kilobyte,
    Megabyte,
    Gigabyte,
    Terabyte,
    Petabyte,
    Exabyte,

    // Binary units (1024-based)
    Kibibyte,
    Mebibyte,
    Gibibyte,
    Tebibyte,
    Pebibyte,
    Exbibyte,
}

public readonly struct ByteUnitConverter : IFormattable, IComparable<ByteUnitConverter>, IEquatable<ByteUnitConverter>,
    ISpanParsable<ByteUnitConverter>
{
    public long TotalBytes { get; }

    public ByteUnitConverter(long bytes)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(bytes);
        TotalBytes = bytes;
    }

    private static readonly FrozenDictionary<ByteUnitSizeType, long> UnitDictionary =
        new Dictionary<ByteUnitSizeType, long>
        {
            [ByteUnitSizeType.Byte] = 1L,
            // Decimal units (1000-based)
            [ByteUnitSizeType.Kilobyte] = 1_000L,
            [ByteUnitSizeType.Megabyte] = 1_000_000L,
            [ByteUnitSizeType.Gigabyte] = 1_000_000_000L,
            [ByteUnitSizeType.Terabyte] = 1_000_000_000_000L,
            [ByteUnitSizeType.Petabyte] = 1_000_000_000_000_000L,
            [ByteUnitSizeType.Exabyte] = 1_000_000_000_000_000_000L,
            // Binary units (1024-based)
            [ByteUnitSizeType.Kibibyte] = 1024L,
            [ByteUnitSizeType.Mebibyte] = 1_048_576L,
            [ByteUnitSizeType.Gibibyte] = 1_073_741_824L,
            [ByteUnitSizeType.Tebibyte] = 1_099_511_627_776L,
            [ByteUnitSizeType.Pebibyte] = 1_125_899_906_842_624L,
            [ByteUnitSizeType.Exbibyte] = 1_152_921_504_606_846_976L,
        }.ToFrozenDictionary();

    private static readonly FrozenDictionary<ByteUnitSizeType, string> Units = new Dictionary<ByteUnitSizeType, string>
    {
        [ByteUnitSizeType.Byte] = "B",
        [ByteUnitSizeType.Kilobyte] = "KB",
        [ByteUnitSizeType.Megabyte] = "MB",
        [ByteUnitSizeType.Gigabyte] = "GB",
        [ByteUnitSizeType.Terabyte] = "TB",
        [ByteUnitSizeType.Petabyte] = "PB",
        [ByteUnitSizeType.Exabyte] = "EB",
        [ByteUnitSizeType.Kibibyte] = "KiB",
        [ByteUnitSizeType.Mebibyte] = "MiB",
        [ByteUnitSizeType.Gibibyte] = "GiB",
        [ByteUnitSizeType.Tebibyte] = "TiB",
        [ByteUnitSizeType.Pebibyte] = "PiB",
        [ByteUnitSizeType.Exbibyte] = "EiB",
    }.ToFrozenDictionary();

    /// <summary>
    ///     Maps unit symbols to their <see cref="ByteUnitSizeType" />. Lookups are case-insensitive,
    ///     so "MB", "mb" and "Mb" all resolve to <see cref="ByteUnitSizeType.Megabyte" />.
    /// </summary>
    private static readonly FrozenDictionary<string, ByteUnitSizeType> UnitSymbols =
        new Dictionary<string, ByteUnitSizeType>(StringComparer.OrdinalIgnoreCase)
        {
            ["B"] = ByteUnitSizeType.Byte,
            ["BYTE"] = ByteUnitSizeType.Byte,
            ["BYTES"] = ByteUnitSizeType.Byte,
            // Decimal units (1000-based)
            ["K"] = ByteUnitSizeType.Kilobyte,
            ["KB"] = ByteUnitSizeType.Kilobyte,
            ["M"] = ByteUnitSizeType.Megabyte,
            ["MB"] = ByteUnitSizeType.Megabyte,
            ["G"] = ByteUnitSizeType.Gigabyte,
            ["GB"] = ByteUnitSizeType.Gigabyte,
            ["T"] = ByteUnitSizeType.Terabyte,
            ["TB"] = ByteUnitSizeType.Terabyte,
            ["P"] = ByteUnitSizeType.Petabyte,
            ["PB"] = ByteUnitSizeType.Petabyte,
            ["E"] = ByteUnitSizeType.Exabyte,
            ["EB"] = ByteUnitSizeType.Exabyte,
            // Binary units (1024-based)
            ["KIB"] = ByteUnitSizeType.Kibibyte,
            ["MIB"] = ByteUnitSizeType.Mebibyte,
            ["GIB"] = ByteUnitSizeType.Gibibyte,
            ["TIB"] = ByteUnitSizeType.Tebibyte,
            ["PIB"] = ByteUnitSizeType.Pebibyte,
            ["EIB"] = ByteUnitSizeType.Exbibyte,
        }.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);

    private static readonly FrozenDictionary<string, ByteUnitSizeType>.AlternateLookup<ReadOnlySpan<char>>
        UnitSymbolLookup =
            UnitSymbols.GetAlternateLookup<ReadOnlySpan<char>>();

    private static ByteUnitConverter FromUnit(long value, ByteUnitSizeType type) => new(value * UnitDictionary[type]);

    // Factory methods for decimal units
    public static ByteUnitConverter FromBytes(long value) => new(value);
    public static ByteUnitConverter FromKilobytes(long value) => FromUnit(value, ByteUnitSizeType.Kilobyte);
    public static ByteUnitConverter FromMegabytes(long value) => FromUnit(value, ByteUnitSizeType.Megabyte);
    public static ByteUnitConverter FromGigabytes(long value) => FromUnit(value, ByteUnitSizeType.Gigabyte);
    public static ByteUnitConverter FromTerabytes(long value) => FromUnit(value, ByteUnitSizeType.Terabyte);
    public static ByteUnitConverter FromPetabytes(long value) => FromUnit(value, ByteUnitSizeType.Petabyte);
    public static ByteUnitConverter FromExabytes(long value) => FromUnit(value, ByteUnitSizeType.Exabyte);

    // Factory methods for binary units
    public static ByteUnitConverter FromKibibytes(long value) => FromUnit(value, ByteUnitSizeType.Kibibyte);
    public static ByteUnitConverter FromMebibytes(long value) => FromUnit(value, ByteUnitSizeType.Mebibyte);
    public static ByteUnitConverter FromGibibytes(long value) => FromUnit(value, ByteUnitSizeType.Gibibyte);
    public static ByteUnitConverter FromTebibytes(long value) => FromUnit(value, ByteUnitSizeType.Tebibyte);
    public static ByteUnitConverter FromPebibytes(long value) => FromUnit(value, ByteUnitSizeType.Pebibyte);
    public static ByteUnitConverter FromExbibytes(long value) => FromUnit(value, ByteUnitSizeType.Exbibyte);

    /// <summary>
    ///     Automatically selects the most appropriate unit for display
    /// </summary>
    public ByteUnitSizeType GetBestUnit(bool useBinaryUnits = false)
    {
        var totalBytes = TotalBytes; // Copy to local variable for closure

        ByteUnitSizeType[] orderedUnits = useBinaryUnits
            ?
            [
                ByteUnitSizeType.Exbibyte, ByteUnitSizeType.Pebibyte, ByteUnitSizeType.Tebibyte,
                ByteUnitSizeType.Gibibyte, ByteUnitSizeType.Mebibyte, ByteUnitSizeType.Kibibyte, ByteUnitSizeType.Byte,
            ]
            :
            [
                ByteUnitSizeType.Exabyte, ByteUnitSizeType.Petabyte, ByteUnitSizeType.Terabyte,
                ByteUnitSizeType.Gigabyte, ByteUnitSizeType.Megabyte, ByteUnitSizeType.Kilobyte, ByteUnitSizeType.Byte,
            ];

        return orderedUnits.FirstOrDefault(unit => totalBytes >= UnitDictionary[unit], ByteUnitSizeType.Byte);
    }

    private static string Format(long value, ByteUnitSizeType unit, string? format, IFormatProvider? formatProvider)
    {
        var numericValue = value / (double)UnitDictionary[unit];
        var formattedValue = numericValue.ToString(format ?? "F2", formatProvider ?? CultureInfo.CurrentCulture);
        return $"{formattedValue} {Units[unit]}";
    }

    public override string ToString() => ToString("F2", CultureInfo.CurrentCulture);

    public string ToString(ByteUnitSizeType unit) => Format(TotalBytes, unit, "F2", CultureInfo.CurrentCulture);

    public string ToString(ByteUnitSizeType unit, string? format) =>
        Format(TotalBytes, unit, format, CultureInfo.CurrentCulture);

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        var bestUnit = GetBestUnit();
        return Format(TotalBytes, bestUnit, format, formatProvider);
    }

    /// <summary>
    ///     Parses a size with an optional unit suffix, for example "10MB", "1.5 GiB", "512kb" or "1024".
    ///     A missing suffix is interpreted as bytes.
    /// </summary>
    /// <param name="input">The text to parse.</param>
    /// <param name="formatProvider">Format provider for the numeric part. Defaults to the current culture.</param>
    /// <returns>The parsed value.</returns>
    /// <exception cref="FormatException">The input is not a valid size.</exception>
    public static ByteUnitConverter Parse(string input, IFormatProvider? formatProvider = null)
    {
        ArgumentNullException.ThrowIfNull(input);

        return Parse(input.AsSpan(), formatProvider);
    }

    /// <inheritdoc cref="Parse(string, IFormatProvider?)" />
    public static ByteUnitConverter Parse(ReadOnlySpan<char> input, IFormatProvider? formatProvider = null)
    {
        if (!TryParse(input, formatProvider, out var result))
        {
            throw new FormatException($"'{input}' is not a valid byte size.");
        }

        return result;
    }

    /// <summary>
    ///     Tries to parse a size with an optional unit suffix, for example "10MB", "1.5 GiB", "512kb" or "1024".
    ///     A missing suffix is interpreted as bytes.
    /// </summary>
    /// <param name="input">The text to parse.</param>
    /// <param name="result">The parsed value, or the default value if parsing failed.</param>
    /// <returns><c>true</c> if the input was parsed successfully; otherwise <c>false</c>.</returns>
    public static bool TryParse(string? input, out ByteUnitConverter result) =>
        TryParse(input.AsSpan(), null, out result);

    /// <inheritdoc cref="TryParse(string?, out ByteUnitConverter)" />
    /// <param name="formatProvider">Format provider for the numeric part. Defaults to the current culture.</param>
    public static bool TryParse(string? input, IFormatProvider? formatProvider, out ByteUnitConverter result) =>
        TryParse(input.AsSpan(), formatProvider, out result);

    /// <summary>
    ///     Tries to parse a size written the same way everywhere, independent of the machine's culture.
    /// </summary>
    /// <param name="input">The text to parse.</param>
    /// <param name="result">The parsed value, or the default value if parsing failed.</param>
    /// <returns><c>true</c> if the input was parsed successfully; otherwise <c>false</c>.</returns>
    /// <remarks>
    ///     For sizes that come from a configuration file, an environment variable, a command line or any
    ///     other text a machine wrote for another machine. Those mean one thing wherever they are read,
    ///     while <see cref="TryParse(string?, out ByteUnitConverter)" /> follows the current culture and
    ///     would take the separator in "1.5GB" for a thousands separator under, say, a German locale -
    ///     turning one and a half gigabytes into fifteen, quietly and only on some machines.
    /// </remarks>
    public static bool TryParseInvariant(string? input, out ByteUnitConverter result) =>
        TryParse(input.AsSpan(), CultureInfo.InvariantCulture, out result);

    /// <inheritdoc cref="TryParseInvariant(string?, out ByteUnitConverter)" />
    public static bool TryParseInvariant(ReadOnlySpan<char> input, out ByteUnitConverter result) =>
        TryParse(input, CultureInfo.InvariantCulture, out result);

    /// <inheritdoc cref="TryParse(string?, IFormatProvider?, out ByteUnitConverter)" />
    public static bool TryParse(ReadOnlySpan<char> input, IFormatProvider? formatProvider,
        out ByteUnitConverter result)
    {
        result = default;

        var trimmed = input.Trim();

        if (trimmed.IsEmpty)
        {
            return false;
        }

        // The unit is the trailing run of letters; everything before it is the numeric part.
        var unitStart = trimmed.Length;

        while (unitStart > 0 && char.IsLetter(trimmed[unitStart - 1]))
        {
            unitStart--;
        }

        var unitSpan = trimmed[unitStart..];
        var numberSpan = trimmed[..unitStart].TrimEnd();

        if (numberSpan.IsEmpty)
        {
            return false;
        }

        var unit = ByteUnitSizeType.Byte;

        if (!unitSpan.IsEmpty && !UnitSymbolLookup.TryGetValue(unitSpan, out unit))
        {
            return false;
        }

        const NumberStyles styles = NumberStyles.Float | NumberStyles.AllowThousands;

        if (!double.TryParse(numberSpan, styles, formatProvider ?? CultureInfo.CurrentCulture, out var value))
        {
            return false;
        }

        var totalBytes = value * UnitDictionary[unit];

        if (totalBytes < 0 || !double.IsFinite(totalBytes) || totalBytes > long.MaxValue)
        {
            return false;
        }

        result = new((long)totalBytes);

        return true;
    }

    static ByteUnitConverter IParsable<ByteUnitConverter>.Parse(string input, IFormatProvider? provider) =>
        Parse(input, provider);

    static bool IParsable<ByteUnitConverter>.TryParse(string? input, IFormatProvider? provider,
        out ByteUnitConverter result) =>
        TryParse(input, provider, out result);

    static ByteUnitConverter ISpanParsable<ByteUnitConverter>.Parse(ReadOnlySpan<char> input,
        IFormatProvider? provider) =>
        Parse(input, provider);

    static bool ISpanParsable<ByteUnitConverter>.TryParse(ReadOnlySpan<char> input, IFormatProvider? provider,
        out ByteUnitConverter result) =>
        TryParse(input, provider, out result);

    public int CompareTo(ByteUnitConverter other) => TotalBytes.CompareTo(other.TotalBytes);

    public bool Equals(ByteUnitConverter other) => TotalBytes.Equals(other.TotalBytes);

    public override bool Equals(object? obj) => obj is ByteUnitConverter other && Equals(other);

    public override int GetHashCode() => TotalBytes.GetHashCode();

    public static bool operator ==(ByteUnitConverter left, ByteUnitConverter right) => left.Equals(right);

    public static bool operator !=(ByteUnitConverter left, ByteUnitConverter right) => !left.Equals(right);

    public static bool operator <(ByteUnitConverter left, ByteUnitConverter right) => left.CompareTo(right) < 0;

    public static bool operator <=(ByteUnitConverter left, ByteUnitConverter right) => left.CompareTo(right) <= 0;

    public static bool operator >(ByteUnitConverter left, ByteUnitConverter right) => left.CompareTo(right) > 0;

    public static bool operator >=(ByteUnitConverter left, ByteUnitConverter right) => left.CompareTo(right) >= 0;

    // Arithmetic operators
    public static ByteUnitConverter operator +(ByteUnitConverter left, ByteUnitConverter right) =>
        new(left.TotalBytes + right.TotalBytes);

    public static ByteUnitConverter operator -(ByteUnitConverter left, ByteUnitConverter right) =>
        new(Math.Max(0, left.TotalBytes - right.TotalBytes));

    public static ByteUnitConverter operator *(ByteUnitConverter left, long multiplier) =>
        new(left.TotalBytes * multiplier);

    public static ByteUnitConverter operator /(ByteUnitConverter left, long divisor) => new(left.TotalBytes / divisor);
}
