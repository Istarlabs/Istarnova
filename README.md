# Istarnova

A .NET library in several packages: extensions, helpers and source generators for the things that come up in every
project, and the building blocks of a Blazor Server application on PostgreSQL. Written for modern C# with an eye on
allocation and type safety.

## Packages

All of them share one version and are released together.

| Package | What it holds | Pulls in |
| --- | --- | --- |
| `Istarnova` | Extensions and helpers, the source generators, Font Awesome constants, resources in 10 languages | NetEscapades.EnumGenerators |
| `Istarnova.Export` | `FileExporter`: Excel and formula-safe CSV, the target of `[FileExportable]` | ClosedXML, CsvHelper |
| `Istarnova.EfCore` | Base entity and configurations, audit stamping, repository contracts | EF Core |
| `Istarnova.AspNetCore` | Service defaults, security headers, trusted proxies, rate limiting, cultures, NLog | OpenTelemetry, NLog |
| `Istarnova.Blazor` | Per-circuit state, the circuit handler, `UserFacingError`, `ComponentLogger` | `Istarnova.EfCore` |
| `Istarnova.Blazor.FluentUI` | Localized grids, page frame, navigation, error boundary, dialog and CRUD bases | `Istarnova.Blazor`, FluentUI Blazor 5 |

```bash
dotnet add package Istarnova
dotnet add package Istarnova.Export        # only for Excel/CSV export
```

## Documentation

The [wiki](https://github.com/Istarlabs/Istarnova/wiki) explains the packages:
[features](https://github.com/Istarlabs/Istarnova/wiki/Features), [examples](https://github.com/Istarlabs/Istarnova/wiki/Examples), [source generators](https://github.com/Istarlabs/Istarnova/wiki/Source-generators),
[Blazor building blocks](https://github.com/Istarlabs/Istarnova/wiki/Blazor-building-blocks) and [development](https://github.com/Istarlabs/Istarnova/wiki/Development).

## Contributing

Feedback and pull requests are welcome: see [CONTRIBUTING.md](CONTRIBUTING.md) and the [Code of Conduct](CODE_OF_CONDUCT.md).
`python3 Scripts/dotnet.py verify` builds, tests and checks the warnings on the lines you changed.

## Third-party notices

The `Istarnova.Icons.FontAwesome` constants are the class names and code points of the free
[Font Awesome](https://fontawesome.com) solid and brands icons; no font or icon artwork is bundled.
Font Awesome Free is licensed under [CC BY 4.0 (icons), SIL OFL 1.1 (fonts) and MIT (code)](https://fontawesome.com/license/free).
Brand icons are trademarks of their respective owners.

## License

Istarnova is licensed under the [GNU General Public License v3.0 or later](LICENSE.md).

Copyright © 2023-2026 The Istarnova Contributors — the people in [AUTHORS.md](AUTHORS.md). Every source file carries this as two SPDX lines in its header.
