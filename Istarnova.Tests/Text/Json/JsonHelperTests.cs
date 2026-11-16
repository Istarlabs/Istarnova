#region "Header"

// File: JsonHelperTests.cs
// Created 2026/08/14
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using System.Text.Json.Nodes;
using Istarnova.Text.Json;

namespace Istarnova.Tests.Text.Json;

public class JsonHelperTests
{
    #region "Fields"

    private const string Json = """
        {
            "scalar": 5,
            "nested": { "name": "value" },
            "items": [ { "name": "first" }, { "name": "second" } ]
        }
        """;

    #endregion

    #region "GetPropertyValue Tests"

    [Fact]
    public void GetPropertyValue_ShouldReturnValue_WhenPathResolves()
    {
        // Arrange
        var node = JsonNode.Parse(Json);

        // Act & Assert
        node.GetPropertyValue<string>("nested.name").Should().Be("value");
        node.GetPropertyValue<int>("scalar").Should().Be(5);
    }

    [Fact]
    public void GetPropertyValue_ShouldReturnValue_WhenPathIndexesAnArray()
    {
        // Arrange
        var node = JsonNode.Parse(Json);

        // Act
        var result = node.GetPropertyValue<string>("items.[1].name");

        // Assert
        result.Should().Be("second");
    }

    [Theory]
    [InlineData("scalar.name")] // a scalar in the middle of the path
    [InlineData("items.name")] // an array addressed by property name
    [InlineData("nested.missing")] // a property that is not there
    [InlineData("items.[9].name")] // an index out of range
    [InlineData("items.[x].name")] // an index that is not a number
    public void GetPropertyValue_ShouldReturnDefault_WhenPathDoesNotResolve(string path)
    {
        // Arrange
        var node = JsonNode.Parse(Json);

        // Act - every one of these is the same kind of caller mistake and has to answer the same way.
        // Indexing a scalar or an array by name used to throw InvalidOperationException instead.
        var result = node.GetPropertyValue<string>(path);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region "TryDeserializeFile Tests"

    [Fact]
    public void TryDeserializeFile_ShouldReturnTheContent_WhenTheFileHoldsIt()
    {
        // Arrange
        var path = Path.GetTempFileName();
        File.WriteAllText(path, """["first","second"]""");

        try
        {
            // Act
            var read = JsonHelper.TryDeserializeFile<List<string>>(path, out var result);

            // Assert
            read.Should().BeTrue();
            result.Should().BeEquivalentTo("first", "second");
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void TryDeserializeFile_ShouldReturnFalse_WhenTheFileIsNotThere()
    {
        // Act
        var read = JsonHelper.TryDeserializeFile<List<string>>(
            Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.json"), out var result);

        // Assert - an absent file is an allowed state, not an exception.
        read.Should().BeFalse();
        result.Should().BeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void TryDeserializeFile_ShouldReturnFalse_WhenThereIsNoPath(string? path)
    {
        // Act
        var read = JsonHelper.TryDeserializeFile<List<string>>(path, out var result);

        // Assert
        read.Should().BeFalse();
        result.Should().BeNull();
    }

    [Fact]
    public void TryDeserializeFile_ShouldReturnFalse_WhenTheFileIsNotReadableAsJson()
    {
        // Arrange
        var path = Path.GetTempFileName();
        File.WriteAllText(path, "{not json");

        try
        {
            // Act
            var read = JsonHelper.TryDeserializeFile<List<string>>(path, out var result);

            // Assert
            read.Should().BeFalse();
            result.Should().BeNull();
        }
        finally
        {
            File.Delete(path);
        }
    }

    #endregion

    #region "DeserializeAsJsonObject Tests"

    [Fact]
    public void DeserializeAsJsonObject_ShouldReturnTheObject_WhenJsonHoldsOne()
    {
        // Act
        var result = JsonHelper.DeserializeAsJsonObject(Json);

        // Assert
        result.GetPropertyValue<int>("scalar").Should().Be(5);
        result.GetPropertyValue<string>("nested.name").Should().Be("value");
    }

    [Theory]
    [InlineData(null)] // nothing has been written yet
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("{not json")] // malformed
    [InlineData("[1,2,3]")] // valid JSON, but nothing a property can be written onto
    [InlineData("42")]
    [InlineData("\"text\"")]
    public void DeserializeAsJsonObject_ShouldReturnAnEmptyObject_WhenJsonCannotTakeAProperty(string? json)
    {
        // Act
        var result = JsonHelper.DeserializeAsJsonObject(json);

        // Assert - never null, so the caller can write into it without a guard of its own.
        result.Should().NotBeNull();
        result.Count.Should().Be(0);
    }

    [Fact]
    public void DeserializeAsJsonObject_ShouldKeepWhatIsThere_WhenAPropertyIsWritten()
    {
        // Arrange
        var result = JsonHelper.DeserializeAsJsonObject("""{"kept":"yes"}""");

        // Act
        result["added"] = "also yes";

        // Assert
        result.GetPropertyValue<string>("kept").Should().Be("yes");
        result.GetPropertyValue<string>("added").Should().Be("also yes");
    }

    #endregion
}
