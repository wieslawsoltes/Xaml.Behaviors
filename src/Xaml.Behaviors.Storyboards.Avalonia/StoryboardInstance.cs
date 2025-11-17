// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.IO;
using Avalonia.Controls;

namespace Avalonia.Xaml.Behaviors.Storyboards;

/// <summary>
/// Represents a running storyboard instance along with the callbacks used to control it.
/// </summary>
public sealed class StoryboardInstance : IDisposable
{
    private readonly Action? _pauseAction;
    private readonly Action? _resumeAction;
    private readonly Action? _stopAction;
    private readonly Action? _removeAction;
    private readonly Action<TimeSpan, SeekOrigin>? _seekAction;
    private readonly Action? _skipToFillAction;
    private readonly Action<double>? _setSpeedRatioAction;
    private readonly Action? _onDispose;
    private bool _isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="StoryboardInstance"/> class.
    /// </summary>
    /// <param name="host">The element that owns the storyboard.</param>
    /// <param name="key">The registry key for interactive lookups.</param>
    /// <param name="handoffBehavior">The requested handoff behavior.</param>
    /// <param name="keepAlive">Indicates whether the registry should keep the entry alive after completion.</param>
    /// <param name="pauseAction">Callback used to pause the storyboard.</param>
    /// <param name="resumeAction">Callback used to resume the storyboard.</param>
    /// <param name="stopAction">Callback used to stop the storyboard.</param>
    /// <param name="removeAction">Callback used to remove the storyboard without stopping it.</param>
    /// <param name="seekAction">Callback used to seek the storyboard.</param>
    /// <param name="skipToFillAction">Callback used to skip the storyboard to its fill state.</param>
    /// <param name="setSpeedRatioAction">Callback used to adjust the speed ratio.</param>
    /// <param name="onDispose">Callback invoked the first time the instance is disposed.</param>
    public StoryboardInstance(
        StyledElement host,
        string key,
        StoryboardHandoffBehavior handoffBehavior,
        bool keepAlive = false,
        Action? pauseAction = null,
        Action? resumeAction = null,
        Action? stopAction = null,
        Action? removeAction = null,
        Action<TimeSpan, SeekOrigin>? seekAction = null,
        Action? skipToFillAction = null,
        Action<double>? setSpeedRatioAction = null,
        Action? onDispose = null)
    {
        Host = host ?? throw new ArgumentNullException(nameof(host));
        Key = !string.IsNullOrWhiteSpace(key)
            ? key
            : throw new ArgumentException("Storyboard keys cannot be null or whitespace.", nameof(key));
        HandoffBehavior = handoffBehavior;
        KeepAlive = keepAlive;
        _pauseAction = pauseAction;
        _resumeAction = resumeAction;
        _stopAction = stopAction;
        _removeAction = removeAction;
        _seekAction = seekAction;
        _skipToFillAction = skipToFillAction;
        _setSpeedRatioAction = setSpeedRatioAction;
        _onDispose = onDispose;
    }

    /// <summary>
    /// Raised when the instance is disposed by the registry or controller.
    /// </summary>
    public event EventHandler? Disposed;

    /// <summary>
    /// Gets the host element that owns the storyboard.
    /// </summary>
    public StyledElement Host { get; }

    /// <summary>
    /// Gets the registry key used to look up this instance.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// Gets the requested handoff behavior.
    /// </summary>
    public StoryboardHandoffBehavior HandoffBehavior { get; }

    /// <summary>
    /// Gets a value indicating whether the registry should keep the entry alive after completion.
    /// </summary>
    public bool KeepAlive { get; }

    /// <summary>
    /// Gets the insertion order layer used when composing animations.
    /// </summary>
    public int Layer { get; internal set; }

    /// <summary>
    /// Gets a value indicating whether the storyboard has been disposed.
    /// </summary>
    public bool IsDisposed => _isDisposed;

    /// <summary>
    /// Requests the storyboard to pause.
    /// </summary>
    public void Pause()
    {
        EnsureNotDisposed();
        _pauseAction?.Invoke();
    }

    /// <summary>
    /// Requests the storyboard to resume.
    /// </summary>
    public void Resume()
    {
        EnsureNotDisposed();
        _resumeAction?.Invoke();
    }

    /// <summary>
    /// Requests the storyboard to stop.
    /// </summary>
    public void Stop()
    {
        EnsureNotDisposed();
        _stopAction?.Invoke();
    }

    /// <summary>
    /// Requests the storyboard entry to be removed.
    /// </summary>
    public void Remove()
    {
        EnsureNotDisposed();
        _removeAction?.Invoke();
    }

    /// <summary>
    /// Requests the storyboard to seek to the specified position.
    /// </summary>
    /// <param name="offset">The seek offset.</param>
    /// <param name="origin">The seek origin.</param>
    public void Seek(TimeSpan offset, SeekOrigin origin)
    {
        EnsureNotDisposed();
        _seekAction?.Invoke(offset, origin);
    }

    /// <summary>
    /// Requests the storyboard to skip to its fill state.
    /// </summary>
    public void SkipToFill()
    {
        EnsureNotDisposed();
        _skipToFillAction?.Invoke();
    }

    /// <summary>
    /// Requests the storyboard to adjust its speed ratio.
    /// </summary>
    /// <param name="ratio">The new speed ratio.</param>
    public void SetSpeedRatio(double ratio)
    {
        EnsureNotDisposed();
        _setSpeedRatioAction?.Invoke(ratio);
    }

    /// <summary>
    /// Disposes the storyboard instance and releases all callbacks.
    /// </summary>
    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _isDisposed = true;
        _onDispose?.Invoke();
        Disposed?.Invoke(this, EventArgs.Empty);
    }

    private void EnsureNotDisposed()
    {
        if (_isDisposed)
        {
            throw new ObjectDisposedException(nameof(StoryboardInstance));
        }
    }
}
