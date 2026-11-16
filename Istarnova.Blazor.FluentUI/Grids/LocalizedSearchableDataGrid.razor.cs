#region "Header"

// File: LocalizedSearchableDataGrid.razor.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using Istarnova.Blazor.Components;
using Istarnova.Blazor.Errors;
using Istarnova.Blazor.Logging;
using Istarnova.EfCore.Repositories;
using Istarnova.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.FluentUI.AspNetCore.Components;

namespace Istarnova.Blazor.FluentUI.Grids;

/// <summary>
///     A <see cref="LocalizedDataGrid{TGridItem}" /> that pages a repository query and searches it.
/// </summary>
/// <typeparam name="TGridItem">The entity the grid lists.</typeparam>
public sealed partial class LocalizedSearchableDataGrid<TGridItem> : ComponentBase, IRefreshableList
    where TGridItem : class
{
    #region "Variables"

    private readonly PaginationState _pagination = new();

    private RenderFragment? _emptyContent;

    private string _errorMessage = string.Empty;

    private bool _loading;

    private GridItemsProvider<TGridItem>? _provider;

    // The component owns the term rather than taking it as a parameter: nothing outside needs it, and a parameter
    // assigned from inside is overwritten the next time the parent renders.
    private string _searchTerm = string.Empty;

    #endregion

    #region "Properties"

    /// <summary>
    ///     The column definitions.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    ///     Ordering to apply when the grid has no sort of its own.
    /// </summary>
    [Parameter]
    public Func<IQueryable<TGridItem>, IQueryable<TGridItem>>? DefaultOrder { get; set; }

    /// <summary>
    ///     Shown when the list is empty and nothing was typed.
    /// </summary>
    [Parameter]
    public string? EmptyMessage { get; set; }

    /// <summary>
    ///     Shown when a load failed, ahead of what went wrong.
    /// </summary>
    [Parameter]
    public string? ErrorMessage { get; set; }

    /// <summary>
    ///     Whether and how the header row is generated.
    /// </summary>
    [Parameter]
    public DataGridGeneratedHeaderType GenerateHeader { get; set; } = DataGridGeneratedHeaderType.Default;

    /// <summary>
    ///     The column widths as a CSS <c>grid-template-columns</c> value, or null to let the grid size the columns itself.
    /// </summary>
    [Parameter]
    public string? GridTemplateColumns { get; set; }

    /// <summary>
    ///     How many rows a page holds.
    /// </summary>
    [Parameter]
    public int ItemsPerPage { get; set; } = 10;

    /// <summary>
    ///     Whether cells may wrap.
    /// </summary>
    [Parameter]
    public bool MultiLine { get; set; } = true;

    /// <summary>
    ///     What the caller adds to every query: its own filter, and the includes its columns read.
    /// </summary>
    [Parameter]
    public Func<IQueryable<TGridItem>, IQueryable<TGridItem>>? Narrow { get; set; }

    /// <summary>
    ///     Shown when a search matched nothing; falls back to <see cref="EmptyMessage" />.
    /// </summary>
    [Parameter]
    public string? NoMatchMessage { get; set; }

    /// <summary>
    ///     Placeholder of the search box.
    /// </summary>
    [Parameter]
    public string? Placeholder { get; set; }

    /// <summary>
    ///     Where the rows come from. Searching is offered when it also implements <see cref="ISearchQueryableRepository{T}" />.
    /// </summary>
    [Parameter]
    [EditorRequired]
    public IQueryableRepository<TGridItem> Repository { get; set; } = null!;

    /// <summary>
    ///     Whether the columns can be resized.
    /// </summary>
    [Parameter]
    public bool ResizableColumns { get; set; }

    /// <summary>
    ///     What the search box holds right now, empty when nothing is typed. For a <see cref="Narrow" /> that has to know
    ///     whether a search is running.
    /// </summary>
    public string SearchTerm
        => _searchTerm;

    /// <summary>
    ///     Whether the search box holds nothing, or only white space, so no search is running.
    /// </summary>
    public bool IsSearchTermEmpty
        => _searchTerm.IsEmptyOrWhiteSpace();

    /// <summary>
    ///     Whether the paginator is rendered.
    /// </summary>
    [Parameter]
    public bool ShowPaginator { get; set; } = true;

    /// <summary>
    ///     Whether a search box is offered at all.
    /// </summary>
    private bool CanSearch
        => Repository is ISearchQueryableRepository<TGridItem>;

    /// <summary>
    ///     What the grid's empty row says: that nothing matched, or that there is nothing yet.
    /// </summary>
    private string EmptyStateMessage
        => IsSearchTermEmpty
            ? EmptyMessage ?? string.Empty
            : NoMatchMessage ?? EmptyMessage ?? string.Empty;

    /// <summary>
    ///     What the wrapped grid receives beyond its declared parameters. Only what is set: a null handed down as an
    ///     undeclared attribute would be the one path no page takes today.
    /// </summary>
    private Dictionary<string, object>? GridAttributes
        => GridTemplateColumns is { } columns
            ? new() { [nameof(GridTemplateColumns)] = columns }
            : null;

    [Inject]
    private ILogger<LocalizedSearchableDataGrid<TGridItem>> Logger { get; set; } = null!;

    #endregion

    #region "Methods"

    /// <summary>
    ///     Fetches the current page again, for a caller that changed a row behind the grid's back.
    /// </summary>
    public Task RefreshAsync()
        => ReloadAsync(_pagination.CurrentPageIndex);

    /// <summary>
    ///     Fetches the first page, for a caller that changed what <see cref="Narrow" /> filters by.
    /// </summary>
    public Task ReloadFromFirstPageAsync()
        => ReloadAsync(0);

    protected override void OnInitialized()
    {
        _pagination.ItemsPerPage = ItemsPerPage;
        _provider = LoadPageAsync;

        _emptyContent = builder =>
        {
            builder.OpenComponent<FluentMessageBar>(0);
            builder.AddComponentParameter(1, nameof(FluentMessageBar.Intent), MessageBarIntent.Info);
            builder.AddComponentParameter(2, nameof(FluentMessageBar.ChildContent),
                (RenderFragment)(content => content.AddContent(0, EmptyStateMessage)));
            builder.CloseComponent();
        };
    }

    private async ValueTask<GridItemsProviderResult<TGridItem>> LoadPageAsync(GridItemsProviderRequest<TGridItem> request)
    {
        _loading = true;
        _errorMessage = string.Empty;

        try
        {
            var (context, source) = !IsSearchTermEmpty && Repository is ISearchQueryableRepository<TGridItem> searchable
                ? searchable.CreateSearchQuery(_searchTerm)
                : Repository.CreateQuery();

            // Disposed with this one call and never held in a field. Two provider runs overlap whenever something
            // re-renders the grid while a query is in flight - typing in the search box does it every keystroke - and
            // the second run disposing the field killed the context the first was still reading from.
            using var _ = context;

            var query = Narrow is null ? source : Narrow(source);

            query = request.GetSortByProperties().Count != 0
                ? request.ApplySorting(query)
                : DefaultOrder?.Invoke(query) ?? query;

            var totalCount = await query.CountAsync(request.CancellationToken);

            var items = await query
                .Skip(request.StartIndex)
                .Take(request.Count ?? _pagination.ItemsPerPage)
                .AsNoTracking()
                .ToListAsync(request.CancellationToken);

            return GridItemsProviderResult.From(items, totalCount);
        }
        catch (OperationCanceledException)
        {
            // The grid cancels the run in flight whenever it starts another, which typing in the search box does on every
            // keystroke. That is the search working. Nothing is reported and no state is written: the run that replaced
            // this one carries the errors.
            return GridItemsProviderResult.From(Array.Empty<TGridItem>(), 0);
        }
        catch (Exception exception)
        {
            Logger.LogComponentLoadFailed(exception, typeof(TGridItem).Name);
            _errorMessage = UserFacingError.Describe(exception, ErrorMessage ?? string.Empty);

            return GridItemsProviderResult.From(Array.Empty<TGridItem>(), 0);
        }
        finally
        {
            _loading = false;

            // The error bar and the grid's own empty row only know what to show once the run is over.
            await InvokeAsync(StateHasChanged);
        }
    }

    /// <summary>
    ///     Back to the first page, because the term changed what there is to page through.
    /// </summary>
    private Task OnSearchChangedAsync()
        => ReloadAsync(0);

    /// <summary>
    ///     Shows the grid as loading, then fetches the given page.
    /// </summary>
    /// <param name="pageIndex">The page to land on.</param>
    private async Task ReloadAsync(int pageIndex)
    {
        // Cleared here and not only in the provider: an error hides nothing but still shows, and the provider is what
        // would clear it - so a term that failed would keep its message over the results of the next one.
        _errorMessage = string.Empty;
        _loading = true;

        StateHasChanged();

        await _pagination.SetCurrentPageIndexAsync(pageIndex);
    }

    #endregion
}
