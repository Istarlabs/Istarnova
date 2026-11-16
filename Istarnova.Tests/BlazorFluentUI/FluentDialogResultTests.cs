#region "Header"

// File: FluentDialogResultTests.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using Istarnova.Blazor.FluentUI.Models;

namespace Istarnova.Tests.BlazorFluentUI;

public class FluentDialogResultTests
{
    private sealed class Record;

    [Fact]
    public void OkUpdate_ShouldCarryTheRecord()
    {
        var record = new Record();

        var result = FluentDialogResult<Record>.OkUpdate(record);

        result.Result.Should().Be(FluentDialogResult.OkUpdate);
        result.Result.IsOkUpdate().Should().BeTrue();
        result.Data.Should().BeSameAs(record);
    }

    [Fact]
    public void Cancel_ShouldCarryNoRecord()
    {
        var result = FluentDialogResult<Record>.CancelDelete();

        result.Result.IsCancelDelete().Should().BeTrue();
        result.Data.Should().BeNull();
    }

    [Theory]
    [InlineData(FluentDialogResult.Ok)]
    [InlineData(FluentDialogResult.ErrorCreate)]
    [InlineData(FluentDialogResult.None)]
    public void TheCheckMethods_ShouldAnswerTrueForTheirOwnValueOnly(FluentDialogResult value)
    {
        var checks = new Dictionary<FluentDialogResult, bool>
        {
            [FluentDialogResult.Ok] = value.IsOk(),
            [FluentDialogResult.ErrorCreate] = value.IsErrorCreate(),
            [FluentDialogResult.None] = value.IsNone(),
        };

        checks.Where(check => check.Value).Select(check => check.Key).Should().Equal(value);
    }
}
