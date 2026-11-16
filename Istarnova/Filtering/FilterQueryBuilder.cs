#region "Header"

// File: FilterQueryBuilder.cs
// Created 2026/11/07
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

#region "Usings"

using System.Linq.Expressions;
using System.Reflection;

#endregion

namespace Istarnova.Filtering;

/// <summary>
///     Turns the rules of a <see cref="FilterSet" /> into one predicate a query provider can translate. It answers what
///     <see cref="FilterSet.Apply{TItem}" /> answers for a list in memory: the same rules keep the same rows.
/// </summary>
internal static class FilterQueryBuilder
{
    #region "Variables"

    private static readonly MethodInfo ToLower = typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes)!;
    private static readonly MethodInfo Contains = typeof(string).GetMethod(nameof(string.Contains), [typeof(string)])!;
    private static readonly MethodInfo StartsWith = typeof(string).GetMethod(nameof(string.StartsWith), [typeof(string)])!;
    private static readonly MethodInfo EndsWith = typeof(string).GetMethod(nameof(string.EndsWith), [typeof(string)])!;
    private static readonly MethodInfo IsNullOrWhiteSpace = typeof(string).GetMethod(nameof(string.IsNullOrWhiteSpace), [typeof(string)])!;

    #endregion

    #region "Methods"

    public static IQueryable<TItem> Apply<TItem>(IQueryable<TItem> query, FilterSet filterSet, IReadOnlyList<FilterProperty<TItem>> properties)
    {
        var item = Expression.Parameter(typeof(TItem), "item");
        var predicates = new List<Expression>();

        foreach (var rule in filterSet.Rules)
        {
            if (properties.FirstOrDefault(property => property.Name == rule.Property) is not { } property
                || !rule.IsComplete(property.Kind))
            {
                continue;
            }

            if (property.Selector is not { } selector)
            {
                throw new InvalidOperationException($"The filter property {property.Name} has no selector, so it cannot filter a query.");
            }

            var member = new ParameterReplacer(selector.Parameters[0], item).Visit(selector.Body);

            predicates.Add(Predicate(rule, property.Kind, member));
        }

        if (predicates.Count == 0)
        {
            return query;
        }

        Func<Expression, Expression, Expression> join = filterSet.MatchAny
            ? (left, right) => Expression.OrElse(left, right)
            : (left, right) => Expression.AndAlso(left, right);

        var combined = predicates.Aggregate(join);

        return query.Where(Expression.Lambda<Func<TItem, bool>>(combined, item));
    }

    private static Expression Predicate(FilterRule rule, FilterKind kind, Expression member)
        => kind switch
        {
            FilterKind.Text => TextPredicate(rule, member),
            FilterKind.Number => ComparablePredicate(rule.Operator, Expression.Convert(member, typeof(decimal?)), Captured(rule.Number)),
            FilterKind.Date => ComparablePredicate(rule.Operator, Expression.Convert(member, typeof(DateOnly?)), Captured(rule.Date)),
            var _ => FlagPredicate(rule.Operator, Expression.Convert(member, typeof(bool?))),
        };

    /// <summary>
    ///     Compared case-insensitively on the lower-cased text, which every provider translates and a list in memory can run.
    ///     A missing text is none of "contains", "starts with", "ends with" or "equal to", and is every "does not".
    /// </summary>
    private static Expression TextPredicate(FilterRule rule, Expression member)
    {
        var isNull = Expression.Equal(member, Expression.Constant(null, typeof(string)));
        var lowered = Expression.Call(member, ToLower);
        var needle = Captured(rule.Text?.ToLower());

        return rule.Operator switch
        {
            FilterOperator.Contains => Expression.AndAlso(Expression.Not(isNull), Expression.Call(lowered, Contains, needle)),
            FilterOperator.StartsWith => Expression.AndAlso(Expression.Not(isNull), Expression.Call(lowered, StartsWith, needle)),
            FilterOperator.EndsWith => Expression.AndAlso(Expression.Not(isNull), Expression.Call(lowered, EndsWith, needle)),
            FilterOperator.NotContains => Expression.OrElse(isNull, Expression.Not(Expression.Call(lowered, Contains, needle))),
            FilterOperator.IsEqualTo => Expression.AndAlso(Expression.Not(isNull), Expression.Equal(lowered, needle)),
            FilterOperator.IsNotEqualTo => Expression.OrElse(isNull, Expression.NotEqual(lowered, needle)),
            FilterOperator.IsEmpty => Expression.Call(IsNullOrWhiteSpace, member),
            FilterOperator.IsNotEmpty => Expression.Not(Expression.Call(IsNullOrWhiteSpace, member)),
            var _ => Expression.Constant(false),
        };
    }

    /// <summary>
    ///     Numbers and days: a missing value is "empty", is "not equal to" anything, and is on neither side of a comparison.
    /// </summary>
    private static Expression ComparablePredicate(FilterOperator filterOperator, Expression actual, Expression target)
    {
        var isNull = Expression.Equal(actual, Expression.Constant(null, actual.Type));

        return filterOperator switch
        {
            FilterOperator.IsEmpty => isNull,
            FilterOperator.IsNotEmpty => Expression.Not(isNull),
            FilterOperator.IsNotEqualTo => Expression.OrElse(isNull, Expression.NotEqual(actual, target)),
            FilterOperator.IsEqualTo => Expression.Equal(actual, target),
            FilterOperator.GreaterThan => Expression.GreaterThan(actual, target),
            FilterOperator.GreaterThanOrEqualTo => Expression.GreaterThanOrEqual(actual, target),
            FilterOperator.LessThan => Expression.LessThan(actual, target),
            FilterOperator.LessThanOrEqualTo => Expression.LessThanOrEqual(actual, target),
            var _ => Expression.Constant(false),
        };
    }

    private static Expression FlagPredicate(FilterOperator filterOperator, Expression actual)
    {
        var isTrue = Expression.Equal(actual, Expression.Constant(true, typeof(bool?)));

        return filterOperator == FilterOperator.IsTrue ? isTrue : Expression.Not(isTrue);
    }

    /// <summary>
    ///     A value as something EF Core turns into a query parameter, not into text spliced into the statement.
    /// </summary>
    private static MemberExpression Captured<TValue>(TValue value)
        => Expression.Property(Expression.Constant(new Box<TValue>(value)), nameof(Box<TValue>.Value));

    #endregion

    #region "Types"

    private sealed record Box<TValue>(TValue Value);

    private sealed class ParameterReplacer(ParameterExpression from, ParameterExpression to) : ExpressionVisitor
    {
        protected override Expression VisitParameter(ParameterExpression node)
            => node == from ? to : base.VisitParameter(node);
    }

    #endregion
}
