#region "Header"

// File: ColumnFilterTests.cs
// Created 2026/11/07
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

#region "Usings"

using Bunit;
using Istarnova.Blazor.FluentUI;
using Istarnova.Blazor.FluentUI.Components;
using Istarnova.Filtering;
using Microsoft.Extensions.DependencyInjection;

#endregion

namespace Istarnova.Tests.BlazorFluentUI;

public class ColumnFilterTests : BunitContext
{
    #region "Constants"

    private static readonly FilterProperty<Row> Name = FilterProperties.Text<Row>("Name", "Name", row => row.Name);

    #endregion

    #region "Constructor"

    public ColumnFilterTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddIstarnovaFluentUI();
    }

    #endregion

    #region "Methods"

    [Theory]
    [InlineData("en", "Filter Name", "begins with", "does not contain")]
    [InlineData("de", "Name filtern", "beginnt mit", "enthält nicht")]
    [InlineData("ja", "Nameを絞り込む", "で始まる", "を含まない")]
    [InlineData("es", "Filtrar Name", "empieza por", "no contiene")]
    [InlineData("fr", "Filtrer Name", "commence par", "ne contient pas")]
    [InlineData("it", "Filtra Name", "inizia con", "non contiene")]
    [InlineData("pl", "Filtruj: Name", "zaczyna się od", "nie zawiera")]
    [InlineData("pt-PT", "Filtrar Name", "começa com", "não contém")]
    [InlineData("tr", "Name için filtre", "ile başlar", "içermez")]
    [InlineData("uk", "Фільтр: Name", "починається з", "не містить")]
    public void ColumnFilter_ShouldSpeakTheLanguageOfTheSession(string language, string title, string startsWith, string notContains)
    {
        using var _ = new CultureScope(language);
        var cut = Render<ColumnFilter<Row>>(parameters => parameters
            .Add(filter => filter.Property, Name)
            .Add(filter => filter.FilterSet, new FilterSet()));

        cut.Markup.Should().Contain(title);

        cut.Find("fluent-button").Click();

        cut.Markup.Should().Contain(startsWith);
        cut.Markup.Should().Contain(notContains);
    }

    [Fact]
    public void ColumnFilter_ShouldStartARule_WhenItIsOpened_AndRemoveIt_WhenCleared()
    {
        var changed = 0;
        var set = new FilterSet();
        var cut = Render<ColumnFilter<Row>>(parameters => parameters
            .Add(filter => filter.Property, Name)
            .Add(filter => filter.FilterSet, set)
            .Add(filter => filter.OnChanged, () => changed++));

        cut.Find("fluent-button").Click();

        set.Rules.Should().ContainSingle().Which.Property.Should().Be("Name");
        changed.Should().Be(0);

        cut.FindAll("fluent-button").First(button => button.TextContent.Contains("Clear")).Click();

        set.Rules.Should().BeEmpty();
        changed.Should().Be(1);
    }

    [Fact]
    public void ColumnFilter_ShouldEditTheRuleTheSetAlreadyHolds_AndLeaveTheOthersAlone()
    {
        var set = new FilterSet();

        set.Rules.Add(new() { Property = "Count", Operator = FilterOperator.GreaterThan, Number = 3m });
        set.Rules.Add(new() { Property = "Name", Operator = FilterOperator.IsEqualTo, Text = "x" });

        var cut = Render<ColumnFilter<Row>>(parameters => parameters
            .Add(filter => filter.Property, Name)
            .Add(filter => filter.FilterSet, set));

        cut.Find("fluent-button").Click();
        cut.FindAll("fluent-button").First(button => button.TextContent.Contains("Clear")).Click();

        set.Rules.Should().ContainSingle().Which.Property.Should().Be("Count");
    }

    #endregion

    #region "Types"

    private sealed record Row(string? Name, int Count);

    #endregion
}
