// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.DragAndDrop;
#else
namespace Avalonia.Xaml.Interactions.DragAndDrop;
#endif

/// <summary>
/// Decides whether a pointer press reaching a drag source belongs to that drag source.
/// </summary>
/// <remarks>
/// A press belongs to the drag source when the pressed element is the drag source itself or one of its visual
/// descendants that is not inside another (nested) drag source. Templated content often has its own data context
/// (the text of a <c>Button</c> or a <c>ContentPresenter</c> showing non-control content), so the data contexts of the
/// pressed element and of the drag source are only compared when the pressed element is not a visual descendant of the
/// drag source (for example content of a popup).
/// </remarks>
internal static class DragSourcePressFilter
{
    /// <summary>
    /// Determines whether a press on <paramref name="source"/> starts a drag from <paramref name="dragSource"/>.
    /// </summary>
    /// <param name="dragSource">The element the drag behavior is attached to.</param>
    /// <param name="source">The element that raised the pointer press.</param>
    /// <returns><see langword="true"/> when the press belongs to <paramref name="dragSource"/>.</returns>
    public static bool BelongsToDragSource(Control dragSource, object? source)
    {
#if UNO
        var current = source as DependencyObject;
#else
        var current = source as Visual;
#endif
        while (current is not null)
        {
            if (ReferenceEquals(current, dragSource))
            {
                return true;
            }

            if (IsDragSource(current))
            {
                // The press belongs to a nested drag source.
                return false;
            }

            current = current.GetVisualParent();
        }

        return source is Control control && dragSource.DataContext == control.DataContext;
    }

    private static bool IsDragSource(AvaloniaObject element)
    {
        if (element.GetValue(Interaction.BehaviorsProperty) is not BehaviorCollection behaviors)
        {
            return false;
        }

        for (var i = 0; i < behaviors.Count; i++)
        {
            if (behaviors[i] is ContextDragBehaviorBase or ContextDragWithDirectionBehavior or TypedDragBehaviorBase)
            {
                return true;
            }
        }

        return false;
    }
}
