using System;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Trigger that calls <see cref="IRenderTargetBitmapRenderHost.Render"/> periodically.
/// </summary>
public class RenderTargetBitmapTrigger : StyledElementTrigger
{
    private DispatcherTimer? _timer;

    /// <summary>
    /// Identifies the <see cref="Target"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<IRenderTargetBitmapRenderHost?> TargetProperty =
        AvaloniaProperty.Register<RenderTargetBitmapTrigger, IRenderTargetBitmapRenderHost?>(nameof(Target));

    /// <summary>
    /// Identifies the <see cref="MillisecondsPerTick"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<int> MillisecondsPerTickProperty =
        AvaloniaProperty.Register<RenderTargetBitmapTrigger, int>(nameof(MillisecondsPerTick), 16);

    /// <summary>
    /// Gets or sets the render host that should be rendered. This is an avalonia property.
    /// </summary>
    [ResolveByName]
    public IRenderTargetBitmapRenderHost? Target
    {
        get => GetValue(TargetProperty);
        set => SetValue(TargetProperty, value);
    }

    /// <summary>
    /// Gets or sets the interval, in milliseconds, between render ticks. This is an avalonia property.
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
        Target?.Render();
        Interaction.ExecuteActions(AssociatedObject, Actions, e);
    }
}
