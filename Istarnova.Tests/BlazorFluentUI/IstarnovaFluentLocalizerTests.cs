#region "Header"

// File: IstarnovaFluentLocalizerTests.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using System.Resources;
using Istarnova.Blazor.FluentUI.Localization;

namespace Istarnova.Tests.BlazorFluentUI;

public class IstarnovaFluentLocalizerTests
{
    private readonly IstarnovaFluentLocalizer _localizer = new();

    [Theory]
    [InlineData("en", "Go to first page")]
    [InlineData("de", "Zur ersten Seite")]
    [InlineData("ja", "最初のページへ")]
    public void Indexer_ShouldAnswerInTheLanguageOfTheSession(string language, string expected)
    {
        using var _ = new CultureScope(language);

        _localizer["Paginator_GoFirstPage"].Should().Be(expected);
    }

    [Fact]
    public void Indexer_ShouldFillInTheArguments()
    {
        using var _ = new CultureScope("de");

        _localizer["Paginator_Status", 2, 5].Should().Be("Seite 2 von 5");
    }

    [Fact]
    public void Indexer_ShouldFallBackToTheLibrarysEnglish_ForALanguageWithoutTranslations()
    {
        using var _ = new CultureScope("nl");

        _localizer["Paginator_GoFirstPage"].Should().Be("Go to first page");
    }

    [Theory]
    [InlineData("es", "Cerrar")]
    [InlineData("fr", "Fermer")]
    [InlineData("it", "Chiudi")]
    [InlineData("pl", "Zamknij")]
    [InlineData("pt-PT", "Fechar")]
    [InlineData("tr", "Kapat")]
    [InlineData("uk", "Закрити")]
    public void Indexer_ShouldSpeakEveryShippedLanguage(string language, string expected)
    {
        using var _ = new CultureScope(language);

        _localizer["LayoutHamburger_Close"].Should().Be(expected);
    }

    [Fact]
    public void Indexer_ShouldAskTheApplicationsResourcesFirst()
    {
        var localizer = new IstarnovaFluentLocalizer(new ResourceManager("Istarnova.Tests.ApplicationLocalizerTexts", typeof(IstarnovaFluentLocalizerTests).Assembly));

        localizer["Paginator_GoFirstPage"].Should().Be("Zum Anfang");
        localizer["Paginator_GoLastPage"].Should().Be("Go to last page");
    }

    [Fact]
    public void Resources_ShouldCarryTheSameKeysInAllThreeLanguages()
    {
        var keys = new[] { "MessageBox_ButtonYes", "DataGrid_EmptyContent", "Tabs_MoreItems", "Wizard_LabelButtonNext", "TimeAgo_SecondAgo" };

        foreach (var language in new[] { "en", "de", "ja" })
        {
            using var _ = new CultureScope(language);

            foreach (var key in keys)
            {
                _localizer[key].Should().NotBeNullOrWhiteSpace();
            }
        }
    }
}
