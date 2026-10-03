// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Threading.Tasks;
#if WINUI
using Microsoft.UI.Dispatching;
#else
using Windows.ApplicationModel.Core;
#endif
using Windows.UI.Core;

namespace Xaml.Interactivity;

/// <summary>
/// Uno Platform counterpart of <c>Avalonia.Threading.Dispatcher</c> used by the shared sources.
/// </summary>
internal static class Dispatcher
{
    /// <summary>
    /// Gets the dispatcher of the UI thread.
    /// </summary>
    public static UIThreadDispatcher UIThread => UIThreadDispatcher.Instance;
}

/// <summary>
/// Makes <c>Dispatcher.UIThread</c> resolve inside dependency objects, where Uno's generated
/// <c>Dispatcher</c> instance property (a <see cref="CoreDispatcher"/>) hides the <see cref="Dispatcher"/> type.
/// </summary>
internal static class CoreDispatcherCompatExtensions
{
    extension(CoreDispatcher dispatcher)
    {
        /// <summary>
        /// Gets the dispatcher of the UI thread.
        /// </summary>
        public UIThreadDispatcher UIThread => UIThreadDispatcher.Instance;
    }
}

/// <summary>
/// Posts and invokes work on the Uno Platform UI thread with the Avalonia dispatcher API shape.
/// </summary>
internal sealed class UIThreadDispatcher
{
    private UIThreadDispatcher()
    {
    }

    /// <summary>
    /// Gets the shared instance.
    /// </summary>
    public static UIThreadDispatcher Instance { get; } = new();

#if WINUI
    // Native WinUI has no main view dispatcher (CoreApplication.MainView): the dispatcher queue of the UI thread is
    // captured the first time the behaviors run on it (a behavior is attached or a window is tracked).
    private static DispatcherQueue? s_uiThread;

    /// <summary>
    /// Records the dispatcher queue of the calling thread as the UI thread, if it has one and none is known yet.
    /// </summary>
    internal static void CaptureCurrentThread()
    {
        if (s_uiThread is null && DispatcherQueue.GetForCurrentThread() is { } current)
        {
            s_uiThread = current;
        }
    }

    /// <summary>
    /// Records the dispatcher queue of the UI thread, if none is known yet.
    /// </summary>
    /// <param name="dispatcherQueue">The dispatcher queue of the UI thread.</param>
    internal static void Capture(DispatcherQueue dispatcherQueue) => s_uiThread ??= dispatcherQueue;

    private static DispatcherQueue Queue
    {
        get
        {
            CaptureCurrentThread();
            return s_uiThread ?? throw new InvalidOperationException("The UI thread is not known yet: attach a behavior or track a window (WindowTracker) first.");
        }
    }

    /// <summary>
    /// Determines whether the calling thread is the UI thread.
    /// </summary>
    public bool CheckAccess()
    {
        CaptureCurrentThread();
        return s_uiThread?.HasThreadAccess ?? false;
    }
#else
    private static CoreDispatcher Core => CoreApplication.MainView.Dispatcher;

    /// <summary>
    /// Determines whether the calling thread is the UI thread.
    /// </summary>
    public bool CheckAccess() => Core.HasThreadAccess;
#endif

    /// <summary>
    /// Throws when the calling thread is not the UI thread.
    /// </summary>
    public void VerifyAccess()
    {
        if (!CheckAccess())
        {
            throw new InvalidOperationException("The calling thread cannot access this object because a different thread owns it.");
        }
    }

    /// <summary>
    /// Queues the action on the UI thread.
    /// </summary>
    public void Post(System.Action action)
    {
        ArgumentNullException.ThrowIfNull(action);
#if WINUI
        if (!Queue.TryEnqueue(() => action()))
        {
            throw new InvalidOperationException("The UI thread is shutting down.");
        }
#else
        _ = Core.RunAsync(CoreDispatcherPriority.Normal, () => action());
#endif
    }

    /// <summary>
    /// Runs the action on the UI thread and waits for it.
    /// </summary>
    public void Invoke(System.Action action)
    {
        ArgumentNullException.ThrowIfNull(action);
        if (CheckAccess())
        {
            action();
            return;
        }

        InvokeAsync(action).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Queues the action on the UI thread.
    /// </summary>
    public Task InvokeAsync(System.Action action)
    {
        ArgumentNullException.ThrowIfNull(action);
        var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        Post(() =>
        {
            try
            {
                action();
                completion.SetResult();
            }
            catch (Exception exception)
            {
                completion.SetException(exception);
            }
        });
        return completion.Task;
    }

    /// <summary>
    /// Queues the function on the UI thread.
    /// </summary>
    public Task<T> InvokeAsync<T>(Func<T> function)
    {
        ArgumentNullException.ThrowIfNull(function);
        var completion = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
        Post(() =>
        {
            try
            {
                completion.SetResult(function());
            }
            catch (Exception exception)
            {
                completion.SetException(exception);
            }
        });
        return completion.Task;
    }

    /// <summary>
    /// Queues the asynchronous function on the UI thread.
    /// </summary>
    public Task InvokeAsync(Func<Task> function)
    {
        ArgumentNullException.ThrowIfNull(function);
        return InvokeAsync<Task>(function).Unwrap();
    }

    /// <summary>
    /// Queues the asynchronous function on the UI thread.
    /// </summary>
    public Task<T> InvokeAsync<T>(Func<Task<T>> function)
    {
        ArgumentNullException.ThrowIfNull(function);
        return InvokeAsync<Task<T>>(function).Unwrap();
    }
}
