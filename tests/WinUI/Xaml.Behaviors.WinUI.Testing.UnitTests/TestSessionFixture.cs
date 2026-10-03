// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Xaml.Behaviors.Uno.Headless.UnitTests;
using Xaml.Behaviors.WinUI.Testing;
using Xunit;

[assembly: AssemblyFixture(typeof(TestSessionFixture))]

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Xaml.Behaviors.Uno.Headless.UnitTests;

/// <summary>
/// Starts the WinUI test session with the options of the shared harness tests (WinUI twin of the Uno fixture).
/// </summary>
public sealed class TestSessionFixture : IAsyncLifetime
{
    /// <summary>The width of the test window content.</summary>
    public const int Width = 800;

    /// <summary>The height of the test window content.</summary>
    public const int Height = 600;

    /// <summary>The requested rasterization scale (a WinUI window uses the scale of its monitor).</summary>
    public const float Scale = 2f;

    /// <summary>Gets the options the session is started with.</summary>
    public static WinUITestSessionOptions Options { get; } = new()
    {
        Width = Width,
        Height = Height,
        Scale = Scale,
        ApplicationFactory = static () => new TestApplication(),
    };

    /// <inheritdoc />
    public async ValueTask InitializeAsync() => await WinUITestSession.StartAsync(Options);

    /// <inheritdoc />
    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}

/// <summary>
/// The application of the test session.
/// </summary>
public sealed partial class TestApplication : WinUITestApplication
{
    /// <summary>The key of the resource added by the application.</summary>
    public const string MarkerKey = "TestApplicationMarker";

    /// <summary>Gets a value indicating whether <c>OnLaunched</c> ran.</summary>
    public bool Launched { get; private set; }

    /// <inheritdoc />
    /// <remarks>The resources of a code-only WinUI application are not available in its constructor.</remarks>
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);
        Resources[MarkerKey] = nameof(TestApplication);
        Launched = true;
    }
}
