#region "Header"

// File: ControllerSourceGenerator.cs
// Created 2026/02/12
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using System.Collections.Immutable;
using System.Text;
using Istarnova.SourceGenerator.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Istarnova.SourceGenerator.Controller;

/// <summary>
/// Incremental source generator that generates API controllers from repository interfaces marked with GenerateApiControllerAttribute.
/// </summary>
[Generator]
public class ControllerSourceGenerator : IIncrementalGenerator
{
    private const string GenerateApiControllerAttributeFullName = "Istarnova.SourceGenerator.Attributes.Controller.GenerateApiControllerAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var interfaceDeclarations = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                GenerateApiControllerAttributeFullName,
                predicate: static (node, _) => node is InterfaceDeclarationSyntax,
                transform: static (ctx, _) => GetInterfaceInfo(ctx))
            .Where(static info => info is { });

        context.RegisterSourceOutput(interfaceDeclarations, static (spc, interfaceInfo) =>
        {
            if (interfaceInfo is null)
            {
                return;
            }

            var info = interfaceInfo.Value;

            // Generate Controller
            var controllerCode = GenerateController(info);
            spc.AddSource($"{info.EntityName}Controller.g.cs", controllerCode);

            // Generate DTOs
            var dtoCode = GenerateDtos(info);
            spc.AddSource($"{info.EntityName}Dtos.g.cs", dtoCode);

            // Generate Mappers
            var mapperCode = GenerateMappers(info);
            spc.AddSource($"{info.EntityName}Mappers.g.cs", mapperCode);
        });
    }

    private static InterfaceInfo? GetInterfaceInfo(GeneratorAttributeSyntaxContext context)
    {
        if (context.TargetSymbol is not INamedTypeSymbol interfaceSymbol)
        {
            return null;
        }

        var entityType = GetEntityType(interfaceSymbol);
        if (entityType is null)
        {
            return null;
        }

        var attribute = context.Attributes.FirstOrDefault();
        if (attribute is null)
        {
            return null;
        }

        var config = ParseAttributeConfig(attribute);
        var methods = GetAllInterfaceMethods(interfaceSymbol);
        var entityProperties = GetEntityProperties(entityType);

        return new InterfaceInfo(
            interfaceSymbol.Name,
            GeneratedNamespace.Of(interfaceSymbol),
            entityType.Name,
            GeneratedNamespace.Of(entityType),
            config,
            methods,
            entityProperties);
    }

    private static INamedTypeSymbol? GetEntityType(INamedTypeSymbol interfaceSymbol)
    {
        foreach (var baseInterface in interfaceSymbol.AllInterfaces.Concat([interfaceSymbol]))
        {
            if ((baseInterface.Name.StartsWith("IRepository") && baseInterface.TypeArguments.Length > 0) ||
                baseInterface is { Name: "IBaseRepository", TypeArguments.Length: > 0 })
            {
                return baseInterface.TypeArguments[0] as INamedTypeSymbol;
            }
        }

        return null;
    }

    private static ControllerConfig ParseAttributeConfig(AttributeData attribute)
    {
        string? route = null;
        var authRequired = false;
        string? parentRoute = null;

        foreach (var namedArg in attribute.NamedArguments)
        {
            switch (namedArg.Key)
            {
                case "Route":
                    route = namedArg.Value.Value?.ToString();
                    break;
                case "AuthRequired":
                    authRequired = (bool)(namedArg.Value.Value ?? false);
                    break;
                case "ParentRoute":
                    parentRoute = namedArg.Value.Value?.ToString();
                    break;
            }
        }

        return new(route, authRequired, parentRoute);
    }

    private static ImmutableArray<MethodInfo> GetAllInterfaceMethods(INamedTypeSymbol interfaceSymbol)
    {
        var methods = new List<MethodInfo>();

        // Get methods from this interface
        methods.AddRange(interfaceSymbol.GetMembers()
            .OfType<IMethodSymbol>()
            .Where(m => m.MethodKind == MethodKind.Ordinary)
            .Select(ToMethodInfo));

        // Get methods from all base interfaces
        foreach (var baseInterface in interfaceSymbol.AllInterfaces)
        {
            methods.AddRange(baseInterface.GetMembers()
                .OfType<IMethodSymbol>()
                .Where(m => m.MethodKind == MethodKind.Ordinary)
                .Select(ToMethodInfo));
        }

        return [.. methods];
    }

    private static MethodInfo ToMethodInfo(IMethodSymbol method)
    {
        var parameters = method.Parameters
            .Select(p => new ParameterInfo(
                p.Name,
                p.Type.ToDisplayString(),
                p.Type.Name,
                IsComplexType(p.Type),
                IsIdParameter(p)))
            .ToImmutableArray();

        var returnTypeInfo = AnalyzeReturnType(method.ReturnType);

        return new(
            method.Name,
            parameters,
            returnTypeInfo);
    }

    private static ImmutableArray<EntityPropertyInfo> GetEntityProperties(INamedTypeSymbol entityType)
    {
        return
        [
            .. entityType.GetMembers()
                .OfType<IPropertySymbol>()
                .Where(p => p.DeclaredAccessibility == Accessibility.Public)
                .Select(p => new EntityPropertyInfo(
                    p.Name,
                    p.Type.ToDisplayString(),
                    IsNavigationProperty(p),
                    HasPublicSetter(p),
                    ShouldExcludeFromCreateDto(p),
                    ShouldExcludeFromUpdateDto(p))),
        ];
    }

    private static string GenerateController(InterfaceInfo info)
    {
        var entityName = info.EntityName;
        var controllerName = $"{entityName}Controller";
        var route = info.Config.Route ?? $"api/{ToKebabCase(entityName)}s";
        var authAttribute = info.Config.AuthRequired ? "[Authorize]" : "";

        var sb = new StringBuilder();

        sb.AppendLine("// <auto-generated />");
        sb.AppendLine("using Microsoft.AspNetCore.Mvc;");
        if (info.Config.AuthRequired)
        {
            sb.AppendLine("using Microsoft.AspNetCore.Authorization;");
        }

        sb.AppendLine(GeneratedNamespace.Using(info.EntityNamespace));
        sb.AppendLine("using Generated.Dtos;");
        sb.AppendLine("using Generated.Mappers;");
        sb.AppendLine("using System.Linq;");
        sb.AppendLine();
        sb.AppendLine("namespace Generated.Controllers;");
        sb.AppendLine();
        sb.AppendLine($"{authAttribute}");
        sb.AppendLine($"[ApiController]");
        sb.AppendLine($"[Route(\"{route}\")]");
        sb.AppendLine($"public class {controllerName} : ControllerBase");
        sb.AppendLine("{");
        sb.AppendLine($"    private readonly {info.InterfaceName} _repository;");
        sb.AppendLine();
        sb.AppendLine($"    public {controllerName}({info.InterfaceName} repository)");
        sb.AppendLine("    {");
        sb.AppendLine("        _repository = repository;");
        sb.AppendLine("    }");
        sb.AppendLine();

        var generatedEndpoints = new HashSet<string>();

        foreach (var method in info.Methods)
        {
            var endpoint = GenerateEndpointForMethod(method, entityName, info.InterfaceName);
            var endpointSignature = $"{method.Name}_{string.Join("_", method.Parameters.Select(p => p.TypeName))}";

            if (!string.IsNullOrEmpty(endpoint) && !generatedEndpoints.Contains(endpointSignature))
            {
                sb.AppendLine(endpoint);
                sb.AppendLine();
                generatedEndpoints.Add(endpointSignature);
            }
        }

        sb.AppendLine("}");

        return sb.ToString();
    }

    private static string GenerateEndpointForMethod(MethodInfo method, string entityName, string repoInterface)
    {
        var methodName = method.Name;

        var httpMethod = DetermineHttpMethod(methodName);
        var routePath = GenerateRoutePath(methodName, method.Parameters);
        var actionName = methodName.Replace("Async", "");

        var sb = new StringBuilder();

        var returnTypeInfo = method.ReturnType;

        sb.AppendLine($"    [{httpMethod}(\"{routePath}\")]");
        sb.AppendLine($"    public async Task<ActionResult{GetActionResultType(returnTypeInfo, entityName)}> {actionName}({GenerateParameters(method.Parameters, entityName)})");
        sb.AppendLine("    {");

        sb.Append(GenerateMethodBody(method, entityName, returnTypeInfo));

        sb.AppendLine("    }");

        return sb.ToString();
    }

    private static string GetActionResultType(ReturnTypeInfo returnInfo, string entityName)
    {
        if (returnInfo.IsCollection)
        {
            return $"<IEnumerable<{entityName}Dto>>";
        }

        if (returnInfo.IsEntity)
        {
            return $"<{entityName}Dto>";
        }

        if (returnInfo.IsPrimitive)
        {
            return $"<{returnInfo.TypeName}>";
        }

        return "";
    }

    private static string DetermineHttpMethod(string methodName)
    {
        if (methodName.StartsWith("Get") || methodName.StartsWith("Search") ||
            methodName.StartsWith("Query") || methodName == "Count")
        {
            return "HttpGet";
        }

        if (methodName.StartsWith("Create") || methodName.StartsWith("Add"))
        {
            return "HttpPost";
        }

        if (methodName.StartsWith("Update") || methodName.StartsWith("Modify"))
        {
            return "HttpPut";
        }

        if (methodName.StartsWith("Delete") || methodName.StartsWith("Remove"))
        {
            return "HttpDelete";
        }

        return "HttpGet";
    }

    private static string GenerateRoutePath(string methodName, ImmutableArray<ParameterInfo> parameters)
    {
        if (methodName.StartsWith("GetAll") && !methodName.Contains("By"))
        {
            return "";
        }

        if (methodName.StartsWith("Get") && parameters.Length == 1 && parameters[0].IsIdParameter)
        {
            return $"{{{parameters[0].Name}}}";
        }

        if (methodName.StartsWith("GetBy") || methodName.StartsWith("SearchBy"))
        {
            var suffix = ExtractSuffixFromMethodName(methodName);
            if (!string.IsNullOrEmpty(suffix))
            {
                return $"by-{ToKebabCase(suffix)}";
            }
        }

        if (methodName == "DeleteByIdAsync" || methodName.StartsWith("DeleteById"))
        {
            return $"{{{GetIdParameterName(parameters)}}}";
        }

        var cleanName = methodName
            .Replace("Async", "")
            .Replace("GetAll", "")
            .Replace("Get", "")
            .Replace("Create", "")
            .Replace("Update", "")
            .Replace("Delete", "")
            .Replace("Search", "search")
            .Replace("By", "/by-");

        return !string.IsNullOrEmpty(cleanName) && cleanName != methodName
            ? ToKebabCase(cleanName)
            : "";
    }

    private static string ExtractSuffixFromMethodName(string methodName)
    {
        if (!methodName.Contains("By"))
        {
            return string.Empty;
        }

        var parts = methodName.Split(["By"], StringSplitOptions.None);

        return parts.Length > 1 ? parts[1].Replace("Async", "").Replace("Id", "") : "";
    }

    private static bool IsIdParameter(IParameterSymbol parameter)
        => parameter.Name.EndsWith("Id") || parameter.Name == "id" ||
           parameter.Name.EndsWith("Key") || parameter.Name == "key";

    private static string GetIdParameterName(ImmutableArray<ParameterInfo> parameters)
    {
        var idParam = parameters.FirstOrDefault(p => p.IsIdParameter);

        return idParam.Name ?? "id";
    }

    private static ReturnTypeInfo AnalyzeReturnType(ITypeSymbol returnType)
    {
        if (returnType is not INamedTypeSymbol namedType)
        {
            return new(false, false, false, false, false, returnType.Name);
        }

        var innerType = returnType;
        if (namedType is { Name: "Task", TypeArguments.Length: > 0 })
        {
            innerType = namedType.TypeArguments[0];
        }

        if (innerType is not INamedTypeSymbol innerNamed)
        {
            return new(false, false, false, false, false, innerType.Name);
        }

        if (innerNamed.Name is "List" or "IEnumerable" or "ICollection" or "IQueryable")
        {
            return new(true, false, false, false, false, innerNamed.Name);
        }

        if (innerNamed.SpecialType != SpecialType.None ||
            innerNamed.Name is "String" or "Guid" or "DateTime")
        {
            return new(false, false, false, true, false, innerNamed.ToDisplayString());
        }

        if (innerNamed.Name == "Boolean")
        {
            return new(false, false, false, false, true, "bool");
        }

        var isNullable = innerNamed.NullableAnnotation == NullableAnnotation.Annotated;

        return new(false, true, isNullable, false, false, innerNamed.Name);
    }

    private static string GenerateParameters(ImmutableArray<ParameterInfo> parameters, string entityName)
    {
        var sb = new StringBuilder();

        for (var i = 0; i < parameters.Length; i++)
        {
            var param = parameters[i];

            if (param.Name == "cancellationToken")
            {
                continue;
            }

            if (param.IsComplexType)
            {
                var dtoName = param.TypeName == entityName ? $"Create{entityName}Dto" : param.FullTypeName;
                sb.Append($"[FromBody] {dtoName} {param.Name}");
            }
            else
            {
                sb.Append($"{param.FullTypeName} {param.Name}");
            }

            if (i < parameters.Length - 1 && parameters[i + 1].Name != "cancellationToken")
            {
                sb.Append(", ");
            }
        }

        return sb.ToString().TrimEnd(',', ' ');
    }

    private static bool IsComplexType(ITypeSymbol type)
    {
        if (type is { TypeKind: TypeKind.Class, SpecialType: SpecialType.None })
        {
            return type.Name != "String";
        }

        return false;
    }

    private static string GenerateMethodBody(MethodInfo method, string entityName, ReturnTypeInfo returnInfo)
    {
        var sb = new StringBuilder();
        var methodCall = BuildRepositoryMethodCall(method);

        if (returnInfo.IsCollection)
        {
            sb.AppendLine($"        var entities = await {methodCall};");
            sb.AppendLine("        return Ok(entities.Select(e => e.ToDto()));");
        }
        else if (returnInfo.IsEntity)
        {
            sb.AppendLine($"        var entity = await {methodCall};");
            if (returnInfo.IsNullable)
            {
                sb.AppendLine("        if (entity == null) return NotFound();");
            }
            sb.AppendLine("        return Ok(entity.ToDto());");
        }
        else if (returnInfo.IsPrimitive)
        {
            sb.AppendLine($"        var result = await {methodCall};");
            sb.AppendLine("        return Ok(result);");
        }
        else if (returnInfo.IsBool)
        {
            if (method.Name.StartsWith("Create") || method.Name.StartsWith("Add"))
            {
                var itemParam = method.Parameters.FirstOrDefault(p => p.TypeName == entityName);
                if (itemParam.Name is { })
                {
                    sb.AppendLine($"        var entity = {itemParam.Name}.ToEntity();");
                    sb.AppendLine($"        var success = await {methodCall};");
                    sb.AppendLine("        if (!success) return BadRequest();");
                    sb.AppendLine($"        return CreatedAtAction(nameof(Get), new {{ id = entity.Id }}, entity.ToDto());");
                }
                else
                {
                    sb.AppendLine($"        var success = await {methodCall};");
                    sb.AppendLine("        return success ? Ok() : BadRequest();");
                }
            }
            else if (method.Name.StartsWith("Update") || method.Name.StartsWith("Delete"))
            {
                sb.AppendLine($"        var success = await {methodCall};");
                sb.AppendLine("        return success ? NoContent() : NotFound();");
            }
            else
            {
                sb.AppendLine($"        var result = await {methodCall};");
                sb.AppendLine("        return Ok(result);");
            }
        }
        else
        {
            sb.AppendLine($"        var result = await {methodCall};");
            sb.AppendLine("        return Ok(result);");
        }

        return sb.ToString();
    }

    private static string BuildRepositoryMethodCall(MethodInfo method)
    {
        var parameters = method.Parameters
            .Where(p => p.Name != "cancellationToken")
            .Select(p => p.Name);

        return $"_repository.{method.Name}({string.Join(", ", parameters)})";
    }

    private static string GenerateDtos(InterfaceInfo info)
    {
        var entityName = info.EntityName;
        var properties = info.EntityProperties;

        var sb = new StringBuilder();

        sb.AppendLine("// <auto-generated />");
        sb.AppendLine("using System;");
        sb.AppendLine();
        sb.AppendLine("namespace Generated.Dtos;");
        sb.AppendLine();

        // Main DTO
        sb.AppendLine($"public record {entityName}Dto");
        sb.AppendLine("{");
        foreach (var prop in properties)
        {
            if (prop.IsNavigationProperty || !prop.HasPublicSetter)
            {
                continue;
            }

            sb.AppendLine($"    public {prop.TypeName} {prop.Name} {{ get; init; }}");
        }
        sb.AppendLine("}");
        sb.AppendLine();

        // Create DTO
        sb.AppendLine($"public record Create{entityName}Dto");
        sb.AppendLine("{");
        foreach (var prop in properties)
        {
            if (prop.ExcludeFromCreateDto)
            {
                continue;
            }

            sb.AppendLine($"    public {prop.TypeName} {prop.Name} {{ get; init; }}");
        }
        sb.AppendLine("}");
        sb.AppendLine();

        // Update DTO
        sb.AppendLine($"public record Update{entityName}Dto");
        sb.AppendLine("{");
        foreach (var prop in properties)
        {
            if (prop.ExcludeFromUpdateDto)
            {
                continue;
            }

            sb.AppendLine($"    public {prop.TypeName} {prop.Name} {{ get; init; }}");
        }
        sb.AppendLine("}");

        return sb.ToString();
    }

    private static bool ShouldExcludeFromCreateDto(IPropertySymbol prop)
        => IsNavigationProperty(prop) || !HasPublicSetter(prop) ||
           prop.Name is "Id" or "CreatedAt" or "UpdatedAt" or "CreatedBy" or "UpdatedBy";

    private static bool ShouldExcludeFromUpdateDto(IPropertySymbol prop)
        => IsNavigationProperty(prop) || !HasPublicSetter(prop) ||
           prop.Name is "Id" or "CreatedAt" or "CreatedBy";

    private static bool HasPublicSetter(IPropertySymbol prop)
        => prop.SetMethod is { DeclaredAccessibility: Accessibility.Public };

    private static bool IsNavigationProperty(IPropertySymbol property)
    {
        var type = property.Type;

        if (type is not INamedTypeSymbol namedType)
        {
            return false;
        }

        if (namedType.OriginalDefinition.ToString().StartsWith("System.Collections"))
        {
            return true;
        }

        if (property.IsVirtual)
        {
            return true;
        }

        var typeName = namedType.Name;

        return typeName.EndsWith("Entity") || typeName.EndsWith("Model");
    }

    private static string GenerateMappers(InterfaceInfo info)
    {
        var entityName = info.EntityName;
        var properties = info.EntityProperties;

        var sb = new StringBuilder();

        sb.AppendLine("// <auto-generated />");
        sb.AppendLine(GeneratedNamespace.Using(info.EntityNamespace));
        sb.AppendLine("using Generated.Dtos;");
        sb.AppendLine();
        sb.AppendLine("namespace Generated.Mappers;");
        sb.AppendLine();
        sb.AppendLine($"public static class {entityName}Mappers");
        sb.AppendLine("{");

        // ToDto
        sb.AppendLine($"    public static {entityName}Dto ToDto(this {entityName} entity)");
        sb.AppendLine("    {");
        sb.AppendLine($"        return new {entityName}Dto");
        sb.AppendLine("        {");
        foreach (var prop in properties.Where(p => !p.IsNavigationProperty && p.HasPublicSetter))
        {
            sb.AppendLine($"            {prop.Name} = entity.{prop.Name},");
        }
        sb.AppendLine("        };");
        sb.AppendLine("    }");
        sb.AppendLine();

        // ToEntity (from Create)
        sb.AppendLine($"    public static {entityName} ToEntity(this Create{entityName}Dto dto)");
        sb.AppendLine("    {");
        sb.AppendLine($"        return new {entityName}");
        sb.AppendLine("        {");
        foreach (var prop in properties.Where(p => !p.ExcludeFromCreateDto))
        {
            sb.AppendLine($"            {prop.Name} = dto.{prop.Name},");
        }
        sb.AppendLine("        };");
        sb.AppendLine("    }");
        sb.AppendLine();

        // UpdateFrom
        sb.AppendLine($"    public static void UpdateFrom(this {entityName} entity, Update{entityName}Dto dto)");
        sb.AppendLine("    {");
        foreach (var prop in properties.Where(p => !p.ExcludeFromUpdateDto))
        {
            sb.AppendLine($"        entity.{prop.Name} = dto.{prop.Name};");
        }
        sb.AppendLine("    }");

        sb.AppendLine("}");

        return sb.ToString();
    }

    private static string ToKebabCase(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        var sb = new StringBuilder();
        sb.Append(char.ToLower(input[0]));

        for (var i = 1; i < input.Length; i++)
        {
            if (char.IsUpper(input[i]))
            {
                sb.Append('-');
                sb.Append(char.ToLower(input[i]));
            }
            else
            {
                sb.Append(input[i]);
            }
        }

        return sb.ToString();
    }

    private readonly record struct InterfaceInfo(
        string InterfaceName,
        string InterfaceNamespace,
        string EntityName,
        string EntityNamespace,
        ControllerConfig Config,
        ImmutableArray<MethodInfo> Methods,
        ImmutableArray<EntityPropertyInfo> EntityProperties);

    private readonly record struct ControllerConfig(
        string? Route,
        bool AuthRequired,
        string? ParentRoute);

    private readonly record struct MethodInfo(
        string Name,
        ImmutableArray<ParameterInfo> Parameters,
        ReturnTypeInfo ReturnType);

    private readonly record struct ParameterInfo(
        string Name,
        string FullTypeName,
        string TypeName,
        bool IsComplexType,
        bool IsIdParameter);

    private readonly record struct ReturnTypeInfo(
        bool IsCollection,
        bool IsEntity,
        bool IsNullable,
        bool IsPrimitive,
        bool IsBool,
        string TypeName);

    private readonly record struct EntityPropertyInfo(
        string Name,
        string TypeName,
        bool IsNavigationProperty,
        bool HasPublicSetter,
        bool ExcludeFromCreateDto,
        bool ExcludeFromUpdateDto);
}

