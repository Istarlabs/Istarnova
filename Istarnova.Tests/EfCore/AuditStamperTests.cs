#region "Header"

// File: AuditStamperTests.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using Istarnova.EfCore.Auditing;
using Microsoft.EntityFrameworkCore;

namespace Istarnova.Tests.EfCore;

public class AuditStamperTests
{
    private static readonly DateTime Moment = new(2026, 10, 23, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Apply_ShouldStampAnAddedEntityWithUserAndTime()
    {
        using var context = new EfCoreTestContext();
        var entity = new EfCoreTestContext.Plain { Name = "new" };
        context.Add(entity);

        AuditStamper.Apply(context.ChangeTracker, "user-1", Moment);

        entity.CreatedBy.Should().Be("user-1");
        entity.ModifiedBy.Should().Be("user-1");
        entity.CreatedOn.Should().Be(Moment);
        entity.ModifiedOn.Should().Be(Moment);
    }

    [Fact]
    public void Apply_ShouldStampOnlyTheModificationOfAnUpdate_AndKeepWhatTheRowWasCreatedWith()
    {
        using var context = new EfCoreTestContext();
        var created = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var entity = new EfCoreTestContext.Plain { PlainId = 7, Name = "old", CreatedBy = "creator", CreatedOn = created, ModifiedOn = created };
        context.Attach(entity);

        entity.Name = "changed";
        entity.CreatedBy = "somebody else";
        context.ChangeTracker.DetectChanges();

        AuditStamper.Apply(context.ChangeTracker, "editor", Moment);

        entity.ModifiedBy.Should().Be("editor");
        entity.ModifiedOn.Should().Be(Moment);
        entity.CreatedBy.Should().Be("creator");
        entity.CreatedOn.Should().Be(created);
        context.Entry(entity).Property(row => row.CreatedBy).IsModified.Should().BeFalse();
    }

    [Fact]
    public void Apply_ShouldLeaveAnUnchangedEntityAlone()
    {
        using var context = new EfCoreTestContext();
        var entity = new EfCoreTestContext.Plain { PlainId = 1, Name = "same" };
        context.Attach(entity);

        AuditStamper.Apply(context.ChangeTracker, "user-1", Moment);

        entity.ModifiedBy.Should().BeNull();
    }

    [Fact]
    public void Apply_ShouldSetOnlyTimestamps_WhenThereIsNoUser()
    {
        using var context = new EfCoreTestContext();
        var entity = new EfCoreTestContext.Plain { Name = "new" };
        context.Add(entity);

        AuditStamper.Apply(context.ChangeTracker, null, Moment);

        entity.CreatedBy.Should().BeNull();
        entity.CreatedOn.Should().Be(Moment);
    }

    [Fact]
    public void Apply_ShouldRefuseNoTracker()
    {
        var act = () => AuditStamper.Apply(null!, "user-1");

        act.Should().Throw<ArgumentNullException>();
    }
}
