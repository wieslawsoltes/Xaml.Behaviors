// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Xaml.Behaviors.Uno.Headless;
using Xaml.Behaviors.Uno.Headless.XUnit;
using Xaml.Interactivity;
using Xunit;

namespace Xaml.Interactions.DragAndDrop.UnitTests;

public sealed class RecordingTreeViewDropHandler : BaseTreeViewDropHandler
{
    public List<bool> Executions { get; } = [];

    public bool WillChangeParent { get; set; }

    protected override (bool Valid, bool WillSourceItemBeMovedToDifferentParent) Validate(TreeView treeView, DragEventArgs e, object? sourceContext, object? targetContext, bool execute)
    {
        if (execute)
        {
            Executions.Add(true);
        }

        return (sourceContext is not null, WillChangeParent);
    }
}

public class TreeViewDropHandlerTests
{
    private static UnoHeadlessSession Session => UnoHeadlessSession.Current;

    private static async Task<(Border Source, TreeView TreeView, TreeViewItem Target)> ShowAsync(RecordingTreeViewDropHandler handler, RecordingDragHandler dragHandler)
    {
        var source = new Border { Width = 40, Height = 40, Background = new SolidColorBrush(Colors.Red), HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top };
        Interaction.GetBehaviors(source).Add(new ContextDragBehavior { Context = "payload", Handler = dragHandler });

        var parent = new TreeViewNode { Content = "parent", IsExpanded = true };
        parent.Children.Add(new TreeViewNode { Content = "child" });
        var treeView = new TreeView { Width = 200, Height = 200, Margin = new Thickness(100, 0, 0, 0), HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top };
        treeView.RootNodes.Add(parent);
        Interaction.GetBehaviors(treeView).Add(new ContextDropBehavior { Handler = handler });

        // The headless session has no theme: the tree view needs the Fluent templates.
        var host = new Grid { Children = { source, treeView } };
        host.Resources.MergedDictionaries.Add(new XamlControlsResources());
        await Session.ShowAsync(host);
        for (var i = 0; i < 20 && treeView.ContainerFromNode(parent.Children[0]) is null; i++)
        {
            await Session.WaitForIdleAsync();
        }

        var target = treeView.ContainerFromNode(parent.Children[0]) as TreeViewItem;
        Assert.SkipWhen(target is null, "The tree view items were not realized.");
        return (source, treeView, target!);
    }

    [UnoHeadlessFact]
    public async Task Dragging_Over_An_Item_Applies_And_Clears_Direction_Classes()
    {
        var input = MouseInput.TryCreate();
        Assert.SkipWhen(input is null, "Input injection is not available.");
        var handler = new RecordingTreeViewDropHandler();
        var dragHandler = new RecordingDragHandler();
        var (source, _, target) = await ShowAsync(handler, dragHandler);

        await input!.PressAndMoveAsync(MouseInput.Center(source), MouseInput.Center(target), steps: 10);
        for (var i = 0; i < 100 && !target.Classes.Any(); i++)
        {
            await Task.Delay(5);
        }

        Assert.True(target.Classes.Contains("DraggingUp") || target.Classes.Contains("DraggingDown"));

        await input.UpAsync();
        for (var i = 0; i < 400 && dragHandler.Calls.Count < 2; i++)
        {
            await Task.Delay(5);
        }

        Assert.Single(handler.Executions);
        Assert.False(target.Classes.Contains("DraggingUp") || target.Classes.Contains("DraggingDown"));
    }

    [UnoHeadlessFact]
    public async Task Moving_To_A_Different_Parent_Highlights_The_Parent_Item()
    {
        var input = MouseInput.TryCreate();
        Assert.SkipWhen(input is null, "Input injection is not available.");
        var handler = new RecordingTreeViewDropHandler { WillChangeParent = true };
        var dragHandler = new RecordingDragHandler();
        var (source, treeView, target) = await ShowAsync(handler, dragHandler);
        var parentItem = (TreeViewItem)treeView.ContainerFromNode(treeView.RootNodes[0]);

        await input!.PressAndMoveAsync(MouseInput.Center(source), MouseInput.Center(target), steps: 10);
        for (var i = 0; i < 100 && !parentItem.Classes.Contains("TargetHighlight"); i++)
        {
            await Task.Delay(5);
        }

        Assert.True(parentItem.Classes.Contains("TargetHighlight"));
        Assert.False(target.Classes.Contains("TargetHighlight"));

        await input.UpAsync();
        for (var i = 0; i < 400 && dragHandler.Calls.Count < 2; i++)
        {
            await Task.Delay(5);
        }
    }
}
