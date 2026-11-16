#region "Header"

// File: UserFacingErrorTests.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using System.Globalization;
using Istarnova.Blazor.Errors;
using Microsoft.EntityFrameworkCore;

namespace Istarnova.Tests.Blazor;

public class UserFacingErrorTests
{
    private sealed class AppException : Exception;

    [Fact]
    public void Describe_ShouldUseTheFallback_ForAnUnknownException()
        => UserFacingError.Describe(new InvalidOperationException(), "fallback").Should().Be("fallback");

    [Fact]
    public void Describe_ShouldAskTheApplicationFirst()
    {
        var text = UserFacingError.Describe(new AppException(), "fallback", exception => exception is AppException ? "mine" : null);

        text.Should().Be("mine");
    }

    [Fact]
    public void Describe_ShouldFallBackWhenTheApplicationDoesNotKnowTheException()
    {
        var text = UserFacingError.Describe(new InvalidOperationException(), "fallback", _ => null);

        text.Should().Be("fallback");
    }

    [Theory]
    [InlineData("en", "changed elsewhere")]
    [InlineData("de", "zwischenzeitlich")]
    [InlineData("ja", "別の場所で変更")]
    public void Describe_ShouldSpeakTheLanguageOfTheSession_ForAConcurrencyConflict(string language, string expected)
    {
        var previous = CultureInfo.CurrentUICulture;

        try
        {
            CultureInfo.CurrentUICulture = new CultureInfo(language);

            UserFacingError.Describe(new DbUpdateConcurrencyException(), "fallback").Should().Contain(expected);
        }
        finally
        {
            CultureInfo.CurrentUICulture = previous;
        }
    }
}
