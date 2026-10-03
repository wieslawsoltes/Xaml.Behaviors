// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Uno.UI.Hosting;
using Uno.UI.Runtime.Skia.Headless;

namespace Xaml.Behaviors.Uno.Headless.Internal;

/// <summary>
/// Runs the Uno headless Skia host on a dedicated background thread (the host's blocking <c>Run</c> loop; the host
/// itself creates the event-loop thread that becomes the UI thread) and creates the session window once the
/// application has launched.
/// </summary>
internal static class UnoHeadlessHostLauncher
{
    private const string ThreadName = "Uno headless host";

    /// <summary>
    /// Starts the host and returns a task that completes with the session once its window has been created.
    /// </summary>
    /// <param name="options">The validated session options.</param>
    public static Task<UnoHeadlessSession> LaunchAsync(UnoHeadlessSessionOptions options)
    {
        TaskCompletionSource<UnoHeadlessSession> started = new(TaskCreationOptions.RunContinuationsAsynchronously);

        Thread thread = new(() => RunHost(options, started))
        {
            IsBackground = true,
            Name = ThreadName,
        };
        // The host and UI threads outlive the caller: do not let them inherit the caller's async-local state (for
        // example the xUnit context of whichever test or fixture happened to start the session).
        using (ExecutionContext.SuppressFlow())
        {
            thread.Start();
        }

        return started.Task.WaitAsync(options.StartupTimeout);
    }

    private static void RunHost(UnoHeadlessSessionOptions options, TaskCompletionSource<UnoHeadlessSession> started)
    {
        try
        {
            HeadlessHost? headlessHost = null;
            UnoPlatformHost host = UnoPlatformHostBuilder.Create()
                .App(() => CreateApplication(options, started, headlessHost!))
                .UseHeadless(headless => headless
                    .WithSize(options.Width, options.Height)
                    .WithScale(options.Scale))
                .Build();
            headlessHost = host as HeadlessHost
                ?? throw new InvalidOperationException("The Uno platform host is not the headless host.");

            // Blocks for the lifetime of the application.
            host.Run();

            started.TrySetException(new InvalidOperationException("The Uno headless host exited before the session window was created."));
        }
        catch (Exception exception)
        {
            started.TrySetException(exception);
        }
    }

    // Runs on the UI thread, from Application.Start. OnLaunched runs synchronously right after this returns,
    // so the window is created from a queued item, after the application has fully launched.
    private static Application CreateApplication(UnoHeadlessSessionOptions options, TaskCompletionSource<UnoHeadlessSession> started, HeadlessHost host)
    {
        Application application;
        try
        {
            application = options.ApplicationFactory?.Invoke() ?? new UnoHeadlessApplication();
        }
        catch (Exception exception)
        {
            started.TrySetException(exception);
            throw;
        }

        DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        bool queued = dispatcherQueue.TryEnqueue(() =>
        {
            try
            {
                Window window = new()
                {
                    Content = new Grid(),
                };
                window.Activate();

                started.TrySetResult(new UnoHeadlessSession(options, application, window, dispatcherQueue, host));
            }
            catch (Exception exception)
            {
                started.TrySetException(exception);
            }
        });

        if (!queued)
        {
            started.TrySetException(new InvalidOperationException("The Uno headless dispatcher queue rejected the window creation."));
        }

        return application;
    }
}
