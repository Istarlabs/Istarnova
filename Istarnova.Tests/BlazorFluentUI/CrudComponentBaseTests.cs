#region "Header"

// File: CrudComponentBaseTests.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using Bunit;
using Istarnova.Blazor.FluentUI;
using Istarnova.Blazor.FluentUI.Components;
using Istarnova.Blazor.FluentUI.Models;
using Istarnova.EfCore.Repositories;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;

namespace Istarnova.Tests.BlazorFluentUI;

public class CrudComponentBaseTests : BunitContext
{
    private sealed class Banner
    {
        public int BannerId { get; set; }

        public string Name { get; set; } = string.Empty;
    }

    private sealed class EmptyDialog : ComponentBase
    {
    }

    private sealed class BannerRepository : IRepository<Banner, int>, IQueryableRepository<Banner>, ISearchQueryableRepository<Banner>
    {
        private readonly List<Banner> _banners = [new() { BannerId = 1, Name = "Amber" }, new() { BannerId = 2, Name = "Kaeya" }];

        public List<string> Searches { get; } = [];

        public int Disposed { get; private set; }

        public int Count => _banners.Count;

        public Func<(IDisposable Context, IQueryable<Banner> Query)> CreateQuery
            => () => (new Handle(this), _banners.AsQueryable());

        public Func<string, (IDisposable Context, IQueryable<Banner> Query)> CreateSearchQuery
            => term =>
            {
                Searches.Add(term);

                return (new Handle(this), _banners.Where(banner => banner.Name.Contains(term)).AsQueryable());
            };

        public Banner? this[int id] => _banners.FirstOrDefault(banner => banner.BannerId == id);

        public Task<bool> CreateAsync(Banner item, CancellationToken cancellationToken = default) => Task.FromResult(true);

        public Task<bool> DeleteAsync(Banner item, CancellationToken cancellationToken = default) => Task.FromResult(true);

        public Task<bool> DeleteByIdAsync(int id, CancellationToken cancellationToken = default) => Task.FromResult(true);

        public Task<List<Banner>> GetAllAsync(CancellationToken cancellationToken = default) => Task.FromResult(_banners);

        public Task<Banner?> GetAsync(int id, CancellationToken cancellationToken = default) => Task.FromResult(this[id]);

        public Task<bool> UpdateAsync(Banner item, CancellationToken cancellationToken = default) => Task.FromResult(true);

        private sealed class Handle(BannerRepository owner) : IDisposable
        {
            public void Dispose() => owner.Disposed++;
        }
    }

    private sealed class BannerPage : CrudComponentBase<BannerPage, Banner, int, EmptyDialog>
    {
        public string? Term { get; set; }

        protected override string EntityDisplayName => "Banner";

        protected override string? SearchTerm => Term;

        public IQueryable<Banner>? Loaded => Items;

        public string Title => DialogMessages.AddNewTitle;

        public Task ReloadForTestAsync() => ReloadAsync();

        public Task RemoveForTestAsync(Func<Task<bool>> delete, Func<Task> onDeleted) => RemoveAsync("failed", delete, onDeleted);

        public bool KeyOf(Banner banner, out int key) => TryGetKey(banner, out key);

        protected override string GetEntityName(Banner entity) => entity.Name;

        protected override IQueryable<Banner> ApplyDefaultOrdering(IQueryable<Banner> query) => query.OrderByDescending(banner => banner.Name);

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
        }
    }

    private readonly BannerRepository _repository = new();

    public CrudComponentBaseTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddIstarnovaFluentUI();
        Services.AddSingleton<IRepository<Banner, int>>(_repository);
    }

    [Theory]
    [InlineData("en", "New entry: Banner")]
    [InlineData("de", "Neuer Eintrag: Banner")]
    [InlineData("ja", "新規登録: Banner")]
    public void DialogMessages_ShouldNameTheRecord_InTheLanguageOfTheSession(string language, string expected)
    {
        using var _ = new CultureScope(language);

        var cut = Render<BannerPage>();

        cut.Instance.Title.Should().Be(expected);
    }

    [Fact]
    public void FluentDialogMessageDictionary_ShouldLetAPageOverrideASingleText()
    {
        var messages = new FluentDialogMessageDictionary("Banner") { DeleteConfirmMessage = "Really?" };

        messages.DeleteConfirmMessage.Should().Be("Really?");
        messages.DeleteTitle.Should().Be("Delete entry: Banner");
    }

    [Fact]
    public void OnInitialized_ShouldLoadTheRowsInTheDefaultOrder()
    {
        var cut = Render<BannerPage>();

        cut.Instance.Loaded!.Select(banner => banner.Name).Should().Equal("Kaeya", "Amber");
    }

    [Fact]
    public async Task ReloadAsync_ShouldAskTheRepositoryToSearch_WhenThePageHasATerm()
    {
        var cut = Render<BannerPage>();
        cut.Instance.Term = "Am";

        await cut.InvokeAsync(() => cut.Instance.ReloadForTestAsync());

        _repository.Searches.Should().Equal("Am");
        cut.Instance.Loaded!.Select(banner => banner.Name).Should().Equal("Amber");
    }

    [Fact]
    public async Task ReloadAsync_ShouldReleaseThePreviousContext()
    {
        var cut = Render<BannerPage>();

        await cut.InvokeAsync(() => cut.Instance.ReloadForTestAsync());

        _repository.Disposed.Should().Be(1);
    }

    [Fact]
    public void Dispose_ShouldReleaseTheCurrentContext()
    {
        var cut = Render<BannerPage>();

        cut.Instance.Dispose();

        _repository.Disposed.Should().Be(1);
    }

    [Fact]
    public void TryGetKey_ShouldReadTheKeyByConvention()
    {
        var cut = Render<BannerPage>();

        cut.Instance.KeyOf(new() { BannerId = 7 }, out var key).Should().BeTrue();
        key.Should().Be(7);
    }

    [Fact]
    public async Task RemoveAsync_ShouldReloadTheList_WhenTheRecordWasDeleted()
    {
        var cut = Render<BannerPage>();
        var reloaded = false;

        await cut.InvokeAsync(() => cut.Instance.RemoveForTestAsync(() => Task.FromResult(true), () =>
        {
            reloaded = true;

            return Task.CompletedTask;
        }));

        reloaded.Should().BeTrue();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task RemoveAsync_ShouldNotReloadTheList_WhenTheDeleteFailed(bool throws)
    {
        var cut = Render<BannerPage>();
        var reloaded = false;

        await cut.InvokeAsync(() => cut.Instance.RemoveForTestAsync(
            () => throws ? throw new InvalidOperationException("db down") : Task.FromResult(false),
            () =>
            {
                reloaded = true;

                return Task.CompletedTask;
            }));

        reloaded.Should().BeFalse();
    }
}
