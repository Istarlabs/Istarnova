// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

using Istarnova.Blazor.Input;
using Istarnova.Input;
using Microsoft.AspNetCore.Components.Web;

namespace Istarnova.Tests.Blazor;

public class KeyboardEventArgsExtensionsTests
{
    [Theory]
    [InlineData("Enter", true)]
    [InlineData("enter", false)]
    [InlineData("Escape", false)]
    [InlineData("", false)]
    public void IsEnter_ShouldOnlyBeTrue_WhenTheBrowserReportsEnter(string key, bool expected)
        => new KeyboardEventArgs { Key = key }.IsEnter().Should().Be(expected);

    [Theory]
    [InlineData("Escape", true)]
    [InlineData("Esc", false)]
    [InlineData("Enter", false)]
    public void IsEscape_ShouldOnlyBeTrue_WhenTheBrowserReportsEscape(string key, bool expected)
        => new KeyboardEventArgs { Key = key }.IsEscape().Should().Be(expected);

    [Fact]
    public void ToKeyboardKey_ShouldKeepTheRawString_WhenTheKeyIsUnknown()
    {
        var key = new KeyboardEventArgs { Key = "Dead" }.ToKeyboardKey();

        key.Key.Should().Be(Keys.Unknown);
        key.JsKey.Should().Be("Dead");
    }

    [Fact]
    public void IsKey_ShouldTreatTheKeypadDigitAsTheRowDigit_WhenTheBrowserReportsZero()
    {
        var args = new KeyboardEventArgs { Key = "0" };

        args.IsKey(KeyboardKey.D0).Should().BeTrue();
        args.IsKey(KeyboardKey.NumPad0).Should().BeFalse();
    }

    [Fact]
    public void ToKeyboardKey_ShouldNotThrow_WhenTheKeyIsNull()
        => new KeyboardEventArgs { Key = null! }.ToKeyboardKey().Key.Should().Be(Keys.Unknown);
}
