#region "Header"

// File: RepositoryLogger.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using Microsoft.Extensions.Logging;

namespace Istarnova.EfCore.Logging;

/// <summary>
///     The log messages every repository shares. EventIds 2000 to 2079 belong to this class; an application that adds
///     messages of its own to a repository keeps to 2080 and above.
/// </summary>
public static partial class RepositoryLogger
{
    #region "Methods"

    /// <summary>
    ///     Logs info about a repository operation.
    /// </summary>
    [LoggerMessage(EventId = 2000, Level = LogLevel.Information,
        Message = "{message}")]
    public static partial void LogRepositoryInfo(this ILogger logger, string message);

    /// <summary>
    ///     Logs a failed repository operation with its exception.
    /// </summary>
    [LoggerMessage(EventId = 2001, Level = LogLevel.Error,
        Message = "Repository operation failed")]
    public static partial void LogRepositoryError(this ILogger logger, Exception exception);

    /// <summary>
    ///     Logs a failed repository operation with additional data.
    /// </summary>
    [LoggerMessage(EventId = 2002, Level = LogLevel.Error,
        Message = "Error occurred: {message}; Data: {obj}")]
    public static partial void LogRepositoryErrorWithJob(this ILogger logger, string message, string obj);

    /// <summary>
    ///     Logs a failed repository operation on one record.
    /// </summary>
    [LoggerMessage(EventId = 2003, Level = LogLevel.Error,
        Message = "Error occurred: {message} with Id {id}")]
    public static partial void LogRepositoryError(this ILogger logger, string message, long id);

    /// <summary>
    ///     Logs a failed repository operation on one record.
    /// </summary>
    [LoggerMessage(EventId = 2006, Level = LogLevel.Error,
        Message = "Error occurred: {message} with Id {id}")]
    public static partial void LogRepositoryError(this ILogger logger, string message, Guid id);

    /// <summary>
    ///     Logs a successful create.
    /// </summary>
    [LoggerMessage(EventId = 2010, Level = LogLevel.Information,
        Message = "Successfully created {objectName} with Id = {id}")]
    public static partial void LogRepositoryCreated(this ILogger logger, string objectName, int id);

    /// <summary>
    ///     Logs a successful create.
    /// </summary>
    [LoggerMessage(EventId = 2011, Level = LogLevel.Information,
        Message = "Successfully created {objectName} with Id = {id}")]
    public static partial void LogRepositoryCreated(this ILogger logger, string objectName, long id);

    /// <summary>
    ///     Logs a successful create.
    /// </summary>
    [LoggerMessage(EventId = 2012, Level = LogLevel.Information,
        Message = "Successfully created {objectName} with Id = {id}")]
    public static partial void LogRepositoryCreated(this ILogger logger, string objectName, string id);

    /// <summary>
    ///     Logs a successful update.
    /// </summary>
    [LoggerMessage(EventId = 2020, Level = LogLevel.Information,
        Message = "Successfully updated {objectName} with Id = {id}")]
    public static partial void LogRepositoryUpdate(this ILogger logger, string objectName, int id);

    /// <summary>
    ///     Logs a successful update.
    /// </summary>
    [LoggerMessage(EventId = 2021, Level = LogLevel.Information,
        Message = "Successfully updated {objectName} with Id = {id}")]
    public static partial void LogRepositoryUpdate(this ILogger logger, string objectName, long id);

    /// <summary>
    ///     Logs a successful update.
    /// </summary>
    [LoggerMessage(EventId = 2022, Level = LogLevel.Information,
        Message = "Successfully updated {objectName} with Id = {id}")]
    public static partial void LogRepositoryUpdate(this ILogger logger, string objectName, string id);

    /// <summary>
    ///     Logs a successful delete.
    /// </summary>
    [LoggerMessage(EventId = 2031, Level = LogLevel.Information,
        Message = "Successfully deleted {objectName} with Id = {id}")]
    public static partial void LogRepositoryDeleted(this ILogger logger, string objectName, long id);

    /// <summary>
    ///     Logs a successful delete.
    /// </summary>
    [LoggerMessage(EventId = 2032, Level = LogLevel.Information,
        Message = "Successfully deleted {objectName} with Id = {id}")]
    public static partial void LogRepositoryDeleted(this ILogger logger, string objectName, string id);

    /// <summary>
    ///     Logs that an entity was not found by its id.
    /// </summary>
    [LoggerMessage(EventId = 2050, Level = LogLevel.Warning,
        Message = "{objectName} with Id = {id} not found")]
    public static partial void LogRepositoryNotFound(this ILogger logger, string objectName, int id);

    /// <summary>
    ///     Logs that an entity was not found by its id.
    /// </summary>
    [LoggerMessage(EventId = 2051, Level = LogLevel.Warning,
        Message = "{objectName} with Id = {id} not found")]
    public static partial void LogRepositoryNotFound(this ILogger logger, string objectName, long id);

    /// <summary>
    ///     Logs that an entity was not found by its id.
    /// </summary>
    [LoggerMessage(EventId = 2052, Level = LogLevel.Warning,
        Message = "{objectName} with Id = {id} not found")]
    public static partial void LogRepositoryNotFound(this ILogger logger, string objectName, string id);

    /// <summary>
    ///     Logs how many records a read returned.
    /// </summary>
    [LoggerMessage(EventId = 2053, Level = LogLevel.Debug,
        Message = "Retrieved {count} {objectName} records")]
    public static partial void LogRepositoryGetAll(this ILogger logger, string objectName, int count);

    /// <summary>
    ///     Logs an exception of an operation ("Create", "Update").
    /// </summary>
    [LoggerMessage(EventId = 2060, Level = LogLevel.Error,
        Message = "Exception during {operation} operation on {objectName}")]
    public static partial void LogRepositoryException(this ILogger logger, Exception exception, string operation, string objectName);

    /// <summary>
    ///     Logs a concurrency conflict.
    /// </summary>
    [LoggerMessage(EventId = 2061, Level = LogLevel.Error,
        Message = "Concurrency conflict occurred while updating {objectName}")]
    public static partial void LogRepositoryConcurrencyException(this ILogger logger, Exception exception, string objectName);

    /// <summary>
    ///     Logs an exception of an operation on one known row.
    /// </summary>
    [LoggerMessage(EventId = 2062, Level = LogLevel.Error,
        Message = "Exception during {operation} operation on {objectName} with Id = {id}")]
    public static partial void LogRepositoryException(this ILogger logger, Exception exception, string operation, string objectName, long id);

    /// <summary>
    ///     Logs validation failures before a database operation.
    /// </summary>
    [LoggerMessage(EventId = 2070, Level = LogLevel.Warning,
        Message = "Validation failed for {objectName}: {validationErrors}")]
    public static partial void LogRepositoryValidationFailed(this ILogger logger, string objectName, string validationErrors);

    #endregion
}
