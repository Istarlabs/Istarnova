// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

using Istarnova.Collections.Generics;

namespace Istarnova.Tests.Collections.Generics;

public class ArrayExtensionsTests
{
    [Fact]
    public void Transpose_ShouldTranspose2DArray_WhenValidArrayProvided()
    {
        // Arrange
        var array = new int[,]
        {
            { 1, 2, 3 },
            { 4, 5, 6 }
        };

        // Act
        var result = array.Transpose();

        // Assert
        result.GetLength(0).Should().Be(3); // 3 rows in transposed array
        result.GetLength(1).Should().Be(2); // 2 columns in transposed array
        result[0, 0].Should().Be(1);
        result[0, 1].Should().Be(4);
        result[1, 0].Should().Be(2);
        result[1, 1].Should().Be(5);
        result[2, 0].Should().Be(3);
        result[2, 1].Should().Be(6);
    }

    [Fact]
    public void Transpose_ShouldThrowArgumentNullException_WhenArrayIsNull()
    {
        // Arrange
        int[,]? nullArray = null;

        // Act & Assert
        var action = () => nullArray!.Transpose();
        action.Should().Throw<ArgumentNullException>();
    }
}
