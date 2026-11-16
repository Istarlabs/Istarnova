#region "Header"

// File: EntityEntryExtensionsTests.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using Istarnova.EfCore.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Istarnova.Tests.EfCore;

public class EntityEntryExtensionsTests
{
    [Fact]
    public void ApplyEdit_ShouldCopyTheEdit_AndMakeTheCarriedRowVersionTheOriginalOne()
    {
        using var context = new EfCoreTestContext();
        var row = new EfCoreTestContext.Guarded { GuardedId = 1, Name = "read", RowVersion = 5 };
        context.Attach(row);

        context.Entry(row).ApplyEdit(new EfCoreTestContext.Guarded { GuardedId = 1, Name = "edited", RowVersion = 42 });

        row.Name.Should().Be("edited");
        context.Entry(row).Property(entity => entity.RowVersion).OriginalValue.Should().Be(42u);
        context.Entry(row).Property(entity => entity.RowVersion).IsModified.Should().BeFalse();
    }

    [Fact]
    public void ApplyEdit_ShouldKeepTheOriginalRowVersion_WhenTheEditCarriesNone()
    {
        using var context = new EfCoreTestContext();
        var row = new EfCoreTestContext.Guarded { GuardedId = 1, Name = "read", RowVersion = 5 };
        context.Attach(row);

        context.Entry(row).ApplyEdit(new EfCoreTestContext.Guarded { GuardedId = 1, Name = "edited", RowVersion = 0 });

        context.Entry(row).Property(entity => entity.RowVersion).OriginalValue.Should().Be(5u);
    }

    [Fact]
    public void ApplyEdit_ShouldJustCopy_WhenTheTableDoesNotDetectConcurrentEdits()
    {
        using var context = new EfCoreTestContext();
        var row = new EfCoreTestContext.Plain { PlainId = 1, Name = "read" };
        context.Attach(row);

        context.Entry(row).ApplyEdit(new EfCoreTestContext.Plain { PlainId = 1, Name = "edited" });

        row.Name.Should().Be("edited");
    }
}
