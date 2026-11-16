#region "Header"

// File: GeneratedNamespace.cs
// Created 2026/09/17
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

#region "Usings"

using Microsoft.CodeAnalysis;

#endregion

namespace Istarnova.SourceGenerator.Common;

/// <summary>
///     The namespace a generated file belongs in, with the global namespace as an empty string.
/// </summary>
internal static class GeneratedNamespace
{
    #region "Methods"

    /// <summary>
    ///     The namespace of a symbol, or an empty string for the global namespace, where
    ///     <see cref="ISymbol.ToDisplayString(SymbolDisplayFormat?)" /> returns <c>&lt;global namespace&gt;</c>.
    /// </summary>
    public static string Of(ITypeSymbol symbol)
        => symbol.ContainingNamespace is { IsGlobalNamespace: false } containingNamespace
            ? containingNamespace.ToDisplayString()
            : string.Empty;

    /// <summary>
    ///     A file-scoped namespace declaration, or an empty string for the global namespace.
    /// </summary>
    public static string Declaration(string @namespace)
        => @namespace.Length == 0 ? string.Empty : $"namespace {@namespace};";

    /// <summary>
    ///     A using directive, or an empty string for the global namespace, which needs none.
    /// </summary>
    public static string Using(string @namespace)
        => @namespace.Length == 0 ? string.Empty : $"using {@namespace};";

    #endregion
}
