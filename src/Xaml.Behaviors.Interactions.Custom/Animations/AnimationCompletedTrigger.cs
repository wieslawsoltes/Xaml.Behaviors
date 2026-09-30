// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Threading.Tasks;
#if UNO
using Microsoft.UI.Dispatching;
using Xaml.Interactivity;
#else
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Runs a specified <see cref="Animation.Animation"/> and executes actions when it completes.
/// </summary>
public partial class AnimationCompletedTrigger : AttachedToVisualTreeTrigger
{

    /// <summary>
    /// Gets or sets the animation to run. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial Animation.Animation? Animation { get; set; }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        if (AssociatedObject is null)
        {
            return;
        }

        if (Animation is null)
        {
            Execute(parameter: null);
            return;
        }

        _ = Run();

        async Task Run()
        {
            await AnimationRunner.RunAsync(Animation, AssociatedObject);
            Dispatcher.UIThread.Post(() => Execute(parameter: null));
        }
    }

    private void Execute(object? parameter)
    {
        if (!IsEnabled)
        {
            return;
        }

        Interaction.ExecuteActions(AssociatedObject, Actions, parameter);
    }
}
