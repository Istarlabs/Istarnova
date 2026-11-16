// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

using Istarnova.Collections.Generics;

namespace Istarnova.Tests.Collections.Generics;

public class HtmlAttributesDictionaryTests
{
    [Fact]
    public void BuildClass_ShouldReturnNull_WhenNoClassIsSet()
    {
        // Arrange
        var attributes = new HtmlAttributesDictionary();

        // Act
        var result = attributes.BuildClass();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void BuildClass_ShouldReturnEveryClass_WhenSeveralWereAdded()
    {
        // Arrange
        var attributes = new HtmlAttributesDictionary()
            .AddClass("card")
            .AddClass("card--wide");

        // Act
        var result = attributes.BuildClass();

        // Assert
        result.Should().Be("card card--wide");
    }

    [Fact]
    public void BuildStyle_ShouldReturnNull_WhenNoStyleIsSet()
    {
        // Arrange
        var attributes = new HtmlAttributesDictionary();

        // Act
        var result = attributes.BuildStyle();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void BuildStyle_ShouldReturnEveryDeclaration_WhenSeveralWereAdded()
    {
        // Arrange
        var attributes = new HtmlAttributesDictionary()
            .AddStyle("color", "red")
            .AddStyle("gap", "1rem");

        // Act
        var result = attributes.BuildStyle();

        // Assert
        result.Should().Be("color: red; gap: 1rem");
    }

    [Fact]
    public void BuildStyle_ShouldNormalize_WhenTheAttributeWasSetDirectly()
    {
        // Arrange
        var attributes = new HtmlAttributesDictionary
        {
            [HtmlAttributesDictionary.Style] = "color:red;gap:1rem;",
        };

        // Act
        var result = attributes.BuildStyle();

        // Assert
        result.Should().Be("color: red; gap: 1rem");
    }

    [Theory]
    [InlineData("plain", "plain")]
    [InlineData("a & b", "a &amp; b")]
    [InlineData("<b>\"x\"</b>", "&lt;b&gt;&quot;x&quot;&lt;/b&gt;")]
    [InlineData("it's", "it&#39;s")]
    [InlineData("", "")]
    [InlineData("ä<", "ä&lt;")]
    public void ToHtmlString_ShouldEscapeTheValue_WhenItHoldsSpecialCharacters(string value, string expected)
    {
        var attributes = new HtmlAttributesDictionary { ["title"] = value };

        attributes.ToHtmlString().Should().Be($"title=\"{expected}\"");
    }

    [Fact]
    public void ToHtmlString_ShouldSeparateAttributesWithASpace_WhenSeveralAreSet()
    {
        var attributes = new HtmlAttributesDictionary { ["id"] = "a", ["title"] = "b" };

        attributes.ToHtmlString().Should().Be("id=\"a\" title=\"b\"");
    }

    [Fact]
    public void AddClass_ShouldNotAddTwice_WhenTheClassIsAlreadyThere()
    {
        var attributes = new HtmlAttributesDictionary { [HtmlAttributesDictionary.Class] = "card  wide" };

        attributes.AddClass("wide").BuildClass().Should().Be("card wide");
        attributes.AddClass("tall").BuildClass().Should().Be("card wide tall");
    }

    [Fact]
    public void HasClass_ShouldCompareWholeNamesCaseSensitive_WhenTheAttributeHoldsSeveral()
    {
        var attributes = new HtmlAttributesDictionary { [HtmlAttributesDictionary.Class] = "card-wide Card" };

        attributes.HasClass("card").Should().BeFalse();
        attributes.HasClass("Card").Should().BeTrue();
        attributes.HasClass("card-wide").Should().BeTrue();
        new HtmlAttributesDictionary().HasClass("card").Should().BeFalse();
    }

    [Fact]
    public void RemoveClass_ShouldRemoveTheAttribute_WhenTheLastClassGoes()
    {
        var attributes = new HtmlAttributesDictionary().AddClass("card");

        attributes.RemoveClass("missing").BuildClass().Should().Be("card");
        attributes.RemoveClass("card").ContainsKey(HtmlAttributesDictionary.Class).Should().BeFalse();
    }

    [Fact]
    public void Merge_ShouldAddEachClassOnce_WhenBothSidesHaveClasses()
    {
        var attributes = new HtmlAttributesDictionary().AddClass("a").AddClass("b");
        var other = new HtmlAttributesDictionary { [HtmlAttributesDictionary.Class] = "b  c" };

        attributes.Merge(other).BuildClass().Should().Be("a b c");
    }

    [Fact]
    public void GetStyles_ShouldKeepTheValueAfterTheFirstColon_AndSkipBrokenDeclarations()
    {
        var attributes = new HtmlAttributesDictionary
        {
            [HtmlAttributesDictionary.Style] = " background : url(a:b) ;color:;novalue; : red ;;gap:1rem",
        };

        attributes.GetStyles().Should().BeEquivalentTo(new Dictionary<string, string>
        {
            ["background"] = "url(a:b)",
            ["gap"] = "1rem",
        });
    }
}
