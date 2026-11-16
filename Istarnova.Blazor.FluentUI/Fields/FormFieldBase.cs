#region "Header"

// File: FormFieldBase.cs
// Created 2026/10/30
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

#region "Usings"

using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;

#endregion

namespace Istarnova.Blazor.FluentUI.Fields;

/// <summary>
///     What every form field declares: the binding, the label and the cell it sits in. A parameter the field does not
///     declare reaches the inner FluentUI input as a string and throws when that input's parameter is typed, so a field
///     lists what the code base really uses instead of capturing the rest.
/// </summary>
/// <typeparam name="TValue">The type of the bound value.</typeparam>
public abstract class FormFieldBase<TValue> : ComponentBase
{
    #region "Variables"

    private readonly string _generatedId = $"field-{Guid.NewGuid():N}";

    #endregion

    #region "Properties"

    [Parameter]
    public bool Autofocus { get; set; }

    [Parameter]
    public bool Center { get; set; }

    [Parameter]
    public bool? Disabled { get; set; }

    [Parameter]
    public bool Full { get; set; }

    [Parameter]
    public string? Hint { get; set; }

    /// <summary>The label points at it; left out, the field makes one up.</summary>
    [Parameter]
    public string? Id { get; set; }

    [Parameter]
    public string? Label { get; set; }

    [Parameter]
    public bool ReadOnly { get; set; }

    [Parameter]
    public bool? Required { get; set; }

    [Parameter]
    public TValue Value { get; set; } = default!;

    [Parameter]
    public EventCallback<TValue> ValueChanged { get; set; }

    [Parameter]
    public Expression<Func<TValue>>? ValueExpression { get; set; }

    /// <summary>
    ///     What the inner input is given as its expression. A one-way <c>Value=</c> carries none, and an input inside an
    ///     <c>EditForm</c> throws without one, which takes the circuit down.
    /// </summary>
    protected Expression<Func<TValue>> BoundExpression
        => ValueExpression ?? (() => Value);

    protected string FieldId
        => Id ?? _generatedId;

    #endregion

    #region "Methods"

    /// <summary>
    ///     Copies the named parameters the caller gave into <paramref name="target" />, to be handed on with
    ///     <c>@attributes</c>. Some FluentUI inputs fill a bound from the type when it is left out, so forwarding the
    ///     wrapper's own default would overwrite that.
    /// </summary>
    protected static void CollectGiven<TBound>(ParameterView parameters, Dictionary<string, object> target, params string[] names)
    {
        target.Clear();

        foreach (var name in names)
        {
            if (parameters.TryGetValue<TBound>(name, out var bound) && bound is { })
            {
                target[name] = bound;
            }
        }
    }

    #endregion
}
