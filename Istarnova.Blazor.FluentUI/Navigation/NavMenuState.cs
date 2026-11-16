#region "Header"

// File: NavMenuState.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

namespace Istarnova.Blazor.FluentUI.Navigation;

/// <summary>
///     Which groups of the menu are open. Kept outside the entries because the menu re-renders on every navigation, and a
///     fixed <c>Expanded</c> value would fold it shut again each time.
/// </summary>
internal sealed class NavMenuState
{
    #region "Variables"

    private readonly Dictionary<NavigationMenuObject, int> _foldVersions = [];

    #endregion

    #region "Properties"

    /// <summary>
    ///     Groups the user - or the current page - has opened.
    /// </summary>
    public HashSet<NavigationMenuObject> ExpandedGroups { get; } = [];

    #endregion

    #region "Methods"

    /// <summary>
    ///     FluentUI animates a group by writing its height into the element, and folding it through the <c>Expanded</c>
    ///     parameter alone leaves that height behind: the group is shut, its entries are hidden and the space stays. A new
    ///     key renders the group afresh.
    /// </summary>
    public (NavigationMenuObject Group, int Version) FoldKey(NavigationMenuObject group)
        => (group, _foldVersions.GetValueOrDefault(group));

    public bool IsExpanded(NavigationMenuObject group)
        => ExpandedGroups.Contains(group);

    /// <summary>
    ///     Counts that the menu itself, not the user, opened or folded a group.
    /// </summary>
    public void MarkFolded(NavigationMenuObject group)
        => _foldVersions[group] = _foldVersions.GetValueOrDefault(group) + 1;

    public void SetExpanded(NavigationMenuObject group, bool expanded)
    {
        if (expanded)
        {
            ExpandedGroups.Add(group);

            return;
        }

        ExpandedGroups.Remove(group);
    }

    #endregion
}
