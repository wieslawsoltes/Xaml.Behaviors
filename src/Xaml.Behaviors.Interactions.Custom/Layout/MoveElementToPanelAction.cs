// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Moves the associated or target element to a specified <see cref="Panel"/>.
/// </summary>
public sealed class MoveElementToPanelAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <see cref="TargetPanel"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Panel?> TargetPanelProperty =
        AvaloniaProperty.Register<MoveElementToPanelAction, Panel?>(nameof(TargetPanel));

    /// <summary>
    /// Gets or sets the panel to move the element into. If not set, the sender is used as target.
    /// </summary>
    [ResolveByName]
    public Panel? TargetPanel
    {
        get => GetValue(TargetPanelProperty);
        set => SetValue(TargetPanelProperty, value);
    }

    /// <inheritdoc />
    public override object Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        if (sender is not Control element)
        {
            return false;
        }

        var target = TargetPanel ?? sender as Panel;
        if (target is null)
        {
            return false;
        }

        if (element.Parent is Panel source)
        {
            source.Children.Remove(element);
        }

        target.Children.Add(element);
        return true;
    }
}
