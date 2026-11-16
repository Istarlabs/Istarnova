#region "Header"

// File: ParameterSubstitutionVisitor.cs
// Created 2026/03/06
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

#region "Usings"

using System.Linq.Expressions;

#endregion

namespace Istarnova.Generics.Linq;

/// <summary>
///     Expression visitor that substitutes one parameter with another in expression trees.
/// </summary>
/// <param name="source">The parameter to replace.</param>
/// <param name="destination">The parameter to replace with.</param>
public sealed class ParameterSubstitutionVisitor(ParameterExpression source, ParameterExpression destination)
    : ExpressionVisitor
{
    #region "Variables"

    private readonly ParameterExpression _destination =
        destination ?? throw new ArgumentNullException(nameof(destination));

    private readonly ParameterExpression _source = source ?? throw new ArgumentNullException(nameof(source));

    #endregion

    #region "Methods"

    protected override Expression VisitParameter(ParameterExpression node) =>
        ReferenceEquals(node, _source) ? _destination : base.VisitParameter(node);

    #endregion
}
