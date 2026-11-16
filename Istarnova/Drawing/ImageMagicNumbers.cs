#region "Header"

// File: ImageMagicNumbers.cs
// Created 2026/08/09
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

namespace Istarnova.Drawing;

public static class ImageMagicNumbers
{
    #region "Properties"

    public static ReadOnlySpan<byte> Bmp => "BM"u8;
    public static ReadOnlySpan<byte> Gif => "GIF8"u8;
    public static ReadOnlySpan<byte> Jpeg => [0xff, 0xd8, 0xff];
    public static ReadOnlySpan<byte> Png => [0x89, 0x50, 0x4e, 0x47];
    public static ReadOnlySpan<byte> Svg => "<svg"u8; // <svg

    /// <summary>A TIFF written most significant byte first: "MM" for Motorola, then 42.</summary>
    public static ReadOnlySpan<byte> TiffBigEndian => "MM\0*"u8;

    /// <summary>A TIFF written least significant byte first: "II" for Intel, then 42.</summary>
    public static ReadOnlySpan<byte> TiffLittleEndian => "II*\0"u8;

    public static ReadOnlySpan<byte> WebPMarker => "WEBP"u8;
    public static ReadOnlySpan<byte> WebPRiff => "RIFF"u8;

    #endregion

    #region "Methods"

    extension(Span<byte> data)
    {
        #region "Methods"

        public string? GetMimeTypeByMagicNumber()
        {
            if (data.StartsWith(Png))
            {
                return "image/png";
            }

            if (data.StartsWith(Jpeg))
            {
                return "image/jpeg";
            }

            if (data.StartsWith(Gif))
            {
                return "image/gif";
            }

            if (data.StartsWith(WebPRiff) && data.Length >= 12 && data[8..12].SequenceEqual(WebPMarker))
            {
                return "image/webp";
            }

            if (data.StartsWith(Bmp))
            {
                return "image/bmp";
            }

            if (data.StartsWith(TiffLittleEndian) || data.StartsWith(TiffBigEndian))
            {
                return "image/tiff";
            }

            return data.StartsWith(Svg) ? "image/svg+xml" : null;
        }

        #endregion
    }

    #endregion
}
