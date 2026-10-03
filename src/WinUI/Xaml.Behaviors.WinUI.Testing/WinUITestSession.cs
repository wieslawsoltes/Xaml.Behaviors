// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Xaml.Behaviors.Uno.Headless.Internal;
using Xaml.Behaviors.WinUI.Testing.Internal;

namespace Xaml.Behaviors.WinUI.Testing;

/// <summary>
/// A WinUI 3 application with a test window, running on a dedicated UI thread for the lifetime of the process.
/// </summary>
/// <remarks>
/// <para>
/// The session offers the API of the Uno Platform headless session: elements are shown as the content of the test
/// window, <see cref="RunJobs"/> runs the queued UI work synchronously (by processing the messages of the UI thread)
/// and <see cref="Keyboard"/> and <see cref="Mouse"/> inject real input into the test window.
/// </para>
/// <para>
/// Injected input goes through the operating system: the test window is brought to the foreground before each input
/// event, so the session needs an interactive desktop and the input of the user must not interfere while the tests
/// run.
/// </para>
/// </remarks>
public sealed class WinUITestSession
{
    private const int MaxJobs = 100_000;
    private static SessionStart? s_start;
    private readonly System.Collections.Generic.List<Exception> _unhandledExceptions = [];

    private WinUITestSession(WinUITestSessionOptions options, Application application, Window window, DispatcherQueue dispatcherQueue)
    {
        Options = options;
        Application = application;
        Window = window;
        DispatcherQueue = dispatcherQueue;
        WindowHandle = WinRT.Interop.WindowNative.GetWindowHandle(window);
        application.UnhandledException += OnUnhandledException;
        Keyboard = new WinUITestKeyboard(this);
        Mouse = new WinUITestMouse(this);
    }

    /// <summary>
    /// Gets the started session.
    /// </summary>
    /// <exception cref="InvalidOperationException">The session has not started yet.</exception>
    public static WinUITestSession Current
    {
        get
        {
            SessionStart? start = Volatile.Read(ref s_start);
            return start is not null && start.Task.IsCompletedSuccessfully
                ? start.Task.Result
                : throw new InvalidOperationException("The WinUI test session has not started (use [WinUIFact] or WinUITestSession.StartAsync).");
        }
    }

    /// <summary>
    /// Gets the started session, or <c>null</c> when it has not started (yet).
    /// </summary>
    public static WinUITestSession? CurrentOrNull
    {
        get
        {
            SessionStart? start = Volatile.Read(ref s_start);
            return start is not null && start.Task.IsCompletedSuccessfully ? start.Task.Result : null;
        }
    }

    /// <summary>
    /// Gets the options the session was started with.
    /// </summary>
    public WinUITestSessionOptions Options { get; }

    /// <summary>
    /// Gets the application.
    /// </summary>
    public Application Application { get; }

    /// <summary>
    /// Gets the test window.
    /// </summary>
    public Window Window { get; }

    /// <summary>
    /// Gets the dispatcher queue of the UI thread.
    /// </summary>
    public DispatcherQueue DispatcherQueue { get; }

    /// <summary>
    /// Gets the keyboard of the session.
    /// </summary>
    public WinUITestKeyboard Keyboard { get; }

    /// <summary>
    /// Gets the mouse of the session.
    /// </summary>
    public WinUITestMouse Mouse { get; }

    /// <summary>
    /// Gets a value indicating whether the calling thread is the UI thread.
    /// </summary>
    public bool HasThreadAccess => DispatcherQueue.HasThreadAccess;

    internal nint WindowHandle { get; }

    /// <summary>
    /// Starts the session with the specified options, or returns the session already started with them.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <returns>The session.</returns>
    /// <exception cref="InvalidOperationException">The session was already started with other options.</exception>
    public static Task<WinUITestSession> StartAsync(WinUITestSessionOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        options.Validate();

        SessionStart start = GetOrCreateStart(options);
        return ReferenceEquals(start.Options, options)
            ? start.Task
            : throw new InvalidOperationException("The WinUI test session was already started with different options. Only one session can run per process.");
    }

    /// <summary>
    /// Returns the session, starting it with <see cref="WinUITestSessionOptions.Default"/> when needed.
    /// </summary>
    /// <returns>The session.</returns>
    public static Task<WinUITestSession> GetOrStartAsync()
        => GetOrCreateStart(WinUITestSessionOptions.Default).Task;

    /// <summary>
    /// Runs an action on the UI thread.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <returns>A task completed when the action has run.</returns>
    public Task RunAsync(Action action)
    {
        ArgumentNullException.ThrowIfNull(action);
        return DispatcherQueueInvoker.InvokeAsync(DispatcherQueue, action);
    }

    /// <summary>
    /// Runs a function on the UI thread.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="function">The function.</param>
    /// <returns>The result of the function.</returns>
    public Task<TResult> RunAsync<TResult>(Func<TResult> function)
    {
        ArgumentNullException.ThrowIfNull(function);
        return DispatcherQueueInvoker.InvokeAsync(DispatcherQueue, function);
    }

    /// <summary>
    /// Runs an asynchronous function on the UI thread.
    /// </summary>
    /// <param name="function">The function.</param>
    /// <returns>A task completed when the task of the function completes.</returns>
    public Task RunAsync(Func<Task> function)
    {
        ArgumentNullException.ThrowIfNull(function);
        return DispatcherQueueInvoker.InvokeAsync(DispatcherQueue, function);
    }

    /// <summary>
    /// Runs an asynchronous function on the UI thread.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="function">The function.</param>
    /// <returns>The result of the function.</returns>
    public Task<TResult> RunAsync<TResult>(Func<Task<TResult>> function)
    {
        ArgumentNullException.ThrowIfNull(function);
        return DispatcherQueueInvoker.InvokeAsync(DispatcherQueue, function);
    }

    /// <summary>
    /// Shows an element as the content of the test window and waits until it is loaded.
    /// </summary>
    /// <typeparam name="TElement">The type of the element.</typeparam>
    /// <param name="element">The element.</param>
    /// <param name="cancellationToken">A token that cancels the wait.</param>
    /// <returns>The element.</returns>
    public Task<TElement> ShowAsync<TElement>(TElement element, CancellationToken cancellationToken = default)
        where TElement : FrameworkElement
    {
        ArgumentNullException.ThrowIfNull(element);
        return RunAsync(() => ShowCoreAsync(element, cancellationToken));
    }

    /// <summary>
    /// Completes once the work queued on the UI thread before the call has run.
    /// </summary>
    /// <returns>A task completed when the UI thread is idle.</returns>
    public Task WaitForIdleAsync() => DispatcherQueueInvoker.YieldAsync(DispatcherQueue);

    /// <summary>
    /// Runs the queued UI work (the pending messages and dispatcher queue items of the UI thread) and the layout.
    /// </summary>
    /// <returns>The number of processed messages.</returns>
    /// <exception cref="InvalidOperationException">The caller is not on the UI thread.</exception>
    public int RunJobs()
    {
        EnsureThreadAccess();

        var total = 0;
        int count;
        do
        {
            count = 0;
            while (count < MaxJobs && NativeMethods.PeekMessage(out var message, 0, 0, 0, NativeMethods.PM_REMOVE))
            {
                NativeMethods.TranslateMessage(message);
                NativeMethods.DispatchMessage(message);
                count++;
            }

            UpdateLayout();
            total += count;
        }
        while (count > 0 && total < MaxJobs);

        return total;
    }

    /// <summary>
    /// Shows an element as the content of the test window, then runs the queued UI work until it is loaded.
    /// </summary>
    /// <typeparam name="TElement">The type of the element.</typeparam>
    /// <param name="element">The element.</param>
    /// <returns>The element.</returns>
    /// <exception cref="InvalidOperationException">The caller is not on the UI thread, or the element did not load.</exception>
    public TElement Show<TElement>(TElement element)
        where TElement : FrameworkElement
    {
        ArgumentNullException.ThrowIfNull(element);
        EnsureThreadAccess();

        Window.Content = element;
        Mouse.StartNewSequence();
        EnsureForeground();
        RunJobs();
        var stopwatch = Stopwatch.StartNew();
        while (!element.IsLoaded && stopwatch.Elapsed < TimeSpan.FromSeconds(5))
        {
            Pump(TimeSpan.FromMilliseconds(5));
        }

        return element.IsLoaded
            ? element
            : throw new InvalidOperationException("The element was not loaded after running the queued UI work.");
    }

    /// <summary>
    /// Returns and clears the exceptions that the UI thread did not handle since the last call.
    /// </summary>
    /// <returns>The exceptions, oldest first.</returns>
    /// <remarks>
    /// A WinUI application terminates on an unhandled exception of the UI thread: the session handles them (the test
    /// framework integration fails the running test with them).
    /// </remarks>
    public Exception[] TakeUnhandledExceptions()
    {
        lock (_unhandledExceptions)
        {
            var exceptions = _unhandledExceptions.ToArray();
            _unhandledExceptions.Clear();
            return exceptions;
        }
    }

    private void OnUnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        e.Handled = true;
        lock (_unhandledExceptions)
        {
            _unhandledExceptions.Add(e.Exception ?? new InvalidOperationException(e.Message));
        }
    }

    internal void EnsureThreadAccess()
    {
        if (!HasThreadAccess)
        {
            throw new InvalidOperationException("This member must be called on the UI thread (for example from a [WinUIFact] test).");
        }
    }

    /// <summary>
    /// Processes the messages of the UI thread for at least the specified time.
    /// </summary>
    internal void Pump(TimeSpan duration)
    {
        var stopwatch = Stopwatch.StartNew();
        RunJobs();
        while (stopwatch.Elapsed < duration)
        {
            var remaining = duration - stopwatch.Elapsed;
            var timeout = (uint)Math.Clamp(remaining.TotalMilliseconds, 1, 15);
            NativeMethods.MsgWaitForMultipleObjectsEx(0, 0, timeout, NativeMethods.QS_ALLINPUT, 0x0004);
            RunJobs();
        }
    }

    /// <summary>
    /// Brings the test window to the foreground, so it receives the injected input.
    /// </summary>
    internal void EnsureForeground()
    {
        var foreground = NativeMethods.GetForegroundWindow();
        if (foreground == WindowHandle)
        {
            return;
        }

        var currentThread = NativeMethods.GetCurrentThreadId();
        var foregroundThread = foreground != 0 ? NativeMethods.GetWindowThreadProcessId(foreground, out _) : 0;
        var attached = foregroundThread != 0 && foregroundThread != currentThread &&
                       NativeMethods.AttachThreadInput(foregroundThread, currentThread, true);
        try
        {
            NativeMethods.ShowWindow(WindowHandle, NativeMethods.SW_SHOW);
            NativeMethods.BringWindowToTop(WindowHandle);
            NativeMethods.SetForegroundWindow(WindowHandle);
        }
        finally
        {
            if (attached)
            {
                NativeMethods.AttachThreadInput(foregroundThread, currentThread, false);
            }
        }

        Pump(TimeSpan.FromMilliseconds(30));
    }

    /// <summary>
    /// Ensures the test window is the window under a screen position, so the injected mouse input reaches it.
    /// </summary>
    internal void EnsureWindowAt(int screenX, int screenY)
    {
        for (var attempt = 0; attempt < 10; attempt++)
        {
            var hit = NativeMethods.WindowFromPoint(new NativeMethods.POINT { X = screenX, Y = screenY });
            if (hit != 0 && NativeMethods.GetAncestor(hit, NativeMethods.GA_ROOT) == WindowHandle)
            {
                return;
            }

            NativeMethods.ShowWindow(WindowHandle, NativeMethods.SW_RESTORE);
            EnsureForeground();
            Pump(TimeSpan.FromMilliseconds(50));
        }

        throw new InvalidOperationException($"The test window is not the window at ({screenX}, {screenY}) on the screen: is the position outside the window or is another window in front of it?");
    }

    /// <summary>
    /// Brings the test window to the foreground without processing the messages of the UI thread (the asynchronous
    /// input may run inside a modal loop, for example of a drag and drop operation).
    /// </summary>
    internal void EnsureForegroundAsyncSafe()
    {
        if (NativeMethods.GetForegroundWindow() == WindowHandle)
        {
            return;
        }

        NativeMethods.ShowWindow(WindowHandle, NativeMethods.SW_SHOW);
        NativeMethods.BringWindowToTop(WindowHandle);
        NativeMethods.SetForegroundWindow(WindowHandle);
    }

    private void UpdateLayout()
    {
        if (Window.Content is UIElement content)
        {
            content.UpdateLayout();
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
            EnsureForeground();

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

    private static SessionStart GetOrCreateStart(WinUITestSessionOptions options)
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

    private sealed class SessionStart(WinUITestSessionOptions options)
    {
        private readonly TaskCompletionSource<WinUITestSession> _completion = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public WinUITestSessionOptions Options { get; } = options;

        public Task<WinUITestSession> Task => _completion.Task;

        public void Launch()
        {
            var thread = new Thread(Run)
            {
                IsBackground = true,
                Name = "WinUI test session",
            };
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

            _ = System.Threading.Tasks.Task.Delay(Options.StartupTimeout).ContinueWith(
                _ => _completion.TrySetException(new TimeoutException($"The WinUI test session did not start within {Options.StartupTimeout}.")),
                TaskScheduler.Default);
        }

        private void Run()
        {
            try
            {
                NativeMethods.SetProcessDpiAwarenessContext(NativeMethods.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2);
                WinRT.ComWrappersSupport.InitializeComWrappers();
                Application.Start(_ =>
                {
                    try
                    {
                        var dispatcherQueue = DispatcherQueue.GetForCurrentThread();
                        SynchronizationContext.SetSynchronizationContext(new DispatcherQueueSynchronizationContext(dispatcherQueue));
                        var application = Options.ApplicationFactory?.Invoke() ?? new WinUITestApplication(Options.MetadataProviders);

                        // The application is launched (OnLaunched) after this callback: create the window afterwards.
                        if (!dispatcherQueue.TryEnqueue(() => CreateSession(application, dispatcherQueue)))
                        {
                            _completion.TrySetException(new InvalidOperationException("The UI thread of the WinUI test session is shutting down."));
                        }
                    }
                    catch (Exception exception)
                    {
                        _completion.TrySetException(exception);
                    }
                });
            }
            catch (Exception exception)
            {
                _completion.TrySetException(exception);
            }
        }

        private void CreateSession(Application application, DispatcherQueue dispatcherQueue)
        {
            try
            {
                var window = new Window { Title = "WinUI test session" };
                var session = new WinUITestSession(Options, application, window, dispatcherQueue);
                var scale = GetDpiScale(session.WindowHandle);
                window.AppWindow.ResizeClient(new Windows.Graphics.SizeInt32(
                    (int)Math.Round(Options.Width / Options.Scale * scale),
                    (int)Math.Round(Options.Height / Options.Scale * scale)));
                // Always on top at a fixed position, so no other window (for example the console of the test runner)
                // receives the injected mouse input.
                if (window.AppWindow.Presenter is Microsoft.UI.Windowing.OverlappedPresenter presenter)
                {
                    presenter.IsAlwaysOnTop = true;
                }

                window.AppWindow.Move(new Windows.Graphics.PointInt32(32, 32));
                window.Activate();
                Options.Initialized?.Invoke(session);
                _completion.TrySetResult(session);
            }
            catch (Exception exception)
            {
                _completion.TrySetException(exception);
            }
        }

        private static double GetDpiScale(nint hwnd)
        {
            var dpi = NativeMethods.GetDpiForWindow(hwnd);
            return dpi > 0 ? dpi / 96.0 : 1.0;
        }
    }
}
