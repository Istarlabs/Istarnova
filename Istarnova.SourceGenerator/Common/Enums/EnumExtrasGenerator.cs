#region "Header"

// File: EnumExtrasGenerator.cs
// Created 2026/08/22
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

#region "Usings"

using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace Istarnova.SourceGenerator.Common.Enums;

/// <summary>
///     Incremental source generator that adds an Istarnova convenience layer to enums marked with
///     <c>[EnumExtras]</c> (and <c>[EnumExtensions]</c> from NetEscapades). The generated code is a
///     second <c>partial</c> part of the <c>{Enum}Extensions</c> class that NetEscapades already emits,
///     so it can call the NetEscapades siblings (<c>GetValues()</c>, <c>ToStringFast()</c>) directly.
/// </summary>
[Generator]
public class EnumExtrasGenerator : IIncrementalGenerator
{
    #region "Constants"

    private const string EnumExtensionsAttributeName = "EnumExtensionsAttribute";
    private const string EnumExtensionsAttributeNamespace = "NetEscapades.EnumGenerators";

    private const string EnumExtrasAttributeFullName =
        "Istarnova.SourceGenerator.Attributes.Common.Enums.EnumExtrasAttribute";

    #endregion

    #region "Methods"

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var enumDeclarations = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                EnumExtrasAttributeFullName,
                static (node, _) => node is EnumDeclarationSyntax,
                static (ctx, _) => GetEnumInfo(ctx))
            .Where(static info => info is { });

        context.RegisterSourceOutput(enumDeclarations, static (spc, enumInfo) =>
        {
            if (enumInfo is null)
            {
                return;
            }

            var info = enumInfo.Value;

            // The whole point is to reuse the NetEscapades-generated partial. Without it we have
            // nothing to dock onto and the emitted ToStringFast()/GetValues() calls would not exist.
            if (!info.HasEnumExtensions)
            {
                ReportDiagnostic(spc, info, "EE001", "Missing [EnumExtensions]",
                    $"Enum '{info.EnumName}' is marked with [EnumExtras] but not with " +
                    "[EnumExtensions]. Add NetEscapades' [EnumExtensions] so the convenience layer " +
                    "can dock onto the generated extensions class.");
                return;
            }

            var sourceCode = GenerateExtensions(info);
            spc.AddSource($"{info.Namespace}.{info.EnumName}_EnumExtensionsExtra.g.cs".TrimStart('.'), sourceCode);
        });
    }

    private static void AppendDisplayList(StringBuilder sb, EnumInfo info)
    {
        sb.AppendLine("    /// <summary>Key/display-name pairs for all values (UI dropdowns).</summary>");
        sb.AppendLine($"    public static EnumKeyValue<{info.EnumFullName}>[] GetDisplayList()");
        sb.AppendLine("    {");
        sb.AppendLine("        var values = GetValues();");
        sb.AppendLine($"        var result = new EnumKeyValue<{info.EnumFullName}>[values.Length];");
        sb.AppendLine("        for (var i = 0; i < values.Length; i++)");
        sb.AppendLine("        {");
        sb.AppendLine(
            $"            result[i] = new EnumKeyValue<{info.EnumFullName}>(values[i], values[i].GetDisplayName());");
        sb.AppendLine("        }");
        sb.AppendLine();
        sb.AppendLine("        return result;");
        sb.AppendLine("    }");
        sb.AppendLine();
    }

    private static void AppendDisplayName(
        StringBuilder sb,
        EnumInfo info,
        IReadOnlyDictionary<string, string> resourceManagerFieldByType)
    {
        var hasExplicit = info.Members.Any(m => m.Kind is DisplayKind.Localized or DisplayKind.Literal);

        sb.AppendLine("    /// <summary>Resolves the display name (localized where available).</summary>");

        // No explicit display metadata anywhere -> delegate straight to the NetEscapades sibling.
        if (!hasExplicit)
        {
            sb.AppendLine($"    public static string GetDisplayName(this {info.EnumFullName} value)");
            sb.AppendLine("        => value.ToStringFast();");
            sb.AppendLine();
            return;
        }

        sb.AppendLine($"    public static string GetDisplayName(this {info.EnumFullName} value) => value switch");
        sb.AppendLine("    {");

        foreach (var member in info.Members)
        {
            var memberRef = $"{info.EnumFullName}.{member.Name}";

            switch (member.Kind)
            {
                case DisplayKind.Localized:
                    var field = resourceManagerFieldByType[member.ResourceTypeFullName!];
                    sb.AppendLine(
                        $"        {memberRef} => {field}.GetString(\"{Escape(member.DisplayText!)}\") ?? nameof({memberRef}),");
                    break;

                case DisplayKind.Literal:
                    sb.AppendLine($"        {memberRef} => \"{Escape(member.DisplayText!)}\",");
                    break;

                // NameOnly members fall through to the default arm.
            }
        }

        sb.AppendLine("        _ => value.ToStringFast(),");
        sb.AppendLine("    };");
        sb.AppendLine();
    }

    private static void AppendFlagHelpers(StringBuilder sb, EnumInfo info)
    {
        sb.AppendLine("    /// <summary>Combines multiple flags into a single value.</summary>");
        sb.AppendLine($"    public static {info.EnumFullName} CombineFlags(params {info.EnumFullName}[] values)");
        sb.AppendLine("    {");
        sb.AppendLine("        long combined = 0;");
        sb.AppendLine("        foreach (var value in values)");
        sb.AppendLine("        {");
        sb.AppendLine("            combined |= (long)value;");
        sb.AppendLine("        }");
        sb.AppendLine();
        sb.AppendLine($"        return ({info.EnumFullName})combined;");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine("    /// <summary>Splits a combined flags value into its individual set flags.</summary>");
        sb.AppendLine($"    public static {info.EnumFullName}[] GetFlags(this {info.EnumFullName} value)");
        sb.AppendLine("    {");
        sb.AppendLine($"        var result = new global::System.Collections.Generic.List<{info.EnumFullName}>();");
        sb.AppendLine("        var combined = (long)value;");
        sb.AppendLine("        foreach (var candidate in GetValues())");
        sb.AppendLine("        {");
        sb.AppendLine("            var flag = (long)candidate;");
        sb.AppendLine("            if (flag != 0 && (combined & flag) == flag)");
        sb.AppendLine("            {");
        sb.AppendLine("                result.Add(candidate);");
        sb.AppendLine("            }");
        sb.AppendLine("        }");
        sb.AppendLine();
        sb.AppendLine("        return result.ToArray();");
        sb.AppendLine("    }");
        sb.AppendLine();
    }

    private static void AppendNavigation(StringBuilder sb, EnumInfo info)
    {
        sb.AppendLine("    /// <summary>Next value in declaration order (circular).</summary>");
        sb.AppendLine($"    public static {info.EnumFullName} GetNext(this {info.EnumFullName} value)");
        sb.AppendLine("    {");
        sb.AppendLine("        var values = GetValues();");
        sb.AppendLine("        var index = global::System.Array.IndexOf(values, value);");
        sb.AppendLine("        return index < 0 ? default : values[(index + 1) % values.Length];");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine("    /// <summary>Previous value in declaration order (circular).</summary>");
        sb.AppendLine($"    public static {info.EnumFullName} GetPrevious(this {info.EnumFullName} value)");
        sb.AppendLine("    {");
        sb.AppendLine("        var values = GetValues();");
        sb.AppendLine("        var index = global::System.Array.IndexOf(values, value);");
        sb.AppendLine("        return index < 0 ? default : values[index == 0 ? values.Length - 1 : index - 1];");
        sb.AppendLine("    }");
        sb.AppendLine();
    }

    private static void AppendNumericHelpers(StringBuilder sb, EnumInfo info)
    {
        sb.AppendLine("    /// <summary>Underlying value as <see cref=\"long\"/>.</summary>");
        sb.AppendLine($"    public static long ToInt64(this {info.EnumFullName} value) => (long)value;");
        sb.AppendLine();
        sb.AppendLine("    /// <summary>Underlying value as an uppercase hexadecimal string (no prefix).</summary>");
        sb.AppendLine(
            $"    public static string ToHexString(this {info.EnumFullName} value) => ((long)value).ToString(\"X\");");
    }

    private static void AppendTryGetByValue(StringBuilder sb, EnumInfo info)
    {
        sb.AppendLine("    /// <summary>The member with this underlying value; false where no member has it.</summary>");
        sb.AppendLine($"    public static bool TryGetByValue(long value, out {info.EnumFullName} result)");
        sb.AppendLine("    {");
        sb.AppendLine("        switch (value)");
        sb.AppendLine("        {");

        // Aliases share a value, and a switch refuses a duplicate label: the first declared member wins.
        foreach (var member in info.Members.GroupBy(m => m.Value).Select(g => g.First()))
        {
            sb.AppendLine($"            case {member.Value}L:");
            sb.AppendLine($"                result = {info.EnumFullName}.{member.Name};");
            sb.AppendLine("                return true;");
        }

        sb.AppendLine("        }");
        sb.AppendLine();
        sb.AppendLine("        result = default;");
        sb.AppendLine("        return false;");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine("    /// <summary>As the <see cref=\"long\"/> overload; a missing value has no member.</summary>");
        sb.AppendLine($"    public static bool TryGetByValue(long? value, out {info.EnumFullName} result)");
        sb.AppendLine("    {");
        sb.AppendLine("        result = default;");
        sb.AppendLine("        return value is { } number && TryGetByValue(number, out result);");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine("    /// <summary>As the <see cref=\"long\"/> overload.</summary>");
        sb.AppendLine($"    public static bool TryGetByValue(int value, out {info.EnumFullName} result)");
        sb.AppendLine("        => TryGetByValue((long)value, out result);");
        sb.AppendLine();
        sb.AppendLine("    /// <summary>As the <see cref=\"long\"/> overload; a missing value has no member.</summary>");
        sb.AppendLine($"    public static bool TryGetByValue(int? value, out {info.EnumFullName} result)");
        sb.AppendLine("        => TryGetByValue((long?)value, out result);");
        sb.AppendLine();
    }

    private static void AppendTryParseByDisplayName(StringBuilder sb, EnumInfo info)
    {
        sb.AppendLine("    /// <summary>Reverse lookup from a display name to the enum value.</summary>");
        sb.AppendLine(
            $"    public static bool TryParseByDisplayName(string? displayName, out {info.EnumFullName} result, " +
            "global::System.StringComparison comparison = global::System.StringComparison.OrdinalIgnoreCase)");
        sb.AppendLine("    {");
        sb.AppendLine("        result = default;");
        sb.AppendLine("        if (string.IsNullOrWhiteSpace(displayName))");
        sb.AppendLine("        {");
        sb.AppendLine("            return false;");
        sb.AppendLine("        }");
        sb.AppendLine();
        sb.AppendLine("        foreach (var pair in GetDisplayList())");
        sb.AppendLine("        {");
        sb.AppendLine("            if (string.Equals(pair.Value, displayName, comparison))");
        sb.AppendLine("            {");
        sb.AppendLine("                result = pair.Key;");
        sb.AppendLine("                return true;");
        sb.AppendLine("            }");
        sb.AppendLine("        }");
        sb.AppendLine();
        sb.AppendLine("        return false;");
        sb.AppendLine("    }");
        sb.AppendLine();
    }

    private static void AppendValuesDescending(StringBuilder sb, EnumInfo info)
    {
        sb.AppendLine("    /// <summary>The members from the highest underlying value down; an alias appears once.</summary>");
        sb.AppendLine($"    public static {info.EnumFullName}[] GetValuesDescending()");
        sb.AppendLine($"        => new {info.EnumFullName}[]");
        sb.AppendLine("        {");

        // Aliases share a value: the first declared member stands for all of them, as in TryGetByValue.
        foreach (var member in info.Members.GroupBy(m => m.Value).Select(g => g.First()).OrderByDescending(m => m.Value))
        {
            sb.AppendLine($"            {info.EnumFullName}.{member.Name},");
        }

        sb.AppendLine("        };");
        sb.AppendLine();
    }

    private static string Escape(string value)
        => value.Replace("\\", @"\\").Replace("\"", "\\\"");

    private static string GenerateExtensions(EnumInfo info)
    {
        var sb = new StringBuilder();

        sb.AppendLine("// <auto-generated />");
        sb.AppendLine("#nullable enable");
        sb.AppendLine();
        sb.AppendLine("using Istarnova.Common.Enums;");
        sb.AppendLine();
        sb.AppendLine(GeneratedNamespace.Declaration(info.Namespace));
        sb.AppendLine();
        sb.AppendLine("// Second partial part of the class NetEscapades already emits as 'public static partial'.");
        sb.AppendLine($"public static partial class {info.EnumName}Extensions");
        sb.AppendLine("{");

        // Cached ResourceManager fields, one per distinct resource type used by localized members.
        var resourceTypes = info.Members
            .Where(m => m is { Kind: DisplayKind.Localized, ResourceTypeFullName: { } })
            .Select(m => m.ResourceTypeFullName!)
            .Distinct()
            .ToImmutableArray();

        var resourceManagerFieldByType = resourceTypes
            .Select((type, index) => (type, field: $"__resourceManager{index}"))
            .ToDictionary(x => x.type, x => x.field);

        // The resource class's own manager, not one built from the type: the compiler names an embedded resource
        // after the folder it sits in, so a manager made from the type's name misses a resource that was moved.
        foreach (var type in resourceTypes)
        {
            sb.AppendLine(
                $"    private static readonly global::System.Resources.ResourceManager {resourceManagerFieldByType[type]} " +
                $"= {type}.ResourceManager;");
        }

        if (resourceTypes.Length > 0)
        {
            sb.AppendLine();
        }

        if (info.GenerateDisplayNames)
        {
            AppendDisplayName(sb, info, resourceManagerFieldByType);
            AppendDisplayList(sb, info);
            AppendTryParseByDisplayName(sb, info);
        }

        if (info.GenerateNavigation)
        {
            AppendNavigation(sb, info);
        }

        if (info is { GenerateFlagHelpers: true, IsFlags: true })
        {
            AppendFlagHelpers(sb, info);
        }

        AppendValuesDescending(sb, info);
        AppendTryGetByValue(sb, info);
        AppendNumericHelpers(sb, info);

        sb.AppendLine("}");

        return sb.ToString();
    }

    private static bool GetBoolNamedArg(AttributeData attribute, string name, bool defaultValue)
    {
        foreach (var namedArgument in attribute.NamedArguments)
        {
            if (namedArgument.Key == name && namedArgument.Value.Value is bool value)
            {
                return value;
            }
        }

        return defaultValue;
    }

    private static EnumInfo? GetEnumInfo(GeneratorAttributeSyntaxContext context)
    {
        if (context.TargetSymbol is not INamedTypeSymbol { TypeKind: TypeKind.Enum } enumSymbol)
        {
            return null;
        }

        var attribute = context.Attributes.FirstOrDefault();
        if (attribute is null)
        {
            return null;
        }

        var hasEnumExtensions = enumSymbol.GetAttributes().Any(a =>
            a.AttributeClass is { Name: EnumExtensionsAttributeName } cls &&
            cls.ContainingNamespace.ToDisplayString() == EnumExtensionsAttributeNamespace);

        var isFlags = enumSymbol.GetAttributes().Any(a => a.AttributeClass?.Name == "FlagsAttribute");

        var generateDisplayNames = GetBoolNamedArg(attribute, "GenerateDisplayNames", true);
        var generateNavigation = GetBoolNamedArg(attribute, "GenerateNavigation", true);
        var generateFlagHelpers = GetBoolNamedArg(attribute, "GenerateFlagHelpers", true);

        var members = enumSymbol.GetMembers()
            .OfType<IFieldSymbol>()
            .Where(f => f is { IsStatic: true, HasConstantValue: true })
            .Select(GetMemberInfo)
            .ToImmutableArray();

        return new EnumInfo(
            GeneratedNamespace.Of(enumSymbol),
            enumSymbol.Name,
            enumSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            hasEnumExtensions,
            isFlags,
            generateDisplayNames,
            generateNavigation,
            generateFlagHelpers,
            members,
            enumSymbol.Locations.FirstOrDefault());
    }

    private static MemberInfo GetMemberInfo(IFieldSymbol field)
    {
        // A ulong member above long.MaxValue keeps its bit pattern, the same value a (long) cast of it yields.
        var value = field.ConstantValue is ulong unsignedValue
            ? unchecked((long)unsignedValue)
            : Convert.ToInt64(field.ConstantValue);

        foreach (var attribute in field.GetAttributes())
        {
            var attributeName = attribute.AttributeClass?.Name;

            switch (attributeName)
            {
                // [LocalizedDescription(key, resourceType)] -> runtime, culture-aware resolution.
                // Matched by simple name to stay decoupled from the consumer's namespace.
                case "LocalizedDescriptionAttribute" when attribute.ConstructorArguments.Length >= 2:
                {
                    var key = attribute.ConstructorArguments[0].Value as string;

                    if (!string.IsNullOrEmpty(key) &&
                        attribute.ConstructorArguments[1].Value is INamedTypeSymbol resourceType)
                    {
                        return new(
                            field.Name,
                            value,
                            DisplayKind.Localized,
                            key!,
                            resourceType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
                    }

                    break;
                }
                // [Display(Name = "...")] -> compile-time literal.
                case "DisplayAttribute":
                {
                    var name = attribute.NamedArguments
                        .FirstOrDefault(a => a.Key == "Name").Value.Value as string;

                    if (!string.IsNullOrEmpty(name))
                    {
                        return new(field.Name, value, DisplayKind.Literal, name!, null);
                    }

                    break;
                }
                // [Description("...")] -> compile-time literal.
                case "DescriptionAttribute" when attribute.ConstructorArguments.Length >= 1:
                {
                    var description = attribute.ConstructorArguments[0].Value as string;

                    if (!string.IsNullOrEmpty(description))
                    {
                        return new(field.Name, value, DisplayKind.Literal, description!, null);
                    }

                    break;
                }
            }
        }

        return new(field.Name, value, DisplayKind.NameOnly, null, null);
    }

    private static void ReportDiagnostic(
        SourceProductionContext context,
        EnumInfo enumInfo,
        string id,
        string title,
        string message)
    {
        var diagnostic = Diagnostic.Create(
            new(
                id,
                title,
                message,
                "EnumExtras",
                DiagnosticSeverity.Warning,
                true),
            enumInfo.Location);

        context.ReportDiagnostic(diagnostic);
    }

    #endregion

    private readonly record struct EnumInfo(
        string Namespace,
        string EnumName,
        string EnumFullName,
        bool HasEnumExtensions,
        bool IsFlags,
        bool GenerateDisplayNames,
        bool GenerateNavigation,
        bool GenerateFlagHelpers,
        ImmutableArray<MemberInfo> Members,
        Location? Location);

    private readonly record struct MemberInfo(
        string Name,
        long Value,
        DisplayKind Kind,
        string? DisplayText,
        string? ResourceTypeFullName);

    private enum DisplayKind
    {
        NameOnly,
        Literal,
        Localized,
    }
}
