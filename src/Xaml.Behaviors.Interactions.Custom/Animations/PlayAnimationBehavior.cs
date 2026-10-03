// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
#if UNO
using Xaml.Interactivity;
using PlatformAnimation = Microsoft.UI.Xaml.Media.Animation.Storyboard;
#else
using Avalonia.Xaml.Interactivity;
using PlatformAnimation = Avalonia.Animation.Animation;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Plays a specified <see cref="Animation"/> when the associated element is attached to the visual tree.
/// </summary>
public partial class PlayAnimationBehavior : AttachedToVisualTreeBehavior<Visual>
{

    /// <summary>
    /// Gets or sets the animation that will be played. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial PlatformAnimation? Animation { get; set; }

    /// <inheritdoc />
    protected override IDisposable OnAttachedToVisualTreeOverride()
    {
        AnimationRunner.TryRun(Animation, AssociatedObject);
        return DisposableAction.Empty;
    }
}
