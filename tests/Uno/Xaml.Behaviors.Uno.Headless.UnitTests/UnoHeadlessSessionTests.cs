// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.UI.Dispatching;
using Xunit;

namespace Xaml.Behaviors.Uno.Headless.UnitTests;

/// <summary>
/// Tests of <see cref="UnoHeadlessSession"/> driven from xUnit worker threads (plain <c>[Fact]</c>).
/// </summary>
public class UnoHeadlessSessionTests
{
    [Fact]
    public async Task GetOrStartAsync_Returns_Session_Started_By_Fixture()
    {
        UnoHeadlessSession session = await UnoHeadlessSession.GetOrStartAsync();

        Assert.Same(TestSessionFixture.Options, session.Options);
        Assert.Same(session, UnoHeadlessSession.Current);
        Assert.Same(session, await UnoHeadlessSession.StartAsync(TestSessionFixture.Options));
    }

    [Fact]
    public async Task StartAsync_With_Different_Options_Throws()
    {
        await UnoHeadlessSession.GetOrStartAsync();

        Assert.Throws<InvalidOperationException>(() => { _ = UnoHeadlessSession.StartAsync(new UnoHeadlessSessionOptions()); });
    }

    [Fact]
    public void StartAsync_With_Null_Options_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => { _ = UnoHeadlessSession.StartAsync(null!); });
    }

    [Theory]
    [InlineData(0, 100, 1f)]
    [InlineData(100, -1, 1f)]
    [InlineData(100, 100, 0f)]
    [InlineData(100, 100, float.NaN)]
    [InlineData(100, 100, float.PositiveInfinity)]
    public void StartAsync_With_Invalid_Options_Throws(int width, int height, float scale)
    {
        UnoHeadlessSessionOptions options = new() { Width = width, Height = height, Scale = scale };

        Assert.Throws<ArgumentOutOfRangeException>(() => { _ = UnoHeadlessSession.StartAsync(options); });
    }

    [Fact]
    public void StartAsync_With_Invalid_Timeout_Throws()
    {
        UnoHeadlessSessionOptions options = new() { StartupTimeout = TimeSpan.Zero };

        Assert.Throws<ArgumentOutOfRangeException>(() => { _ = UnoHeadlessSession.StartAsync(options); });
    }

    [Fact]
    public async Task Session_Uses_Custom_Application_Factory()
    {
        UnoHeadlessSession session = await UnoHeadlessSession.GetOrStartAsync();

        TestApplication application = Assert.IsType<TestApplication>(session.Application);
        Assert.True(application.Launched);
        Assert.Equal(
            nameof(TestApplication),
            await session.RunAsync(() => (string)session.Application.Resources[TestApplication.MarkerKey]));
    }

    [Fact]
    public async Task Window_Bounds_Reflect_Size_And_Scale()
    {
        UnoHeadlessSession session = await UnoHeadlessSession.GetOrStartAsync();

        Windows.Foundation.Rect bounds = await session.RunAsync(() => session.Window.Bounds);

        Assert.Equal(TestSessionFixture.Width / TestSessionFixture.Scale, bounds.Width);
        Assert.Equal(TestSessionFixture.Height / TestSessionFixture.Scale, bounds.Height);
    }

    [Fact]
    public async Task Test_Thread_Has_No_Thread_Access()
    {
        UnoHeadlessSession session = await UnoHeadlessSession.GetOrStartAsync();

        Assert.False(session.HasThreadAccess);
        Assert.False(session.DispatcherQueue.HasThreadAccess);
    }

    [Fact]
    public async Task RunAsync_Action_Runs_On_Ui_Thread()
    {
        UnoHeadlessSession session = await UnoHeadlessSession.GetOrStartAsync();
        bool hadThreadAccess = false;
        DispatcherQueue? currentQueue = null;

        await session.RunAsync(() =>
        {
            hadThreadAccess = session.HasThreadAccess;
            currentQueue = DispatcherQueue.GetForCurrentThread();
        });

        Assert.True(hadThreadAccess);
        Assert.Same(session.DispatcherQueue, currentQueue);
    }

    [Fact]
    public async Task RunAsync_Func_Returns_Value()
    {
        UnoHeadlessSession session = await UnoHeadlessSession.GetOrStartAsync();

        int managedThreadId = await session.RunAsync(() => Environment.CurrentManagedThreadId);

        Assert.NotEqual(Environment.CurrentManagedThreadId, managedThreadId);
        Assert.Equal(managedThreadId, await session.RunAsync(() => Environment.CurrentManagedThreadId));
    }

    [Fact]
    public async Task RunAsync_Async_Func_Resumes_On_Ui_Thread()
    {
        UnoHeadlessSession session = await UnoHeadlessSession.GetOrStartAsync();

        bool afterYield = false;
        await session.RunAsync(async () =>
        {
            await Task.Yield();
            afterYield = session.HasThreadAccess;
        });

        Assert.True(afterYield);
    }

    [Fact]
    public async Task RunAsync_Async_Func_Returns_Value()
    {
        UnoHeadlessSession session = await UnoHeadlessSession.GetOrStartAsync();

        bool result = await session.RunAsync(async () =>
        {
            await Task.Delay(1);
            return session.HasThreadAccess;
        });

        Assert.True(result);
    }

    [Fact]
    public async Task RunAsync_Action_Propagates_Exception()
    {
        UnoHeadlessSession session = await UnoHeadlessSession.GetOrStartAsync();

        InvalidOperationException exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => session.RunAsync(() => throw new InvalidOperationException("action")));

        Assert.Equal("action", exception.Message);
    }

    [Fact]
    public async Task RunAsync_Func_Propagates_Exception()
    {
        UnoHeadlessSession session = await UnoHeadlessSession.GetOrStartAsync();

        await Assert.ThrowsAsync<FormatException>(() => session.RunAsync(new Func<int>(() => throw new FormatException())));
    }

    [Fact]
    public async Task RunAsync_Async_Func_Propagates_Exception_After_Await()
    {
        UnoHeadlessSession session = await UnoHeadlessSession.GetOrStartAsync();

        await Assert.ThrowsAsync<NotSupportedException>(() => session.RunAsync(async () =>
        {
            await Task.Yield();
            throw new NotSupportedException();
        }));
    }

    [Fact]
    public async Task RunAsync_Async_Func_With_Result_Propagates_Synchronous_Exception()
    {
        UnoHeadlessSession session = await UnoHeadlessSession.GetOrStartAsync();

        await Assert.ThrowsAsync<ArithmeticException>(() => session.RunAsync(new Func<Task<int>>(() => throw new ArithmeticException())));
    }

    [Fact]
    public async Task RunAsync_Async_Func_Propagates_Cancellation()
    {
        UnoHeadlessSession session = await UnoHeadlessSession.GetOrStartAsync();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => session.RunAsync(() => Task.FromCanceled(new CancellationToken(canceled: true))));
        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => session.RunAsync(() => Task.FromCanceled<int>(new CancellationToken(canceled: true))));
    }

    [Fact]
    public async Task RunAsync_Flows_Async_Local_State()
    {
        UnoHeadlessSession session = await UnoHeadlessSession.GetOrStartAsync();
        AsyncLocal<string> local = new() { Value = "flowed" };

        string? observed = await session.RunAsync(() => local.Value);

        Assert.Equal("flowed", observed);
    }

    [Fact]
    public async Task RunAsync_Null_Delegates_Throw()
    {
        UnoHeadlessSession session = await UnoHeadlessSession.GetOrStartAsync();

        Assert.Throws<ArgumentNullException>(() => { _ = session.RunAsync((Action)null!); });
        Assert.Throws<ArgumentNullException>(() => { _ = session.RunAsync((Func<int>)null!); });
        Assert.Throws<ArgumentNullException>(() => { _ = session.RunAsync((Func<Task>)null!); });
        Assert.Throws<ArgumentNullException>(() => { _ = session.RunAsync((Func<Task<int>>)null!); });
        Assert.Throws<ArgumentNullException>(() => { _ = session.ShowAsync<Microsoft.UI.Xaml.FrameworkElement>(null!); });
    }

    [Fact]
    public async Task WaitForIdleAsync_Runs_After_Previously_Queued_Work()
    {
        UnoHeadlessSession session = await UnoHeadlessSession.GetOrStartAsync();
        int completed = 0;

        for (int i = 0; i < 10; i++)
        {
            Assert.True(session.DispatcherQueue.TryEnqueue(() => Interlocked.Increment(ref completed)));
        }

        await session.WaitForIdleAsync();

        Assert.Equal(10, Volatile.Read(ref completed));
    }
}
