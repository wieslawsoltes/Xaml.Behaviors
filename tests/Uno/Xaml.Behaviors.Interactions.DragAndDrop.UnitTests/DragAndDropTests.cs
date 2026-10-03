// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Xaml.Behaviors.Uno.Headless;
using Xaml.Behaviors.Uno.Headless.XUnit;
using Xaml.Interactivity;
using Xunit;

namespace Xaml.Interactions.DragAndDrop.UnitTests;

/// <summary>
/// End-to-end drag and drop tests: the WinUI drag operation (<c>StartDragAsync</c>/<c>CanDrag</c>) runs in-process
/// on Uno Platform and is driven with injected mouse input.
/// </summary>
public class DragAndDropTests
{
    private static UnoHeadlessSession Session => UnoHeadlessSession.Current;

    private static MouseInput RequireInput()
    {
        var input = MouseInput.TryCreate();
        Assert.SkipWhen(input is null, "Input injection is not available.");
        return input!;
    }

    private static Border CreateBox(double left = 0) => new()
    {
        Width = 60,
        Height = 60,
        Margin = new Thickness(left, 0, 0, 0),
        Background = new SolidColorBrush(Colors.Red),
        HorizontalAlignment = HorizontalAlignment.Left,
        VerticalAlignment = VerticalAlignment.Top,
    };

    /// <summary>
    /// Drags from the center of <paramref name="source"/> to the center of <paramref name="target"/>, releases and waits
    /// until <paramref name="completed"/> is true (the WinUI drag operation completes asynchronously).
    /// </summary>
    private static async Task DragAsync(MouseInput input, FrameworkElement source, FrameworkElement target, Func<bool> completed)
    {
        await input.PressAndMoveAsync(MouseInput.Center(source), MouseInput.Center(target), steps: 10);
        await input.UpAsync();
        await WaitUntilAsync(completed);
    }

    private static async Task WaitUntilAsync(Func<bool> condition)
    {
        for (var i = 0; i < 400 && !condition(); i++)
        {
            await Task.Delay(5);
        }

        await Session.WaitForIdleAsync();
    }

#if WINUI
    // DependencyObject is a class on native WinUI (an interface on Uno Platform).
    private static T? FindDescendant<T>(DependencyObject element) where T : DependencyObject
#else
    private static T? FindDescendant<T>(DependencyObject element) where T : class, DependencyObject
#endif
    {
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
        {
            var child = VisualTreeHelper.GetChild(element, i);
            if (child is T found)
            {
                return found;
            }

            if (FindDescendant<T>(child) is { } descendant)
            {
                return descendant;
            }
        }

        return null;
    }

    /// <summary>Makes <paramref name="source"/> a WinUI drag source providing data written by <paramref name="fill"/>.</summary>
    private static void MakeDragSource(UIElement source, Action<DataPackage> fill, List<DataPackageOperation> results)
    {
        source.CanDrag = true;
        source.DragStarting += (_, e) =>
        {
            fill(e.Data);
            e.AllowedOperations = DataPackageOperation.Copy;
        };
        source.DropCompleted += (_, e) => results.Add(e.DropResult);
    }

    [UnoHeadlessFact]
    public async Task ContextDragBehavior_Drops_Context_On_ContextDropBehavior()
    {
        var input = RequireInput();
        var source = CreateBox();
        var target = CreateBox(200);
        var dragHandler = new RecordingDragHandler();
        var dropHandler = new RecordingDropHandler();
        Interaction.GetBehaviors(source).Add(new ContextDragBehavior { Context = "payload", Handler = dragHandler });
        Interaction.GetBehaviors(target).Add(new ContextDropBehavior { Context = "target", Handler = dropHandler });
        await Session.ShowAsync(new Grid { Children = { source, target } });

        Assert.True(target.AllowDrop);

        await DragAsync(input, source, target, () => dragHandler.Calls.Count == 2);

        Assert.Equal(["Before", "After"], dragHandler.Calls.Select(c => c.Name).ToArray());
        Assert.All(dragHandler.Calls, c => Assert.Equal("payload", c.Context));
        Assert.Equal(["Enter", "Over", "Drop"], dropHandler.Calls.Where(c => c != "Leave").ToArray());
        Assert.Equal("payload", dropHandler.SourceContext);
        Assert.Equal("target", dropHandler.TargetContext);
        Assert.Equal(DataPackageOperation.Move, dropHandler.DropEffects);
    }

    [UnoHeadlessFact]
    public async Task ContextDropBehavior_Rejects_When_Handler_Is_Invalid()
    {
        var input = RequireInput();
        var source = CreateBox();
        var target = CreateBox(200);
        var dragHandler = new RecordingDragHandler();
        var dropHandler = new RecordingDropHandler { IsValid = false };
        Interaction.GetBehaviors(source).Add(new ContextDragBehavior { Context = "payload", Handler = dragHandler });
        Interaction.GetBehaviors(target).Add(new ContextDropBehavior { Handler = dropHandler });
        await Session.ShowAsync(new Grid { Children = { source, target } });

        await DragAsync(input, source, target, () => dragHandler.Calls.Count == 2);

        Assert.Contains("Enter", dropHandler.Calls);
        Assert.DoesNotContain("Drop", dropHandler.Calls);
        Assert.Contains("Leave", dropHandler.Calls);
    }

    [UnoHeadlessFact]
    public async Task ContextDropBehavior_Uses_DataContext_When_Context_Is_Not_Set()
    {
        var input = RequireInput();
        var source = CreateBox();
        source.DataContext = "source-data";
        var target = CreateBox(200);
        target.DataContext = "target-data";
        var dragHandler = new RecordingDragHandler();
        var dropHandler = new RecordingDropHandler();
        Interaction.GetBehaviors(source).Add(new ContextDragBehavior { Handler = dragHandler });
        Interaction.GetBehaviors(target).Add(new ContextDropBehavior { Handler = dropHandler });
        await Session.ShowAsync(new Grid { Children = { source, target } });

        await DragAsync(input, source, target, () => dragHandler.Calls.Count == 2);

        Assert.Equal("source-data", dropHandler.SourceContext);
        Assert.Equal("target-data", dropHandler.TargetContext);
    }

    [UnoHeadlessFact]
    public async Task TypedDragBehavior_Drags_Matching_DataContext_Only()
    {
        var input = RequireInput();
        var source = CreateBox();
        source.DataContext = 42;
        var target = CreateBox(200);
        var dragHandler = new RecordingDragHandler();
        var dropHandler = new RecordingDropHandler();
        Interaction.GetBehaviors(source).Add(new TypedDragBehavior { DataType = typeof(int), Handler = dragHandler });
        Interaction.GetBehaviors(target).Add(new ContextDropBehavior { Handler = dropHandler });
        var first = new Grid { Children = { source, target } };
        await Session.ShowAsync(first);

        await DragAsync(input, source, target, () => dragHandler.Calls.Count == 2);

        Assert.Equal(42, dropHandler.SourceContext);

        // An element has one parent (WinUI throws when it is added to a second one).
        first.Children.Clear();

        var other = CreateBox();
        other.DataContext = "text";
        var otherHandler = new RecordingDragHandler();
        Interaction.GetBehaviors(other).Add(new TypedDragBehavior { DataType = typeof(int), Handler = otherHandler });
        await Session.ShowAsync(new Grid { Children = { other, target } });

        await DragAsync(input, other, target, () => false);

        Assert.Empty(otherHandler.Calls);
    }

    /// <summary>
    /// The text of a button with string content is a <see cref="TextBlock"/> whose data context is the content string
    /// (issue #379): pressing it must start the drag of the button.
    /// </summary>
    [UnoHeadlessFact]
    public async Task ContextDragBehavior_Drags_From_Button_Text_With_Own_DataContext()
    {
        var input = RequireInput();
        var source = new Button
        {
            Content = "Drag me",
            DataContext = "source-data",
            Width = 120,
            Height = 60,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
        };
        var target = CreateBox(200);
        var dragHandler = new RecordingDragHandler();
        var dropHandler = new RecordingDropHandler();
        Interaction.GetBehaviors(source).Add(new ContextDragBehavior { Handler = dragHandler });
        Interaction.GetBehaviors(target).Add(new ContextDropBehavior { Handler = dropHandler });
        await Session.ShowAsync(new Grid { Children = { source, target } });

        var text = FindDescendant<TextBlock>(source);
        Assert.NotNull(text);
        Assert.Equal("Drag me", text.DataContext);

        await input.PressAndMoveAsync(MouseInput.Center(text), MouseInput.Center(target), steps: 10);
        await input.UpAsync();
        await WaitUntilAsync(() => dragHandler.Calls.Count == 2);

        Assert.Equal(["Before", "After"], dragHandler.Calls.Select(c => c.Name).ToArray());
        Assert.Equal("source-data", dropHandler.SourceContext);
    }

    [UnoHeadlessFact]
    public async Task ContextDragWithDirectionBehavior_Drags_From_Button_Text_With_Own_DataContext()
    {
        var input = RequireInput();
        var source = new Button
        {
            Content = "Drag me",
            Width = 120,
            Height = 60,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
        };
        var target = CreateBox(200);
        var dragHandler = new RecordingDragHandler();
        Interaction.GetBehaviors(source).Add(new ContextDragWithDirectionBehavior { Context = "payload", Handler = dragHandler });
        Interaction.GetBehaviors(target).Add(new DragDropCommandsBehavior { DropCommand = new RecordingCommand() });
        await Session.ShowAsync(new Grid { Children = { source, target } });

        var text = FindDescendant<TextBlock>(source);
        Assert.NotNull(text);

        await input.PressAndMoveAsync(MouseInput.Center(text), MouseInput.Center(target), steps: 10);
        await input.UpAsync();
        await WaitUntilAsync(() => dragHandler.Calls.Count == 2);

        Assert.Equal(["Before", "After"], dragHandler.Calls.Select(c => c.Name).ToArray());
    }

    [UnoHeadlessFact]
    public async Task ContextDragWithDirectionBehavior_Provides_Context_And_Direction()
    {
        var input = RequireInput();
        var source = CreateBox();
        var target = CreateBox(200);
        var dragHandler = new RecordingDragHandler();
        var command = new RecordingCommand();
        Interaction.GetBehaviors(source).Add(new ContextDragWithDirectionBehavior { Context = "payload", Handler = dragHandler });
        Interaction.GetBehaviors(target).Add(new DragDropCommandsBehavior { DropCommand = command });
        await Session.ShowAsync(new Grid { Children = { source, target } });

        string? direction = null;
        target.Drop += (_, e) => direction = e.DataView.Properties["Avalonia.Xaml.Interactions.DragAndDrop.Direction"] as string;

        await DragAsync(input, source, target, () => dragHandler.Calls.Count == 2);

        Assert.Equal("down", direction);
        Assert.IsType<DragEventArgs>(Assert.Single(command.Parameters));
    }

    [UnoHeadlessFact]
    public async Task DragDropCommandsBehavior_Executes_Commands_And_Accepts_By_Default()
    {
        var input = RequireInput();
        var source = CreateBox();
        var target = CreateBox(200);
        var results = new List<DataPackageOperation>();
        MakeDragSource(source, data => data.SetText("text"), results);
        var enter = new RecordingCommand();
        var over = new RecordingCommand();
        var drop = new RecordingCommand();
        Interaction.GetBehaviors(target).Add(new DragDropCommandsBehavior { DragEnterCommand = enter, DragOverCommand = over, DropCommand = drop });
        await Session.ShowAsync(new Grid { Children = { source, target } });

        await DragAsync(input, source, target, () => results.Count == 1);

        Assert.NotEmpty(enter.Parameters);
        Assert.NotEmpty(over.Parameters);
        Assert.Single(drop.Parameters);
        // Like on Avalonia, a target that does not change the effects accepts the operation allowed by the source.
        Assert.Equal(DataPackageOperation.Copy, Assert.Single(results));
    }

    [UnoHeadlessFact]
    public void DragDropCommandsBehavior_Tracks_CanExecute()
    {
        var behavior = new DragDropCommandsBehavior();
        var command = new RecordingCommand(p => Equals(p, "yes"));
        var border = new Border();
        Interaction.GetBehaviors(border).Add(behavior);

        behavior.DropCommand = command;
        Assert.True(behavior.CanExecuteDropCommand);

        behavior.CanExecuteCommandParameter = "no";
        Assert.False(behavior.CanExecuteDropCommand);

        behavior.CanExecuteCommandParameter = "yes";
        Assert.True(behavior.CanExecuteDropCommand);
        Assert.True(behavior.CanExecuteDragEnterCommand);
    }

    [UnoHeadlessFact]
    public async Task TextDropBehavior_Executes_Command_With_Text()
    {
        var input = RequireInput();
        var source = CreateBox();
        var target = CreateBox(200);
        var results = new List<DataPackageOperation>();
        MakeDragSource(source, data => data.SetText("hello"), results);
        var command = new RecordingCommand();
        Interaction.GetBehaviors(target).Add(new TextDropBehavior { Command = command });
        await Session.ShowAsync(new Grid { Children = { source, target } });

        await DragAsync(input, source, target, () => results.Count == 1);

        Assert.Equal("hello", Assert.Single(command.Parameters));
        Assert.Equal(DataPackageOperation.Copy, Assert.Single(results));
    }

    [UnoHeadlessFact]
    public async Task TextDropBehavior_Rejects_Other_Data()
    {
        var input = RequireInput();
        var source = CreateBox();
        var target = CreateBox(200);
        var results = new List<DataPackageOperation>();
        MakeDragSource(source, data => data.SetData("custom", "value"), results);
        var command = new RecordingCommand();
        Interaction.GetBehaviors(target).Add(new TextDropBehavior { Command = command });
        await Session.ShowAsync(new Grid { Children = { source, target } });

        await DragAsync(input, source, target, () => results.Count == 1);

        Assert.Empty(command.Parameters);
        Assert.Equal(DataPackageOperation.None, Assert.Single(results));
    }

    private static async Task<StorageFile> CreateFileAsync()
    {
        var path = Path.Combine(Path.GetTempPath(), $"xaml-behaviors-dnd-{Guid.NewGuid():N}.txt");
        await File.WriteAllTextAsync(path, "content");
        return await StorageFile.GetFileFromPathAsync(path);
    }

    [UnoHeadlessFact]
    public async Task FilesDropBehavior_And_AddPreviewFilesAction_Receive_Files()
    {
        var input = RequireInput();
        var file = await CreateFileAsync();
        var source = CreateBox();
        var target = CreateBox(200);
        var results = new List<DataPackageOperation>();
        MakeDragSource(source, data => data.SetStorageItems([file]), results);
        var command = new RecordingCommand();
        Interaction.GetBehaviors(target).Add(new FilesDropBehavior { Command = command });
        await Session.ShowAsync(new Grid { Children = { source, target } });

        await DragAsync(input, source, target, () => results.Count == 1);

        var files = Assert.IsType<IStorageItem[]>(Assert.Single(command.Parameters));
        Assert.Equal(file.Path, Assert.Single(files).Path);
    }

    [UnoHeadlessFact]
    public async Task FilesPreviewBehavior_Previews_Files_While_Dragging()
    {
        var input = RequireInput();
        var file = await CreateFileAsync();
        var source = CreateBox();
        var target = CreateBox(200);
        var results = new List<DataPackageOperation>();
        MakeDragSource(source, data => data.SetStorageItems([file]), results);
        var behavior = new FilesPreviewBehavior();
        Interaction.GetBehaviors(target).Add(behavior);
        await Session.ShowAsync(new Grid { Children = { source, target } });

        await input.PressAndMoveAsync(MouseInput.Center(source), MouseInput.Center(target), steps: 10);
        await WaitUntilAsync(() => behavior.PreviewFiles.Count > 0);
        Assert.Equal(file.Path, Assert.Single(behavior.PreviewFiles).Path);

        await input.UpAsync();
        await WaitUntilAsync(() => results.Count == 1);
        Assert.Empty(behavior.PreviewFiles);
    }

    [UnoHeadlessFact]
    public async Task AddPreviewFilesAction_Adds_Files_To_Items_Without_ItemsSource()
    {
        var input = RequireInput();
        var file = await CreateFileAsync();
        var source = CreateBox();
        var target = CreateBox(200);
        var results = new List<DataPackageOperation>();
        MakeDragSource(source, data => data.SetStorageItems([file]), results);
        var list = new ItemsControl();
        var action = new AddPreviewFilesAction { ItemsControl = list };
        var executed = new List<object>();
        target.AllowDrop = true;
        target.DragOver += (_, e) =>
        {
            executed.Add(action.Execute(target, e));
            e.AcceptedOperation = DataPackageOperation.Copy;
        };
        await Session.ShowAsync(new Grid { Children = { source, target, list } });

        await DragAsync(input, source, target, () => results.Count == 1);

        Assert.Contains(true, executed);
        var item = Assert.IsAssignableFrom<IStorageItem>(Assert.Single(list.Items));
        Assert.Equal(file.Path, item.Path);
    }

    [UnoHeadlessFact]
    public async Task ContentControlFilesDropBehavior_Shows_Content_During_Drag()
    {
        var input = RequireInput();
        var file = await CreateFileAsync();
        var source = CreateBox();
        var target = new ContentControl
        {
            Width = 60,
            Height = 60,
            Margin = new Thickness(200, 0, 0, 0),
            Content = "idle",
            Background = new SolidColorBrush(Colors.Blue),
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            // Independent of the theme resources other tests may load.
            Template = (ControlTemplate)Microsoft.UI.Xaml.Markup.XamlReader.Load(
                """
                <ControlTemplate xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" TargetType="ContentControl">
                  <Border Background="{TemplateBinding Background}">
                    <ContentPresenter Content="{TemplateBinding Content}" />
                  </Border>
                </ControlTemplate>
                """),
        };
        var dragBrush = new SolidColorBrush(Colors.Green);
        var results = new List<DataPackageOperation>();
        MakeDragSource(source, data => data.SetStorageItems([file]), results);
        var command = new RecordingCommand();
        Interaction.GetBehaviors(target).Add(new ContentControlFilesDropBehavior { ContentDuringDrag = "drop here", BackgroundDuringDrag = dragBrush, Command = command });
        await Session.ShowAsync(new Grid { Children = { source, target } });

        await input.PressAndMoveAsync(MouseInput.Center(source), MouseInput.Center(target), steps: 10);
        await WaitUntilAsync(() => Equals(target.Content, "drop here"));
        Assert.Equal("drop here", target.Content);
        Assert.Same(dragBrush, target.Background);

        await input.UpAsync();
        await WaitUntilAsync(() => results.Count == 1);

        Assert.Single(command.Parameters);
        Assert.NotEqual("drop here", target.Content);
        Assert.NotSame(dragBrush, target.Background);
    }

    [UnoHeadlessFact]
    public async Task PanelDragBehavior_Moves_Control_To_PanelDropBehavior_Panel()
    {
        var input = RequireInput();
        var item = CreateBox();
        var sourcePanel = new StackPanel { Width = 100, Height = 100, Background = new SolidColorBrush(Colors.Gray), HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top, Children = { item } };
        var targetPanel = new StackPanel { Width = 100, Height = 100, Margin = new Thickness(200, 0, 0, 0), Background = new SolidColorBrush(Colors.Gray), HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top };
        Interaction.GetBehaviors(item).Add(new PanelDragBehavior());
        Interaction.GetBehaviors(targetPanel).Add(new PanelDropBehavior());
        await Session.ShowAsync(new Grid { Children = { sourcePanel, targetPanel } });

        await DragAsync(input, item, targetPanel, () => targetPanel.Children.Count == 1);

        Assert.Empty(sourcePanel.Children);
        Assert.Same(item, Assert.Single(targetPanel.Children));
    }

    [UnoHeadlessFact]
    public async Task Drop_Behaviors_Toggle_AllowDrop()
    {
        var context = CreateBox();
        var events = CreateBox();
        var other = CreateBox();
        var contextBehavior = new ContextDropBehavior();
        var eventsBehavior = new DragDropCommandsBehavior();
        Interaction.GetBehaviors(context).Add(contextBehavior);
        Interaction.GetBehaviors(events).Add(eventsBehavior);
        await Session.ShowAsync(new Grid { Children = { context, events, other } });

        Assert.True(context.AllowDrop);
        Assert.True(events.AllowDrop);

        eventsBehavior.TargetControl = other;
        Assert.True(other.AllowDrop);

        Interaction.GetBehaviors(context).Remove(contextBehavior);
        Assert.False(context.AllowDrop);
    }

    [UnoHeadlessFact]
    public void DropHandlerBase_Moves_Swaps_And_Inserts()
    {
        var handler = new ExposedDropHandler();
        var items = new List<string> { "a", "b", "c", "d" };

        handler.Move(items, 0, 2);
        Assert.Equal(["b", "c", "a", "d"], items);

        handler.Move(items, 3, 0);
        Assert.Equal(["d", "b", "c", "a"], items);

        handler.Swap(items, 0, 3);
        Assert.Equal(["a", "b", "c", "d"], items);

        var target = new List<string> { "x" };
        handler.Move(items, target, 1, 0);
        Assert.Equal(["a", "c", "d"], items);
        Assert.Equal(["b", "x"], target);

        handler.Swap(items, target, 0, 1);
        Assert.Equal(["x", "c", "d"], items);
        Assert.Equal(["b", "a"], target);

        handler.Insert(items, "z", 1);
        Assert.Equal(["x", "z", "c", "d"], items);
    }

    [UnoHeadlessFact]
    public void DataTransfer_Round_Trips_Through_A_DataPackage()
    {
        var data = new DataTransfer();
        data.Add(DataTransferItem.Create(ContextDropBehaviorBase.ContextDataTransferFormat, "key"));
        data.Add(DataTransferItem.Create(DataFormat.Text, "text"));
        var package = new DataPackage();

        data.ApplyTo(package);
        var view = new DataTransferView(package.GetView());

        Assert.True(view.Contains(ContextDropBehaviorBase.ContextDataTransferFormat));
        Assert.True(view.Contains(DataFormat.Text));
        Assert.False(view.Contains(DataFormat.File));
        Assert.Equal("key", view.TryGetValue(ContextDropBehaviorBase.ContextDataTransferFormat));
#if WINUI
        // Native WinUI reads the data asynchronously (the read was started by Contains): wait for it.
        var read = System.Diagnostics.Stopwatch.StartNew();
        while (view.TryGetText() is null && read.Elapsed < System.TimeSpan.FromSeconds(3))
        {
            Session.RunJobs();
        }

#endif
        Assert.Equal("text", view.TryGetText());
        Assert.Null(view.TryGetFiles());
        Assert.Null(new DataTransferView(null).TryGetValue("missing"));
    }

    [UnoHeadlessFact]
    public void ContextDragBehavior_Defaults_Match_Avalonia()
    {
        var context = new ContextDragBehavior();
        var direction = new ContextDragWithDirectionBehavior();

        Assert.Equal(3, context.HorizontalDragThreshold);
        Assert.Equal(3, context.VerticalDragThreshold);
        Assert.Equal(3, direction.HorizontalDragThreshold);
        Assert.Equal(3, direction.VerticalDragThreshold);
        Assert.Equal("Avalonia.Xaml.Interactions.DragAndDrop.Context", ContextDropBehaviorBase.ContextDataTransferFormat);
    }
}
