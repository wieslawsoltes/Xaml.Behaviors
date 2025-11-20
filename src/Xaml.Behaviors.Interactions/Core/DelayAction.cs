// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Core;

/// <summary>
/// An action that waits for a specified duration.
/// </summary>
public class DelayAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <seealso cref="Duration"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<TimeSpan> DurationProperty =
        AvaloniaProperty.Register<DelayAction, TimeSpan>(nameof(Duration));

    /// <summary>
    /// Gets or sets the duration to wait. This is an avalonia property.
    /// </summary>
    public TimeSpan Duration
    {
        get => GetValue(DurationProperty);
        set => SetValue(DurationProperty, value);
    }

    /// <inheritdoc />
    public override object? Execute(object? sender, object? parameter)
    {
        return Task.Delay(Duration);
    }
}
