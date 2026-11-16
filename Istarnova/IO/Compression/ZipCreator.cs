#region "Header"

// File: ZipCreator.cs
// Created 2026/03/06
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

#region "Usings"

using System.IO.Compression;
using System.Text;

#endregion

namespace Istarnova.IO.Compression;

/// <summary>
///     ZIP archive creator on the native .NET compression APIs.
///     Provides high-performance ZIP file creation with proper resource management.
/// </summary>
public sealed class ZipCreator : IDisposable, IAsyncDisposable
{
    #region "Variables"

    private readonly ZipArchive _archive;
    private readonly MemoryStream _memoryStream;
    private bool _disposed;

    #endregion

    #region "Constructor"

    /// <summary>
    ///     Creates a new ZipCreator instance.
    /// </summary>
    public ZipCreator()
    {
        _memoryStream = new();
        _archive = new(_memoryStream, ZipArchiveMode.Create, true);
    }

    #endregion

    #region "Properties"

    /// <summary>
    ///     Gets the number of entries in the archive.
    /// </summary>
    public int EntryCount => _archive.Entries.Count;

    /// <summary>
    ///     Gets the current size of the ZIP archive in bytes.
    /// </summary>
    public long Size => _memoryStream.Length;

    #endregion

    #region "Methods"

    /// <summary>
    ///     Adds a file to the ZIP archive from byte array.
    /// </summary>
    /// <param name="entryName">The name/path of the entry in the archive</param>
    /// <param name="data">The file data as byte array</param>
    /// <param name="compressionLevel">Compression level (default: Optimal)</param>
    public async Task AddFileAsync(string entryName, byte[] data,
        CompressionLevel compressionLevel = CompressionLevel.Optimal)
    {
        ArgumentNullException.ThrowIfNull(entryName);
        ArgumentNullException.ThrowIfNull(data);

        ObjectDisposedException.ThrowIf(_disposed, this);

        var entry = _archive.CreateEntry(entryName, compressionLevel);
        await using var entryStream = await entry.OpenAsync();
        await entryStream.WriteAsync(data);
    }

    /// <summary>
    ///     Adds a file to the ZIP archive from stream.
    /// </summary>
    /// <param name="entryName">The name/path of the entry in the archive</param>
    /// <param name="dataStream">The stream containing file data</param>
    /// <param name="compressionLevel">Compression level (default: Optimal)</param>
    public async Task AddFileAsync(string entryName, Stream dataStream,
        CompressionLevel compressionLevel = CompressionLevel.Optimal)
    {
        ArgumentNullException.ThrowIfNull(entryName);
        ArgumentNullException.ThrowIfNull(dataStream);

        ObjectDisposedException.ThrowIf(_disposed, this);

        var entry = _archive.CreateEntry(entryName, compressionLevel);
        await using var entryStream = await entry.OpenAsync();
        await dataStream.CopyToAsync(entryStream);
    }

    /// <summary>
    ///     Adds multiple files to the archive in parallel for better performance.
    /// </summary>
    /// <param name="files">Collection of file entries</param>
    /// <param name="compressionLevel">Compression level (default: Optimal)</param>
    public async Task AddFilesAsync(IEnumerable<(string EntryName, byte[] Data)> files,
        CompressionLevel compressionLevel = CompressionLevel.Optimal)
    {
        ArgumentNullException.ThrowIfNull(files);

        // Process files sequentially to avoid thread-safety issues with ZipArchive
        foreach (var (entryName, data) in files)
        {
            await AddFileAsync(entryName, data, compressionLevel);
        }
    }

    /// <summary>
    ///     Adds a text file to the ZIP archive.
    /// </summary>
    /// <param name="entryName">The name/path of the entry in the archive</param>
    /// <param name="text">The text content</param>
    /// <param name="encoding">Text encoding (default: UTF-8)</param>
    /// <param name="compressionLevel">Compression level (default: Optimal)</param>
    public Task AddTextFileAsync(string entryName, string text, Encoding? encoding = null,
        CompressionLevel compressionLevel = CompressionLevel.Optimal)
    {
        ArgumentNullException.ThrowIfNull(entryName);
        ArgumentNullException.ThrowIfNull(text);

        encoding ??= Encoding.UTF8;
        var data = encoding.GetBytes(text);
        return AddFileAsync(entryName, data, compressionLevel);
    }

    /// <summary>
    ///     Saves the ZIP archive to a file.
    /// </summary>
    /// <param name="filePath">Path where to save the ZIP file</param>
    public async Task SaveToFileAsync(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        var data = await ToByteArrayAsync();
        await File.WriteAllBytesAsync(filePath, data);
    }

    /// <summary>
    ///     Gets the completed ZIP archive as byte array.
    ///     Note: This finalizes the archive and disposes internal resources.
    /// </summary>
    /// <returns>ZIP file as byte array</returns>
    public Task<byte[]> ToByteArrayAsync()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        // Finalize the archive
        _archive.Dispose();

        // Get the bytes
        var result = _memoryStream.ToArray();

        // Mark as disposed since archive is now finalized
        _disposed = true;

        return Task.FromResult(result);
    }

    #endregion

    #region "IDisposable Implementation"

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _archive.Dispose();
        _memoryStream.Dispose();
        _disposed = true;
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        await _archive.DisposeAsync();
        await _memoryStream.DisposeAsync();
        _disposed = true;
    }

    #endregion
}
