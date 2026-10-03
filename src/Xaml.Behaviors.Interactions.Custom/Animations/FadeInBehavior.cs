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
/// Plays a simple fade in animation when the associated control is attached.
/// </summary>
public partial class FadeInBehavior : AttachedToVisualTreeBehavior<Visual>
{

    /// <summary>
    /// 
    /// </summary>
    [StyledProperty(DefaultValueExpression = "TimeSpan.FromMilliseconds(500)")]
    public partial TimeSpan InitialDelay { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [StyledProperty(DefaultValueExpression = "TimeSpan.FromMilliseconds(250)")]
    public partial TimeSpan Duration { get; set; }

    /// <summary>
    /// Called when the behavior is attached to the visual tree.
    /// </summary>
    protected override System.IDisposable OnAttachedToVisualTreeOverride()
    {
        if (AssociatedObject is null)
        {
            return DisposableAction.Empty;
        }

        PlatformAnimation animation = AnimationFactory.CreateFadeIn(InitialDelay, Duration);
        AnimationRunner.TryRun(animation, AssociatedObject);

        return DisposableAction.Empty;
    }
}
