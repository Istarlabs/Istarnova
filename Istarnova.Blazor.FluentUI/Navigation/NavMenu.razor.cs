#region "Header"

// File: NavMenu.razor.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using Istarnova.Blazor.State;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace Istarnova.Blazor.FluentUI.Navigation;

/// <summary>
///     The navigation menu: renders the entries of <see cref="INavigationService" />, opens the groups that lead to the
///     current page and folds them shut when the user leaves.
/// </summary>
public sealed partial class NavMenu : IDisposable
{
    #region "Variables"

    private List<NavigationMenuObject> _items = [];

    private string _lastPath = string.Empty;

    private readonly NavMenuState _state = new();

    #endregion

    #region "Properties"

    /// <summary>
    ///     The accessible name of the navigation; "Main navigation" in the session's language when omitted.
    /// </summary>
    [Parameter]
    public string? AriaLabel { get; set; }

    /// <summary>
    ///     The menu to render. Defaults to the main menu of <see cref="INavigationService" />.
    /// </summary>
    [Parameter]
    public IEnumerable<NavigationMenuObject>? Items { get; set; }

    /// <summary>
    ///     A state whose change makes the menu ask its policies again - an entry gated by a policy that depends on it
    ///     has to follow the moment the state changes, not at the next click.
    /// </summary>
    [Parameter]
    public IObservableState? ReevaluateOn { get; set; }

    /// <summary>
    ///     The width of the menu.
    /// </summary>
    [Parameter]
    public string Width { get; set; } = "250px";

    [Inject]
    private NavigationManager NavigationManager { get; set; } = null!;

    [Inject]
    private INavigationService NavigationService { get; set; } = null!;

    #endregion

    #region "Methods"

    public void Dispose()
    {
        NavigationManager.LocationChanged -= OnLocationChanged;
        ReevaluateOn?.Unsubscribe(this);
    }

    protected override void OnInitialized()
    {
        NavigationManager.LocationChanged += OnLocationChanged;
        ReevaluateOn?.Subscribe(this, () => _ = InvokeAsync(StateHasChanged));

        _items = [.. Items ?? NavigationService.BuildMainMenu()];
        _lastPath = GetCurrentPath();

        ExpandGroupsOfCurrentPage();
    }

    private void ExpandGroupsOfCurrentPage()
        => ExpandGroupsOfCurrentPage(_items, GetCurrentPath());

    /// <returns><c>true</c> when one of the given items - or one of their sub-items - is the current page.</returns>
    private bool ExpandGroupsOfCurrentPage(List<NavigationMenuObject> items, string currentPath)
    {
        var containsCurrentPage = false;

        foreach (var item in items)
        {
            // A group may carry a URL of its own (a "patients" group covers every /patients/... page), so both it and its
            // children decide whether the group belongs to the current page.
            var isCurrent = IsCurrentPage(item, currentPath);

            if (item.HasSubItems && ExpandGroupsOfCurrentPage(item.SubItems, currentPath))
            {
                isCurrent = true;
            }

            if (isCurrent && item.HasSubItems)
            {
                _state.ExpandedGroups.Add(item);
            }

            containsCurrentPage |= isCurrent;
        }

        return containsCurrentPage;
    }

    private string GetCurrentPath()
    {
        var path = "/" + NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
        var queryStart = path.IndexOfAny(['?', '#']);

        if (queryStart >= 0)
        {
            path = path[..queryStart];
        }

        return path.TrimEnd('/');
    }

    internal static bool IsCurrentPage(NavigationMenuObject item, string currentPath)
    {
        if (string.IsNullOrEmpty(item.Url))
        {
            return false;
        }

        var url = item.Url.TrimEnd('/');

        // The root link only ever matches the root itself; as a prefix it would match every page.
        if (url.Length == 0)
        {
            return currentPath.Length == 0;
        }

        if (string.Equals(currentPath, url, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return item.Match == NavLinkMatch.Prefix
               && currentPath.StartsWith(url + "/", StringComparison.OrdinalIgnoreCase);
    }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        var path = GetCurrentPath();

        // An address that only gained a fragment or a query is the same page: nothing to open or fold.
        if (string.Equals(path, _lastPath, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        _lastPath = path;

        // Leaving a group's pages folds it shut; one opened by hand stays open only until the next navigation.
        var before = _state.ExpandedGroups.ToHashSet();

        _state.ExpandedGroups.Clear();
        ExpandGroupsOfCurrentPage();

        before.SymmetricExceptWith(_state.ExpandedGroups);

        foreach (var changed in before)
        {
            _state.MarkFolded(changed);
        }

        StateHasChanged();
    }

    #endregion
}
