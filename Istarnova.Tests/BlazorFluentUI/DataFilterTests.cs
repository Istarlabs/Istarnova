#region "Header"

// File: DataFilterTests.cs
// Created 2026/10/28
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using Bunit;
using Istarnova.Blazor.FluentUI;
using Istarnova.Blazor.FluentUI.Components;
using Istarnova.Filtering;
using Microsoft.Extensions.DependencyInjection;

namespace Istarnova.Tests.BlazorFluentUI;

public class DataFilterTests : BunitContext
{
    private sealed record Row(string Name, int Count);

    private static readonly IReadOnlyList<FilterProperty<Row>> Properties =
    [
        new("Name", "Name", FilterKind.Text, row => row.Name),
        new("Count", "Count", FilterKind.Number, row => row.Count),
    ];

    public DataFilterTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddIstarnovaFluentUI();
    }

    [Theory]
    [InlineData("en", "Filter", "Add condition", "contains")]
    [InlineData("de", "Filter", "Bedingung hinzufügen", "enthält")]
    [InlineData("ja", "フィルター", "条件を追加", "を含む")]
    [InlineData("es", "Filtro", "Añadir condición", "contiene")]
    [InlineData("fr", "Filtre", "Ajouter une condition", "contient")]
    [InlineData("it", "Filtro", "Aggiungi condizione", "contiene")]
    [InlineData("pl", "Filtr", "Dodaj warunek", "zawiera")]
    [InlineData("pt-PT", "Filtro", "Adicionar condição", "contém")]
    [InlineData("tr", "Filtre", "Koşul ekle", "içerir")]
    [InlineData("uk", "Фільтр", "Додати умову", "містить")]
    public void DataFilter_ShouldOpenWithOneCondition_InTheLanguageOfTheSession(string language, string button, string add, string contains)
    {
        using var _ = new CultureScope(language);
        var cut = Render<DataFilter<Row>>(parameters => parameters
            .Add(filter => filter.Properties, Properties)
            .Add(filter => filter.FilterSet, new FilterSet()));

        cut.Markup.Should().Contain(button);

        cut.Find("fluent-button").Click();

        cut.Markup.Should().Contain(add);
        cut.Markup.Should().Contain(contains);
    }

    [Theory]
    [InlineData(FilterKind.Number, FilterOperator.GreaterThan)]
    [InlineData(FilterKind.Date, FilterOperator.LessThan)]
    [InlineData(FilterKind.Boolean, FilterOperator.IsTrue)]
    [InlineData(FilterKind.Text, FilterOperator.IsEmpty)]
    public void DataFilter_ShouldRenderTheEditorOfEveryKind(FilterKind kind, FilterOperator filterOperator)
    {
        IReadOnlyList<FilterProperty<Row>> properties = [new("Field", "Field", kind, _ => null)];
        var set = new FilterSet();

        set.Rules.Add(new() { Property = "Field", Operator = filterOperator });

        var cut = Render<DataFilter<Row>>(parameters => parameters
            .Add(filter => filter.Properties, properties)
            .Add(filter => filter.FilterSet, set));

        cut.Find("fluent-button").Click();

        cut.FindAll(".data-filter-row").Should().HaveCount(1);
    }

    [Fact]
    public void DataFilter_ShouldRaiseOnChanged_WhenAConditionIsAdded()
    {
        var changed = 0;
        var set = new FilterSet();
        var cut = Render<DataFilter<Row>>(parameters => parameters
            .Add(filter => filter.Properties, Properties)
            .Add(filter => filter.FilterSet, set)
            .Add(filter => filter.OnChanged, () => changed++));

        cut.Find("fluent-button").Click();
        cut.FindAll("fluent-button").First(button => button.TextContent.Contains("Add condition")).Click();

        set.Rules.Should().HaveCount(2);
        changed.Should().Be(1);
    }
}
