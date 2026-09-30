// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Uno.UI.Runtime.Skia.Headless;
using Xaml.Behaviors.Uno.Headless.Internal;

namespace Xaml.Behaviors.Uno.Headless;

/// <summary>
/// A headless (offscreen) Uno Platform application running on a dedicated UI thread, with one window that
/// tests can place content in.
/// </summary>
/// <remarks>
/// <para>
/// Uno supports a single <see cref="Microsoft.UI.Xaml.Application"/> per process, so there is at most one session
/// per process. It is started once, by the first call to <see cref="StartAsync(UnoHeadlessSessionOptions)"/> or
/// <see cref="GetOrStartAsync"/>, and lives until the process exits.
/// </para>
/// <para>
/// Uno UI objects are thread-affine: create and access them on the UI thread, through the <c>RunAsync</c>
/// overloads (or from a test that already runs there, e.g. <c>[UnoHeadlessFact]</c>). The UI thread has a
/// synchronization context, so awaiting on it resumes on it.
/// </para>
/// </remarks>
public sealed class UnoHeadlessSession
{
    private static SessionStart? s_start;

    private readonly HeadlessHost _host;

    internal UnoHeadlessSession(UnoHeadlessSessionOptions options, Application application, Window window, DispatcherQueue dispatcherQueue, HeadlessHost host)
    {
        _host = host;
        Keyboard = new UnoHeadlessKeyboard(this, host);
        Mouse = new UnoHeadlessMouse(this);
        Options = options;
        Application = application;
        Window = window;
        DispatcherQueue = dispatcherQueue;
    }

    /// <summary>
    /// Gets the running session.
    /// </summary>
    /// <exception cref="InvalidOperationException">The session has not been started, is still starting, or failed to start.</exception>
    public static UnoHeadlessSession Current
    {
        get
        {
            Task<UnoHeadlessSession>? task = Volatile.Read(ref s_start)?.Task;
            return task is { IsCompletedSuccessfully: true }
                ? task.Result
                : throw new InvalidOperationException($"The Uno headless session is not running. Await {nameof(UnoHeadlessSession)}.{nameof(StartAsync)} or {nameof(GetOrStartAsync)} first.");
        }
    }

    /// <summary>
    /// Gets the options the session was started with.
    /// </summary>
    public UnoHeadlessSessionOptions Options { get; }

    /// <summary>
    /// Gets the application instance.
    /// </summary>
    public Application Application { get; }

    /// <summary>
    /// Gets the session window. Its content is replaced by <see cref="ShowAsync{TElement}(TElement, CancellationToken)"/>.
    /// </summary>
    public Window Window { get; }

    /// <summary>
    /// Gets the dispatcher queue of the UI thread.
    /// </summary>
    public DispatcherQueue DispatcherQueue { get; }

    /// <summary>
    /// Gets the keyboard of the session.
    /// </summary>
    public UnoHeadlessKeyboard Keyboard { get; }

    /// <summary>
    /// Gets the mouse of the session.
    /// </summary>
    public UnoHeadlessMouse Mouse { get; }

    /// <summary>
    /// Gets a value indicating whether the calling thread is the UI thread.
    /// </summary>
    public bool HasThreadAccess => DispatcherQueue.HasThreadAccess;

    /// <summary>
    /// Starts the session with the given options, or returns the session already started with the same options instance.
    /// </summary>
    /// <param name="options">The session options.</param>
    /// <returns>A task that completes with the running session.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="options"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">An option has an invalid value.</exception>
    /// <exception cref="InvalidOperationException">A session was already started with different options.</exception>
    public static Task<UnoHeadlessSession> StartAsync(UnoHeadlessSessionOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        options.Validate();

        SessionStart start = GetOrCreateStart(options);
        return ReferenceEquals(start.Options, options)
            ? start.Task
            : throw new InvalidOperationException("The Uno headless session was already started with different options. Only one session can run per process; start it once, before any test uses it (e.g. from an xUnit assembly fixture).");
    }

    /// <summary>
    /// Returns the running session, starting it with <see cref="UnoHeadlessSessionOptions.Default"/> if no session was started yet.
    /// </summary>
    /// <returns>A task that completes with the running session.</returns>
    public static Task<UnoHeadlessSession> GetOrStartAsync()
        => GetOrCreateStart(UnoHeadlessSessionOptions.Default).Task;

    /// <summary>
    /// Runs <paramref name="action"/> on the UI thread (inline when already on it).
    /// </summary>
    /// <param name="action">The action to run.</param>
    /// <returns>A task that completes when the action has run, faulted with any exception it threw.</returns>
    public Task RunAsync(Action action)
    {
        ArgumentNullException.ThrowIfNull(action);
        return DispatcherQueueInvoker.InvokeAsync(DispatcherQueue, action);
    }

    /// <summary>
    /// Runs <paramref name="function"/> on the UI thread (inline when already on it) and returns its result.
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="function">The function to run.</param>
    /// <returns>A task that completes with the function's result, faulted with any exception it threw.</returns>
    public Task<TResult> RunAsync<TResult>(Func<TResult> function)
    {
        ArgumentNullException.ThrowIfNull(function);
        return DispatcherQueueInvoker.InvokeAsync(DispatcherQueue, function);
    }

    /// <summary>
    /// Runs the asynchronous <paramref name="function"/> on the UI thread (inline when already on it). Its
    /// continuations resume on the UI thread.
    /// </summary>
    /// <param name="function">The asynchronous function to run.</param>
    /// <returns>A task that completes with the task returned by <paramref name="function"/>.</returns>
    public Task RunAsync(Func<Task> function)
    {
        ArgumentNullException.ThrowIfNull(function);
        return DispatcherQueueInvoker.InvokeAsync(DispatcherQueue, function);
    }

    /// <summary>
    /// Runs the asynchronous <paramref name="function"/> on the UI thread (inline when already on it) and returns
    /// its result. Its continuations resume on the UI thread.
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="function">The asynchronous function to run.</param>
    /// <returns>A task that completes with the result of the task returned by <paramref name="function"/>.</returns>
    public Task<TResult> RunAsync<TResult>(Func<Task<TResult>> function)
    {
        ArgumentNullException.ThrowIfNull(function);
        return DispatcherQueueInvoker.InvokeAsync(DispatcherQueue, function);
    }

    /// <summary>
    /// Sets <paramref name="element"/> as the content of the session <see cref="Window"/> and waits until it is loaded.
    /// </summary>
    /// <typeparam name="TElement">The element type.</typeparam>
    /// <param name="element">The element to show. It must have been created on the UI thread.</param>
    /// <param name="cancellationToken">A token that stops waiting for the element to load.</param>
    /// <returns>A task that completes with <paramref name="element"/> once its <see cref="FrameworkElement.Loaded"/> event was raised.</returns>
    /// <remarks>
    /// The window is shared by everything running in the session: tests that interleave on the UI thread (async
    /// tests running in parallel) replace each other's content. Disable test parallelization when that matters.
    /// </remarks>
    public Task<TElement> ShowAsync<TElement>(TElement element, CancellationToken cancellationToken = default)
        where TElement : FrameworkElement
    {
        ArgumentNullException.ThrowIfNull(element);
        return RunAsync(() => ShowCoreAsync(element, cancellationToken));
    }

    /// <summary>
    /// Waits until the UI thread has processed the work queued before this call at normal or higher priority
    /// (layout, loaded events, bindings, queued callbacks).
    /// </summary>
    /// <returns>A task that completes once an item queued at <see cref="DispatcherQueuePriority.Low"/> priority has run.</returns>
    public Task WaitForIdleAsync() => DispatcherQueueInvoker.YieldAsync(DispatcherQueue);

    /// <summary>
    /// Runs the work queued on the UI thread (layout, loaded events, bindings, queued callbacks), including the work it
    /// queues, until the queue is empty. The synchronous counterpart of <see cref="WaitForIdleAsync"/>, like Avalonia's
    /// <c>Dispatcher.UIThread.RunJobs()</c>.
    /// </summary>
    /// <returns>The number of queued items that ran.</returns>
    /// <exception cref="InvalidOperationException">The caller is not on the UI thread.</exception>
    public int RunJobs()
    {
        EnsureThreadAccess();
        var total = 0;
        int count;
        do
        {
            count = _host.RunJobs();
            total += count;
        }
        while (count > 0 && total < MaxJobs);

        return total;
    }

    /// <summary>
    /// Sets <paramref name="element"/> as the content of the session <see cref="Window"/> and runs the queued UI work
    /// until it is loaded. The synchronous counterpart of <see cref="ShowAsync{TElement}(TElement, CancellationToken)"/>.
    /// </summary>
    /// <typeparam name="TElement">The element type.</typeparam>
    /// <param name="element">The element to show.</param>
    /// <returns><paramref name="element"/>.</returns>
    /// <exception cref="InvalidOperationException">The caller is not on the UI thread, or the element did not load.</exception>
    public TElement Show<TElement>(TElement element)
        where TElement : FrameworkElement
    {
        ArgumentNullException.ThrowIfNull(element);
        EnsureThreadAccess();

        Window.Content = element;
        Mouse.StartNewSequence();
        RunJobs();
        if (!element.IsLoaded)
        {
            element.UpdateLayout();
            RunJobs();
        }

        return element.IsLoaded
            ? element
            : throw new InvalidOperationException("The element was not loaded after running the queued UI work.");
    }

    private const int MaxJobs = 100_000;

    internal void EnsureThreadAccess()
    {
        if (!HasThreadAccess)
        {
            throw new InvalidOperationException("This member must be called on the UI thread (for example from an [UnoHeadlessFact] test).");
        }
    }

    private async Task<TElement> ShowCoreAsync<TElement>(TElement element, CancellationToken cancellationToken)
        where TElement : FrameworkElement
    {
        if (ReferenceEquals(Window.Content, element) && element.IsLoaded)
        {
            return element;
        }

        TaskCompletionSource loaded = new(TaskCreationOptions.RunContinuationsAsynchronously);
        void OnLoaded(object sender, RoutedEventArgs e) => loaded.TrySetResult();

        element.Loaded += OnLoaded;
        try
        {
            Window.Content = element;
            Mouse.StartNewSequence();

            if (!element.IsLoaded)
            {
                await loaded.Task.WaitAsync(cancellationToken);
            }
        }
        finally
        {
            element.Loaded -= OnLoaded;
        }

        return element;
    }

    private static SessionStart GetOrCreateStart(UnoHeadlessSessionOptions options)
    {
        SessionStart? existing = Volatile.Read(ref s_start);
        if (existing is not null)
        {
            return existing;
        }

        SessionStart candidate = new(options);
        existing = Interlocked.CompareExchange(ref s_start, candidate, null);
        if (existing is not null)
        {
            return existing;
        }

        candidate.Launch();
        return candidate;
    }

    /// <summary>
    /// The one-time start of the process-wide session: the options it was requested with and its outcome.
    /// </summary>
    private sealed class SessionStart(UnoHeadlessSessionOptions options)
    {
        private readonly TaskCompletionSource<UnoHeadlessSession> _completion = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public UnoHeadlessSessionOptions Options { get; } = options;

        public Task<UnoHeadlessSession> Task => _completion.Task;

        public void Launch()
        {
            Task<UnoHeadlessSession> launch;
            try
            {
                launch = UnoHeadlessHostLauncher.LaunchAsync(Options);
            }
            catch (Exception exception)
            {
                _completion.TrySetException(exception);
                return;
            }

            launch.ContinueWith(
                static (antecedent, state) =>
                {
                    TaskCompletionSource<UnoHeadlessSession> completion = (TaskCompletionSource<UnoHeadlessSession>)state!;
                    if (antecedent.IsCompletedSuccessfully)
                    {
                        completion.TrySetResult(antecedent.Result);
                    }
                    else
                    {
                        completion.TrySetException(antecedent.Exception?.InnerExceptions ?? [new TaskCanceledException(antecedent)]);
                    }
                },
                _completion,
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Default);
        }
    }
}
