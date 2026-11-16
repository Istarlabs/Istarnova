#region "Header"

// File: ArrayExtensions.cs
// Created 2026/03/06
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

#region "Usings"

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#endregion

namespace Istarnova.Collections.Generics;

/// <summary>
///     Modern array extension methods with optimized performance and comprehensive error handling.
/// </summary>
public static class ArrayExtensions
{
    #region "Methods"

    /// <param name="array">The jagged array to transpose</param>
    /// <typeparam name="T">The type of array elements</typeparam>
    extension<T>(T[][] array)
    {
        #region "Methods"

        /// <summary>
        ///     Transposes a jagged array to a 2D array (swaps rows and columns).
        /// </summary>
        /// <returns>A new 2D array with transposed dimensions</returns>
        /// <exception cref="ArgumentNullException">Thrown when array is null</exception>
        /// <exception cref="ArgumentException">Thrown when array is empty or contains null/inconsistent rows</exception>
        public T[,] Transpose()
        {
            ArgumentNullException.ThrowIfNull(array);

            if (array.Length == 0)
            {
                throw new ArgumentException("Array cannot be empty", nameof(array));
            }

            var rows = array.Length;
            var cols = ValidateAndGetColumnCount(array);
            var result = new T[cols, rows];

            // Choose strategy based on size and whether parallel processing is beneficial
            if (rows > Environment.ProcessorCount && rows * cols > 10000)
            {
                return TransposeParallel(array, result, rows, cols);
            }

            // Sequential version for smaller arrays
            for (var i = 0; i < rows; i++)
            {
                var row = array[i];
                for (var j = 0; j < cols; j++)
                {
                    result[j, i] = row[j];
                }
            }

            return result;
        }

        /// <summary>
        ///     Transposes a jagged array using parallel processing for large arrays.
        /// </summary>
        /// <returns>A new 2D array with transposed dimensions</returns>
        /// <exception cref="ArgumentNullException">Thrown when array is null</exception>
        /// <exception cref="ArgumentException">Thrown when array is empty or contains null/inconsistent rows</exception>
        public T[,] TransposeParallel()
        {
            ArgumentNullException.ThrowIfNull(array);

            if (array.Length == 0)
            {
                throw new ArgumentException("Array cannot be empty", nameof(array));
            }

            var rows = array.Length;
            var cols = ValidateAndGetColumnCount(array);
            var result = new T[cols, rows];

            return TransposeParallel(array, result, rows, cols);
        }

        #endregion
    }

    /// <param name="array">The 2D array to flatten</param>
    /// <typeparam name="T">The type of array elements</typeparam>
    extension<T>(T[,] array)
    {
        #region "Methods"

        /// <summary>
        ///     Creates a readonly span view of a 2D array for efficient read operations.
        /// </summary>
        /// <returns>A readonly span representing the array</returns>
        public ReadOnlySpan<T> AsReadOnlySpan()
        {
            ArgumentNullException.ThrowIfNull(array);
            return MemoryMarshal.CreateReadOnlySpan(ref array[0, 0], array.Length);
        }

        /// <summary>
        ///     Creates a flattened view of a 2D array as a span for sequential processing.
        /// </summary>
        /// <returns>A span representing the flattened array</returns>
        public Span<T> AsSpan()
        {
            ArgumentNullException.ThrowIfNull(array);
            return MemoryMarshal.CreateSpan(ref array[0, 0], array.Length);
        }

        #endregion
    }

    extension<T>(T[][] array)
    {
        #region "Methods"

        /// <summary>
        ///     Legacy alias for Transpose method. Use Transpose instead.
        /// </summary>
        [Obsolete("Use Transpose instead. This method will be removed in a future version.")]
        public T[,] SwapToColumns() => array.Transpose();

        /// <summary>
        ///     Legacy alias for TransposeParallel method. Use TransposeParallel instead.
        /// </summary>
        [Obsolete("Use TransposeParallel instead. This method will be removed in a future version.")]
        public T[,] SwapToColumnsParallel() => array.TransposeParallel();

        #endregion
    }

    public static bool HasItems<T>(this T[]? array) => array is { Length: > 0 };

    // Legacy method aliases for backwards compatibility
    /// <summary>
    ///     Legacy alias for Transpose method. Use Transpose instead.
    /// </summary>
    [Obsolete("Use Transpose instead. This method will be removed in a future version.")]
    public static T[,] SwapToColumns<T>(this T[,] array) => array.Transpose();

    /// <summary>
    ///     Transposes a 2D array (swaps rows and columns).
    /// </summary>
    /// <typeparam name="T">The type of array elements</typeparam>
    /// <param name="array">The 2D array to transpose</param>
    /// <returns>A new 2D array with transposed dimensions</returns>
    /// <exception cref="ArgumentNullException">Thrown when array is null</exception>
    /// <exception cref="ArgumentException">Thrown when array has more than 2 dimensions</exception>
    public static T[,] Transpose<T>(this T[,] array)
    {
        ArgumentNullException.ThrowIfNull(array);

        if (array.Rank != 2)
        {
            throw new ArgumentException("Array must be exactly 2-dimensional", nameof(array));
        }

        var rows = array.GetLength(0);
        var cols = array.GetLength(1);
        var result = new T[cols, rows];

        // Use parallel processing for large arrays
        if (rows > Environment.ProcessorCount && rows * cols > 10000)
        {
            Parallel.For(0, rows, new()
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount,
            }, i =>
            {
                for (var j = 0; j < cols; j++)
                {
                    result[j, i] = array[i, j];
                }
            });
        }
        else
        {
            // Sequential version for smaller arrays
            for (var i = 0; i < rows; i++)
            {
                for (var j = 0; j < cols; j++)
                {
                    result[j, i] = array[i, j];
                }
            }
        }

        return result;
    }

    private static T[,] TransposeParallel<T>(T[][] source, T[,] result, int rows, int cols)
    {
        Parallel.For(0, rows, new()
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount,
        }, i =>
        {
            var row = source[i];
            for (var j = 0; j < cols; j++)
            {
                result[j, i] = row[j];
            }
        });

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int ValidateAndGetColumnCount<T>(T[][] array)
    {
        if (array[0] == null)
        {
            throw new ArgumentException("Array contains null row at index 0", nameof(array));
        }

        var expectedCols = array[0].Length;

        for (var i = 1; i < array.Length; i++)
        {
            if (array[i] == null)
            {
                throw new ArgumentException($"Array contains null row at index {i}", nameof(array));
            }

            if (array[i].Length != expectedCols)
            {
                throw new ArgumentException(
                    $"Inconsistent row lengths: row 0 has {expectedCols} elements, row {i} has {array[i].Length} elements",
                    nameof(array));
            }
        }

        return expectedCols;
    }

    #endregion
}
