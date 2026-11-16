// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

using Istarnova.Collections.Generics;

namespace Istarnova.Tests.Collections.Generics;

public class CollectionExtensionsSearchTests
{
    private sealed record Person(string? FirstName, string? LastName, int Age);

    private static readonly Person[] People =
    [
        new("Ada", "Lovelace", 36),
        new("Alan", "Turing", 41),
        new(null, "Hopper", 85),
    ];

    [Fact]
    public void SearchInProperties_ShouldMatchAnySelector_WhenGivenExpressions()
        => People.SearchInProperties("ing", person => person.FirstName, person => person.LastName)
            .Select(person => person.LastName)
            .Should().Equal("Turing");

    [Fact]
    public void SearchInProperties_ShouldIgnoreCaseAndNullValues_WhenGivenExpressions()
        => People.SearchInProperties("HOPP", person => person.FirstName, person => person.LastName)
            .Should().ContainSingle().Which.Age.Should().Be(85);

    [Fact]
    public void SearchInProperties_ShouldBeEvaluatedLazily_WhenTheResultIsNotEnumerated()
    {
        var source = new[] { "a" }.Concat(ThrowingSource());

        var result = source.SearchInProperties("a", text => text);

        result.Should().NotBeNull();
    }

    private static IEnumerable<string> ThrowingSource()
    {
        throw new InvalidOperationException("enumerated too early");

#pragma warning disable CS0162
        yield break;
#pragma warning restore CS0162
    }

    [Fact]
    public void SearchInProperties_ShouldMatchNamedStringProperties_WhenGivenNames()
        => People.SearchInProperties("lace", nameof(Person.FirstName), nameof(Person.LastName), nameof(Person.Age))
            .Should().ContainSingle().Which.FirstName.Should().Be("Ada");

    [Fact]
    public void SearchInProperties_ShouldReturnEverything_WhenTermIsBlankOrNoPropertyQualifies()
    {
        People.SearchInProperties(" ", nameof(Person.LastName)).Should().HaveCount(3);
        People.SearchInProperties("x", nameof(Person.Age), "Missing").Should().HaveCount(3);
    }
}
