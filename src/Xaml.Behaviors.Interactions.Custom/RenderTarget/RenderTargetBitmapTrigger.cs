// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
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
/// Trigger that calls <see cref="IRenderTargetBitmapRenderHost.Render"/> periodically.
/// </summary>
public partial class RenderTargetBitmapTrigger : StyledElementTrigger
{
    private DispatcherTimer? _timer;

    /// <summary>
    /// Gets or sets the render host that should be rendered. This is an avalonia property.
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial IRenderTargetBitmapRenderHost? Target { get; set; }

    /// <summary>
    /// Gets or sets the interval, in milliseconds, between render ticks. This is an avalonia property.
    /// </summary>
    [StyledProperty(DefaultValue = 16)]
    public partial int MillisecondsPerTick { get; set; }

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

#if UNO
    private void OnTick(object? sender, object e)
#else
    private void OnTick(object? sender, EventArgs e)
#endif
    {
        Target?.Render();
        Interaction.ExecuteActions(AssociatedObject, Actions, e);
    }
}
