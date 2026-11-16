#region "Header"

// File: JsonStringEnumMemberConverter.cs
// Created 2026/03/06
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

#region "Usings"

using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

#endregion

namespace Istarnova.Text.Json.Converter;

public sealed class JsonStringEnumMemberConverter<T> : JsonConverter<T> where T : struct, Enum
{
    #region "Methods"

    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var enumText = reader.GetString();
        ArgumentNullException.ThrowIfNull(enumText);

        foreach (var field in typeof(T).GetFields())
        {
            if (field.GetCustomAttribute<EnumMemberAttribute>()?.Value == enumText)
            {
                return (T)field.GetValue(null)!;
            }
        }

        return (T)Enum.Parse(typeof(T), enumText, true);
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        var enumMember = value.GetType()
            .GetField(value.ToString())
            ?.GetCustomAttribute<EnumMemberAttribute>();

        var enumValue = enumMember?.Value ?? value.ToString();
        writer.WriteStringValue(enumValue);
    }

    #endregion
}
