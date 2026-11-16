#region "Header"

// File: AuthenticationCircuitHandlerTests.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using System.Security.Claims;
using Istarnova.Blazor.State;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.AspNetCore.Components.Server.Circuits;

namespace Istarnova.Tests.Blazor;

public class AuthenticationCircuitHandlerTests
{
    private interface ITestState : ICircuitUserState
    {
    }

    private sealed class RecordingState : ITestState
    {
        public List<string?> Users { get; } = [];

        public bool Fail { get; set; }

        public Task InitializeAsync(ClaimsPrincipal principal)
        {
            if (Fail)
            {
                throw new InvalidOperationException("no");
            }

            Users.Add(principal.Identity?.Name);

            return Task.CompletedTask;
        }
    }

    private sealed class FakeProvider : AuthenticationStateProvider
    {
        private ClaimsPrincipal _user = Principal("first");

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
            => Task.FromResult(new AuthenticationState(_user));

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "BL0013", Justification = "The fake is the provider; it raises the change event itself.")]
        public void SignIn(string name)
        {
            _user = Principal(name);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        private static ClaimsPrincipal Principal(string name)
            => new(new ClaimsIdentity([new Claim(ClaimTypes.Name, name)], "test", ClaimTypes.Name, ClaimTypes.Role));
    }

    private sealed class WithExtraStep(FakeProvider provider, RecordingState state)
        : AuthenticationCircuitHandler<RecordingState>(provider, state, NullLogger<AuthenticationCircuitHandler<RecordingState>>.Instance)
    {
        public int Extra { get; private set; }

        protected override async Task OnAuthenticationAsync(AuthenticationState authentication)
        {
            await base.OnAuthenticationAsync(authentication);
            Extra++;
        }
    }

    private static AuthenticationCircuitHandler<RecordingState> Handler(FakeProvider provider, RecordingState state)
        => new(provider, state, NullLogger<AuthenticationCircuitHandler<RecordingState>>.Instance);

    [Fact]
    public async Task OnConnectionUpAsync_ShouldInitialiseTheStateForTheSignedInUser()
    {
        var state = new RecordingState();
        var handler = Handler(new FakeProvider(), state);

        await handler.OnConnectionUpAsync(null!, CancellationToken.None);

        state.Users.Should().Equal("first");
    }

    [Fact]
    public async Task AuthenticationChange_ShouldInitialiseTheStateAgain_OnceTheCircuitIsOpen()
    {
        var provider = new FakeProvider();
        var state = new RecordingState();
        using var handler = Handler(provider, state);
        await handler.OnCircuitOpenedAsync(null!, CancellationToken.None);

        provider.SignIn("second");
        await Task.Delay(50);

        state.Users.Should().Equal("second");
    }

    [Fact]
    public async Task Dispose_ShouldStopFollowingTheAuthentication()
    {
        var provider = new FakeProvider();
        var state = new RecordingState();
        var handler = Handler(provider, state);
        await handler.OnCircuitOpenedAsync(null!, CancellationToken.None);

        handler.Dispose();
        provider.SignIn("second");
        await Task.Delay(50);

        state.Users.Should().BeEmpty();
    }

    [Fact]
    public async Task OnConnectionUpAsync_ShouldNotTakeTheCircuitDown_WhenTheStateFails()
    {
        var state = new RecordingState { Fail = true };
        var handler = Handler(new FakeProvider(), state);

        var act = () => handler.OnConnectionUpAsync(null!, CancellationToken.None);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task OnAuthenticationAsync_ShouldBeOverridableForTheApplicationsOwnSteps()
    {
        var state = new RecordingState();
        var handler = new WithExtraStep(new FakeProvider(), state);

        await handler.OnConnectionUpAsync(null!, CancellationToken.None);

        state.Users.Should().HaveCount(1);
        handler.Extra.Should().Be(1);
    }

    [Fact]
    public void AddCircuitUserState_ShouldRegisterTheStateAndTheHandlerOnce()
    {
        var services = new ServiceCollection();

        services.AddCircuitUserState<ITestState, RecordingState>();
        services.AddCircuitUserState<ITestState, RecordingState>();

        services.Should().ContainSingle(descriptor => descriptor.ServiceType == typeof(ITestState) && descriptor.Lifetime == ServiceLifetime.Scoped);
        services.Count(descriptor => descriptor.ServiceType == typeof(CircuitHandler)
                                     && descriptor.ImplementationType == typeof(AuthenticationCircuitHandler<ITestState>))
            .Should().Be(1);
    }
}
