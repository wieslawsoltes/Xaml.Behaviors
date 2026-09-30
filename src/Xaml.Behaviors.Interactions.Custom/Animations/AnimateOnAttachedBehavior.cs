// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactivity;
using PlatformAnimation = Microsoft.UI.Xaml.Media.Animation.Storyboard;
#else
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
using PlatformAnimation = Avalonia.Animation.Animation;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Runs an animation when the associated control is attached to the visual tree.
/// </summary>
public partial class AnimateOnAttachedBehavior : AttachedToVisualTreeBehavior<Control>
{

    /// <summary>
    /// Gets or sets the animation to run.
    /// </summary>
    [StyledProperty]
    public partial PlatformAnimation? Animation { get; set; }

    /// <summary>
    /// Gets or sets the animation builder used to create an animation.
    /// </summary>
    [StyledProperty]
    public partial IAnimationBuilder? AnimationBuilder { get; set; }

    /// <inheritdoc />
    protected override IDisposable OnAttachedToVisualTreeOverride()
    {
        if (AssociatedObject is null)
        {
            return DisposableAction.Empty;
        }

        AnimationRunner.TryBuildAndRun(AssociatedObject, Animation, AnimationBuilder);

        return DisposableAction.Empty;
    }
}
