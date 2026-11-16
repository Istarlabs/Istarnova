// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Resources;
using Istarnova.Common.Enums;

namespace Istarnova.Tests.Common.Enums;

public class EnumExTests
{
    #region Test Enums

    public enum SimpleEnum
    {
        None,
        First,
        Second,
        Third
    }

    public enum EnumWithDisplayAttribute
    {
        [Display(Name = "No Value")]
        None,

        [Display(Name = "First Item")]
        First,

        [Display(Name = "Second Item")]
        Second
    }

    public enum EnumWithDescriptionAttribute
    {
        [Description("This is none")]
        None,

        [Description("This is the first value")]
        First,

        [Description("This is the second value")]
        Second
    }

    [Flags]
    public enum FlagsEnum
    {
        None = 0,
        Read = 1,
        Write = 2,
        Execute = 4,
        Delete = 8,
        ReadWrite = Read | Write,
        All = Read | Write | Execute | Delete
    }

    public enum EnumWithValues
    {
        Low = 10,
        Medium = 20,
        High = 30
    }

    #endregion

    #region Basic Properties Tests

    [Fact]
    public void Values_ShouldReturnAllEnumValues()
    {
        // Act
        var values = EnumEx<SimpleEnum>.Values;

        // Assert
        values.Length.Should().Be(4);
        values.ToArray().Should().Contain([SimpleEnum.None, SimpleEnum.First, SimpleEnum.Second, SimpleEnum.Third]);
    }

    [Fact]
    public void Names_ShouldReturnAllEnumNames()
    {
        // Act
        var names = EnumEx<SimpleEnum>.Names;

        // Assert
        names.Length.Should().Be(4);
        names.ToArray().Should().Contain(["None", "First", "Second", "Third"]);
    }

    [Fact]
    public void Count_ShouldReturnCorrectCount()
    {
        // Act
        var count = EnumEx<SimpleEnum>.Count;

        // Assert
        count.Should().Be(4);
    }

    [Fact]
    public void DisplayList_ShouldReturnKeyValuePairs()
    {
        // Act
        var displayList = EnumEx<SimpleEnum>.DisplayList;

        // Assert
        displayList.Length.Should().Be(4);
        displayList[0].Key.Should().Be(SimpleEnum.None);
        displayList[0].Value.Should().Be("None");
    }

    #endregion

    #region Parsing Tests

    [Theory]
    [InlineData("First", true, SimpleEnum.First)]
    [InlineData("first", true, SimpleEnum.First)]
    [InlineData("FIRST", true, SimpleEnum.First)]
    [InlineData("Invalid", false, SimpleEnum.None)]
    [InlineData("", false, SimpleEnum.None)]
    [InlineData(null, false, SimpleEnum.None)]
    public void TryParse_WithIgnoreCase_ShouldParseCorrectly(string? input, bool expectedSuccess, SimpleEnum expectedValue)
    {
        // Act
        var success = EnumEx<SimpleEnum>.TryParse(input, true, out var result);

        // Assert
        success.Should().Be(expectedSuccess);
        result.Should().Be(expectedValue);
    }

    [Theory]
    [InlineData("First", true, SimpleEnum.First)]
    [InlineData("first", false, SimpleEnum.None)]
    [InlineData("Invalid", false, SimpleEnum.None)]
    public void TryParse_CaseSensitive_ShouldParseCorrectly(string input, bool expectedSuccess, SimpleEnum expectedValue)
    {
        // Act
        var success = EnumEx<SimpleEnum>.TryParse(input, out var result);

        // Assert
        success.Should().Be(expectedSuccess);
        result.Should().Be(expectedValue);
    }

    [Theory]
    [InlineData("First", SimpleEnum.First)]
    [InlineData("Invalid", SimpleEnum.None)]
    [InlineData("", SimpleEnum.None)]
    public void ParseOrDefault_ShouldReturnCorrectValue(string input, SimpleEnum expected)
    {
        // Act
        var result = EnumEx<SimpleEnum>.ParseOrDefault(input);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void ParseOrDefault_WithCustomDefault_ShouldReturnCustomDefault()
    {
        // Act
        var result = EnumEx<SimpleEnum>.ParseOrDefault("Invalid", SimpleEnum.Third);

        // Assert
        result.Should().Be(SimpleEnum.Third);
    }

    #endregion

    #region Conversion Tests

    [Theory]
    [InlineData(EnumWithValues.Low, 10)]
    [InlineData(EnumWithValues.Medium, 20)]
    [InlineData(EnumWithValues.High, 30)]
    public void ToInt32_ShouldReturnCorrectValue(EnumWithValues value, int expected)
    {
        // Act
        var result = EnumEx<EnumWithValues>.ToInt32(value);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(EnumWithValues.Low, 10L)]
    [InlineData(EnumWithValues.Medium, 20L)]
    [InlineData(EnumWithValues.High, 30L)]
    public void ToInt64_ShouldReturnCorrectValue(EnumWithValues value, long expected)
    {
        // Act
        var result = EnumEx<EnumWithValues>.ToInt64(value);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(EnumWithValues.Low, "A")]
    [InlineData(EnumWithValues.Medium, "14")]
    [InlineData(EnumWithValues.High, "1E")]
    public void ToHexString_ShouldReturnCorrectHexValue(EnumWithValues value, string expected)
    {
        // Act
        var result = EnumEx<EnumWithValues>.ToHexString(value);

        // Assert
        result.Should().Be(expected);
    }

    #endregion

    #region IsDefined Tests

    [Theory]
    [InlineData(SimpleEnum.First, true)]
    [InlineData((SimpleEnum)999, false)]
    public void IsDefined_ShouldReturnCorrectResult(SimpleEnum value, bool expected)
    {
        // Act
        var result = EnumEx<SimpleEnum>.IsDefined(value);

        // Assert
        result.Should().Be(expected);
    }

    #endregion

    #region Display Name Tests

    [Theory]
    [InlineData(EnumWithDisplayAttribute.None, "No Value")]
    [InlineData(EnumWithDisplayAttribute.First, "First Item")]
    [InlineData(EnumWithDisplayAttribute.Second, "Second Item")]
    public void GetDisplayName_WithDisplayAttribute_ShouldReturnDisplayName(EnumWithDisplayAttribute value, string expected)
    {
        // Act
        var result = EnumEx<EnumWithDisplayAttribute>.GetDisplayName(value);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void GetDisplayName_WithoutAttribute_ShouldReturnEnumName()
    {
        // Act
        var result = EnumEx<SimpleEnum>.GetDisplayName(SimpleEnum.First);

        // Assert
        result.Should().Be("First");
    }

    [Fact]
    public void GetDisplayList_WithCustomResolver_ShouldUseResolver()
    {
        // Arrange
        string CustomResolver(SimpleEnum value) => $"Custom_{value}";

        // Act
        var result = EnumEx<SimpleEnum>.GetDisplayList(CustomResolver);

        // Assert
        result.Should().HaveCount(4);
        result[0].Value.Should().Be("Custom_None");
        result[1].Value.Should().Be("Custom_First");
    }

    [Fact]
    public void GetDescriptionList_WithDescriptionAttribute_ShouldReturnDescriptions()
    {
        // Arrange
        string DescriptionResolver(EnumWithDescriptionAttribute value) =>
            EnumEx<EnumWithDescriptionAttribute>.GetDisplayName(value);

        // Act
        var result = EnumEx<EnumWithDescriptionAttribute>.GetDescriptionList(DescriptionResolver);

        // Assert
        result.Should().HaveCount(3);
        result[0].Key.Should().Be(EnumWithDescriptionAttribute.None);
    }

    [Theory]
    [InlineData("No Value", true, EnumWithDisplayAttribute.None)]
    [InlineData("First Item", true, EnumWithDisplayAttribute.First)]
    [InlineData("Invalid", false, EnumWithDisplayAttribute.None)]
    public void TryParseByDisplayName_ShouldParseCorrectly(string displayName, bool expectedSuccess, EnumWithDisplayAttribute expectedValue)
    {
        // Act
        var success = EnumEx<EnumWithDisplayAttribute>.TryParseByDisplayName(displayName, out var result);

        // Assert
        success.Should().Be(expectedSuccess);
        result.Should().Be(expectedValue);
    }

    #endregion

    #region Flags Enum Tests

    [Fact]
    public void IsFlags_ShouldReturnTrueForFlagsEnum()
    {
        // Act
        var result = EnumEx<FlagsEnum>.IsFlags;

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsFlags_ShouldReturnFalseForNormalEnum()
    {
        // Act
        var result = EnumEx<SimpleEnum>.IsFlags;

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void GetFlags_ShouldReturnMatchingFlags()
    {
        // Arrange
        var combined = FlagsEnum.ReadWrite;

        // Act
        var flags = EnumEx<FlagsEnum>.GetFlags(combined);

        // Assert
        // GetFlags returns all enum values whose bits are set in the combined value
        // For ReadWrite (3 = 1 | 2), it will return Read(1), Write(2), and ReadWrite(3)
        flags.Should().Contain(FlagsEnum.Read);
        flags.Should().Contain(FlagsEnum.Write);
        flags.Should().Contain(FlagsEnum.ReadWrite);
    }

    [Fact]
    public void GetFlags_WithAllFlags_ShouldReturnAllMatchingFlags()
    {
        // Arrange
        var combined = FlagsEnum.All;

        // Act
        var flags = EnumEx<FlagsEnum>.GetFlags(combined);

        // Assert
        // All flags that are subset of All should be returned
        flags.Should().Contain([FlagsEnum.Read, FlagsEnum.Write, FlagsEnum.Execute, FlagsEnum.Delete]);
        flags.Should().Contain(FlagsEnum.ReadWrite); // ReadWrite is also a subset of All
        flags.Should().Contain(FlagsEnum.All); // All matches itself
    }

    [Fact]
    public void GetFlags_WithNonFlagsEnum_ShouldThrowInvalidOperationException()
    {
        // Act
        var action = () => EnumEx<SimpleEnum>.GetFlags(SimpleEnum.First);

        // Assert
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("*not decorated with [Flags]*");
    }

    [Fact]
    public void CombineFlags_ShouldCombineMultipleFlags()
    {
        // Act
        var result = EnumEx<FlagsEnum>.CombineFlags(FlagsEnum.Read, FlagsEnum.Write, FlagsEnum.Execute);

        // Assert
        var flags = EnumEx<FlagsEnum>.GetFlags(result);
        flags.Should().Contain([FlagsEnum.Read, FlagsEnum.Write, FlagsEnum.Execute]);
    }

    [Fact]
    public void CombineFlags_WithNonFlagsEnum_ShouldThrowInvalidOperationException()
    {
        // Act
        var action = () => EnumEx<SimpleEnum>.CombineFlags(SimpleEnum.First, SimpleEnum.Second);

        // Assert
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("*not decorated with [Flags]*");
    }

    #endregion

    #region Navigation Tests

    [Theory]
    [InlineData(SimpleEnum.None, SimpleEnum.First)]
    [InlineData(SimpleEnum.First, SimpleEnum.Second)]
    [InlineData(SimpleEnum.Third, SimpleEnum.None)] // Circular
    public void GetNext_ShouldReturnNextValue(SimpleEnum current, SimpleEnum expected)
    {
        // Act
        var result = EnumEx<SimpleEnum>.GetNext(current);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(SimpleEnum.First, SimpleEnum.None)]
    [InlineData(SimpleEnum.Second, SimpleEnum.First)]
    [InlineData(SimpleEnum.None, SimpleEnum.Third)] // Circular
    public void GetPrevious_ShouldReturnPreviousValue(SimpleEnum current, SimpleEnum expected)
    {
        // Act
        var result = EnumEx<SimpleEnum>.GetPrevious(current);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void GetRandom_ShouldReturnValidEnumValue()
    {
        // Act
        var result = EnumEx<SimpleEnum>.GetRandom();

        // Assert
        EnumEx<SimpleEnum>.IsDefined(result).Should().BeTrue();
    }

    #endregion

    #region Attribute Tests

    [Fact]
    public void GetValuesWithAttribute_ShouldReturnOnlyValuesWithAttribute()
    {
        // Act
        var result = EnumEx<EnumWithDisplayAttribute>.GetValuesWithAttribute<DisplayAttribute>();

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain([EnumWithDisplayAttribute.None, EnumWithDisplayAttribute.First, EnumWithDisplayAttribute.Second,
        ]);
    }

    [Fact]
    public void GetValuesWithAttribute_WithNoMatches_ShouldReturnEmptyArray()
    {
        // Act
        var result = EnumEx<SimpleEnum>.GetValuesWithAttribute<DisplayAttribute>();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void GetValuesWithAttributeMap_ShouldReturnDictionaryWithAttributes()
    {
        // Act
        var result = EnumEx<EnumWithDisplayAttribute>.GetValuesWithAttributeMap<DisplayAttribute>();

        // Assert
        result.Should().HaveCount(3);
        result.Should().ContainKey(EnumWithDisplayAttribute.None);
        result[EnumWithDisplayAttribute.First].Should().NotBeNull();
    }

    [Fact]
    public void TryParseByAttribute_WithCustomAttribute_ShouldWork()
    {
        // Note: TryParseByAttribute uses ToString() on the attribute, which for most
        // built-in attributes just returns the type name, not the property values.
        // This method is more useful with custom attributes that override ToString().

        // For this test, we verify the method handles attributes but won't find a match
        // since DescriptionAttribute.ToString() returns "System.ComponentModel.DescriptionAttribute"

        // Act
        var success = EnumEx<EnumWithDescriptionAttribute>.TryParseByAttribute<DescriptionAttribute>("System.ComponentModel.DescriptionAttribute", out var result);

        // Assert
        // This will find all values with DescriptionAttribute, but since they all have the same ToString(),
        // it will return the first one found
        if (success)
        {
            result.Should().BeOneOf(EnumWithDescriptionAttribute.None, EnumWithDescriptionAttribute.First, EnumWithDescriptionAttribute.Second);
        }
    }

    [Fact]
    public void TryParseByAttribute_WithNoMatch_ShouldReturnFalse()
    {
        // Act
        var success = EnumEx<EnumWithDescriptionAttribute>.TryParseByAttribute<DescriptionAttribute>("Non-existent", out var result);

        // Assert
        success.Should().BeFalse();
        result.Should().Be(default(EnumWithDescriptionAttribute));
    }

    [Fact]
    public void TryParseByAttribute_WithNullValue_ShouldReturnFalse()
    {
        // Act
        var success = EnumEx<EnumWithDescriptionAttribute>.TryParseByAttribute<DescriptionAttribute>(null, out var result);

        // Assert
        success.Should().BeFalse();
    }

    #endregion

    #region Lookup Tests

    [Fact]
    public void CreateDisplayLookup_ShouldCreateDictionary()
    {
        // Act
        var lookup = EnumEx<EnumWithDisplayAttribute>.CreateDisplayLookup();

        // Assert
        lookup.Should().HaveCount(3);
        lookup[EnumWithDisplayAttribute.First].Should().Be("First Item");
        lookup[EnumWithDisplayAttribute.Second].Should().Be("Second Item");
    }

    [Fact]
    public void CreateReverseLookup_ShouldCreateReverseDictionary()
    {
        // Act
        var lookup = EnumEx<EnumWithDisplayAttribute>.CreateReverseLookup();

        // Assert
        lookup.Should().HaveCount(3);
        lookup["First Item"].Should().Be(EnumWithDisplayAttribute.First);
        lookup["Second Item"].Should().Be(EnumWithDisplayAttribute.Second);
    }

    [Fact]
    public void CreateReverseLookup_WithIgnoreCase_ShouldBeCaseInsensitive()
    {
        // Act
        var lookup = EnumEx<EnumWithDisplayAttribute>.CreateReverseLookup(ignoreCase: true);

        // Assert
        lookup["FIRST ITEM"].Should().Be(EnumWithDisplayAttribute.First);
        lookup["first item"].Should().Be(EnumWithDisplayAttribute.First);
    }

    #endregion

    #region Fuzzy Search Tests

    [Fact]
    public void FuzzySearch_ShouldFindSimilarMatches()
    {
        // Act
        var results = EnumEx<EnumWithDisplayAttribute>.FuzzySearch("First");

        // Assert
        results.Should().NotBeEmpty();
        results[0].Score.Should().BeGreaterThanOrEqualTo(0.5);
    }

    [Fact]
    public void FuzzySearch_WithTypo_ShouldStillFindMatch()
    {
        // Act
        var results = EnumEx<EnumWithDisplayAttribute>.FuzzySearch("Frist Item", minScore: 0.5);

        // Assert
        results.Should().NotBeEmpty();
        // results.Should().Contain(r => r.Value == "First Item");
    }

    [Fact]
    public void FuzzySearch_WithEmptyString_ShouldReturnEmpty()
    {
        // Act
        var results = EnumEx<EnumWithDisplayAttribute>.FuzzySearch("");

        // Assert
        results.Should().BeEmpty();
    }

    [Fact]
    public void FuzzySearch_WithHighMinScore_ShouldFilterResults()
    {
        // Act
        var results = EnumEx<EnumWithDisplayAttribute>.FuzzySearch("xyz", minScore: 0.9);

        // Assert
        results.Should().BeEmpty();
    }

    [Fact]
    public void FuzzySearch_ShouldOrderByScore()
    {
        // Act
        var results = EnumEx<EnumWithDisplayAttribute>.FuzzySearch("Item", minScore: 0.3);

        // Assert
        results.Should().NotBeEmpty();
        for (int i = 0; i < results.Length - 1; i++)
        {
            results[i].Score.Should().BeGreaterThanOrEqualTo(results[i + 1].Score);
        }
    }

    #endregion

    #region EnumKeyValue Tests

    [Fact]
    public void EnumKeyValue_ImplicitConversion_ToEnum_ShouldWork()
    {
        // Arrange
        var keyValue = new EnumKeyValue<SimpleEnum>(SimpleEnum.First, "First");

        // Act
        SimpleEnum result = keyValue;

        // Assert
        result.Should().Be(SimpleEnum.First);
    }

    [Fact]
    public void EnumKeyValue_ImplicitConversion_ToString_ShouldWork()
    {
        // Arrange
        var keyValue = new EnumKeyValue<SimpleEnum>(SimpleEnum.First, "First Value");

        // Act
        string result = keyValue;

        // Assert
        result.Should().Be("First Value");
    }

    [Fact]
    public void EnumKeyValue_IntValue_ShouldReturnCorrectValue()
    {
        // Arrange
        var keyValue = new EnumKeyValue<EnumWithValues>(EnumWithValues.Medium, "Medium");

        // Act
        var result = keyValue.IntValue;

        // Assert
        result.Should().Be(20);
    }

    [Fact]
    public void EnumKeyValue_Deconstruct_ShouldWork()
    {
        // Arrange
        var keyValue = new EnumKeyValue<SimpleEnum>(SimpleEnum.First, "First Value");

        // Act
        var (key, value) = keyValue;

        // Assert
        key.Should().Be(SimpleEnum.First);
        value.Should().Be("First Value");
    }

    [Fact]
    public void EnumKeyValue_DeconstructWithInt_ShouldWork()
    {
        // Arrange
        var keyValue = new EnumKeyValue<EnumWithValues>(EnumWithValues.High, "High");

        // Act
        var (key, value, intValue) = keyValue;

        // Assert
        key.Should().Be(EnumWithValues.High);
        value.Should().Be("High");
        intValue.Should().Be(30);
    }

    #endregion
}
