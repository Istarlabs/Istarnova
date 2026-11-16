#region "Header"

// File: FilterSetTests.cs
// Created 2026/10/28
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using Istarnova.Filtering;

namespace Istarnova.Tests.Filtering;

public class FilterSetTests
{
    private sealed record Volume(string? Title, int Pages, decimal? Price, DateOnly? Released, bool? IsRead);

    private static readonly IReadOnlyList<FilterProperty<Volume>> Properties =
    [
        new("Title", "Title", FilterKind.Text, volume => volume.Title),
        new("Pages", "Pages", FilterKind.Number, volume => volume.Pages),
        new("Price", "Price", FilterKind.Number, volume => volume.Price),
        new("Released", "Released", FilterKind.Date, volume => volume.Released),
        new("IsRead", "Read", FilterKind.Boolean, volume => volume.IsRead),
    ];

    private static readonly Volume[] Volumes =
    [
        new("Naruto Bd. 1", 192, 6.5m, new(2020, 1, 1), true),
        new("Naruto Bd. 2", 200, 7m, new(2021, 6, 1), false),
        new("Bleach Bd. 1", 180, null, null, null),
    ];

    private static string[] Titles(FilterSet set)
        => [.. set.Apply(Volumes, Properties).Select(volume => volume.Title!)];

    private static FilterSet Single(string property, FilterOperator filterOperator, string? text = null, decimal? number = null, DateOnly? date = null)
    {
        var set = new FilterSet();

        set.Rules.Add(new() { Property = property, Operator = filterOperator, Text = text, Number = number, Date = date });

        return set;
    }

    [Fact]
    public void Apply_ShouldKeepEverything_WithoutRules()
        => new FilterSet().Apply(Volumes, Properties).Should().HaveCount(3);

    [Fact]
    public void Apply_ShouldKeepEverything_WhenARuleLacksItsValue()
        => Single("Title", FilterOperator.Contains).Apply(Volumes, Properties).Should().HaveCount(3);

    [Fact]
    public void Apply_ShouldIgnoreARule_WhoseProperty_IsUnknown()
        => Single("Nothing", FilterOperator.IsEqualTo, "x").Apply(Volumes, Properties).Should().HaveCount(3);

    [Fact]
    public void Text_ShouldContainIgnoringCase()
        => Titles(Single("Title", FilterOperator.Contains, "NARUTO")).Should().Equal("Naruto Bd. 1", "Naruto Bd. 2");

    [Fact]
    public void Number_ShouldLeaveOutItemsWithoutValue_ForGreaterThan()
        => Titles(Single("Price", FilterOperator.GreaterThan, number: 6.5m)).Should().Equal("Naruto Bd. 2");

    [Fact]
    public void Number_ShouldFindMissingValues_ForIsEmpty()
        => Titles(Single("Price", FilterOperator.IsEmpty)).Should().Equal("Bleach Bd. 1");

    [Fact]
    public void Number_ShouldKeepItemsWithoutValue_ForIsNotEqualTo()
        => Titles(Single("Pages", FilterOperator.IsNotEqualTo, number: 192m)).Should().Equal("Naruto Bd. 2", "Bleach Bd. 1");

    [Fact]
    public void Date_ShouldCompare_ForLessThanOrEqualTo()
        => Titles(Single("Released", FilterOperator.LessThanOrEqualTo, date: new(2020, 12, 31))).Should().Equal("Naruto Bd. 1");

    [Fact]
    public void Boolean_ShouldTakeWhatIsUnknownAsNo()
        => Titles(Single("IsRead", FilterOperator.IsFalse)).Should().Equal("Naruto Bd. 2", "Bleach Bd. 1");

    [Fact]
    public void Apply_ShouldRequireAllRules_UnlessMatchAny()
    {
        var set = Single("Title", FilterOperator.Contains, "Bleach");

        set.Rules.Add(new() { Property = "Pages", Operator = FilterOperator.GreaterThan, Number = 190m });

        set.Apply(Volumes, Properties).Should().BeEmpty();

        set.MatchAny = true;

        set.Apply(Volumes, Properties).Should().HaveCount(3);
    }
}
