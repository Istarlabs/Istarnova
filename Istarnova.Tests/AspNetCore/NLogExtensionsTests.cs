#region "Header"

// File: NLogExtensionsTests.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using Istarnova.AspNetCore.Logging;
using NLog;
using NLog.Layouts;

namespace Istarnova.Tests.AspNetCore;

public class NLogExtensionsTests
{
    [Fact]
    public void AddIstarnovaNLog_ShouldLetNLogReadTheConnectionStringFromTheConfiguration()
    {
        var builder = TestHostApplication.CreateBuilder(new() { ["ConnectionStrings:PostgresDb"] = "Host=db;Database=app" });

        builder.AddIstarnovaNLog();

        new SimpleLayout("${configsetting:name=ConnectionStrings.PostgresDb}")
            .Render(LogEventInfo.CreateNullEvent())
            .Should().Be("Host=db;Database=app");
    }
}
