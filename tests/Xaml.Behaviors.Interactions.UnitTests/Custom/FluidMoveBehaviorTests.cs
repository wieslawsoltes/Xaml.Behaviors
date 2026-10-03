// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.ObjectModel;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Dispatching;
using Xaml.Interactions.Custom;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.Xaml.Interactions.Custom;
using Avalonia.Xaml.Interactivity;
#endif
using Xunit;

#if UNO
namespace Xaml.Interactions.UnitTests.Custom;
#else
namespace Avalonia.Xaml.Interactions.UnitTests.Custom;
#endif

public class FluidMoveBehaviorTests
{
    private static (ItemsControl ItemsControl, Window Window) Show(ObservableCollection<string> items)
    {
        var itemsControl = new ItemsControl { ItemsSource = items };
        Interaction.GetBehaviors(itemsControl).Add(new FluidMoveBehavior
        {
            AppliesTo = FluidMoveScope.Children,
            Duration = TimeSpan.Zero
        });
        var window = new Window { Content = itemsControl };
        window.Show();
        Dispatcher.UIThread.RunJobs();
        return (itemsControl, window);
    }

    private static object? GetRenderTransform(ItemsControl itemsControl, string item)
    {
        var container = Assert.IsAssignableFrom<Control>(itemsControl.ContainerFromItem(item));
#if WINUI
        // The default render transform of a native WinUI element is an identity MatrixTransform (null on Uno Platform
        // and Avalonia).
        return container.RenderTransform is Microsoft.UI.Xaml.Media.MatrixTransform { Matrix.IsIdentity: true } ? null : container.RenderTransform;
#else
        return container.RenderTransform;
#endif
    }

    [AvaloniaFact]
    public void FluidMoveBehavior_On_ItemsControl_Animates_Moved_Item_Containers()
    {
        var items = new ObservableCollection<string> { "a", "b", "c", "d" };
        var (itemsControl, window) = Show(items);

        items.Move(0, 1);
        Dispatcher.UIThread.RunJobs();

        Assert.IsType<TranslateTransform>(GetRenderTransform(itemsControl, "a"));
        Assert.IsType<TranslateTransform>(GetRenderTransform(itemsControl, "b"));
        Assert.Null(GetRenderTransform(itemsControl, "c"));
        Assert.Null(GetRenderTransform(itemsControl, "d"));
        window.Close();
    }

    [AvaloniaFact]
    public void FluidMoveBehavior_On_ItemsControl_Animates_Items_Whose_Containers_Were_Recreated()
    {
        var items = new ObservableCollection<string> { "a", "b", "c" };
        var (itemsControl, window) = Show(items);

        items.Clear();
        items.Add("c");
        items.Add("b");
        items.Add("a");
        Dispatcher.UIThread.RunJobs();

        Assert.IsType<TranslateTransform>(GetRenderTransform(itemsControl, "a"));
        Assert.Null(GetRenderTransform(itemsControl, "b"));
        Assert.IsType<TranslateTransform>(GetRenderTransform(itemsControl, "c"));
        window.Close();
    }

    [AvaloniaFact]
    public void FluidMoveBehavior_Does_Not_Animate_Items_Added_After_Removal()
    {
        var items = new ObservableCollection<string> { "a", "b" };
        var (itemsControl, window) = Show(items);

        items.Remove("a");
        Dispatcher.UIThread.RunJobs();
        items.Add("a");
        Dispatcher.UIThread.RunJobs();

        // "a" left the panel, so its old position is forgotten; "b" moved up when "a" was removed.
        Assert.Null(GetRenderTransform(itemsControl, "a"));
        Assert.IsType<TranslateTransform>(GetRenderTransform(itemsControl, "b"));
        window.Close();
    }
}
