// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Xaml.Behaviors.Uno.Headless;
using Xaml.Behaviors.Uno.Headless.UnitTests;
using Xunit;

// Start the session with custom options before any test runs (the documented configuration pattern).
[assembly: AssemblyFixture(typeof(TestSessionFixture))]

// All [UnoHeadlessFact] tests share one UI thread and one window; run them one at a time for determinism.
[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Xaml.Behaviors.Uno.Headless.UnitTests;

/// <summary>
/// Starts the process-wide headless session with <see cref="Options"/>.
/// </summary>
public sealed class TestSessionFixture : IAsyncLifetime
{
    /// <summary>The raw pixel width of the test window.</summary>
    public const int Width = 800;

    /// <summary>The raw pixel height of the test window.</summary>
    public const int Height = 600;

    /// <summary>The rasterization scale of the test window.</summary>
    public const float Scale = 2f;

    /// <summary>Gets the options the session is started with.</summary>
    public static UnoHeadlessSessionOptions Options { get; } = new()
    {
        Width = Width,
        Height = Height,
        Scale = Scale,
        ApplicationFactory = static () => new TestApplication(),
    };

    /// <inheritdoc />
    public async ValueTask InitializeAsync() => await UnoHeadlessSession.StartAsync(Options);

    /// <inheritdoc />
    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}

/// <summary>
/// A custom application, used to verify <see cref="UnoHeadlessSessionOptions.ApplicationFactory"/>.
/// </summary>
public sealed class TestApplication : Application
{
    /// <summary>The key of the resource added by the application.</summary>
    public const string MarkerKey = "TestApplicationMarker";

    /// <summary>
    /// Initializes a new instance of the <see cref="TestApplication"/> class.
    /// </summary>
    public TestApplication()
    {
        Resources[MarkerKey] = nameof(TestApplication);
    }

    /// <summary>Gets a value indicating whether <c>OnLaunched</c> ran.</summary>
    public bool Launched { get; private set; }

    /// <inheritdoc />
    protected override void OnLaunched(LaunchActivatedEventArgs args) => Launched = true;
}
