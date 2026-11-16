#region "Header"

// File: MimeFileType.cs
// Created 2026/03/06
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

#region "Usings"

using NetEscapades.EnumGenerators;

#endregion

namespace Istarnova.IO.Mime;

[EnumExtensions]
public enum MimeFileType
{
    None,
    Image,
    PdfDocument,
    WordDocument,
    ExcelDocument,
    Outlook,
    Email,
    Html,
    Text,
    Json,
    Application,
    Stream,
}
