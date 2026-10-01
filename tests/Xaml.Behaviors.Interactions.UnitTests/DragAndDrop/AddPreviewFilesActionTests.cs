// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Collections.ObjectModel;
#if UNO
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactions.DragAndDrop;
#else
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Xaml.Interactions.DragAndDrop;
#endif
using Xunit;

#if UNO
namespace Xaml.Interactions.UnitTests.DragAndDrop;
#else
namespace Avalonia.Xaml.Interactions.UnitTests.DragAndDrop;
#endif

/// <summary>
/// Tests how <see cref="AddPreviewFilesAction"/> fills its items control (storage items can only be created by the
/// platform, so the files are replaced by plain items; the Uno twin also drags real files).
/// </summary>
public class AddPreviewFilesActionTests
{
    [AvaloniaFact]
    public void TryReplaceItems_Replaces_Items_When_ItemsSource_Is_Not_Set()
    {
        var itemsControl = new ItemsControl();
        itemsControl.Items.Add("stale");

        var result = AddPreviewFilesAction.TryReplaceItems(itemsControl, ["a.txt", "b.txt"]);

        Assert.True(result);
        Assert.Equal(["a.txt", "b.txt"], itemsControl.Items);
    }

    [AvaloniaFact]
    public void TryReplaceItems_Replaces_Items_Of_ItemsSource_List()
    {
        var items = new ObservableCollection<object> { "stale" };
        var itemsControl = new ItemsControl { ItemsSource = items };

        var result = AddPreviewFilesAction.TryReplaceItems(itemsControl, ["a.txt", "b.txt"]);

        Assert.True(result);
        Assert.Equal(["a.txt", "b.txt"], items);
    }

    [AvaloniaFact]
    public void TryReplaceItems_Returns_False_For_Read_Only_ItemsSource()
    {
        var items = Array.AsReadOnly(new object[] { "stale" });
        var itemsControl = new ItemsControl { ItemsSource = items };

        var result = AddPreviewFilesAction.TryReplaceItems(itemsControl, ["a.txt"]);

        Assert.False(result);
        Assert.Equal("stale", Assert.Single(items));
    }
}
