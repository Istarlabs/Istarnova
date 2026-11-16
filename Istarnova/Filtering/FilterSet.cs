#region "Header"

// File: FilterSet.cs
// Created 2026/10/28
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

#region "Usings"

using System.Globalization;

#endregion

namespace Istarnova.Filtering;

/// <summary>
///     The conditions a user put together, applied to a list in memory. A rule still lacking its value matches
///     everything, so a condition being typed in does not empty the list.
/// </summary>
public sealed class FilterSet
{
    #region "Properties"

    /// <summary>
    ///     Whether one matching rule is enough; otherwise every rule has to match.
    /// </summary>
    public bool MatchAny { get; set; }

    public List<FilterRule> Rules { get; } = [];

    #endregion

    #region "Methods"

    /// <summary>
    ///     The items matching the rules, in their order.
    /// </summary>
    public IReadOnlyList<TItem> Apply<TItem>(IEnumerable<TItem> items, IReadOnlyList<FilterProperty<TItem>> properties)
    {
        var active = Rules
            .Select(rule => (Rule: rule, Property: properties.FirstOrDefault(property => property.Name == rule.Property)))
            .Where(pair => pair.Property is { } property && pair.Rule.IsComplete(property.Kind))
            .ToList();

        if (active.Count == 0)
        {
            return [.. items];
        }

        return
        [
            .. items.Where(item => MatchAny
                ? active.Any(pair => Matches(pair.Rule, pair.Property!, item))
                : active.All(pair => Matches(pair.Rule, pair.Property!, item))),
        ];
    }

    /// <summary>
    ///     The same conditions as a <c>Where</c> on a query, so a list the database pages and sorts is filtered there and not after
    ///     it has been loaded. A rule still lacking its value matches everything here as well.
    /// </summary>
    /// <param name="query">The query to narrow.</param>
    /// <param name="properties">What can be filtered on; each one a rule names needs a <see cref="FilterProperty{TItem}.Selector" />.</param>
    /// <exception cref="InvalidOperationException">A property a rule uses has no selector.</exception>
    public IQueryable<TItem> ApplyTo<TItem>(IQueryable<TItem> query, IReadOnlyList<FilterProperty<TItem>> properties)
        => FilterQueryBuilder.Apply(query, this, properties);

    private static bool Matches<TItem>(FilterRule rule, FilterProperty<TItem> property, TItem item)
    {
        var value = property.Value(item);

        return property.Kind switch
        {
            FilterKind.Text => MatchesText(rule, value as string ?? value?.ToString()),
            FilterKind.Number => MatchesComparable(rule.Operator, value is null ? null : Convert.ToDecimal(value, CultureInfo.InvariantCulture), rule.Number),
            FilterKind.Date => MatchesComparable(rule.Operator, value is DateOnly day ? day : null, rule.Date),
            var _ => rule.Operator == FilterOperator.IsTrue == (value is true),
        };
    }

    private static bool MatchesText(FilterRule rule, string? actual)
        => rule.Operator switch
        {
            FilterOperator.Contains => actual?.Contains(rule.Text!, StringComparison.CurrentCultureIgnoreCase) == true,
            FilterOperator.NotContains => actual?.Contains(rule.Text!, StringComparison.CurrentCultureIgnoreCase) != true,
            FilterOperator.StartsWith => actual?.StartsWith(rule.Text!, StringComparison.CurrentCultureIgnoreCase) == true,
            FilterOperator.EndsWith => actual?.EndsWith(rule.Text!, StringComparison.CurrentCultureIgnoreCase) == true,
            FilterOperator.IsEqualTo => string.Equals(actual, rule.Text, StringComparison.CurrentCultureIgnoreCase),
            FilterOperator.IsNotEqualTo => !string.Equals(actual, rule.Text, StringComparison.CurrentCultureIgnoreCase),
            FilterOperator.IsEmpty => string.IsNullOrWhiteSpace(actual),
            FilterOperator.IsNotEmpty => !string.IsNullOrWhiteSpace(actual),
            var _ => false,
        };

    private static bool MatchesComparable<T>(FilterOperator filterOperator, T? actual, T? target)
        where T : struct, IComparable<T>
        => filterOperator switch
        {
            FilterOperator.IsEmpty => actual is null,
            FilterOperator.IsNotEmpty => actual is { },
            FilterOperator.IsNotEqualTo => actual is null || actual.Value.CompareTo(target!.Value) != 0,
            var _ when actual is null => false,
            FilterOperator.IsEqualTo => actual.Value.CompareTo(target!.Value) == 0,
            FilterOperator.GreaterThan => actual.Value.CompareTo(target!.Value) > 0,
            FilterOperator.GreaterThanOrEqualTo => actual.Value.CompareTo(target!.Value) >= 0,
            FilterOperator.LessThan => actual.Value.CompareTo(target!.Value) < 0,
            FilterOperator.LessThanOrEqualTo => actual.Value.CompareTo(target!.Value) <= 0,
            var _ => false,
        };

    #endregion
}
