// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.IO;
using Avalonia;

namespace Avalonia.Xaml.Behaviors.Storyboards;

/// <summary>
/// Seeks a storyboard to the specified offset.
/// </summary>
public sealed class SeekStoryboardAction : StoryboardRegistryActionBase
{
    /// <summary>
    /// Identifies the <see cref="Offset"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<TimeSpan> OffsetProperty =
        AvaloniaProperty.Register<SeekStoryboardAction, TimeSpan>(nameof(Offset), TimeSpan.Zero);

    /// <summary>
    /// Identifies the <see cref="Origin"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<SeekOrigin> OriginProperty =
        AvaloniaProperty.Register<SeekStoryboardAction, SeekOrigin>(nameof(Origin), SeekOrigin.Begin);

    /// <summary>
    /// Gets or sets the seek offset applied when the action executes.
    /// </summary>
    public TimeSpan Offset
    {
        get => GetValue(OffsetProperty);
        set => SetValue(OffsetProperty, value);
    }

    /// <summary>
    /// Gets or sets the origin used for calculating the target position.
    /// </summary>
    public SeekOrigin Origin
    {
        get => GetValue(OriginProperty);
        set => SetValue(OriginProperty, value);
    }

    /// <inheritdoc />
    protected override object? ExecuteCore(in StoryboardActionContext context)
    {
        if (!TryGetInstance(context, out var instance))
        {
            return null;
        }

        instance!.Seek(Offset, Origin);
        return null;
    }
}
