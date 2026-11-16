#region "Header"

// File: QueryValueTests.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using System.Linq.Expressions;
using Istarnova.EfCore.Extensions;

namespace Istarnova.Tests.EfCore;

public class QueryValueTests
{
    private sealed record Row(int Id);

    [Fact]
    public void Captured_ShouldReadTheValueOffAnObject_NotCarryItAsAConstant()
    {
        var expression = QueryValue.Captured(5);

        expression.Expression.Should().BeOfType<ConstantExpression>();
        expression.Member.Name.Should().Be("Value");
        Expression.Lambda<Func<int>>(expression).Compile()().Should().Be(5);
    }

    [Fact]
    public void Captured_ShouldBeUsableInAHandBuiltPredicate()
    {
        var row = Expression.Parameter(typeof(Row), "row");
        var predicate = Expression.Lambda<Func<Row, bool>>(
            Expression.Equal(Expression.Property(row, nameof(Row.Id)), QueryValue.Captured(7)), row).Compile();

        predicate(new Row(7)).Should().BeTrue();
        predicate(new Row(8)).Should().BeFalse();
    }
}
