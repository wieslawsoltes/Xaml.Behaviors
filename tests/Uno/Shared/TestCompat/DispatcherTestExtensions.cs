// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Xaml.Behaviors.Uno.Headless;
#if !UNO_TESTS_INTERACTIVITY
using System;
using System.Threading.Tasks;
#endif

namespace Xaml.Behaviors.Uno.TestCompat;

#if UNO_TESTS_INTERACTIVITY
/// <summary>
/// Adds the Avalonia headless <c>Dispatcher.UIThread.RunJobs()</c> to the dispatcher compat of Xaml.Behaviors.Uno.Interactivity.
/// </summary>
internal static class DispatcherTestExtensions
{
    extension(Xaml.Interactivity.UIThreadDispatcher dispatcher)
    {
        /// <summary>Runs the queued UI work (<see cref="UnoHeadlessSession.RunJobs"/>).</summary>
#if WINUI
        // WinUI advances storyboards on frames: render one, like the Avalonia and Uno headless RunJobs complete them.
        public void RunJobs() => UnoHeadlessSession.Current.RenderFrame();
#else
        public void RunJobs() => UnoHeadlessSession.Current.RunJobs();
#endif

        /// <summary>
        /// Runs the queued UI work until <paramref name="until"/> is true: native WinUI opens and closes popups, tool
        /// tips and flyouts over several frames (Uno Platform does it with the queued work).
        /// </summary>
        public void RunJobs(System.Func<bool> until) => DispatcherTestWait.RunJobs(until);
    }
}
#else
/// <summary>
/// The Avalonia <c>Dispatcher</c> used by shared tests of projects without Xaml.Behaviors.Uno.Interactivity.
/// </summary>
internal static class Dispatcher
{
    /// <summary>Gets the UI thread dispatcher.</summary>
    public static TestUIThreadDispatcher UIThread { get; } = new();
}

/// <summary>
/// The Avalonia <c>Dispatcher.UIThread</c> members used by the shared tests, on the Uno headless session.
/// </summary>
internal sealed class TestUIThreadDispatcher
{
    private static UnoHeadlessSession Session => UnoHeadlessSession.Current;

    /// <summary>Runs the queued UI work.</summary>
#if WINUI
    public void RunJobs() => Session.RenderFrame();
#else
    public void RunJobs() => Session.RunJobs();
#endif

    /// <summary>
    /// Runs the queued UI work until <paramref name="until"/> is true: native WinUI opens and closes popups, tool tips
    /// and flyouts over several frames (Uno Platform does it with the queued work).
    /// </summary>
    public void RunJobs(Func<bool> until) => DispatcherTestWait.RunJobs(until);

    /// <summary>Gets a value indicating whether the caller is on the UI thread.</summary>
    public bool CheckAccess() => Session.HasThreadAccess;

    /// <summary>Queues <paramref name="action"/> on the UI thread.</summary>
    public void Post(Action action) => Session.DispatcherQueue.TryEnqueue(() => action());

    /// <summary>Runs <paramref name="action"/> on the UI thread.</summary>
    public Task InvokeAsync(Action action) => Session.RunAsync(action);
}
#endif

/// <summary>
/// Runs the queued UI work until a condition is met.
/// </summary>
internal static class DispatcherTestWait
{
    /// <summary>Runs the queued UI work until <paramref name="until"/> is true (native WinUI: for at most 3 seconds).</summary>
    public static void RunJobs(System.Func<bool> until)
    {
#if WINUI
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        do
        {
            UnoHeadlessSession.Current.RenderFrame();
        }
        while (!until() && stopwatch.Elapsed < System.TimeSpan.FromSeconds(3));
#else
        UnoHeadlessSession.Current.RunJobs();
#endif
    }
}
