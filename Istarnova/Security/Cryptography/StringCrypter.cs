#region "Header"

// File: StringCrypter.cs
// Created 2026/08/17
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

#region "Usings"

using System.Security.Cryptography;
using System.Text;

#endregion

namespace Istarnova.Security.Cryptography;

/// <summary>
///     Password-based string encryption using AES-256-GCM.
/// </summary>
/// <remarks>
///     <para>
///         GCM is authenticated: a ciphertext that was altered after the fact fails to decrypt instead
///         of yielding plausible-looking garbage. The previous implementation used unauthenticated
///         AES-CBC, where an attacker who cannot read a value can still change it undetected.
///     </para>
///     <para>
///         Layout of the Base64 payload:
///         <c>version(1) || salt(16) || nonce(12) || tag(16) || ciphertext</c>. The version byte exists
///         so the key derivation can be strengthened later without making existing ciphertexts
///         undecryptable - the reader picks its parameters from it.
///     </para>
/// </remarks>
public static class StringCrypter
{
    #region "Constants"

    /// <summary>
    ///     Payload layout currently written by <see cref="Encrypt" />.
    /// </summary>
    private const byte CurrentVersion = 1;

    private const int HeaderSize = 1 + SaltSize + NonceSize + TagSize;

    /// <summary>
    ///     PBKDF2 iterations, following the OWASP recommendation for PBKDF2-HMAC-SHA256.
    /// </summary>
    private const int Iterations = 600_000;

    private const int KeySize = 32; // 256 bits for AES-256
    private const int NonceSize = 12; // AesGcm.NonceByteSizes.MaxSize
    private const int SaltSize = 16;
    private const int TagSize = 16; // AesGcm.TagByteSizes.MaxSize

    #endregion

    #region "Methods"

    /// <summary>
    ///     Decrypts a string that was encrypted using the <see cref="Encrypt" /> method.
    /// </summary>
    /// <param name="encryptedValue">Base64 encoded encrypted data</param>
    /// <param name="password">The password used for encryption</param>
    /// <returns>The decrypted plaintext</returns>
    /// <exception cref="ArgumentNullException">Thrown when encryptedValue or password is null</exception>
    /// <exception cref="ArgumentException">Thrown when password is empty</exception>
    /// <exception cref="CryptographicException">
    ///     Thrown when decryption fails - including when the payload was altered after encryption, which
    ///     GCM detects rather than decrypting to garbage.
    /// </exception>
    public static string Decrypt(string encryptedValue, string password)
    {
        ArgumentNullException.ThrowIfNull(encryptedValue);
        ArgumentNullException.ThrowIfNull(password);

        if (string.IsNullOrEmpty(password))
        {
            throw new ArgumentException("Password cannot be empty", nameof(password));
        }

        try
        {
            var payload = Convert.FromBase64String(encryptedValue);

            if (payload.Length < HeaderSize)
            {
                throw new CryptographicException("Invalid encrypted data format");
            }

            if (payload[0] != CurrentVersion)
            {
                throw new CryptographicException($"Unsupported payload version {payload[0]}");
            }

            var salt = payload.AsSpan(1, SaltSize);
            var nonce = payload.AsSpan(1 + SaltSize, NonceSize);
            var tag = payload.AsSpan(1 + SaltSize + NonceSize, TagSize);
            var cipherText = payload.AsSpan(HeaderSize);

            var key = DeriveKey(password, salt);
            var plainTextBytes = new byte[cipherText.Length];

            using (var aes = new AesGcm(key, TagSize))
            {
                aes.Decrypt(nonce, cipherText, tag, plainTextBytes);
            }

            return Encoding.UTF8.GetString(plainTextBytes);
        }
        catch (Exception ex) when (ex is not ArgumentException and not CryptographicException)
        {
            throw new CryptographicException("Decryption failed", ex);
        }
    }

    /// <summary>
    ///     Encrypts a string using AES-256-GCM with a randomly generated salt and nonce.
    /// </summary>
    /// <param name="value">The plaintext to encrypt</param>
    /// <param name="password">The password to derive the encryption key from</param>
    /// <returns>Base64 encoded payload, see the remarks on <see cref="StringCrypter" /> for its layout</returns>
    /// <exception cref="ArgumentNullException">Thrown when value or password is null</exception>
    /// <exception cref="ArgumentException">Thrown when password is empty</exception>
    /// <exception cref="CryptographicException">Thrown when encryption fails</exception>
    public static string Encrypt(string value, string password)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(password);

        if (string.IsNullOrEmpty(password))
        {
            throw new ArgumentException("Password cannot be empty", nameof(password));
        }

        try
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(value);
            var payload = new byte[HeaderSize + plainTextBytes.Length];

            payload[0] = CurrentVersion;

            var salt = payload.AsSpan(1, SaltSize);
            var nonce = payload.AsSpan(1 + SaltSize, NonceSize);
            var tag = payload.AsSpan(1 + SaltSize + NonceSize, TagSize);
            var cipherText = payload.AsSpan(HeaderSize);

            RandomNumberGenerator.Fill(salt);
            RandomNumberGenerator.Fill(nonce);

            var key = DeriveKey(password, salt);

            using (var aes = new AesGcm(key, TagSize))
            {
                aes.Encrypt(nonce, plainTextBytes, cipherText, tag);
            }

            return Convert.ToBase64String(payload);
        }
        catch (Exception ex) when (ex is not ArgumentException)
        {
            throw new CryptographicException("Encryption failed", ex);
        }
    }

    /// <summary>
    ///     Attempts to decrypt a string, returning success status and result.
    /// </summary>
    public static bool TryDecrypt(string encryptedValue, string password, out string decryptedValue)
    {
        try
        {
            decryptedValue = Decrypt(encryptedValue, password);
            return true;
        }
        catch
        {
            decryptedValue = string.Empty;
            return false;
        }
    }

    /// <summary>
    ///     Attempts to encrypt a string, returning success status and result.
    /// </summary>
    public static bool TryEncrypt(string value, string password, out string encryptedValue)
    {
        try
        {
            encryptedValue = Encrypt(value, password);
            return true;
        }
        catch
        {
            encryptedValue = string.Empty;
            return false;
        }
    }

    private static byte[] DeriveKey(string password, ReadOnlySpan<byte> salt)
        => Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, KeySize);

    #endregion
}
