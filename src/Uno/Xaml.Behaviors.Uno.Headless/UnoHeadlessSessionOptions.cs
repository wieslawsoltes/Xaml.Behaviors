// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Microsoft.UI.Xaml;

namespace Xaml.Behaviors.Uno.Headless;

/// <summary>
/// Configures the headless Uno application started by <see cref="UnoHeadlessSession"/>.
/// </summary>
/// <remarks>
/// Only one session can run per process (Uno supports a single <see cref="Microsoft.UI.Xaml.Application"/>),
/// so the options are applied once, by the first call that starts the session.
/// </remarks>
public sealed class UnoHeadlessSessionOptions
{
    /// <summary>
    /// Gets the default options: a 1024x768 window at scale 1 hosting a minimal application.
    /// </summary>
    public static UnoHeadlessSessionOptions Default { get; } = new();

    /// <summary>
    /// Gets the initial width of the session window, in raw pixels. Defaults to <c>1024</c>.
    /// </summary>
    public int Width { get; init; } = 1024;

    /// <summary>
    /// Gets the initial height of the session window, in raw pixels. Defaults to <c>768</c>.
    /// </summary>
    public int Height { get; init; } = 768;

    /// <summary>
    /// Gets the rasterization scale (raw pixels per view pixel). The logical window size is
    /// <c>Width / Scale</c> by <c>Height / Scale</c>. Defaults to <c>1</c>.
    /// </summary>
    public float Scale { get; init; } = 1f;

    /// <summary>
    /// Gets the factory that creates the <see cref="Microsoft.UI.Xaml.Application"/>, or <see langword="null"/>
    /// to use a minimal built-in application.
    /// </summary>
    /// <remarks>
    /// The factory runs on the UI thread while the host starts. Use it to supply application-level resources
    /// (for example theme dictionaries). The application does not need to create a window in
    /// <c>OnLaunched</c>: the session creates and owns its own <see cref="UnoHeadlessSession.Window"/>.
    /// </remarks>
    public Func<Application>? ApplicationFactory { get; init; }

    /// <summary>
    /// Gets how long to wait for the application to start before failing. Defaults to 30 seconds.
    /// </summary>
    public TimeSpan StartupTimeout { get; init; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Throws when any option has an invalid value.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">An option is out of range.</exception>
    internal void Validate()
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(Width, nameof(Width));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(Height, nameof(Height));

        if (!float.IsFinite(Scale) || Scale <= 0f)
        {
            throw new ArgumentOutOfRangeException(nameof(Scale), Scale, "The scale must be a finite, strictly positive value.");
        }

        if (StartupTimeout <= TimeSpan.Zero && StartupTimeout != System.Threading.Timeout.InfiniteTimeSpan)
        {
            throw new ArgumentOutOfRangeException(nameof(StartupTimeout), StartupTimeout, "The startup timeout must be positive or infinite.");
        }
    }
}
