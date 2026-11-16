// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

namespace Istarnova.Tests.SourceGenerator.GlobalNamespace;

public class GlobalNamespaceGeneratorTests
{
    [Fact]
    public void AutoEquatable_ShouldGenerateEquality_WhenTypeHasNoNamespace()
    {
        var first = new global::GlobalNamespaceEntity { GlobalNamespaceEntityId = 7 };
        var second = new global::GlobalNamespaceEntity { GlobalNamespaceEntityId = 7 };

        first.Should().Be(second);
    }

    [Fact]
    public void FileExportable_ShouldGenerateExporter_WhenTypeHasNoNamespace()
    {
        var columns = global::GlobalNamespaceExportRowFileExporter.GetExportColumns();

        columns.Select(column => column.Name).Should().Equal("Full name");
    }

    [Fact]
    public void EnumExtras_ShouldGenerateDisplayName_WhenTypeHasNoNamespace()
    {
        global::GlobalNamespaceEnum.Pending.GetDisplayName().Should().Be("Waiting");
    }
}
