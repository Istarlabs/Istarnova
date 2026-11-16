#region "Header"

// File: BlazorComponentTests.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using System.Linq.Expressions;
using Bunit;
using Istarnova.Blazor.FluentUI;
using Istarnova.Blazor.FluentUI.Errors;
using Istarnova.Blazor.FluentUI.Grids;
using Istarnova.Blazor.FluentUI.Navigation;
using Istarnova.Blazor.FluentUI.Scaffolding;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FluentUI.AspNetCore.Components;

namespace Istarnova.Tests.BlazorFluentUI;

public class BlazorComponentTests : BunitContext
{
    private sealed record Row(string Name);

    private sealed class Boom : ComponentBase
    {
        protected override void BuildRenderTree(RenderTreeBuilder builder)
            => throw new InvalidOperationException("boom");
    }

    private sealed class FixedNavigation(IEnumerable<NavigationMenuObject> items) : INavigationService
    {
        public IEnumerable<NavigationMenuObject> BuildMainMenu() => items;
    }

    public BlazorComponentTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddIstarnovaFluentUI();
    }

    [Theory]
    [InlineData("en", "No data available")]
    [InlineData("de", "Keine Daten vorhanden")]
    [InlineData("ja", "データがありません")]
    public void LocalizedDataGrid_ShouldSayThereIsNoData_InTheLanguageOfTheSession(string language, string expected)
    {
        using var _ = new CultureScope(language);

        var cut = Render<LocalizedDataGrid<Row>>(parameters => parameters
            .Add(grid => grid.Items, Array.Empty<Row>().AsQueryable())
            .Add(grid => grid.ChildContent, NameColumn()));

        cut.WaitForAssertion(() => cut.Markup.Should().Contain(expected));
    }

    [Theory]
    [InlineData("en", "Page 1 of 3", "25 items total")]
    [InlineData("de", "Seite 1 von 3", "25 Einträge gesamt")]
    [InlineData("ja", "1 / 3 ページ", "合計 25 件")]
    public async Task LocalizedPaginator_ShouldWriteThePageAndTheSummaryInTheLanguageOfTheSession(string language, string page, string summary)
    {
        using var _ = new CultureScope(language);
        var state = new PaginationState { ItemsPerPage = 10 };
        await state.SetTotalItemCountAsync(25);

        var cut = Render<LocalizedPaginator>(parameters => parameters.Add(paginator => paginator.State, state));

        cut.Markup.Should().Contain(page);
        cut.Markup.Should().Contain(summary);
    }

    [Fact]
    public void LocalizedDataGrid_ShouldShowTheRowsItIsGiven()
    {
        var rows = new[] { new Row("Amber"), new Row("Kaeya") }.AsQueryable();

        var cut = Render<LocalizedDataGrid<Row>>(parameters => parameters
            .Add(grid => grid.Items, rows)
            .Add(grid => grid.ChildContent, NameColumn()));

        cut.WaitForAssertion(() =>
        {
            cut.Markup.Should().Contain("Amber");
            cut.Markup.Should().Contain("Kaeya");
        });
    }

    [Fact]
    public void LocalizedDataGrid_ShouldShowEveryRow_WhenItDoesNotPage()
    {
        var rows = Enumerable.Range(1, 15).Select(number => new Row($"Row{number:00}")).AsQueryable();

        var cut = Render<LocalizedDataGrid<Row>>(parameters => parameters
            .Add(grid => grid.Items, rows)
            .Add(grid => grid.ShowPaginator, false)
            .Add(grid => grid.ChildContent, NameColumn()));

        cut.WaitForAssertion(() =>
        {
            cut.Markup.Should().Contain("Row15");
            cut.Markup.Should().NotContain("Page 1 of");
        });
    }

    [Fact]
    public void LocalizedDataGrid_ShouldStopAtTheFirstPage_WhenItPages()
    {
        var rows = Enumerable.Range(1, 15).Select(number => new Row($"Row{number:00}")).AsQueryable();

        var cut = Render<LocalizedDataGrid<Row>>(parameters => parameters
            .Add(grid => grid.Items, rows)
            .Add(grid => grid.ChildContent, NameColumn()));

        cut.WaitForAssertion(() =>
        {
            cut.Markup.Should().Contain("Row10");
            cut.Markup.Should().NotContain("Row11");
        });
    }

    [Fact]
    public void LocalizedDataGrid_ShouldPassUnknownAttributesOnToTheGrid()
    {
        var cut = Render<LocalizedDataGrid<Row>>(parameters => parameters
            .Add(grid => grid.Items, Array.Empty<Row>().AsQueryable())
            .Add(grid => grid.ChildContent, NameColumn())
            .AddUnmatched("data-test", "passed"));

        cut.Markup.Should().Contain("data-test=\"passed\"");
    }

    [Fact]
    public async Task LocalizedDataGrid_ShouldRememberThePageItWasLeftOn()
    {
        var pagination = new PaginationState { ItemsPerPage = 10 };
        var rows = Enumerable.Range(1, 30).Select(number => new Row($"Row{number}")).AsQueryable();

        Render<LocalizedDataGrid<Row>>(parameters => parameters
            .Add(grid => grid.Items, rows)
            .Add(grid => grid.Pagination, pagination)
            .Add(grid => grid.ChildContent, NameColumn()));

        var memory = Services.GetRequiredService<GridPageMemory>();
        await pagination.SetCurrentPageIndexAsync(2);
        await DisposeAsync();

        // The grid keeps the memory it found when it started: Dispose also runs while a server-side scope is torn down,
        // where a service lookup threw ObjectDisposedException.
        memory.Recall($"/|{typeof(Row).FullName}").Should().Be(2);
    }

    [Fact]
    public void GridPageMemory_ShouldRememberAPagePerKey()
    {
        var memory = new GridPageMemory();

        memory.Remember("a", 3);

        memory.Recall("a").Should().Be(3);
        memory.Recall("b").Should().Be(0);
    }

    [Theory]
    [InlineData("en", "Back to overview")]
    [InlineData("de", "Zurück zur Übersicht")]
    [InlineData("ja", "一覧に戻る")]
    public void PageScaffold_ShouldOfferTheBackLinkInTheLanguageOfTheSession(string language, string expected)
    {
        using var _ = new CultureScope(language);

        var cut = Render<PageScaffold>(parameters => parameters
            .Add(scaffold => scaffold.Title, "Countries")
            .Add(scaffold => scaffold.BackUrl, "/admin"));

        cut.Markup.Should().Contain("Countries");
        cut.Markup.ToLowerInvariant().Should().Contain(expected.ToLowerInvariant());
    }

    [Fact]
    public void PageScaffold_ShouldPreferTheBreadcrumbsToTheBackLink()
    {
        var cut = Render<PageScaffold>(parameters => parameters
            .Add(scaffold => scaffold.Title, "Detail")
            .Add(scaffold => scaffold.BackUrl, "/admin")
            .Add(scaffold => scaffold.Breadcrumbs, [new PageBreadcrumb("Admin", "/admin"), new PageBreadcrumb("Detail")]));

        cut.Markup.Should().Contain("page-scaffold__breadcrumb");
        cut.Markup.Should().NotContain("page-scaffold__back");
    }

    [Fact]
    public void PageScaffold_ShouldRenderWhatTheApplicationPutNextToTheTitle()
    {
        var cut = Render<PageScaffold>(parameters => parameters
            .Add(scaffold => scaffold.Title, "Detail")
            .Add(scaffold => scaffold.TitleSuffix, "<span id=\"help\">?</span>"));

        cut.Find("#help").TextContent.Should().Be("?");
    }

    [Fact]
    public void SectionHeader_ShouldExposeTheTitleAsAHeading_WithoutAHeadingElementInside()
    {
        var cut = Render<SectionHeader>(parameters => parameters
            .Add(header => header.Title, "Links")
            .Add(header => header.Level, 4));

        var title = cut.Find(".section-header__title");
        title.GetAttribute("role").Should().Be("heading");
        title.GetAttribute("aria-level").Should().Be("4");
        title.TextContent.Should().Contain("Links");
        cut.FindAll("h1, h2, h3, h4, h5, h6").Should().BeEmpty();
    }

    [Fact]
    public void SectionHeader_ShouldRenderTheActions_OnlyWhenGiven()
    {
        var without = Render<SectionHeader>(parameters => parameters.Add(header => header.Title, "Links"));
        without.FindAll(".section-header__actions").Should().BeEmpty();

        var with = Render<SectionHeader>(parameters => parameters
            .Add(header => header.Title, "Links")
            .Add(header => header.Actions, "<button id=\"edit\">Edit</button>"));
        with.Find(".section-header__actions #edit").TextContent.Should().Be("Edit");
    }

    [Fact]
    public void NavMenu_ShouldShowTheEntries_AndHideWhatThePolicyRefuses()
    {
        AddAuthorization().SetAuthorized("someone");
        Services.AddSingleton<INavigationService>(new FixedNavigation(
        [
            new NavigationMenuObject { Title = "Overview", Url = "/" },
            new NavigationMenuObject { Title = "Secret page", Url = "/secret", Policy = "Administrators" },
        ]));

        var cut = Render<NavMenu>();

        cut.Markup.Should().Contain("Overview");
        cut.Markup.Should().NotContain("Secret page");
    }

    [Fact]
    public void NavMenu_ShouldShowAnEntryTheUserMayUse()
    {
        AddAuthorization().SetAuthorized("someone").SetPolicies("Administrators");
        Services.AddSingleton<INavigationService>(new FixedNavigation(
        [
            new NavigationMenuObject { Title = "Secret page", Url = "/secret", Policy = "Administrators" },
        ]));

        var cut = Render<NavMenu>();

        cut.Markup.Should().Contain("Secret page");
    }

    [Fact]
    public void NavMenu_ShouldRenderItemsPassedToIt_InsteadOfAskingTheService()
    {
        AddAuthorization().SetAuthorized("someone");
        Services.AddSingleton<INavigationService>(new FixedNavigation([new NavigationMenuObject { Title = "From the service" }]));

        var cut = Render<NavMenu>(parameters => parameters
            .Add(menu => menu.Items, [new NavigationMenuObject { Title = "Passed in", Url = "/x" }]));

        cut.Markup.Should().Contain("Passed in");
        cut.Markup.Should().NotContain("From the service");
    }

    [Theory]
    [InlineData("en", "Main navigation")]
    [InlineData("de", "Hauptnavigation")]
    public void NavMenu_ShouldNameTheNavigationInTheLanguageOfTheSession(string language, string expected)
    {
        using var _ = new CultureScope(language);
        AddAuthorization().SetAuthorized("someone");
        Services.AddSingleton<INavigationService>(new FixedNavigation([new NavigationMenuObject { Title = "Home", Url = "/" }]));

        var cut = Render<NavMenu>();

        cut.Markup.Should().Contain($"aria-label=\"{expected}\"");
    }

    [Fact]
    public void PageErrorBoundary_ShouldShowAPanelWithAReference_InsteadOfTakingThePageDown()
    {
        var cut = Render<PageErrorBoundary>(parameters => parameters
            .AddChildContent<Boom>());

        cut.Markup.Should().Contain("error-panel");
        cut.Markup.Should().MatchRegex("[0-9A-F]{8}");
    }

    [Fact]
    public void PageErrorBoundary_ShouldRenderItsContent_WhenNothingFails()
    {
        var cut = Render<PageErrorBoundary>(parameters => parameters
            .AddChildContent("<p id=\"fine\">fine</p>"));

        cut.Find("#fine").TextContent.Should().Be("fine");
    }

    private static RenderFragment NameColumn()
        => builder =>
        {
            builder.OpenComponent<PropertyColumn<Row, string>>(0);
            builder.AddComponentParameter(1, nameof(PropertyColumn<Row, string>.Property), (Expression<Func<Row, string>>)(row => row.Name));
            builder.AddComponentParameter(2, nameof(PropertyColumn<Row, string>.Title), "Name");
            builder.CloseComponent();
        };
}
