#region Header
// File: KeyboardKeyTests.cs
// Created 2026/10/29
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later
#endregion

#region Usings

using Istarnova.Input;

#endregion

namespace Istarnova.Tests.Input;

public class KeyboardKeyTests
{
    private static readonly Keys[] GenericModifiers = [Keys.Meta, Keys.Shift, Keys.Control, Keys.Alt];

    public static TheoryData<Keys> SpecificKeys
    {
        get
        {
            var data = new TheoryData<Keys>();
            foreach (var key in Enum.GetValues<Keys>().Where(key => key != Keys.Unknown && !GenericModifiers.Contains(key)))
            {
                data.Add(key);
            }

            return data;
        }
    }

    [Theory]
    [MemberData(nameof(SpecificKeys))]
    public void FromCode_ShouldRoundTrip_WhenKeyIsNotAGenericModifier(Keys key)
    {
        var source = new KeyboardKey(key);

        KeyboardKey.FromWin32(source.ToWin32Key()).Key.Should().Be(key);
        KeyboardKey.FromWinInput(source.ToWinInputKey()).Key.Should().Be(key);
        KeyboardKey.FromWinForm(source.ToWinFormKey()).Key.Should().Be(key);
    }

    [Theory]
    [InlineData("0", Keys.D0)]
    [InlineData("PrintScreen", Keys.PrintScreen)]
    [InlineData("Unidentified", Keys.None)]
    [InlineData("Shift", Keys.Shift)]
    [InlineData("a", Keys.A)]
    public void FromJs_ShouldPickTheFirstKey_WhenSeveralShareTheString(string js, Keys expected)
        => KeyboardKey.FromJs(js).Key.Should().Be(expected);

    [Theory]
    [MemberData(nameof(SpecificKeys))]
    public void IsJs_ShouldAgreeWithFromJs_WhenGivenTheJavaScriptKeyOfAnyKey(Keys key)
    {
        var js = new KeyboardKey(key).ToJavaScriptKey();

        foreach (var other in Enum.GetValues<Keys>().Where(other => other != Keys.Unknown))
        {
            var candidate = new KeyboardKey(other);

            candidate.IsJs(js).Should().Be(KeyboardKey.FromJs(js) == candidate);
        }
    }

    [Fact]
    public void IsJs_ShouldCompareTheRawString_WhenKeyIsUnknownFromJavaScript()
    {
        var key = KeyboardKey.FromJs("Dead");

        key.IsJs("Dead").Should().BeTrue();
        key.IsJs("dead").Should().BeFalse();
        KeyboardKey.Unknown.IsJs("Dead").Should().BeFalse();
        KeyboardKey.FromWin32(0x1234).IsJs("").Should().BeFalse();
    }

    [Fact]
    public void FromJs_ShouldGiveTheSameKey_WhenGivenCharactersInsteadOfAString()
    {
        KeyboardKey.FromJs("Enter".AsSpan()).Should().Be(KeyboardKey.Enter);
        KeyboardKey.FromJs("Dead".AsSpan()).Should().Be(KeyboardKey.FromJs("Dead"));
    }

    [Fact]
    public void FromWin32_ShouldKeepTheRawCode_WhenKeyIsUnknown()
    {
        var key = KeyboardKey.FromWin32(0x1234);

        key.Key.Should().Be(Keys.Unknown);
        key.KeySource.Should().Be(KeySource.Win32);
        key.ToWin32Key().Should().Be(0x1234);
        key.Should().NotBe(KeyboardKey.FromWin32(0x1235));
        key.Should().Be(KeyboardKey.FromWin32(0x1234));
    }

    [Fact]
    public void FromWin32_ShouldReturnLeftKey_WhenCodeIsSharedWithGenericModifier()
        => KeyboardKey.FromWin32(0xA0).Key.Should().Be(Keys.LeftShift);

    [Theory]
    [InlineData(0x10, 16, Keys.Shift)]
    [InlineData(0x11, 17, Keys.Control)]
    [InlineData(0x12, 18, Keys.Alt)]
    public void FromCode_ShouldReturnGenericModifier_WhenCodeIsTheGenericVirtualKey(int win32, int winForm, Keys expected)
    {
        KeyboardKey.FromWin32(win32).Key.Should().Be(expected);
        KeyboardKey.FromWinForm(winForm).Key.Should().Be(expected);
        new KeyboardKey(expected).ToWin32Key().Should().Be(win32);
    }

    [Fact]
    public void Equality_ShouldCompareTheKey_WhenInstancesDiffer()
    {
        (KeyboardKey.A == new KeyboardKey(Keys.A)).Should().BeTrue();
        (KeyboardKey.A != KeyboardKey.B).Should().BeTrue();
        (KeyboardKey.A == null).Should().BeFalse();
    }
}
