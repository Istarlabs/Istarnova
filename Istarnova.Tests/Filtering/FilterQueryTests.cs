#region "Header"

// File: FilterQueryTests.cs
// Created 2026/11/07
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

#region "Usings"

using System.Globalization;
using Istarnova.Filtering;
using Microsoft.EntityFrameworkCore;

#endregion

namespace Istarnova.Tests.Filtering;

/// <summary>
///     The rules of a <see cref="FilterSet" /> applied to a query keep the rows they keep in a list in memory, and become
///     something the Npgsql provider translates.
/// </summary>
public class FilterQueryTests
{
    #region "Constants"

    private static readonly IReadOnlyList<FilterProperty<Row>> Properties =
    [
        FilterProperties.Text<Row>("Name", "Name", row => row.Name),
        FilterProperties.Number<Row, int>("Count", "Count", row => row.Count),
        FilterProperties.Number<Row, decimal?>("Price", "Price", row => row.Price),
        FilterProperties.Date<Row, DateOnly?>("Day", "Day", row => row.Day),
        FilterProperties.Boolean<Row, bool?>("Flag", "Flag", row => row.Flag),
    ];

    private static readonly Row[] Rows =
    [
        new(1, "Naruto Bd. 1", 192, 6.5m, new(2020, 1, 1), true),
        new(2, "Naruto Bd. 2", 200, 7m, new(2021, 6, 1), false),
        new(3, "Bleach Bd. 1", 180, null, null, null),
        new(4, null, 150, 3m, new(2019, 3, 3), true),
        new(5, "  ", 150, 3m, new(2019, 3, 3), true),
    ];

    #endregion

    #region "Methods"

    public static TheoryData<string, FilterOperator, string?, string?> Cases => new()
    {
        { "Name", FilterOperator.Contains, "NARUTO", null },
        { "Name", FilterOperator.NotContains, "naruto", null },
        { "Name", FilterOperator.StartsWith, "bleach", null },
        { "Name", FilterOperator.EndsWith, "bd. 1", null },
        { "Name", FilterOperator.IsEqualTo, "naruto bd. 2", null },
        { "Name", FilterOperator.IsNotEqualTo, "naruto bd. 2", null },
        { "Name", FilterOperator.IsEmpty, null, null },
        { "Name", FilterOperator.IsNotEmpty, null, null },
        { "Count", FilterOperator.GreaterThan, "190", null },
        { "Count", FilterOperator.IsEqualTo, "192", null },
        { "Count", FilterOperator.LessThanOrEqualTo, "150.5", null },
        { "Count", FilterOperator.IsEmpty, null, null },
        { "Price", FilterOperator.IsEmpty, null, null },
        { "Price", FilterOperator.IsNotEmpty, null, null },
        { "Price", FilterOperator.IsNotEqualTo, "6.5", null },
        { "Price", FilterOperator.LessThanOrEqualTo, "6.5", null },
        { "Day", FilterOperator.GreaterThanOrEqualTo, null, "2021-01-01" },
        { "Day", FilterOperator.IsEmpty, null, null },
        { "Day", FilterOperator.IsNotEqualTo, null, "2019-03-03" },
        { "Flag", FilterOperator.IsTrue, null, null },
        { "Flag", FilterOperator.IsFalse, null, null },
    };

    [Theory]
    [MemberData(nameof(Cases))]
    public void ApplyTo_ShouldKeepTheRowsAListKeeps(string property, FilterOperator filterOperator, string? valueText, string? dateText)
    {
        var set = Single(property, filterOperator, valueText, dateText);

        var inMemory = set.Apply(Rows, Properties).Select(row => row.RowId);
        var onQuery = set.ApplyTo(Rows.AsQueryable(), Properties).Select(row => row.RowId);

        onQuery.Should().Equal(inMemory);
    }

    [Fact]
    public void ApplyTo_ShouldMatchAnyOrAll_AsTheSetSays()
    {
        var set = Single("Name", FilterOperator.StartsWith, "bleach", null);

        set.Rules.Add(new() { Property = "Count", Operator = FilterOperator.GreaterThan, Number = 195m });

        set.ApplyTo(Rows.AsQueryable(), Properties).Should().BeEmpty();

        set.MatchAny = true;

        set.ApplyTo(Rows.AsQueryable(), Properties).Select(row => row.RowId).Should().Equal(2, 3);
    }

    [Fact]
    public void ApplyTo_ShouldKeepEverything_WhenARuleLacksItsValueOrItsPropertyIsUnknown()
    {
        var set = Single("Name", FilterOperator.Contains, null, null);

        set.Rules.Add(new() { Property = "Nothing", Operator = FilterOperator.IsEqualTo, Text = "x" });

        set.ApplyTo(Rows.AsQueryable(), Properties).Should().HaveCount(Rows.Length);
    }

    [Fact]
    public void ApplyTo_ShouldRefuseAProperty_WithoutASelector()
    {
        IReadOnlyList<FilterProperty<Row>> listOnly = [new("Name", "Name", FilterKind.Text, row => row.Name)];

        var act = () => Single("Name", FilterOperator.Contains, "x", null).ApplyTo(Rows.AsQueryable(), listOnly);

        act.Should().Throw<InvalidOperationException>().WithMessage("*Name*selector*");
    }

    [Fact]
    public void Value_ShouldBeNull_WhenTheNavigationToThePropertyIsMissing()
    {
        var property = FilterProperties.Text<Wrapper>("City", "City", wrapper => wrapper.Address!.City);
        var set = new FilterSet();

        set.Rules.Add(new() { Property = "City", Operator = FilterOperator.IsEmpty });

        var kept = set.Apply([new Wrapper(null), new Wrapper(new("Hannover"))], [property]);

        kept.Should().ContainSingle().Which.Address.Should().BeNull();
    }

    [Theory]
    [InlineData("Name", FilterOperator.Contains, "naruto", "lower")]
    [InlineData("Name", FilterOperator.StartsWith, "naruto", "LIKE")]
    [InlineData("Name", FilterOperator.IsEmpty, null, "WHERE")]
    [InlineData("Count", FilterOperator.GreaterThan, "190", "WHERE")]
    [InlineData("Price", FilterOperator.IsNotEqualTo, "6.5", "IS NULL")]
    public void ApplyTo_ShouldBecomeSql_TheNpgsqlProviderTranslates(string property, FilterOperator filterOperator, string? valueText, string expected)
    {
        using var context = new FilterQueryContext();

        var sql = Single(property, filterOperator, valueText, null).ApplyTo(context.Rows, Properties).ToQueryString();

        sql.Should().Contain(expected);
    }

    private static FilterSet Single(string property, FilterOperator filterOperator, string? valueText, string? dateText)
    {
        var set = new FilterSet();
        var kind = Properties.First(entry => entry.Name == property).Kind;

        set.Rules.Add(new()
        {
            Property = property,
            Operator = filterOperator,
            Text = kind == FilterKind.Text ? valueText : null,
            Number = kind == FilterKind.Number && valueText is { } number ? decimal.Parse(number, CultureInfo.InvariantCulture) : null,
            Date = dateText is { } day ? DateOnly.Parse(day, CultureInfo.InvariantCulture) : null,
        });

        return set;
    }

    #endregion

    #region "Types"

    private sealed record Row(int RowId, string? Name, int Count, decimal? Price, DateOnly? Day, bool? Flag);

    private sealed record Address(string? City);

    private sealed record Wrapper(Address? Address);

    private sealed class FilterQueryContext : DbContext
    {
        public FilterQueryContext()
            : base(new DbContextOptionsBuilder<FilterQueryContext>().UseNpgsql("Host=localhost;Database=model_only").Options)
        {
        }

        public IQueryable<Row> Rows => Set<Row>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.Entity<Row>().HasKey(row => row.RowId);
    }

    #endregion
}
