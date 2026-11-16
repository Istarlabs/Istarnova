#region "Header"

// File: PageErrorBoundary.razor.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using System.Globalization;
using Istarnova.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.Logging;

namespace Istarnova.Blazor.FluentUI.Errors;

/// <summary>
///     Catches what a page or tab throws and shows a panel in its place, so the circuit survives. The panel carries a short
///     reference the person can read out when they report the problem.
/// </summary>
public sealed partial class PageErrorBoundary : ErrorBoundaryBase, IDisposable
{
    #region "Variables"

    private string _errorReference = string.Empty;

    #endregion

    #region "Properties"

    /// <summary>
    ///     More buttons next to the default ones (reporting the problem to an administrator, say). Receives the reference.
    /// </summary>
    [Parameter]
    public RenderFragment<string>? Actions { get; set; }

    /// <summary>
    ///     Where "go to the start page" leads.
    /// </summary>
    [Parameter]
    public string StartPage { get; set; } = "/";

    private string ErrorReferenceText
        => string.Format(CultureInfo.CurrentCulture, ComponentResources.ErrorReference, _errorReference);

    [Inject]
    private ILogger<PageErrorBoundary> Logger { get; set; } = null!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = null!;

    #endregion

    #region "Methods"

    public void Dispose()
        => NavigationManager.LocationChanged -= OnLocationChanged;

    protected override void OnInitialized()
        => NavigationManager.LocationChanged += OnLocationChanged;

    /// <summary>
    ///     Records the failure under a reference short enough to be read out over the phone.
    /// </summary>
    // Eight hex characters, not a full GUID: it only has to be unique among the errors one instance keeps, and a person
    // copying it into a bug report will not copy thirty-two.
    protected override Task OnErrorAsync(Exception exception)
    {
        _errorReference = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();

        LogUnhandledComponentError(Logger, exception, _errorReference, NavigationManager.ToBaseRelativePath(NavigationManager.Uri));

        return Task.CompletedTask;
    }

    private void GoToStartPage()
    {
        Recover();
        NavigationManager.NavigateTo(StartPage);
    }

    /// <summary>
    ///     Clears the panel when the person navigates away, so the failure of one page does not follow them to the next one.
    /// </summary>
    private void OnLocationChanged(object? sender, LocationChangedEventArgs eventArgs)
    {
        if (CurrentException is null)
        {
            return;
        }

        Recover();
    }

    private void RecoverAndRerender()
        => Recover();

    #endregion
}
