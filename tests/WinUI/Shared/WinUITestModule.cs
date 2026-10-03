// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Runtime.CompilerServices;
using Xaml.Behaviors.WinUI.Testing;

namespace Xaml.Behaviors.WinUI.TestCompat;

/// <summary>
/// Configures the WinUI test session of a test assembly (the WinUI counterpart of the Uno headless session setup).
/// </summary>
internal static class WinUITestModule
{
    /// <summary>
    /// Sets the default session options: the application of the test project (<see cref="TestApp"/>, whose XAML type
    /// information the XAML compiler generates) and, for tests of the behaviors, the window tracking
    /// (<c>WindowTracker</c>) that lets them find the test window.
    /// </summary>
    [ModuleInitializer]
    internal static void Initialize()
    {
        WinUITestSessionOptions.Default = new WinUITestSessionOptions
        {
            ApplicationFactory = static () => new TestApp(),
            Initialized = static session =>
            {
#if WINUI_TESTS_TRACK_WINDOWS
                global::Xaml.Interactivity.WindowTracker.Track(session.Window);
#endif
            },
        };
    }
}
