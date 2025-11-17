// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using Avalonia;

namespace Avalonia.Xaml.Behaviors.Storyboards;

/// <summary>
/// Adjusts the speed ratio of a running storyboard.
/// </summary>
public sealed class SetSpeedRatioStoryboardAction : StoryboardRegistryActionBase
{
    /// <summary>
    /// Identifies the <see cref="SpeedRatio"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<double> SpeedRatioProperty =
        AvaloniaProperty.Register<SetSpeedRatioStoryboardAction, double>(nameof(SpeedRatio), 1d);

    /// <summary>
    /// Gets or sets the desired speed ratio.
    /// </summary>
    public double SpeedRatio
    {
        get => GetValue(SpeedRatioProperty);
        set => SetValue(SpeedRatioProperty, value);
    }

    /// <inheritdoc />
    protected override object? ExecuteCore(in StoryboardActionContext context)
    {
        if (SpeedRatio <= 0)
        {
            throw new InvalidOperationException("SpeedRatio must be greater than zero.");
        }

        if (!TryGetInstance(context, out var instance))
        {
            return null;
        }

        instance!.SetSpeedRatio(SpeedRatio);
        return null;
    }
}
