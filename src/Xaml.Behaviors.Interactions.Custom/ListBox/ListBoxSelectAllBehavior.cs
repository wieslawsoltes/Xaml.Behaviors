// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

#if UNO
using Xaml.Interactivity;
#else
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Selects all items of the associated list box when it is attached to the visual tree.
/// </summary>
/// <remarks>
/// On Uno Platform the associated object is a WinUI <c>ListViewBase</c> (<c>ListView</c>, <c>GridView</c>) with a
/// multiple or extended selection mode.
/// </remarks>
#if UNO
public class ListBoxSelectAllBehavior : AttachedToVisualTreeBehavior<Microsoft.UI.Xaml.Controls.ListViewBase>
#else
public class ListBoxSelectAllBehavior : AttachedToVisualTreeBehavior<Controls.ListBox>
#endif
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected override System.IDisposable OnAttachedToVisualTreeOverride()
    {
#if UNO
        ListViewBaseSelection.SelectAll(AssociatedObject);
#else
        AssociatedObject?.SelectAll();
#endif

        return DisposableAction.Empty;
    }
}
