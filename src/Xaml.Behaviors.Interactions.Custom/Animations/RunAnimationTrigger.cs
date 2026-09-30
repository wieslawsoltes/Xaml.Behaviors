// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Threading.Tasks;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Dispatching;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Runs an animation and invokes actions when the associated control is attached to the visual tree.
/// </summary>
public partial class RunAnimationTrigger : AttachedToVisualTreeTriggerBase<Control>
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
    protected override IDisposable OnAttachedToVisualTreeOverride()
    {
        if (AssociatedObject is null)
        {
            return DisposableAction.Empty;
        }

        Task? task = AnimationRunner.TryBuildAndRunAsync(
            AssociatedObject,
            Animation,
            AnimationBuilder);
        if (task is not null)
        {
            task.ContinueWith(_ =>
            {
                Dispatcher.UIThread.Post(() => Interaction.ExecuteActions(AssociatedObject, Actions, null));
            });
        }

        return DisposableAction.Empty;
    }
}
