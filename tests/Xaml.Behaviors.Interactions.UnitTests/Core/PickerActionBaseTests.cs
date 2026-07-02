using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Xaml.Interactions.Core;
using Xunit;

namespace Avalonia.Xaml.Interactions.UnitTests.Core;

public class PickerActionBaseTests
{
    [AvaloniaFact]
    public async Task TrackPickerOperation_KeepsOperationActiveUntilCompletion()
    {
        var action = new TestPickerAction();
        var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        var task = action.Track(completion.Task);

        Assert.Equal(1, action.ActivePickerOperationCount);

        completion.SetResult();

        await task;
        await WaitForNoActiveOperations(action);
    }

    [AvaloniaFact]
    public async Task TrackPickerOperation_RemovesOperationWhenProviderFails()
    {
        var action = new TestPickerAction();
        var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        var task = action.Track(completion.Task);

        Assert.Equal(1, action.ActivePickerOperationCount);

        completion.SetException(new InvalidOperationException("Picker failed."));

        await Assert.ThrowsAsync<InvalidOperationException>(() => task);
        await WaitForNoActiveOperations(action);
    }

    [AvaloniaFact]
    public async Task OpenFilePickerAction_ReturnsTaskWhenSenderIsVisual()
    {
        var action = new OpenFilePickerAction
        {
            Command = new Command(_ => { })
        };

        var result = action.Execute(new Border(), null);

        var task = Assert.IsAssignableFrom<Task>(result);
        await task;
    }

    [AvaloniaFact]
    public async Task OpenFolderPickerAction_ReturnsTaskWhenSenderIsVisual()
    {
        var action = new OpenFolderPickerAction
        {
            Command = new Command(_ => { })
        };

        var result = action.Execute(new Border(), null);

        var task = Assert.IsAssignableFrom<Task>(result);
        await task;
    }

    [AvaloniaFact]
    public async Task SaveFilePickerAction_ReturnsTaskWhenSenderIsVisual()
    {
        var action = new SaveFilePickerAction
        {
            Command = new Command(_ => { })
        };

        var result = action.Execute(new Border(), null);

        var task = Assert.IsAssignableFrom<Task>(result);
        await task;
    }

    private static async Task WaitForNoActiveOperations(PickerActionBase action)
    {
        for (var i = 0; i < 20; i++)
        {
            if (action.ActivePickerOperationCount == 0)
            {
                return;
            }

            await Task.Delay(10);
        }

        Assert.Equal(0, action.ActivePickerOperationCount);
    }

    private sealed class TestPickerAction : PickerActionBase
    {
        public Task Track(Task task)
        {
            return TrackPickerOperation(task);
        }

        public override object? Execute(object? sender, object? parameter)
        {
            return null;
        }
    }
}
