// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

using Istarnova.Extensions;

namespace Istarnova.Tests.Extensions;

public class StringInvariantIgnoreCaseExtensionsTests
{
    [Theory]
    [InlineData("Hello World", "hello", true)]
    [InlineData("Hello World", "World", false)]
    [InlineData("Hello World", "", false)]
    [InlineData("", "Hello", false)]
    public void StartsWithInvariantIgnoreCase_ShouldIgnoreCase_WhenBothHaveContent(string text, string value, bool expected)
        => text.StartsWithInvariantIgnoreCase(value).Should().Be(expected);

    [Theory]
    [InlineData("Hello World", "WORLD", true)]
    [InlineData("Hello World", "Hello", false)]
    [InlineData("Hello World", "", false)]
    [InlineData("", "World", false)]
    public void EndsWithInvariantIgnoreCase_ShouldIgnoreCase_WhenBothHaveContent(string text, string value, bool expected)
        => text.EndsWithInvariantIgnoreCase(value).Should().Be(expected);
}
