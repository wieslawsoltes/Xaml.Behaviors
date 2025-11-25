using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Xunit;

namespace Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests;

public class AsyncObservableRuntimeTests
{
    [AvaloniaFact]
    public async Task AsyncTrigger_Completes_With_Result()
    {
        dynamic trigger = GeneratedTypeHelper.CreateInstance("SuccessfulTaskAsyncTrigger", "Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests");
        var action = new RecordingAction();
        trigger.Actions.Add(action);
        trigger.Attach(new TestControl());

        await Dispatcher.UIThread.InvokeAsync(() => trigger.SuccessfulTask = Task.FromResult(7));

        await FlushDispatcherAsync();
        await Task.Delay(10);

        Assert.Equal(7, (int)trigger.LastResult);
        Assert.Null(trigger.LastError);
        Assert.False((bool)trigger.IsExecuting);
        Assert.Equal(7, (int)action.SeenParameters.Single());
    }

    [AvaloniaFact]
    public async Task AsyncTrigger_Reports_Faulted_Task()
    {
        dynamic trigger = GeneratedTypeHelper.CreateInstance("FaultedTaskAsyncTrigger", "Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests");
        var action = new RecordingAction();
        trigger.Actions.Add(action);
        trigger.Attach(new TestControl());

        await Dispatcher.UIThread.InvokeAsync(() => trigger.FaultedTask = Task.FromException(new InvalidOperationException("boom")));

        await FlushDispatcherAsync();
        await Task.Delay(10);

        Assert.Empty(action.SeenParameters);
        Assert.IsType<InvalidOperationException>(trigger.LastError);
        Assert.False((bool)trigger.IsExecuting);
    }

    [AvaloniaFact]
    public async Task AsyncTrigger_Handles_Cancellation()
    {
        dynamic trigger = GeneratedTypeHelper.CreateInstance("CanceledTaskAsyncTrigger", "Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests");
        var action = new RecordingAction();
        trigger.Actions.Add(action);
        trigger.Attach(new TestControl());

        await Dispatcher.UIThread.InvokeAsync(() => trigger.CanceledTask = Task.FromCanceled(new CancellationToken(true)));

        await FlushDispatcherAsync();
        await Task.Delay(200);

        Assert.Null(trigger.LastError);
        Assert.False((bool)trigger.IsExecuting);
    }

    [AvaloniaFact]
    public async Task AsyncTrigger_Respects_UseDispatcher_False()
    {
        dynamic trigger = GeneratedTypeHelper.CreateInstance("BackgroundTaskAsyncTrigger", "Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests");
        var action = new RecordingAction();
        trigger.Actions.Add(action);
        trigger.Attach(new TestControl());

        var tcs = new TaskCompletionSource<int>();
        await Dispatcher.UIThread.InvokeAsync(() => trigger.BackgroundTask = tcs.Task);
        tcs.SetResult(3);

        await tcs.Task;
        await Task.Delay(50);

        Assert.Equal(3, (int)trigger.LastResult);
        Assert.False((bool)trigger.IsExecuting);
    }

    [AvaloniaFact]
    public async Task Assembly_AsyncTrigger_Defaults_To_Dispatcher()
    {
        dynamic trigger = GeneratedTypeHelper.CreateInstance("AssemblyAsyncHostBackgroundTaskAsyncTrigger", "Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests");
        var action = new RecordingAction();
        trigger.Actions.Add(action);
        trigger.Attach(new TestControl());

        trigger.BackgroundTask = Task.CompletedTask;

        Assert.Empty(action.SeenParameters);

        await FlushDispatcherAsync();
        await Task.Delay(10);

        Assert.Single(action.SeenParameters);
    }

    [AvaloniaFact]
    public async Task AsyncTrigger_FireOnAttach_False_Waits_For_Change()
    {
        dynamic trigger = GeneratedTypeHelper.CreateInstance("DeferredTaskAsyncTrigger", "Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests");
        var action = new RecordingAction();
        trigger.Actions.Add(action);

        await Dispatcher.UIThread.InvokeAsync(() => trigger.DeferredTask = Task.FromResult(1));
        trigger.Attach(new TestControl());

        await FlushDispatcherAsync();
        await Task.Delay(50);
        Assert.Equal(0, (int)trigger.LastResult);

        await Dispatcher.UIThread.InvokeAsync(() => trigger.DeferredTask = Task.FromResult(2));
        await FlushDispatcherAsync();
        await Task.Delay(50);

        Assert.Equal(2, (int)trigger.LastResult);
    }

    [AvaloniaFact]
    public async Task ObservableTrigger_OnNext_Updates_LastValue()
    {
        dynamic trigger = GeneratedTypeHelper.CreateInstance("IntStreamObservableTrigger", "Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests");
        var action = new RecordingAction();
        trigger.Actions.Add(action);
        trigger.Attach(new TestControl());

        var observable = new TestObservable<int>();
        trigger.IntStream = observable;

        observable.OnNext(5);
        await FlushDispatcherAsync();

        Assert.Equal(5, (int)trigger.LastValue);
        Assert.Equal(5, (int)action.SeenParameters.Single()!);
    }

    [AvaloniaFact]
    public async Task ObservableTrigger_OnError_Sets_LastError()
    {
        dynamic trigger = GeneratedTypeHelper.CreateInstance("IntStreamObservableTrigger", "Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests");
        var action = new RecordingAction();
        trigger.Actions.Add(action);
        trigger.Attach(new TestControl());

        var observable = new TestObservable<int>();
        trigger.IntStream = observable;

        var ex = new InvalidOperationException("stream failure");
        observable.OnError(ex);
        await FlushDispatcherAsync();

        Assert.Same(ex, trigger.LastError);
        Assert.Same(ex, action.SeenParameters.Single());
    }

    [AvaloniaFact]
    public async Task ObservableTrigger_OnCompleted_Sends_Null()
    {
        dynamic trigger = GeneratedTypeHelper.CreateInstance("IntStreamObservableTrigger", "Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests");
        var action = new RecordingAction();
        trigger.Actions.Add(action);
        trigger.Attach(new TestControl());

        var observable = new TestObservable<int>();
        trigger.IntStream = observable;

        observable.OnCompleted();
        await FlushDispatcherAsync();

        Assert.Single(action.SeenParameters);
        Assert.Null(action.SeenParameters[0]);
    }

    [AvaloniaFact]
    public void ObservableTrigger_UseDispatcher_False_Runs_Immediately()
    {
        dynamic trigger = GeneratedTypeHelper.CreateInstance("BackgroundStreamObservableTrigger", "Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests");
        var action = new RecordingAction();
        trigger.Actions.Add(action);
        trigger.Attach(new TestControl());

        var observable = new TestObservable<string>();
        trigger.BackgroundStream = observable;

        observable.OnNext("hello");

        Assert.Equal("hello", (string)trigger.LastValue);
        Assert.Equal("hello", (string)action.SeenParameters.Single()!);
    }

    [AvaloniaFact]
    public async Task ObservableTrigger_FireOnAttach_False_Waits_For_Reassignment()
    {
        dynamic trigger = GeneratedTypeHelper.CreateInstance("DeferredStreamObservableTrigger", "Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests");
        var action = new RecordingAction();
        trigger.Actions.Add(action);

        var initial = new TestObservable<int>();
        trigger.DeferredStream = initial;
        trigger.Attach(new TestControl());

        initial.OnNext(1);
        await FlushDispatcherAsync();
        Assert.Empty(action.SeenParameters);

        var replacement = new TestObservable<int>();
        trigger.DeferredStream = replacement;
        replacement.OnNext(2);
        await FlushDispatcherAsync();

        Assert.Equal(2, (int)action.SeenParameters.Single()!);
    }

    [AvaloniaFact]
    public async Task ObservableTrigger_Uses_SourceObject_When_Property_Not_Set()
    {
        var host = new RuntimeAsyncObservableHost();
        var observable = new TestObservable<int>();
        host.IntStream = observable;

        dynamic trigger = GeneratedTypeHelper.CreateInstance("IntStreamObservableTrigger", "Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests");
        var action = new RecordingAction();
        trigger.Actions.Add(action);

        trigger.Attach(new TestControl());
        trigger.SourceObject = host;

        observable.OnNext(7);
        await FlushDispatcherAsync();

        Assert.Equal(7, (int)trigger.LastValue);
        Assert.Equal(7, (int)action.SeenParameters.Single());
    }

    [AvaloniaFact]
    public async Task AsyncTrigger_Dispatcher_Path_Drops_Stale_Task()
    {
        var host = new RuntimeAsyncObservableHost();
        dynamic trigger = GeneratedTypeHelper.CreateInstance("DeferredTaskAsyncTrigger", "Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests");
        var action = new RecordingAction();
        trigger.Actions.Add(action);
        trigger.Attach(host);

        var first = new TaskCompletionSource<int>();
        trigger.DeferredTask = first.Task;

        var second = new TaskCompletionSource<int>();
        trigger.DeferredTask = second.Task;

        first.SetResult(1);
        second.SetResult(2);

        await FlushDispatcherAsync();
        await Task.Delay(50);

        Assert.Equal(2, (int)trigger.LastResult);
        Assert.Single(action.SeenParameters);
        Assert.Equal(2, (int)action.SeenParameters.Single()!);
    }

    [AvaloniaFact]
    public async Task AsyncTrigger_Uses_SourceObject_When_Property_Not_Set()
    {
        var host = new RuntimeAsyncObservableHost();
        dynamic trigger = GeneratedTypeHelper.CreateInstance("SuccessfulTaskAsyncTrigger", "Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests");
        var action = new RecordingAction();
        trigger.Actions.Add(action);

        trigger.SourceObject = host;
        host.SuccessfulTask = Task.FromResult(9);

        trigger.Attach(host);

        await FlushDispatcherAsync();
        await Task.Delay(20);

        Assert.Equal(9, (int)trigger.LastResult);
        Assert.Equal(9, (int)action.SeenParameters.Single());
    }

    [AvaloniaFact]
    public async Task AsyncTrigger_Ignores_Stale_Task_When_Reassigned()
    {
        var host = new RuntimeAsyncObservableHost();
        dynamic trigger = GeneratedTypeHelper.CreateInstance("BackgroundTaskAsyncTrigger", "Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests");
        var action = new RecordingAction();
        trigger.Actions.Add(action);
        trigger.Attach(host);

        var first = new TaskCompletionSource<int>();
        trigger.BackgroundTask = first.Task;

        var second = new TaskCompletionSource<int>();
        trigger.BackgroundTask = second.Task;

        second.SetResult(2);
        first.SetResult(1);

        await Task.Delay(50);

        Assert.Equal(2, (int)trigger.LastResult);
        Assert.Single(action.SeenParameters);
        Assert.Equal(2, (int)action.SeenParameters.Single()!);
    }

    private static async Task FlushDispatcherAsync()
    {
        await Dispatcher.UIThread.InvokeAsync(() => { });
    }
}
