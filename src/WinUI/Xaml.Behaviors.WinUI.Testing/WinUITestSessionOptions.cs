// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Markup;

namespace Xaml.Behaviors.WinUI.Testing;

/// <summary>
/// Options of the WinUI test session (<see cref="WinUITestSession"/>).
/// </summary>
public sealed class WinUITestSessionOptions
{
    /// <summary>
    /// Gets or sets the options used when a test starts the session without options.
    /// </summary>
    /// <remarks>Set it before the session starts, for example from a module initializer of the test assembly.</remarks>
    public static WinUITestSessionOptions Default { get; set; } = new();

    /// <summary>
    /// Gets the width of the test window content, in raw pixels at <see cref="Scale"/>. Defaults to <c>1024</c>.
    /// </summary>
    public int Width { get; init; } = 1024;

    /// <summary>
    /// Gets the height of the test window content, in raw pixels at <see cref="Scale"/>. Defaults to <c>768</c>.
    /// </summary>
    public int Height { get; init; } = 768;

    /// <summary>
    /// Gets the rasterization scale the size is given for: the logical size of the window content is
    /// <c>Width / Scale</c> by <c>Height / Scale</c>. Defaults to <c>1</c>.
    /// </summary>
    /// <remarks>
    /// A WinUI window renders with the scale of its monitor, so only the logical size follows the options (the
    /// options match the Uno Platform headless harness).
    /// </remarks>
    public float Scale { get; init; } = 1f;

    /// <summary>
    /// Gets the factory of the application, or <c>null</c> for the default test application, which loads the WinUI
    /// control resources (<c>XamlControlsResources</c>) and resolves XAML types through
    /// <see cref="MetadataProviders"/>.
    /// </summary>
    public Func<Application>? ApplicationFactory { get; init; }

    /// <summary>
    /// Gets the XAML metadata providers of the default test application, in addition to the WinUI controls provider
    /// (for example the provider the XAML compiler generates for a test assembly with XAML pages).
    /// </summary>
    public IReadOnlyList<Func<IXamlMetadataProvider>> MetadataProviders { get; init; } = [];

    /// <summary>
    /// Gets a callback invoked on the UI thread once the application and the test window are created.
    /// </summary>
    public Action<WinUITestSession>? Initialized { get; init; }

    /// <summary>
    /// Gets the maximum time to wait for the application to start.
    /// </summary>
    public TimeSpan StartupTimeout { get; init; } = TimeSpan.FromSeconds(60);

    /// <summary>
    /// Gets the minimum time the UI thread processes its messages after each injected input event, so the input
    /// reaches the window before the next event.
    /// </summary>
    public TimeSpan InputDelay { get; init; } = TimeSpan.FromMilliseconds(20);

    internal void Validate()
    {
        if (Width <= 0 || Height <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(Width), "The window size must be positive.");
        }

        if (!float.IsFinite(Scale) || Scale <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(Scale), "The scale must be a positive number.");
        }

        if (StartupTimeout <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(StartupTimeout), "The startup timeout must be positive.");
        }
    }
}
