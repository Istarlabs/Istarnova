// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

namespace Istarnova.Tests.SourceGenerator.Equatable;

public class AutoEquatableGeneratorTests
{
    [Fact]
    public void Equals_ShouldBeTrue_WhenKeysMatch()
    {
        var fromQuery = new SingleKeyEntity { SingleKeyEntityId = 3, Name = "Vitamin D" };
        var fromGraph = new SingleKeyEntity { SingleKeyEntityId = 3 };

        fromQuery.Should().Be(fromGraph);
        (fromQuery == fromGraph).Should().BeTrue();
        fromQuery.GetHashCode().Should().Be(fromGraph.GetHashCode());
    }

    [Fact]
    public void Equals_ShouldBeFalse_WhenKeysDiffer()
    {
        var first = new SingleKeyEntity { SingleKeyEntityId = 3 };
        var second = new SingleKeyEntity { SingleKeyEntityId = 24 };

        first.Should().NotBe(second);
    }

    [Fact]
    public void Equals_ShouldBeFalse_WhenBothAreUnsaved()
    {
        var first = new SingleKeyEntity { Name = "first" };
        var second = new SingleKeyEntity { Name = "second" };

        first.Should().NotBe(second);
        (first == second).Should().BeFalse();
        new List<SingleKeyEntity> { first }.Remove(second).Should().BeFalse();
    }

    [Fact]
    public void Equals_ShouldBeTrue_WhenAnUnsavedObjectIsComparedWithItself()
    {
        var unsaved = new SingleKeyEntity();

        unsaved.Equals(unsaved).Should().BeTrue();
        new List<SingleKeyEntity> { unsaved }.Should().Contain(unsaved);
    }

    [Fact]
    public void Equals_ShouldCompareByKey_WhenOnlyPartOfACompositeKeyIsDefault()
    {
        var first = new CompositeKeyEntity { OwnerId = 0, ItemId = 7 };
        var second = new CompositeKeyEntity { OwnerId = 0, ItemId = 7 };

        first.Should().Be(second);
    }

    [Fact]
    public void Equals_ShouldBeFalse_WhenEveryPartOfACompositeKeyIsDefault()
    {
        new CompositeKeyEntity().Should().NotBe(new CompositeKeyEntity());
    }

    [Fact]
    public void Equals_ShouldCompareByKey_WhenTheKeyIsAStringAtItsDefault()
    {
        new StringKeyEntity().Should().Be(new StringKeyEntity());
    }
}
