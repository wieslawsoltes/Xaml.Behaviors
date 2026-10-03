// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Threading.Tasks;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xaml.Behaviors.Uno.Headless.XUnit;
using Xunit;

namespace Xaml.Behaviors.Uno.Headless.UnitTests;

/// <summary>
/// Tests of <see cref="UnoHeadlessFactAttribute"/> / <see cref="UnoHeadlessTheoryAttribute"/> and of the
/// session helpers used from the UI thread.
/// </summary>
public class UnoHeadlessFactTests : IAsyncLifetime
{
    private readonly bool _constructedOnUiThread;
    private bool _initializedOnUiThread;

    public UnoHeadlessFactTests()
    {
        _constructedOnUiThread = DispatcherQueue.GetForCurrentThread()?.HasThreadAccess == true;
    }

    public static TheoryData<object> NonSerializableData => new() { new object(), new object() };

    public ValueTask InitializeAsync()
    {
        _initializedOnUiThread = UnoHeadlessSession.Current.HasThreadAccess;
        return ValueTask.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        Assert.True(UnoHeadlessSession.Current.HasThreadAccess);
        return ValueTask.CompletedTask;
    }

    [UnoHeadlessFact]
    public void Fact_Runs_On_Ui_Thread()
    {
        Assert.True(UnoHeadlessSession.Current.HasThreadAccess);
        Assert.Same(UnoHeadlessSession.Current.DispatcherQueue, DispatcherQueue.GetForCurrentThread());
    }

    [UnoHeadlessFact]
    public void Test_Class_Is_Constructed_And_Initialized_On_Ui_Thread()
    {
        Assert.True(_constructedOnUiThread);
        Assert.True(_initializedOnUiThread);
    }

    [UnoHeadlessFact]
    public async Task Async_Fact_Resumes_On_Ui_Thread()
    {
        await Task.Yield();
        Assert.True(UnoHeadlessSession.Current.HasThreadAccess);

        await Task.Delay(10);
        Assert.True(UnoHeadlessSession.Current.HasThreadAccess);
    }

    [UnoHeadlessFact]
    public void Fact_Can_Access_Test_Context()
    {
        Assert.Equal(nameof(Fact_Can_Access_Test_Context), TestContext.Current.TestMethod?.MethodName);
    }

    [UnoHeadlessFact(Timeout = 30_000)]
    public async Task Fact_With_Timeout_Runs_On_Ui_Thread_With_Test_Context()
    {
        Assert.True(UnoHeadlessSession.Current.HasThreadAccess);
        Assert.Equal(nameof(Fact_With_Timeout_Runs_On_Ui_Thread_With_Test_Context), TestContext.Current.TestMethod?.MethodName);

        await Task.Delay(1, TestContext.Current.CancellationToken);

        Assert.True(UnoHeadlessSession.Current.HasThreadAccess);
    }

    [UnoHeadlessFact]
    public void Fact_Can_Create_Ui_Elements()
    {
        Border border = new() { Width = 10 };

        Assert.Equal(10, border.Width);
    }

    [UnoHeadlessTheory]
    [InlineData(1)]
    [InlineData(2)]
    public void Theory_Rows_Run_On_Ui_Thread(int value)
    {
        Assert.InRange(value, 1, 2);
        Assert.True(UnoHeadlessSession.Current.HasThreadAccess);
    }

    [UnoHeadlessTheory]
    [MemberData(nameof(NonSerializableData))]
    public void Delay_Enumerated_Theory_Runs_On_Ui_Thread(object value)
    {
        Assert.NotNull(value);
        Assert.True(UnoHeadlessSession.Current.HasThreadAccess);
    }

    [UnoHeadlessFact(Skip = "Verifies that skipped facts are reported as skipped.")]
    public void Skipped_Fact_Does_Not_Run()
    {
        throw new InvalidOperationException("A skipped test must not run.");
    }

    [UnoHeadlessFact]
    public async Task ShowAsync_Sets_Window_Content_And_Waits_For_Loaded()
    {
        UnoHeadlessSession session = UnoHeadlessSession.Current;
        Border border = new();
        int loadedCount = 0;
        border.Loaded += (_, _) => loadedCount++;

        Border shown = await session.ShowAsync(border);

        Assert.Same(border, shown);
        Assert.Same(border, session.Window.Content);
        Assert.True(border.IsLoaded);
        Assert.Equal(1, loadedCount);
        Assert.NotNull(border.XamlRoot);
    }

    [UnoHeadlessFact]
    public async Task ShowAsync_Same_Element_Twice_Returns_Immediately()
    {
        UnoHeadlessSession session = UnoHeadlessSession.Current;
        Grid grid = new();
        int loadedCount = 0;
        grid.Loaded += (_, _) => loadedCount++;

        await session.ShowAsync(grid);
        await session.ShowAsync(grid);

        Assert.Equal(1, loadedCount);
    }

    [UnoHeadlessFact]
    public async Task ShowAsync_Replaces_Previous_Content()
    {
        UnoHeadlessSession session = UnoHeadlessSession.Current;
        Border first = new();
        Border second = new();

        await session.ShowAsync(first);
        await session.ShowAsync(second);
        await session.WaitForIdleAsync();

        Assert.Same(second, session.Window.Content);
        Assert.False(first.IsLoaded);
        Assert.True(second.IsLoaded);
    }

    [UnoHeadlessFact]
    public async Task Shown_Element_Is_Arranged_To_Window_Size()
    {
        UnoHeadlessSession session = UnoHeadlessSession.Current;
        Border border = new();

        await session.ShowAsync(border);
        await session.WaitForIdleAsync();

        Assert.Equal(TestSessionFixture.Width / TestSessionFixture.Scale, border.ActualWidth);
        Assert.Equal(TestSessionFixture.Height / TestSessionFixture.Scale, border.ActualHeight);
    }

    [UnoHeadlessFact]
    public async Task Text_Is_Measured()
    {
        UnoHeadlessSession session = UnoHeadlessSession.Current;
        TextBlock textBlock = new()
        {
            Text = "Hello, headless Uno!",
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
        };

        await session.ShowAsync(textBlock);
        await session.WaitForIdleAsync();

        Assert.True(textBlock.ActualWidth > 0, $"ActualWidth was {textBlock.ActualWidth}.");
        Assert.True(textBlock.ActualHeight > 0, $"ActualHeight was {textBlock.ActualHeight}.");
    }

    [UnoHeadlessFact]
    public async Task Loaded_Event_Of_Nested_Element_Fires()
    {
        UnoHeadlessSession session = UnoHeadlessSession.Current;
        TaskCompletionSource<bool> loaded = new(TaskCreationOptions.RunContinuationsAsynchronously);
        Button button = new() { Content = "Click" };
        button.Loaded += (_, _) => loaded.TrySetResult(true);

        await session.ShowAsync(new StackPanel { Children = { button } });

        Assert.True(await loaded.Task.WaitAsync(TimeSpan.FromSeconds(10), TestContext.Current.CancellationToken));
    }

    [UnoHeadlessFact]
    public async Task RunAsync_On_Ui_Thread_Runs_Inline()
    {
        UnoHeadlessSession session = UnoHeadlessSession.Current;
        bool ran = false;

        Task task = session.RunAsync(() => ran = true);

        Assert.True(ran);
        Assert.True(task.IsCompletedSuccessfully);
        await task;
    }

    [UnoHeadlessFact]
    public async Task RunAsync_On_Ui_Thread_Propagates_Exception()
    {
        UnoHeadlessSession session = UnoHeadlessSession.Current;

        await Assert.ThrowsAsync<InvalidOperationException>(() => session.RunAsync(() => throw new InvalidOperationException()));
        await Assert.ThrowsAsync<InvalidOperationException>(() => session.RunAsync(new Func<Task>(() => throw new InvalidOperationException())));
    }

    [UnoHeadlessFact]
    public async Task WaitForIdleAsync_On_Ui_Thread_Runs_Queued_Work()
    {
        UnoHeadlessSession session = UnoHeadlessSession.Current;
        bool ran = false;
        session.DispatcherQueue.TryEnqueue(() => ran = true);

        Assert.False(ran);
        await session.WaitForIdleAsync();

        Assert.True(ran);
        Assert.True(session.HasThreadAccess);
    }
}
