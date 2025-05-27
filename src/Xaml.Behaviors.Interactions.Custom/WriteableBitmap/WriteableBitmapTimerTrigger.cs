// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// A trigger that fires its actions on a timer and passes a <see cref="WriteableBitmap"/> as parameter.
/// </summary>
public class WriteableBitmapTimerTrigger : StyledElementTrigger
{
    /// <summary>
    /// Identifies the <see cref="Bitmap"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<WriteableBitmap?> BitmapProperty =
        AvaloniaProperty.Register<WriteableBitmapTimerTrigger, WriteableBitmap?>(nameof(Bitmap));

    /// <summary>
    /// Identifies the <see cref="MillisecondsPerTick"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<int> MillisecondsPerTickProperty =
        AvaloniaProperty.Register<WriteableBitmapTimerTrigger, int>(nameof(MillisecondsPerTick), 16);

    private DispatcherTimer? _timer;

    /// <summary>
    /// Gets or sets the bitmap passed to actions. This is an avalonia property.
    /// </summary>
    [ResolveByName]
    public WriteableBitmap? Bitmap
    {
        get => GetValue(BitmapProperty);
        set => SetValue(BitmapProperty, value);
    }

    /// <summary>
    /// Gets or sets the timer interval in milliseconds. This is an avalonia property.
    /// </summary>
    public int MillisecondsPerTick
    {
        get => GetValue(MillisecondsPerTickProperty);
        set => SetValue(MillisecondsPerTickProperty, value);
    }

    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();
        StartTimer();
    }

    /// <inheritdoc />
    protected override void OnDetaching()
    {
        StopTimer();
        base.OnDetaching();
    }

    private void StartTimer()
    {
        if (_timer is not null)
        {
            return;
        }

        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(MillisecondsPerTick) };
        _timer.Tick += OnTick;
        _timer.Start();
    }

    private void StopTimer()
    {
        if (_timer is null)
        {
            return;
        }

        _timer.Tick -= OnTick;
        _timer.Stop();
        _timer = null;
    }

    private void OnTick(object? sender, EventArgs e)
    {
        if (!IsEnabled)
        {
            return;
        }

        Interaction.ExecuteActions(AssociatedObject, Actions, Bitmap);
    }
}
