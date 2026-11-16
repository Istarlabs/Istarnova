#region "Header"

// File: ObservableStateTests.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using Istarnova.Blazor.State;

namespace Istarnova.Tests.Blazor;

public class ObservableStateTests
{
    private sealed class CountingState : ObservableState
    {
        public void Change() => NotifyChanged();
    }

    [Fact]
    public void NotifyChanged_ShouldCallEverySubscriber()
    {
        var state = new CountingState();
        var calls = 0;
        state.Subscribe(new object(), () => calls++);
        state.Subscribe(new object(), () => calls++);

        state.Change();

        calls.Should().Be(2);
    }

    [Fact]
    public void Subscribe_ShouldReplaceTheEarlierRegistrationOfTheSameOwner()
    {
        var state = new CountingState();
        var owner = new object();
        var first = 0;
        var second = 0;
        state.Subscribe(owner, () => first++);
        state.Subscribe(owner, () => second++);

        state.Change();

        first.Should().Be(0);
        second.Should().Be(1);
    }

    [Fact]
    public void Unsubscribe_ShouldStopTheCalls_AndAcceptAnOwnerThatNeverSubscribed()
    {
        var state = new CountingState();
        var owner = new object();
        var calls = 0;
        state.Subscribe(owner, () => calls++);

        state.Unsubscribe(owner);
        state.Unsubscribe(new object());
        state.Change();

        calls.Should().Be(0);
    }

    [Fact]
    public void NotifyChanged_ShouldSurviveASubscriberThatUnsubscribesWhileBeingCalled()
    {
        var state = new CountingState();
        var owner = new object();
        var other = 0;
        state.Subscribe(owner, () => state.Unsubscribe(owner));
        state.Subscribe(new object(), () => other++);

        var act = state.Change;

        act.Should().NotThrow();
        other.Should().Be(1);
    }

    [Fact]
    public void Dispose_ShouldDropAllSubscribers()
    {
        var state = new CountingState();
        var calls = 0;
        state.Subscribe(new object(), () => calls++);

        state.Dispose();
        state.Change();

        calls.Should().Be(0);
    }

    [Fact]
    public void Subscribe_ShouldRefuseNulls()
    {
        var state = new CountingState();

        state.Invoking(subject => subject.Subscribe(null!, () => { })).Should().Throw<ArgumentNullException>();
        state.Invoking(subject => subject.Subscribe(new object(), null!)).Should().Throw<ArgumentNullException>();
    }
}
