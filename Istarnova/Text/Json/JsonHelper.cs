#region "Header"

// File: JsonHelper.cs
// Created 2026/08/17
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

#region "Usings"

using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization.Metadata;
using JsonSerializer = System.Text.Json.JsonSerializer;

#endregion

namespace Istarnova.Text.Json;

/// <summary>
///     Provides utility methods for serializing to and deserializing from JSON.
///     This class supports both synchronous and asynchronous operations and allows
///     for customization of serialization settings via <see cref="JsonTypeInfo{T}" />.
/// </summary>
public static class JsonHelper
{
    #region "Methods"

    extension(JsonNode? node)
    {
        #region "Methods"

        public T? GetPropertyValue<T>(string propertyPath)
        {
            if (node is null || string.IsNullOrEmpty(propertyPath))
            {
                return default;
            }

            var targetNode = NavigateToProperty(node, propertyPath);

            if (targetNode is null)
            {
                return default;
            }

            try
            {
                return targetNode.Deserialize<T>();
            }
            catch // silent catch for deserialization errors
            {
                return default;
            }
        }

        public T? GetPropertyValueOrDefault<T>(string propertyPath, T? defaultValue = default)
        {
            if (node is null || string.IsNullOrEmpty(propertyPath))
            {
                return defaultValue;
            }

            var targetNode = NavigateToProperty(node, propertyPath);

            if (targetNode is null)
            {
                return defaultValue;
            }

            try
            {
                return targetNode.Deserialize<T>() ?? defaultValue;
            }
            catch // silent catch for deserialization errors
            {
                return defaultValue;
            }
        }

        public void SetPropertyValue<T>(string propertyPath, T? value)
        {
            if (node is null || string.IsNullOrEmpty(propertyPath))
            {
                return;
            }

            var pathSegments = propertyPath.Split('.');
            var currentNode = node;

            foreach (var segment in pathSegments)
            {
                currentNode![segment] ??= new JsonObject();

                currentNode = currentNode?[segment];
            }

            var lastSegment = pathSegments[^1];
            currentNode![lastSegment] = JsonValue.Create(value);
        }

        public bool TryGetPropertyValue<T>(string propertyPath, out T? value)
        {
            value = default;

            if (node is null || string.IsNullOrEmpty(propertyPath))
            {
                return false;
            }

            var targetNode = NavigateToProperty(node, propertyPath);
            if (targetNode is null)
            {
                return false;
            }

            try
            {
                value = targetNode.Deserialize<T>();
                return true;
            }
            catch // silent catch for deserialization errors
            {
                return false;
            }
        }

        #endregion
    }

    /// <summary>
    ///     Deserializes the JSON string to an object of type T.
    /// </summary>
    /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <param name="typeInfo">Optional type information for custom serialization settings.</param>
    /// <returns>The deserialized object of type T.</returns>
    public static T? Deserialize<T>(string json, JsonTypeInfo<T>? typeInfo = null) =>
        typeInfo is { }
            ? JsonSerializer.Deserialize(json, typeInfo)
            : JsonSerializer.Deserialize<T>(json, JsonSerializerOptions.Default);

    public static JsonElement DeserializeAsJsonElement([StringSyntax(StringSyntaxAttribute.Json)] string? json)
    {
        if (string.IsNullOrEmpty(json))
        {
            return default;
        }

        using var doc = JsonDocument.Parse(json);

        return doc.RootElement;
    }

    public static JsonNode? DeserializeAsJsonNode([StringSyntax(StringSyntaxAttribute.Json)] string? json) =>
        string.IsNullOrEmpty(json) ? null : JsonNode.Parse(json);

    /// <summary>
    ///     Reads JSON as an object to write into.
    /// </summary>
    /// <param name="json">The JSON to read. Null, blank and unreadable are all allowed.</param>
    /// <returns>
    ///     The object the JSON holds, or an empty object - never null - when it holds nothing that can
    ///     take a property: null, blank, malformed, or an array or bare value. Use
    ///     <see cref="TryDeserialize{T}" /> where "empty" and "broken" have to be told apart.
    /// </returns>
    public static JsonObject DeserializeAsJsonObject([StringSyntax(StringSyntaxAttribute.Json)] string? json) =>
        json is { } && TryDeserialize<JsonObject>(json, out var jsonObject) ? jsonObject : [];

    /// <summary>
    ///     Asynchronously deserializes the JSON string to an object of type <typeparamref name="T" />.
    /// </summary>
    /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <param name="typeInfo">Optional type information for custom serialization settings.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains the deserialized object of type
    ///     <typeparamref name="T" />.
    /// </returns>
    public static Task<T?> DeserializeAsync<T>(string json, JsonTypeInfo<T>? typeInfo = null)
    {
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        return DeserializeAsync(stream, typeInfo);
    }

    /// <summary>
    ///     Asynchronously deserializes the JSON from a stream to an object of type <typeparamref name="T" />.
    /// </summary>
    /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
    /// <param name="stream">The stream containing the JSON to deserialize.</param>
    /// <param name="typeInfo">Optional type information for custom serialization settings.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains the deserialized object of type
    ///     <typeparamref name="T" />.
    /// </returns>
    public static async Task<T?> DeserializeAsync<T>(Stream stream, JsonTypeInfo<T>? typeInfo = null) =>
        typeInfo is { }
            ? await JsonSerializer.DeserializeAsync(stream, typeInfo)
            : await JsonSerializer.DeserializeAsync<T>(stream, JsonSerializerOptions.Default);

    /// <summary>
    ///     Serializes an object of type <typeparamref name="T" /> to a JSON string.
    /// </summary>
    /// <typeparam name="T">The type of the object to serialize.</typeparam>
    /// <param name="value">The object to serialize.</param>
    /// <param name="typeInfo">Optional type information for custom serialization settings.</param>
    /// <returns>The serialized JSON string.</returns>
    public static string Serialize<T>(T value, JsonTypeInfo<T>? typeInfo = null) =>
        typeInfo is { }
            ? JsonSerializer.Serialize(value, typeInfo)
            : JsonSerializer.Serialize(value, JsonSerializerOptions.Default);

    /// <summary>
    ///     Asynchronously serializes an object of type <typeparamref name="T" /> to a JSON string.
    /// </summary>
    /// <typeparam name="T">The type of the object to serialize.</typeparam>
    /// <param name="value">The object to serialize.</param>
    /// <param name="typeInfo">Optional type information for custom serialization settings.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static async Task<string> SerializeAsync<T>(T value, JsonTypeInfo<T>? typeInfo = null)
    {
        using var stream = new MemoryStream();

        if (typeInfo is { })
        {
            await JsonSerializer.SerializeAsync(stream, value, typeInfo);
        }
        else
        {
            await JsonSerializer.SerializeAsync(stream, value, JsonSerializerOptions.Default);
        }

        return Encoding.UTF8.GetString(stream.GetBuffer(), 0, (int)stream.Length);
    }

    /// <summary>
    ///     Asynchronously serializes an object of type <typeparamref name="T" /> and writes it to a stream.
    /// </summary>
    /// <typeparam name="T">The type of the object to serialize.</typeparam>
    /// <param name="stream">The stream to write the serialized JSON to.</param>
    /// <param name="value">The object to serialize.</param>
    /// <param name="typeInfo">Optional type information for custom serialization settings.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static async Task SerializeAsync<T>(Stream stream, T value, JsonTypeInfo<T>? typeInfo = null)
    {
        ArgumentNullException.ThrowIfNull(stream);

        if (typeInfo is { })
        {
            await JsonSerializer.SerializeAsync(stream, value, typeInfo);
        }
        else
        {
            await JsonSerializer.SerializeAsync(stream, value, JsonSerializerOptions.Default);
        }
    }

    /// <summary>
    ///     Attempts to deserialize JSON to an object, returning success status.
    /// </summary>
    /// <typeparam name="T">The type to deserialize to</typeparam>
    /// <param name="json">JSON string to deserialize</param>
    /// <param name="result">The deserialized object</param>
    /// <param name="typeInfo">Optional type information</param>
    /// <returns>True if deserialization succeeded</returns>
    public static bool TryDeserialize<T>(string json, [NotNullWhen(true)] out T? result,
        JsonTypeInfo<T>? typeInfo = null)
    {
        try
        {
            result = Deserialize(json, typeInfo);
            return result is { };
        }
        catch
        {
            result = default;
            return false;
        }
    }

    /// <summary>
    ///     Attempts to read a file and deserialize what it holds.
    /// </summary>
    /// <param name="path">The file. It is allowed not to exist.</param>
    /// <param name="result">The deserialized object</param>
    /// <param name="typeInfo">Optional type information</param>
    /// <returns>
    ///     False when the file is missing, unreadable or does not hold the expected shape - all of which
    ///     leave <paramref name="result" /> null. Use it where an absent file is an allowed state; where
    ///     it is an error the caller has to ask <see cref="File.Exists" /> itself, because this cannot
    ///     tell the two apart.
    /// </returns>
    public static bool TryDeserializeFile<T>(string? path, [NotNullWhen(true)] out T? result,
        JsonTypeInfo<T>? typeInfo = null)
    {
        result = default;

        if (string.IsNullOrEmpty(path) || !File.Exists(path))
        {
            return false;
        }

        try
        {
            return TryDeserialize(File.ReadAllText(path), out result, typeInfo);
        }
        catch (IOException)
        {
            return false;
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }
    }

    /// <summary>
    ///     Attempts to serialize an object to JSON, returning success status.
    /// </summary>
    /// <typeparam name="T">The type to serialize</typeparam>
    /// <param name="value">Object to serialize</param>
    /// <param name="result">The serialized JSON string</param>
    /// <param name="typeInfo">Optional type information</param>
    /// <returns>True if serialization succeeded</returns>
    public static bool TrySerialize<T>(T value, [NotNullWhen(true)] out string? result,
        JsonTypeInfo<T>? typeInfo = null)
    {
        try
        {
            result = Serialize(value, typeInfo);
            return true;
        }
        catch
        {
            result = null;
            return false;
        }
    }

    /// <summary>
    ///     Walks a dotted property path, where a segment in brackets indexes an array: "items.[2].name".
    /// </summary>
    /// <returns>
    ///     The node at the path, or null when the path does not resolve - a missing property, an index
    ///     that is not a number or out of range, or a segment addressing something that is neither an
    ///     object nor an array. Callers treat null as absent, not as an error.
    /// </returns>
    private static JsonNode? NavigateToProperty(JsonNode? node, string propertyPath)
    {
        var currentNode = node;

        foreach (var segment in propertyPath.Split('.'))
        {
            if (currentNode is null)
            {
                return null;
            }

            if (segment.StartsWith('[') && segment.EndsWith(']'))
            {
                currentNode = int.TryParse(segment.Trim('[', ']'), out var index)
                              && currentNode is JsonArray array
                              && index >= 0
                              && index < array.Count
                    ? array[index]
                    : null;
            }
            else
            {
                currentNode = currentNode is JsonObject owner ? owner[segment] : null;
            }
        }

        return currentNode;
    }

    #endregion
}
