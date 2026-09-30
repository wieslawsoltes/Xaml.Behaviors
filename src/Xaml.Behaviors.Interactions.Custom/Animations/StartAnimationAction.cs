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
/// Starts an <see cref="Animation.Animation"/> on the associated control.
/// </summary>
public partial class StartAnimationAction : AvaloniaObject, IAction
{

    /// <summary>
    /// 
    /// </summary>
    [StyledProperty]
    public partial Animation.Animation? Animation { get; set; }

    /// <summary>
    /// Executes the action.
    /// </summary>
    /// <param name="sender">The control initiating the action.</param>
    /// <param name="parameter">Optional parameter.</param>
    /// <returns>True if the animation was started; otherwise, false.</returns>
    public object Execute(object? sender, object? parameter)
    {
        return AnimationRunner.TryRun(Animation, sender as Control);
    }
}
