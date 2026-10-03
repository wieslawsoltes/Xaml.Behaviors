// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Clears the selection of the associated list box when it is attached to the visual tree.
/// </summary>
/// <remarks>
/// On Uno Platform the associated object is a WinUI <c>ListViewBase</c> (<c>ListView</c>, <c>GridView</c>).
/// </remarks>
#if UNO
public class ListBoxUnselectAllBehavior : AttachedToVisualTreeBehavior<Microsoft.UI.Xaml.Controls.ListViewBase>
#else
public class ListBoxUnselectAllBehavior : AttachedToVisualTreeBehavior<ListBox>
#endif
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected override System.IDisposable OnAttachedToVisualTreeOverride()
    {
#if UNO
        ListViewBaseSelection.UnselectAll(AssociatedObject);
#else
        AssociatedObject?.UnselectAll();
#endif

        return DisposableAction.Empty;
    }
}
