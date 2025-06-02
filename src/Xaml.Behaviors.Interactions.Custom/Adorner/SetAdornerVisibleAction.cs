// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Sets the visibility of an <see cref="AdornerVisibilityBehavior"/> when executed.
/// </summary>
public class SetAdornerVisibleAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <see cref="TargetBehavior"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<AdornerVisibilityBehavior?> TargetBehaviorProperty =
        AvaloniaProperty.Register<SetAdornerVisibleAction, AdornerVisibilityBehavior?>(nameof(TargetBehavior));

    /// <summary>
    /// Identifies the <see cref="IsVisible"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<bool> IsVisibleProperty =
        AvaloniaProperty.Register<SetAdornerVisibleAction, bool>(nameof(IsVisible));

    /// <summary>
    /// Gets or sets the target behavior. This is an avalonia property.
    /// </summary>
    [ResolveByName]
    public AdornerVisibilityBehavior? TargetBehavior
    {
        get => GetValue(TargetBehaviorProperty);
        set => SetValue(TargetBehaviorProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the adorner should be visible. This is an avalonia property.
    /// </summary>
    public bool IsVisible
    {
        get => GetValue(IsVisibleProperty);
        set => SetValue(IsVisibleProperty, value);
    }

    /// <inheritdoc />
    public override object Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        var behavior = TargetBehavior ?? FindBehavior(sender as StyledElement);
        if (behavior is null)
        {
            return false;
        }

        behavior.IsVisible = IsVisible;
        return true;
    }

    private static AdornerVisibilityBehavior? FindBehavior(StyledElement? element)
    {
        if (element is null)
        {
            return null;
        }

        var behaviors = Interaction.GetBehaviors(element);
        foreach (var behavior in behaviors)
        {
            if (behavior is AdornerVisibilityBehavior avb)
            {
                return avb;
            }
        }

        return null;
    }
}
