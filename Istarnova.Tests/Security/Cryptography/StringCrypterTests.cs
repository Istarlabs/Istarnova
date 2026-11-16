#region "Header"

// File: StringCrypterTests.cs
// Created 2026/08/14
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using System.Security.Cryptography;
using Istarnova.Security.Cryptography;

namespace Istarnova.Tests.Security.Cryptography;

public class StringCrypterTests
{
    #region "Round Trip Tests"

    [Theory]
    [InlineData("")]
    [InlineData("a")]
    [InlineData("Ein Satz mit Umlauten: äöüß")]
    [InlineData("🔐 emoji and \n newlines \t tabs")]
    public void Decrypt_ShouldReturnOriginal_WhenGivenOwnCiphertext(string plainText)
    {
        // Arrange
        var encrypted = StringCrypter.Encrypt(plainText, "correct horse battery staple");

        // Act
        var result = StringCrypter.Decrypt(encrypted, "correct horse battery staple");

        // Assert
        result.Should().Be(plainText);
    }

    [Fact]
    public void Encrypt_ShouldProduceDifferentCiphertexts_WhenCalledTwiceOnSameInput()
    {
        // Act - salt and nonce are random per call
        var first = StringCrypter.Encrypt("same input", "password");
        var second = StringCrypter.Encrypt("same input", "password");

        // Assert
        first.Should().NotBe(second);
    }

    #endregion

    #region "Authentication Tests"

    [Fact]
    public void Decrypt_ShouldThrow_WhenCiphertextWasAltered()
    {
        // Arrange
        var encrypted = StringCrypter.Encrypt("transfer 100 euro", "password");
        var payload = Convert.FromBase64String(encrypted);

        // Flip a bit in the ciphertext body, past the 45 byte header
        payload[^1] ^= 0x01;

        // Act
        var action = () => StringCrypter.Decrypt(Convert.ToBase64String(payload), "password");

        // Assert - this is the point of GCM: tampering is detected, not decrypted to garbage
        action.Should().Throw<CryptographicException>();
    }

    [Fact]
    public void Decrypt_ShouldThrow_WhenPasswordIsWrong()
    {
        // Arrange
        var encrypted = StringCrypter.Encrypt("secret", "password");

        // Act
        var action = () => StringCrypter.Decrypt(encrypted, "not the password");

        // Assert
        action.Should().Throw<CryptographicException>();
    }

    [Fact]
    public void Decrypt_ShouldThrow_WhenPayloadVersionIsUnknown()
    {
        // Arrange
        var encrypted = StringCrypter.Encrypt("secret", "password");
        var payload = Convert.FromBase64String(encrypted);

        payload[0] = 0xFF;

        // Act
        var action = () => StringCrypter.Decrypt(Convert.ToBase64String(payload), "password");

        // Assert
        action.Should().Throw<CryptographicException>();
    }

    [Fact]
    public void Decrypt_ShouldThrow_WhenPayloadIsTooShortToHoldAHeader()
    {
        // Act
        var action = () => StringCrypter.Decrypt(Convert.ToBase64String(new byte[10]), "password");

        // Assert
        action.Should().Throw<CryptographicException>();
    }

    #endregion

    #region "Argument Tests"

    [Fact]
    public void Encrypt_ShouldThrowArgumentException_WhenPasswordIsEmpty()
    {
        // Act
        var action = () => StringCrypter.Encrypt("value", string.Empty);

        // Assert
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void TryDecrypt_ShouldReturnFalse_WhenInputIsNotValidBase64()
    {
        // Act
        var succeeded = StringCrypter.TryDecrypt("not base64 at all!", "password", out var result);

        // Assert
        succeeded.Should().BeFalse();
        result.Should().BeEmpty();
    }

    [Fact]
    public void TryEncrypt_ShouldReturnTrue_WhenInputIsValid()
    {
        // Act
        var succeeded = StringCrypter.TryEncrypt("value", "password", out var result);

        // Assert
        succeeded.Should().BeTrue();
        result.Should().NotBeEmpty();
    }

    #endregion
}
