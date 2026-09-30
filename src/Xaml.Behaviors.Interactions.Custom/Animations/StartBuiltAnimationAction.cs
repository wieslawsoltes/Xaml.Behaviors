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
/// Starts an animation built in code on the associated control.
/// </summary>
public partial class StartBuiltAnimationAction : AvaloniaObject, IAction
{

    /// <summary>
    /// Gets or sets the animation to run.
    /// </summary>
    [StyledProperty]
    public partial Animation.Animation? Animation { get; set; }

    /// <summary>
    /// Gets or sets the animation builder used to create an animation.
    /// </summary>
    [StyledProperty]
    public partial IAnimationBuilder? AnimationBuilder { get; set; }

    /// <inheritdoc />
    public object Execute(object? sender, object? parameter)
    {
        return AnimationRunner.TryBuildAndRun(sender as Control, Animation, AnimationBuilder);
    }
}
