#region "Header"

// File: ByteExtensions.cs
// Created 2026/08/17
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

#region "Usings"

using Istarnova.Common;
using Istarnova.Drawing;

#endregion

namespace Istarnova.Extensions;

public static class ByteExtensions
{
    #region "Constants"

    /// <summary>
    ///     What a data URL claims when the bytes identify themselves as nothing in particular. The
    ///     media type may be left out entirely, in which case a reader assumes text/plain - which
    ///     these bytes are not.
    /// </summary>
    private const string UnknownMediaType = "application/octet-stream";

    #endregion

    #region "Methods"

    extension(byte[] bytes)
    {
        #region "Methods"

        public string ToBase45() => Base45.Encode(bytes);

        /// <summary>
        ///     The bytes as a base64 data URL, with the media type read out of the bytes themselves.
        /// </summary>
        /// <remarks>
        ///     For handing an image straight to an <c>img</c> tag without a route of its own to serve
        ///     it from. The media type comes from
        ///     <see cref="ImageMagicNumbers.GetMimeTypeByMagicNumber" />, so a caller that stored a
        ///     picture and no longer remembers what kind still produces a URL a browser understands -
        ///     and one that hands over something unrecognised gets
        ///     <see cref="UnknownMediaType" /> rather than a lie.
        ///     <para>
        ///         <see cref="StringExtensions" /> has the way back: <c>dataUrl.ToBytes()</c>.
        ///     </para>
        /// </remarks>
        /// <returns>An empty string for null or empty input, since there is nothing to point at.</returns>
        public string ToDataUrl()
            => bytes is not { Length: > 0 }
                ? string.Empty
                : bytes.ToDataUrl(bytes.AsSpan().GetMimeTypeByMagicNumber() ?? UnknownMediaType);

        /// <summary>
        ///     The bytes as a base64 data URL of the given media type.
        /// </summary>
        /// <param name="mediaType">
        ///     What the bytes are, e.g. <c>image/jpeg</c>. Used as given - a caller that knows the type
        ///     is believed over the magic number, which is the point of this overload.
        /// </param>
        public string ToDataUrl(string mediaType)
            => bytes is not { Length: > 0 }
                ? string.Empty
                : $"data:{mediaType};base64,{Convert.ToBase64String(bytes)}";

        #endregion
    }

    #endregion
}
