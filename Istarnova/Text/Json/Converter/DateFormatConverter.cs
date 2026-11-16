#region "Header"

// File: DateFormatConverter.cs
// Created 2026/03/06
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

#region "Usings"

using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

#endregion

namespace Istarnova.Text.Json.Converter;

public sealed class DateFormatConverter : JsonConverter<DateTime>
{
    #region "Constants"

    private const string Format = "dd.MM.yyyy HH:mm:ss";

    #endregion

    #region "Methods"

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        DateTime.ParseExact(reader.GetString()!, Format, CultureInfo.InvariantCulture);

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(Format));
    }

    #endregion
}
