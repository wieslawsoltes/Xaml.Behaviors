using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Dispatching;
using Xaml.Interactions.Core;
using System.Collections.Generic;
using Windows.Storage;
#else
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.Xaml.Interactions.Core;
#endif
using Xunit;

#if UNO
namespace Xaml.Interactions.UnitTests.Core;
#else
namespace Avalonia.Xaml.Interactions.UnitTests.Core;
#endif

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
            Command = new Command(_ => { }),
#if UNO
            // The headless Uno host has no system pickers (Avalonia headless: a storage provider without pickers).
            StorageProvider = new HeadlessStorageProvider(),
#endif
        };

        var result = action.Execute(new Border(), null);

        var task = Assert.IsAssignableFrom<Task>(result);
        await task;
    }

    [AvaloniaFact]
    public async Task TrackDispatchedPickerOperation_RunsOperationOnUiThread()
    {
        var action = new TestPickerAction();
        var operationWasOnUiThread = false;
        Task? task = null;

        await Task.Run(() =>
        {
            task = action.TrackDispatched(() =>
            {
                operationWasOnUiThread = Dispatcher.UIThread.CheckAccess();
                return Task.CompletedTask;
            });
        });

        await Assert.IsAssignableFrom<Task>(task);

        Assert.True(operationWasOnUiThread);
        await WaitForNoActiveOperations(action);
    }

    [AvaloniaFact]
    public async Task OpenFolderPickerAction_ReturnsTaskWhenSenderIsVisual()
    {
        var action = new OpenFolderPickerAction
        {
            Command = new Command(_ => { }),
#if UNO
            // The headless Uno host has no system pickers (Avalonia headless: a storage provider without pickers).
            StorageProvider = new HeadlessStorageProvider(),
#endif
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
            Command = new Command(_ => { }),
#if UNO
            // The headless Uno host has no system pickers (Avalonia headless: a storage provider without pickers).
            StorageProvider = new HeadlessStorageProvider(),
#endif
        };

        var result = action.Execute(new Border(), null);

        var task = Assert.IsAssignableFrom<Task>(result);
        await task;
    }

    [AvaloniaFact]
    [RequiresUnreferencedCode("Tests intentionally exercise a reflection binding by property name.")]
    public void SaveFilePickerAction_FileTypeChoicesProperty_IsOwnedBySaveFilePickerAction()
    {
#if !UNO
        Assert.Equal(typeof(SaveFilePickerAction), SaveFilePickerAction.FileTypeChoicesProperty.OwnerType);
#endif

        // A binding by name resolves the property only on its owner; otherwise it reads the CLR property once
        // and misses the later changes.
        var action = new SaveFilePickerAction { FileTypeChoices = "Text|*.txt" };
        var textBlock = new TextBlock();
#if UNO
        textBlock.SetBinding(TextBlock.TextProperty, new Binding
        {
            Path = new PropertyPath(nameof(SaveFilePickerAction.FileTypeChoices)),
            Source = action
        });
#else
        textBlock.Bind(TextBlock.TextProperty, new Binding
        {
            Path = nameof(SaveFilePickerAction.FileTypeChoices),
            Source = action
        });
#endif

        Assert.Equal("Text|*.txt", textBlock.Text);

        action.FileTypeChoices = "Images|*.png";

        Assert.Equal("Images|*.png", textBlock.Text);
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

#if UNO
    private sealed class HeadlessStorageProvider : IStorageProvider
    {
        public Task<IReadOnlyList<IStorageFile>> OpenFilePickerAsync(FilePickerOpenOptions options)
            => Task.FromResult<IReadOnlyList<IStorageFile>>([]);

        public Task<IStorageFile?> SaveFilePickerAsync(FilePickerSaveOptions options)
            => Task.FromResult<IStorageFile?>(null);

        public Task<IReadOnlyList<IStorageFolder>> OpenFolderPickerAsync(FolderPickerOpenOptions options)
            => Task.FromResult<IReadOnlyList<IStorageFolder>>([]);

        public Task<IStorageFolder?> TryGetFolderFromPathAsync(Uri folderPath)
            => Task.FromResult<IStorageFolder?>(null);
    }

#endif
    private sealed class TestPickerAction : PickerActionBase
    {
        public Task Track(Task task)
        {
            return TrackPickerOperation(task);
        }

        public Task TrackDispatched(Func<Task> operation)
        {
            return TrackDispatchedPickerOperation(operation);
        }

        public override object? Execute(object? sender, object? parameter)
        {
            return null;
        }
    }
}
