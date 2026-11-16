#region "Header"

// File: ComponentLogger.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using Microsoft.Extensions.Logging;

namespace Istarnova.Blazor.Logging;

/// <summary>
///     The log messages every page, tab and dialog shares, the way the repository logger is that for the repositories.
///     EventIds 6010 to 6049 belong to the libraries (6010 to 6022 here, 6030 to 6039 the error boundary, 6040 to 6049 the circuit
///     handler); an application adds its own from 6100.
/// </summary>
public static partial class ComponentLogger
{
    #region "Methods"

    /// <summary>
    ///     Logs that something could not be read, where no single record was being read.
    /// </summary>
    /// <param name="logger">The ILogger.</param>
    /// <param name="exception">The exception that occurred.</param>
    /// <param name="what">What was being loaded (nameof(YourType)).</param>
    [LoggerMessage(EventId = 6010, Level = LogLevel.Error, Message = "Loading {what} failed")]
    public static partial void LogComponentLoadFailed(this ILogger logger, Exception exception, string what);

    /// <summary>
    ///     Logs that something could not be read for one record.
    /// </summary>
    /// <param name="logger">The ILogger.</param>
    /// <param name="exception">The exception that occurred.</param>
    /// <param name="what">What was being loaded (nameof(YourType)).</param>
    /// <param name="ownerName">The name of the id below (nameof(PatientId)).</param>
    /// <param name="ownerId">The record it was being loaded for, or <c>null</c> when that is unknown.</param>
    [LoggerMessage(EventId = 6011, Level = LogLevel.Error, Message = "Loading {what} failed for {ownerName} = {ownerId}")]
    public static partial void LogComponentLoadFailed(this ILogger logger, Exception exception, string what, string ownerName, long? ownerId);

    /// <summary>
    ///     Logs that something could not be written, where it belongs to no single record.
    /// </summary>
    /// <param name="logger">The ILogger.</param>
    /// <param name="exception">The exception that occurred.</param>
    /// <param name="what">What was being saved (nameof(YourType)).</param>
    [LoggerMessage(EventId = 6012, Level = LogLevel.Error, Message = "Saving {what} failed")]
    public static partial void LogComponentSaveFailed(this ILogger logger, Exception exception, string what);

    /// <summary>
    ///     Logs that something could not be written for one record.
    /// </summary>
    /// <param name="logger">The ILogger.</param>
    /// <param name="exception">The exception that occurred.</param>
    /// <param name="what">What was being saved (nameof(YourType)).</param>
    /// <param name="ownerName">The name of the id below (nameof(PregnancyId)).</param>
    /// <param name="ownerId">The record it was being saved for, or <c>null</c> when that is unknown.</param>
    [LoggerMessage(EventId = 6013, Level = LogLevel.Error, Message = "Saving {what} failed for {ownerName} = {ownerId}")]
    public static partial void LogComponentSaveFailed(this ILogger logger, Exception exception, string what, string ownerName, long? ownerId);

    /// <summary>
    ///     Logs that something other than a plain read or write failed - an export, a download, a share.
    /// </summary>
    /// <param name="logger">The ILogger.</param>
    /// <param name="exception">The exception that occurred.</param>
    /// <param name="action">What was being done, as one word ("Export", "Download").</param>
    /// <param name="what">What it was being done to (nameof(YourType)).</param>
    /// <param name="ownerName">The name of the id below (nameof(FileItemId)).</param>
    /// <param name="ownerId">The record it concerned, or <c>null</c> when that is unknown.</param>
    [LoggerMessage(EventId = 6014, Level = LogLevel.Error, Message = "{action} of {what} failed for {ownerName} = {ownerId}")]
    public static partial void LogComponentActionFailed(this ILogger logger, Exception exception, string action, string what, string ownerName, long? ownerId);

    /// <summary>
    ///     Logs how many rows a screen loaded.
    /// </summary>
    /// <param name="logger">The ILogger.</param>
    /// <param name="what">What was loaded (nameof(YourType)).</param>
    /// <param name="count">How many.</param>
    [LoggerMessage(EventId = 6015, Level = LogLevel.Debug, Message = "Loaded {count} {what} record(s)")]
    public static partial void LogComponentLoaded(this ILogger logger, string what, int count);

    /// <summary>
    ///     Logs that a component caught an exception it has no more specific message for.
    /// </summary>
    /// <param name="logger">The ILogger.</param>
    /// <param name="exception">The exception that occurred.</param>
    /// <param name="componentName">The component that caught it (typeof(TComponent).Name).</param>
    [LoggerMessage(EventId = 6020, Level = LogLevel.Error, Message = "Unhandled failure in {componentName}")]
    public static partial void LogComponentFailure(this ILogger logger, Exception exception, string componentName);

    /// <summary>
    ///     Logs that a grid could not work out the key of the type it is editing.
    /// </summary>
    /// <param name="logger">The ILogger.</param>
    /// <param name="entityName">The entity type (typeof(TEntity).Name).</param>
    [LoggerMessage(EventId = 6021, Level = LogLevel.Error, Message = "Could not read the id of {entityName}")]
    public static partial void LogComponentIdUnreadable(this ILogger logger, string entityName);

    /// <summary>
    ///     Logs that a list stays empty because its repository cannot be queried.
    /// </summary>
    /// <param name="logger">The ILogger.</param>
    /// <param name="entityName">The entity type (typeof(TEntity).Name).</param>
    [LoggerMessage(EventId = 6022, Level = LogLevel.Warning,
        Message = "The repository of {entityName} does not implement IQueryableRepository; nothing is loaded")]
    public static partial void LogComponentRepositoryNotQueryable(this ILogger logger, string entityName);

    #endregion
}
