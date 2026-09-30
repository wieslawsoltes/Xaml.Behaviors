// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xaml.Behaviors.Uno.Headless;
using Xaml.Interactions.UnitTests;
using Xunit;

// The session loads the WinUI control templates (XamlControlsResources) before any test runs, so every test sees the
// same controls (ScrollViewer, TabView, TextBox, ...) regardless of the test order, like the Avalonia test application.
[assembly: AssemblyFixture(typeof(InteractionsSessionFixture))]

// All [UnoHeadlessFact] tests share one UI thread and one window; run them one at a time for determinism.
[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Xaml.Interactions.UnitTests;

/// <summary>
/// Starts the headless session with <see cref="InteractionsTestApplication"/>.
/// </summary>
public sealed class InteractionsSessionFixture : IAsyncLifetime
{
    /// <summary>Gets the options the session is started with.</summary>
    public static UnoHeadlessSessionOptions Options { get; } = new()
    {
        ApplicationFactory = static () => new InteractionsTestApplication(),
    };

    /// <inheritdoc />
    public async ValueTask InitializeAsync() => await UnoHeadlessSession.StartAsync(Options);

    /// <inheritdoc />
    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}

/// <summary>
/// The test application: loads the WinUI control templates.
/// </summary>
public sealed class InteractionsTestApplication : Application
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InteractionsTestApplication"/> class.
    /// </summary>
    public InteractionsTestApplication()
    {
        Resources.MergedDictionaries.Add(new XamlControlsResources());
    }
}
