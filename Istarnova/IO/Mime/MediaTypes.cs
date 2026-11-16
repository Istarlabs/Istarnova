#region "Header"

// File: MediaTypes.cs
// Created 2026/10/27
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using Istarnova.Extensions;

namespace Istarnova.IO.Mime;

/// <summary>
///     The media types ("MIME types") applications keep writing as string literals, and the file extension each is
///     known by.
/// </summary>
public static class MediaTypes
{
    #region "Constants"

    public const string OctetStream = "application/octet-stream";
    public const string Pdf = "application/pdf";
    public const string Zip = "application/zip";
    public const string Json = "application/json";
    public const string Xml = "application/xml";
    public const string Rtf = "application/rtf";
    public const string Dicom = "application/dicom";
    public const string Word = "application/msword";
    public const string WordOpenXml = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
    public const string Excel = "application/vnd.ms-excel";
    public const string ExcelOpenXml = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    public const string Email = "application/rfc822";
    public const string Outlook = "application/vnd.ms-outlook";

    public const string PlainText = "text/plain";
    public const string Csv = "text/csv";
    public const string Html = "text/html";
    public const string TextXml = "text/xml";

    public const string Jpeg = "image/jpeg";
    public const string Png = "image/png";
    public const string Gif = "image/gif";
    public const string WebP = "image/webp";
    public const string Bmp = "image/bmp";
    public const string Tiff = "image/tiff";
    public const string Heic = "image/heic";
    public const string Svg = "image/svg+xml";

    public const string Mp4 = "video/mp4";

    public const string Azw3 = "application/vnd.amazon.ebook";
    public const string Cbr = "application/vnd.comicbook-rar";
    public const string Cbz = "application/vnd.comicbook+zip";
    public const string Epub = "application/epub+zip";
    public const string Fb2 = "application/x-fictionbook+xml";
    public const string Mobi = "application/x-mobipocket-ebook";

    // The first extension of an entry is the one a media type is written out as.
    private static readonly (string MediaType, string[] Extensions)[] Known =
    [
        (Pdf, [".pdf"]),
        (Zip, [".zip"]),
        (Json, [".json"]),
        (Xml, [".xml"]),
        (TextXml, [".xml"]),
        (Rtf, [".rtf"]),
        (Dicom, [".dcm", ".dicom"]),
        (Word, [".doc"]),
        (WordOpenXml, [".docx"]),
        (Excel, [".xls"]),
        (ExcelOpenXml, [".xlsx"]),
        (PlainText, [".txt"]),
        (Csv, [".csv"]),
        (Html, [".html", ".htm"]),
        (Jpeg, [".jpg", ".jpeg"]),
        (Png, [".png"]),
        (Gif, [".gif"]),
        (WebP, [".webp"]),
        (Bmp, [".bmp"]),
        (Tiff, [".tif", ".tiff"]),
        (Heic, [".heic"]),
        (Svg, [".svg"]),
        (Mp4, [".mp4"]),
        (Pdf, [".pdf"]),
        (Epub, [".epub"]),
        (Mobi, [".mobi"]),
        (Azw3, [".azw3"]),
        (Fb2, [".fb2"]),
        (Cbr, [".cbr"]),
        (Cbz, [".cbz"]),
    ];

    private static readonly string[] Ebooks = [Epub, Mobi, Azw3, Fb2, Cbr, Cbz];

    private static readonly string[] Documents = [Pdf, Rtf, Word, WordOpenXml, Excel, ExcelOpenXml, PlainText, Csv];

    #endregion

    #region "Methods"

    /// <summary>
    ///     Whether the media type is an eBook or comic archive format known here (EPUB, MOBI, AZW3, FB2, CBR, CBZ).
    /// </summary>
    /// <param name="mediaType">The media type, in any case; parameters are ignored.</param>
    public static bool IsKnownEbook(string? mediaType)
        => IsOneOf(mediaType, Ebooks);

    /// <summary>
    ///     Whether the media type is a known image format.
    /// </summary>
    /// <param name="mediaType">The media type, in any case; parameters are ignored.</param>
    public static bool IsKnownImage(string? mediaType)
        => IsKnownWithPrefix(mediaType, "image/");

    /// <summary>
    ///     Whether the media type is a known video format.
    /// </summary>
    /// <param name="mediaType">The media type, in any case; parameters are ignored.</param>
    public static bool IsKnownVideo(string? mediaType)
        => IsKnownWithPrefix(mediaType, "video/");

    /// <summary>
    ///     Whether the media type is a known document format (PDF, RTF, Word, Excel, plain text, CSV).
    /// </summary>
    /// <param name="mediaType">The media type, in any case; parameters are ignored.</param>
    public static bool IsKnownDocument(string? mediaType)
        => IsOneOf(mediaType, Documents);

    /// <summary>
    ///     Whether the file extension stands for a known eBook or comic archive format.
    /// </summary>
    /// <param name="extension">The extension with or without its dot, or a file name or path; in any case.</param>
    public static bool IsKnownEbookExtension(string? extension)
        => IsKnownEbook(FromExtension(extension));

    /// <summary>
    ///     Whether the file extension stands for a known image format.
    /// </summary>
    /// <param name="extension">The extension with or without its dot, or a file name or path; in any case.</param>
    public static bool IsKnownImageExtension(string? extension)
        => IsKnownImage(FromExtension(extension));

    /// <summary>
    ///     Whether the file extension stands for a known video format.
    /// </summary>
    /// <param name="extension">The extension with or without its dot, or a file name or path; in any case.</param>
    public static bool IsKnownVideoExtension(string? extension)
        => IsKnownVideo(FromExtension(extension));

    /// <summary>
    ///     Whether the file extension stands for a known document format.
    /// </summary>
    /// <param name="extension">The extension with or without its dot, or a file name or path; in any case.</param>
    public static bool IsKnownDocumentExtension(string? extension)
        => IsKnownDocument(FromExtension(extension));

    /// <summary>
    ///     The extension a media type is written out as.
    /// </summary>
    /// <param name="mediaType">The media type, in any case; parameters such as "; charset=utf-8" are ignored.</param>
    /// <returns>The extension with its dot, or null where the media type is not known here.</returns>
    public static string? GetExtension(string? mediaType)
    {
        var essence = Essence(mediaType);

        return essence is null
            ? null
            : Array.Find(Known, entry => entry.MediaType.Equals(essence, StringComparison.OrdinalIgnoreCase))
                .Extensions?[0];
    }

    /// <summary>
    ///     The media type a file extension stands for.
    /// </summary>
    /// <param name="extension">The extension with or without its dot, or a file name or path; in any case.</param>
    /// <returns>The media type, or null where the extension is not known here.</returns>
    public static string? FromExtension(string? extension)
    {
        if (extension.IsEmpty())
        {
            return null;
        }

        var dotted = extension.Contains('.') ? Path.GetExtension(extension) : $".{extension}";

        return Array.Find(Known, entry => entry.Extensions.Contains(dotted, StringComparer.OrdinalIgnoreCase))
            .MediaType;
    }

    private static bool IsOneOf(string? mediaType, string[] candidates)
    {
        var essence = Essence(mediaType);

        return essence is { } && candidates.Contains(essence, StringComparer.OrdinalIgnoreCase);
    }

    private static bool IsKnownWithPrefix(string? mediaType, string prefix)
    {
        var essence = Essence(mediaType);

        return essence is { } && essence.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
                              && Array.Exists(Known, entry => entry.MediaType.Equals(essence, StringComparison.OrdinalIgnoreCase));
    }

    private static string? Essence(string? mediaType)
    {
        if (string.IsNullOrWhiteSpace(mediaType))
        {
            return null;
        }

        var end = mediaType.IndexOf(';');

        return (end < 0 ? mediaType : mediaType[..end]).Trim();
    }

    #endregion
}
