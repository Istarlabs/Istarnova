# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Istarnova is a .NET library of two kinds, published as several NuGet packages with one shared version: extensions,
helpers and source generators for daily development, and, since 2026-10, the building blocks of a Blazor Server application
on PostgreSQL. English documentation, code and comments.

**Multi-project architecture:**
- `Istarnova/` - Main library; its package also carries the resources, the attributes and the generator
- `Istarnova.Export/` - `FileExporter` (ClosedXML, CsvHelper), a package of its own so the core stays light
- `Istarnova.Resources/` - Localized resources (English plus 9 languages), shipped inside the `Istarnova` package
- `Istarnova.SourceGenerator/` - Roslyn source generators (netstandard2.0)
- `Istarnova.SourceGenerator.Attributes/` - Attributes for generators
- `Istarnova.EfCore/` - Base entity and configuration, audit stamping, builder extensions, repository contracts, `RepositoryLogger`
- `Istarnova.Blazor/` - Per-circuit state, the circuit handler, `UserFacingError`, `ComponentLogger`
- `Istarnova.Blazor.FluentUI/` - Localized grids, page frame, navigation, error boundary, dialog and CRUD bases, FluentUI registration
- `Istarnova.AspNetCore/` - Service defaults, security headers, trusted proxies, rate limiting, cultures, NLog setup
- `Istarnova.Tests/` - xUnit tests with FluentAssertions, bUnit for the components, a TestHost for the hosting pieces

Packable are `Istarnova`, `Istarnova.Export` and the four building blocks (`IsPackable` true; `Directory.Build.props`
makes everything else non-packable and holds the version and package metadata). Applications of the same author may still
use a `ProjectReference` to a checkout next to their own: **a change here then reaches them at their next build.**

## Development Commands

Build, test and publish go through `Scripts/dotnet.py`; a hook refuses the raw `dotnet build/test`.

```bash
# Build, tests, and the warnings on the lines this branch changed
python3 Scripts/dotnet.py verify

# Tests only, or a subset
python3 Scripts/dotnet.py test --filter "FullyQualifiedName~StringExtensionsTests"

# Pack every package into nupkg/ and list them, showing the version the next publish would get
python3 Scripts/dotnet.py publish --dry-run

# Pack and push them to nuget.org, the feed in [nuget] (Scripts/project.toml)
python3 Scripts/dotnet.py publish
```

Every publish picks its own version: the highest one on the feed with its last number raised by one
(1.1.0-preview.2 -> 1.1.0-preview.3, 1.1.0 -> 1.1.1). To start a new line - 1.2.0, 2.0.0, or the
release that ends a preview line - set `<Version>` in `Directory.Build.props` above what is
published; that version wins. After a push the script writes the published version into the
`.csproj`, and that change is committed.

The `Istarnova` package bundles Istarnova.Resources (every satellite in its culture folder), the attributes and the
source generator - see the comments in `Istarnova/Istarnova.csproj`. The building blocks reference Istarnova.Resources
with `PrivateAssets="all"`, so it never becomes a package dependency of its own. A change to that packaging is only
verified once a fresh project restores the packages from a feed and the generators run in it.

## Code Style & Conventions

### File Headers
Every .cs file must use this region-based header:
```csharp
#region Header
// File: ClassName.cs
// Created YYYY/MM/DD
// Developer: Pascal Klose
// SPDX-FileCopyrightText: YYYY The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later
#endregion
```

### Organization Patterns
- **Regions**: Code organized with regions like "Properties", "Methods", "Constructor", "Usings"
- **Extensions**: All extension methods in `<Type>Extensions.cs` files under `Istarnova.Extensions` namespace
- **Static classes**: Extensions, helpers, and utilities are typically static classes

### Modern .NET Practices
- Use `Span<T>`, `ReadOnlySpan<T>` for performance (see `StringExtensions.ToBytes()`, `AlphanumComparator`)
- Prefer `ArgumentNullException.ThrowIfNull()` over manual null checks
- Use `GeneratedRegex` attribute for regex patterns (see `RegularExpressions.cs`)
- Leverage pattern matching and switch expressions
- Use `NetEscapades.EnumGenerators` for enum extensions (see `CsvSeparator`, `MimeFileType`)
- Use current C# features like collection expressions, primary constructors, and extension syntax

### Language Requirements
- **Code**: Must be in English (class names, method names, variables)
- **Comments**: Must be in English (inline comments, XML documentation)
- **XML Documentation**: Must be in English (`<summary>`, `<param>`, `<returns>` tags)
- **README**: English, and short; explanations go to the GitHub wiki
- All public APIs must have XML documentation comments

## Architecture & Key Systems

### 1. Source Generators (Roslyn-based)

The project uses three incremental source generators in the `Istarnova.SourceGenerator` project (netstandard2.0):

**FileExporter Generator** (`Data/FileExporter/FileExporterSourceGenerator.cs`)
- Marks classes with `[FileExportable]` attribute
- Properties use `[ColumnHeader("Name")]` for custom headers
- Generates `<ClassName>FileExporter` extension class with `GetExportColumns()` and conversion methods
- Supports Excel (.xlsx) via ClosedXML or CSV exports; the generated code needs `Istarnova.Export`, and without it the
  generator reports `ISTAR001` instead (new rules go into `AnalyzerReleases.Unshipped.md`)
- Generated files: `<ClassName>_FileExporter.g.cs`

**Controller Generator** (`Controller/ControllerSourceGenerator.cs`)
- Generates API controllers from classes marked with `[GenerateApiController]`
- Uses static methods and pattern matching for improved clarity
- Simplifies controller scaffolding

**AutoEquatable Generator** (`Data/Equatable/AutoEquatableGenerator.cs`)
- Generates `Equals()` and `GetHashCode()` implementations
- Uses `[AutoEquatable]` attribute

**FileExporter attributes namespace**:
```csharp
using Istarnova.SourceGenerator.Attributes.Data.FileExporter;
```

**Generator debugging**: All generators use incremental generation pattern. Check for `System.Diagnostics.Debugger.Launch()` if debugging is needed.

### 2. Localization System
- Separate `Istarnova.Resources` project with .resx files
- Resources exposed as `internal` via `InternalsVisibleTo` attribute
- Supports German (`de`) and Japanese translations: `ja-JP` in the older areas (`DateResources`, `ClaimsResources`), the **neutral `ja`
  in the newer ones** (`BlazorResources`, `ComponentResources`, `DialogResources`, `FluentLibraryResources`). An application asks for `ja`,
  and a satellite named `ja-JP` is not found for it - the session falls back to English without a word
- Used by `LocalizedTimeSpan`, `ClaimsPrincipalExtensions`, and by the Blazor projects (`InternalsVisibleTo` them)
- Access via `DateResources.ResourceManager.GetString("Key")` or similar
- Regenerate designer with `PublicResXFileCodeGenerator` (happens on build)

### 3. Performance-Critical Components
- **AlphanumComparator**: Singleton pattern (`Instance` property), uses `Span<T>` for natural sorting
- **EnumEx<T>**: Caches values via `Lazy<T[]>` to avoid repeated reflection
- **File export operations**: Async by default (`ExportAsync`, `ExportToExcelAsync`)
- **StringExtensions**: Zero-allocation operations with `Span<T>` where appropriate

### 4. Major Feature Areas
- **Extensions**: Organized by type in `Extensions/` folder (String, DateTime, Number, Color, Object, Array, Collection, etc.)
- **Data Export** (`Istarnova.Export`): FileExporter system, ClosedXML for Excel and CsvHelper for CSV
- **Collections**: `HtmlAttributesDictionary` - specialized dictionary for HTML attributes
- **Security**: `ClaimsPrincipalExtensions`, `StringCrypter`
- **UI Components**: `BootstrapColors.cs`, `FontAwesome/` icon char codes
- **Compression & IO**: `ZipCreator`, `ByteUnitConverter`

### 5. The Blazor and EF Core building blocks

`Istarnova.EfCore`, `.Blazor`, `.Blazor.FluentUI` and `.AspNetCore` are what the three applications share; each application keeps
only what is its own and plugs it in through a hook (`SeedDate` of the seeded configuration, `ApplicationErrorMessage` of the
component base, `OnEntitySavedAsync` of the dialog base, `OnAuthenticationAsync` of the circuit handler, `ReevaluateOn` of the
menu, `Actions` of the error boundary, `TitleSuffix` of the page frame). Before adding a switch to a component, ask whether an
application can supply the difference instead.

Rules that cost time:

- **EventIds**: `RepositoryLogger` owns 2000 to 2079, `ComponentLogger` 6010 to 6022, the error boundary 6030 to 6039, the circuit
  handler 6040 to 6049. An application starts its own at 2080 (repositories) and 6100 (UI). Log through `[LoggerMessage]` only.
- **Package versions** (EF Core, FluentUI, NLog.Web, Npgsql) are set in these projects; an application with a lower pin fails with `NU1605`.
  Raise them here and in the applications together.
- **A `FluentDataGrid` is never used raw**: the sort and column menus, the empty message and the paginator would be English. A
  wrapper declares every parameter it accepts, or the Razor compiler hands the rest on as a string and the circuit dies at render time.
- **`ShowPaginator="false"` shows every row**: the pagination state is not handed to the grid then.
- **Health endpoints** need `MapDefaultEndpoints(allowAnonymous: true)` behind a fallback policy that requires a user.
- A Razor component of this library needs its `@using` in the application's `_Imports.razor`; without it the tag is an unknown HTML
  element and only the warning RZ10012 says so.
- A change to a text means all three languages, and a test in each (`CultureScope`).

Components are tested with bUnit (`BunitContext`, `JSInterop.Mode = Loose`, `Services.AddIstarnovaFluentUI()`), the hosting pieces
with `TestHostApplication`, the model mapping with Npgsql's model-only context (no database).

The shared skills `blazor-ui`, `data-layer` and `logging` in `~/dev/tools/devscripts/skills` describe all of this for the
applications; a change to a building block that changes how it is used is a change to those skills too.

## Testing Standards

### Framework Setup
- **xUnit** with `FluentAssertions` for assertions
- **Moq** available for mocking
- Global usings: `Xunit` and `FluentAssertions` in `.csproj`

### Test Organization
- Mirror main library structure: `Istarnova.Tests/Extensions/StringExtensionsTests.cs`
- Test classes named `<TypeUnderTest>Tests`
- Methods: `MethodName_ShouldExpectedBehavior_WhenCondition()`
- Use `[Theory]` with `[InlineData]` for parameterized tests

### Example Pattern
```csharp
[Theory]
[InlineData("short", 100, "short")]
[InlineData("very long text here", 10, "very long  ...")]
public void DisplayShortText_ShouldTruncateCorrectly(string input, int maxLength, string expected)
{
    var result = input.DisplayShortText(maxLength);
    result.Should().Be(expected);
}
```

## Project Dependencies

Versions live in the `.csproj` files; `python3 Scripts/dotnet.py packages` lists them.

### Main Library Dependencies
- **NetEscapades.EnumGenerators** - Fast enum extension generation

### Istarnova.Export Dependencies
- **ClosedXML** - Excel export
- **CsvHelper** - CSV export (`FormulaSafeCsvWriter` escapes formula injection)

### Source Generator Dependencies
- **Microsoft.CodeAnalysis.CSharp** - Source generator infrastructure
- **Microsoft.CodeAnalysis.Analyzers** - Analyzer rules

### Test Dependencies
- **xunit**, **FluentAssertions**, **Moq**, **coverlet.collector**, **bunit**, **Microsoft.AspNetCore.TestHost**, **Npgsql.EntityFrameworkCore.PostgreSQL**

### Building-block Dependencies
- **Microsoft.FluentUI.AspNetCore.Components** (and `.Icons`) - the components; **Microsoft.Extensions.Http.Resilience**,
  **Microsoft.Extensions.ServiceDiscovery**, **OpenTelemetry.\*** and **NLog.Web.AspNetCore** - the hosting pieces; EF Core comes through `Istarnova.EfCore`

## Common Workflows

### Adding New Extensions
1. Create or extend `<Type>Extensions.cs` in `Istarnova/Extensions/`
2. Use static class with public static extension methods
3. Add XML doc comments with `<summary>` and `<param>` tags
4. Create corresponding test file in `Istarnova.Tests/Extensions/`
5. Add region headers for organization (#region "Properties", etc.)

### Working with Source Generators
- Source generators must target netstandard2.0 (required for Roslyn)
- Use incremental generation pattern (inherit from `IIncrementalGenerator`)
- Generated files use `.g.cs` suffix
- Generators are referenced as analyzers in the main project with `OutputItemType="Analyzer" ReferenceOutputAssembly="false"`

### Multi-Configuration Support
Solution supports `Debug|Release` × `Any CPU|x64|x86` configurations.
