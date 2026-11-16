// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

using Istarnova.Text.Json;

namespace Istarnova.Tests.Text.Json;

public class JsonHelperSerializeTests
{
    [Fact]
    public async Task SerializeAsync_ShouldReturnExactlyTheWrittenText_WhenTheBufferIsLargerThanTheContent()
    {
        var text = new string('ä', 3000);

        var json = await JsonHelper.SerializeAsync(new[] { text, "€", "end" });

        json.Should().StartWith("[\"").And.EndWith("\"end\"]");
        System.Text.Json.JsonSerializer.Deserialize<string[]>(json)![0].Should().Be(text);
    }
}
