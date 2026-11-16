#region "Header"

// File: LocalizedDataGrid.razor.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using Istarnova.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FluentUI.AspNetCore.Components;

namespace Istarnova.Blazor.FluentUI.Grids;

/// <summary>
///     A data grid that wraps <see cref="FluentDataGrid{TGridItem}" /> with the texts of the session's language and an
///     integrated paginator. A raw <see cref="FluentDataGrid{TGridItem}" /> draws its menus, its empty message and its
///     paginator in English.
/// </summary>
/// <typeparam name="TGridItem">The type of data represented by each row in the grid.</typeparam>
public sealed partial class LocalizedDataGrid<TGridItem> : IDisposable
{
    #region "Variables"

    private const int DefaultItemsPerPage = 10;

    private RenderFragment _emptyContentFragment = null!;

    private RenderFragment _loadingContentFragment = null!;

    private string _memoryKey = string.Empty;

    private GridPageMemory? _pageMemory;

    private bool _pageRestored;

    private PaginationState? _paginationState;

    #endregion

    #region "Properties"

    /// <summary>
    ///     Additional HTML attributes passed through to the underlying FluentDataGrid.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    ///     Whether the number of items per page follows the height available to the grid.
    /// </summary>
    [Parameter]
    public bool AutoItemsPerPage { get; set; }

    /// <summary>
    ///     The column definitions.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    ///     Custom content to display when no data is available.
    /// </summary>
    [Parameter]
    public RenderFragment? EmptyContent { get; set; }

    /// <summary>
    ///     Whether and how the header row is generated.
    /// </summary>
    [Parameter]
    public DataGridGeneratedHeaderType GenerateHeader { get; set; } = DataGridGeneratedHeaderType.Default;

    [Parameter]
    public IQueryable<TGridItem>? Items { get; set; }

    /// <summary>
    ///     The number of items per page, used only when <see cref="Pagination" /> is not provided. Default is 10.
    /// </summary>
    [Parameter]
    public int ItemsPerPage { get; set; } = DefaultItemsPerPage;

    [Parameter]
    public GridItemsProvider<TGridItem>? ItemsProvider { get; set; }

    /// <summary>
    ///     Whether the grid is loading data.
    /// </summary>
    [Parameter]
    public bool? Loading { get; set; }

    /// <summary>
    ///     Custom content to display while loading.
    /// </summary>
    [Parameter]
    public RenderFragment? LoadingContent { get; set; }

    /// <summary>
    ///     Whether cells may wrap. Defaults to true, since single-line rows clip action buttons in the last column.
    /// </summary>
    [Parameter]
    public bool MultiLine { get; set; } = true;

    [Parameter]
    public PaginationState? Pagination { get; set; }

    [Parameter]
    public bool ResizableColumns { get; set; }

    /// <summary>
    ///     Whether the grid pages; without the paginator it shows every row. Default is true.
    /// </summary>
    [Parameter]
    public bool ShowPaginator { get; set; } = true;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = null!;

    [Inject]
    private IServiceProvider Services { get; set; } = null!;

    #endregion

    #region "Methods"

    public void Dispose()
    {
        if (ShowPaginator && _paginationState is { } state)
        {
            _pageMemory?.Remember(_memoryKey, state.CurrentPageIndex);
        }
    }

    protected override void OnInitialized()
    {
        // Remembers the page the grid was left on when the application registered GridPageMemory (AddIstarnovaFluentUI
        // does). It is looked up here and kept: Dispose also runs while the service provider is being torn down, and asking
        // a disposed provider throws.
        _pageMemory = Services.GetService<GridPageMemory>();
        _memoryKey = $"{NavigationManager.ToAbsoluteUri(NavigationManager.Uri).AbsolutePath}|{typeof(TGridItem).FullName}";
        InitializeContentFragments();
    }

    /// <summary>
    ///     Opens on the page the grid was left on; the grid moves back to the last page by itself when fewer rows came back.
    /// </summary>
    protected override async Task OnParametersSetAsync()
    {
        if (_pageRestored || !ShowPaginator || _paginationState is null)
        {
            return;
        }

        _pageRestored = true;

        if (_pageMemory?.Recall(_memoryKey) is > 0 and var pageIndex)
        {
            await _paginationState.SetCurrentPageIndexAsync(pageIndex);
        }
    }

    protected override void OnParametersSet()
    {
        // The page somebody is on lives in this object, so it is created once and kept. Building a new one here threw
        // that away on every parameter change, and a re-render of the page behind the grid is a parameter change -
        // opening a dialog from a row sent the grid back to page one.
        if (Pagination is { } supplied)
        {
            _paginationState = supplied;
        }
        else
        {
            _paginationState ??= new();

            if (_paginationState.ItemsPerPage != ItemsPerPage)
            {
                _paginationState.ItemsPerPage = ItemsPerPage;
            }
        }
    }

    private void InitializeContentFragments()
    {
        _emptyContentFragment = EmptyContent ?? (builder =>
        {
            builder.OpenComponent<FluentMessageBar>(0);
            builder.AddAttribute(1, "Intent", MessageBarIntent.Info);
            builder.AddAttribute(2, "ChildContent",
                (RenderFragment)(content => content.AddContent(0, ComponentResources.NoDataText)));
            builder.CloseComponent();
        });

        _loadingContentFragment = LoadingContent ?? (builder =>
        {
            builder.OpenComponent<FluentStack>(0);
            builder.AddAttribute(1, "Orientation", Orientation.Horizontal);
            builder.AddAttribute(2, "Style", "gap:0.5rem; align-items:center; padding:1rem;");
            builder.AddAttribute(3, "ChildContent", (RenderFragment)(content =>
            {
                content.OpenComponent<FluentSpinner>(0);
                content.AddAttribute(1, "Style", "width:24px; height:24px;");
                content.CloseComponent();

                content.OpenElement(2, "span");
                content.AddContent(3, ComponentResources.LoadingText);
                content.CloseElement();
            }));
            builder.CloseComponent();
        });
    }

    #endregion
}
