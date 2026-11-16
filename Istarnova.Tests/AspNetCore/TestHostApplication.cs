#region "Header"

// File: TestHostApplication.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;

namespace Istarnova.Tests.AspNetCore;

/// <summary>
///     A web application on the in-memory test server, for looking at what the building blocks do to a request.
/// </summary>
public static class TestHostApplication
{
    #region "Methods"

    public static WebApplicationBuilder CreateBuilder(Dictionary<string, string?>? settings = null)
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();

        if (settings is not null)
        {
            builder.Configuration.AddInMemoryCollection(settings);
        }

        return builder;
    }

    public static async Task<(WebApplication App, HttpClient Client)> StartAsync(WebApplicationBuilder builder, Action<WebApplication> configure)
    {
        var app = builder.Build();
        configure(app);
        await app.StartAsync();

        return (app, app.GetTestClient());
    }

    #endregion
}
