#region "Header"

// File: MimeFileTypeExtensions.cs
// Created 2026/03/06
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

namespace Istarnova.IO.Mime;

public static partial class MimeFileTypeExtensions
{
    #region "Methods"

    public static MimeFileType GetFileTypeByMimeType(this string? mimeType)
    {
        if (string.IsNullOrEmpty(mimeType))
        {
            return MimeFileType.None;
        }

        if (mimeType.Contains("image"))
        {
            return MimeFileType.Image;
        }

        if (mimeType.Contains("pdf"))
        {
            return MimeFileType.PdfDocument;
        }

        if (mimeType.Contains("msword"))
        {
            return MimeFileType.WordDocument;
        }

        if (mimeType.Contains("vnd.ms-excel"))
        {
            return MimeFileType.ExcelDocument;
        }

        if (mimeType.Contains("rfc822"))
        {
            return MimeFileType.Email;
        }

        if (mimeType.Contains("vnd.ms-outlook"))
        {
            return MimeFileType.Outlook;
        }

        if (mimeType.Contains("html"))
        {
            return MimeFileType.Html;
        }

        if (mimeType.Contains("text") || mimeType.Contains("plain"))
        {
            return MimeFileType.Text;
        }

        if (mimeType.Contains("json"))
        {
            return MimeFileType.Json;
        }

        return mimeType.Contains("application") ? MimeFileType.Application : MimeFileType.None;
    }

    public static string GetMimeTypeFromFileType(this MimeFileType fileType)
    {
        return fileType switch
        {
            MimeFileType.Image => MediaTypes.Jpeg,
            MimeFileType.PdfDocument => MediaTypes.Pdf,
            MimeFileType.WordDocument => MediaTypes.Word,
            MimeFileType.ExcelDocument => MediaTypes.Excel,
            MimeFileType.Email => MediaTypes.Email,
            MimeFileType.Outlook => MediaTypes.Outlook,
            MimeFileType.Html => MediaTypes.Html,
            MimeFileType.Text => MediaTypes.PlainText,
            MimeFileType.Json => MediaTypes.Json,
            MimeFileType.None or MimeFileType.Application or MimeFileType.Stream => MediaTypes.OctetStream,
            var _ => throw new ArgumentOutOfRangeException(nameof(fileType), fileType, null),
        };
    }

    #endregion
}
