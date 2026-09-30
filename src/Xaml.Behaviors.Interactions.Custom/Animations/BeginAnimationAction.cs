// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactivity;
using PlatformAnimation = Microsoft.UI.Xaml.Media.Animation.Storyboard;
#else
using Avalonia.Controls;
using Avalonia.Metadata;
using Avalonia.Xaml.Interactivity;
using PlatformAnimation = Avalonia.Animation.Animation;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Starts an <see cref="Animation.Animation"/> on a specified control when executed.
/// </summary>
public partial class BeginAnimationAction : StyledElementAction
{

    /// <summary>
    /// Gets or sets the animation to run. This is an avalonia property.
    /// </summary>
    [StyledProperty(Content = true)]
    public partial PlatformAnimation? Animation { get; set; }

    /// <summary>
    /// Gets or sets the control on which the animation will run. This is an avalonia property.
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial Control? TargetControl { get; set; }

    /// <inheritdoc />
    public override object Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        Control? control = TargetControl ?? sender as Control;
        return AnimationRunner.TryRun(Animation, control);
    }
}
