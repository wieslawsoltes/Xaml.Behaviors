// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xaml.Behaviors.Uno.Headless;
using Xaml.Interactions.Custom.Controls.UnitTests;
using Xunit;

// Start the session with the WinUI control templates (XamlControlsResources) before any test runs:
// the control behaviors need templated controls (ScrollViewer presenters, list and tree containers).
[assembly: AssemblyFixture(typeof(ControlsSessionFixture))]

// All [UnoHeadlessFact] tests share one UI thread and one window; run them one at a time for determinism.
[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Xaml.Interactions.Custom.Controls.UnitTests;

/// <summary>
/// Starts the process-wide headless session with an application that merges the WinUI control resources.
/// </summary>
public sealed class ControlsSessionFixture : IAsyncLifetime
{
    /// <summary>Gets the options the session is started with.</summary>
    public static UnoHeadlessSessionOptions Options { get; } = new()
    {
        ApplicationFactory = static () => new ControlsTestApplication(),
    };

    /// <inheritdoc />
    public async ValueTask InitializeAsync() => await UnoHeadlessSession.StartAsync(Options);

    /// <inheritdoc />
    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}

/// <summary>
/// A test application with the WinUI control styles and templates.
/// </summary>
public sealed class ControlsTestApplication : Application
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ControlsTestApplication"/> class.
    /// </summary>
    public ControlsTestApplication()
    {
        Resources.MergedDictionaries.Add(new XamlControlsResources());
    }
}
