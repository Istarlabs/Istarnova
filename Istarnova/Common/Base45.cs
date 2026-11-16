#region "Header"

// File: Base45.cs
// Created 2026/03/06
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

namespace Istarnova.Common;

public static class Base45
{
    #region "Constants"

    private const string Alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ $%*+-./:";
    private const int MaxStackAlloc = 512;
    private static readonly int[] DecodeTable = BuildDecodeTable();

    #endregion

    #region "Methods"

    public static byte[] Decode(ReadOnlySpan<char> input)
    {
        var outputLength = input.Length / 3 * 2 + (input.Length % 3 == 2 ? 1 : 0);
        var output = new byte[outputLength];
        var outPos = 0;
        var inPos = 0;

        while (inPos + 2 < input.Length)
        {
            var a = DecodeTable[input[inPos]];
            var b = DecodeTable[input[inPos + 1]];
            var c = DecodeTable[input[inPos + 2]];

            if (a < 0 || b < 0 || c < 0)
            {
                throw new FormatException($"Invalid Base45 character at position {inPos}");
            }

            var n = a + b * 45 + c * 45 * 45;
            output[outPos] = (byte)(n >> 8);
            output[outPos + 1] = (byte)(n & 0xFF);
            outPos += 2;
            inPos += 3;
        }

        if (inPos + 1 < input.Length)
        {
            var a = DecodeTable[input[inPos]];
            var b = DecodeTable[input[inPos + 1]];

            if (a < 0 || b < 0)
            {
                throw new FormatException($"Invalid Base45 character at position {inPos}");
            }

            output[outPos] = (byte)(a + b * 45);
        }

        return output;
    }

    public static string Encode(ReadOnlySpan<byte> input)
    {
        var outputLength = input.Length / 2 * 3 + (input.Length % 2 == 1 ? 2 : 0);
        var output = outputLength <= MaxStackAlloc
            ? stackalloc char[outputLength]
            : new char[outputLength];

        var outPos = 0;
        var inPos = 0;

        while (inPos + 1 < input.Length)
        {
            var n = (input[inPos] << 8) | input[inPos + 1];
            output[outPos + 2] = Alphabet[n / (45 * 45)];
            n %= 45 * 45;
            output[outPos + 1] = Alphabet[n / 45];
            output[outPos] = Alphabet[n % 45];
            outPos += 3;
            inPos += 2;
        }

        if (inPos < input.Length)
        {
            var n = input[inPos];
            output[outPos + 1] = Alphabet[n / 45];
            output[outPos] = Alphabet[n % 45];
        }

        return new(output);
    }

    private static int[] BuildDecodeTable()
    {
        var table = new int[256];
        table.AsSpan().Fill(-1);
        for (var i = 0; i < Alphabet.Length; i++)
        {
            table[Alphabet[i]] = i;
        }

        return table;
    }

    #endregion
}
