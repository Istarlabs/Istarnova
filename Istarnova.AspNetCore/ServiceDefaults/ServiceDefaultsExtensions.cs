#region "Header"

// File: ServiceDefaultsExtensions.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Istarnova.AspNetCore.ServiceDefaults;

/// <summary>
///     The common Aspire services - service discovery, resilience, health checks, OpenTelemetry - so an application needs
///     no <c>ServiceDefaults</c> project of its own. See https://aka.ms/dotnet/aspire/service-defaults.
/// </summary>
public static class ServiceDefaultsExtensions
{
    #region "Variables"

    private const string AlivenessEndpointPath = "/alive";

    private const string HealthEndpointPath = "/health";

    #endregion

    #region "Methods"

    extension<TBuilder>(TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        #region "Methods"

        /// <summary>
        ///     A liveness check, so the container platform can tell whether the application answers.
        /// </summary>
        public TBuilder AddDefaultHealthChecks()
        {
            builder.Services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

            return builder;
        }

        /// <summary>
        ///     OpenTelemetry, health checks, service discovery and resilience for every <see cref="HttpClient" />.
        /// </summary>
        public TBuilder AddServiceDefaults()
        {
            builder.ConfigureOpenTelemetry();
            builder.AddDefaultHealthChecks();
            builder.Services.AddServiceDiscovery();

            builder.Services.ConfigureHttpClientDefaults(http =>
            {
                http.AddStandardResilienceHandler();
                http.AddServiceDiscovery();
            });

            return builder;
        }

        /// <summary>
        ///     Logs, metrics and traces through OpenTelemetry. The exporter is switched on by
        ///     <c>OTEL_EXPORTER_OTLP_ENDPOINT</c>.
        /// </summary>
        public TBuilder ConfigureOpenTelemetry()
        {
            builder.Logging.AddOpenTelemetry(logging =>
            {
                logging.IncludeFormattedMessage = true;
                logging.IncludeScopes = true;
            });

            builder.Services.AddOpenTelemetry()
                .WithMetrics(metrics =>
                {
                    metrics.AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddRuntimeInstrumentation();
                })
                .WithTracing(tracing =>
                {
                    tracing.AddSource(builder.Environment.ApplicationName)
                        .AddAspNetCoreInstrumentation(options =>
                            // Health checks are polled all day and would drown the traces.
                            options.Filter = context =>
                                !context.Request.Path.StartsWithSegments(HealthEndpointPath)
                                && !context.Request.Path.StartsWithSegments(AlivenessEndpointPath))
                        .AddHttpClientInstrumentation(options =>
                            // The query and the path of an address can be the secret (a feed address, an authkey): a trace
                            // keeps scheme and host only.
                            options.EnrichWithHttpRequestMessage = (activity, request) =>
                                activity.SetTag("url.full", request.RequestUri?.GetLeftPart(UriPartial.Authority)));
                });

            if (!string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]))
            {
                builder.Services.AddOpenTelemetry().UseOtlpExporter();
            }

            return builder;
        }

        #endregion
    }

    /// <summary>
    ///     <c>/health</c> (everything) and <c>/alive</c> (the liveness checks), in every environment: container
    ///     orchestration needs them outside development too.
    /// </summary>
    /// <param name="app">The application.</param>
    /// <param name="allowAnonymous">
    ///     Whether the endpoints answer without a signed-in user. An application whose fallback policy requires one needs this,
    ///     or the container's health check is sent to the login page.
    /// </param>
    public static WebApplication MapDefaultEndpoints(this WebApplication app, bool allowAnonymous = false)
    {
        var health = app.MapHealthChecks(HealthEndpointPath);

        var alive = app.MapHealthChecks(AlivenessEndpointPath, new()
        {
            Predicate = registration => registration.Tags.Contains("live"),
        });

        if (allowAnonymous)
        {
            health.AllowAnonymous();
            alive.AllowAnonymous();
        }

        return app;
    }

    #endregion
}
