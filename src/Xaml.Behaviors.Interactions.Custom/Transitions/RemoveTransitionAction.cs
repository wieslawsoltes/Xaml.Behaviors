// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactivity;
using TransitionBase = Microsoft.UI.Xaml.Media.Animation.Transition;
#else
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Removes a <see cref="TransitionBase"/> from the <see cref="Avalonia.Animation.Transitions"/> collection on the target element.
/// </summary>
public partial class RemoveTransitionAction : StyledElementAction
{

    /// <summary>
    /// Gets or sets the transition to remove. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial TransitionBase? Transition { get; set; }

    /// <summary>
    /// Gets or sets the target styled element. This is an avalonia property.
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial StyledElement? StyledElement { get; set; }

    /// <inheritdoc />
    public override object Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        StyledElement? target = (StyledElement?)GetValue(StyledElementProperty) ?? sender as StyledElement;
        return TransitionOperations.Remove(target, Transition);
    }
}
