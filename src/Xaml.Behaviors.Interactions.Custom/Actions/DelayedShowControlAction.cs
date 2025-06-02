using System;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Shows the associated or target control after a specified delay when executed.
/// </summary>
public sealed class DelayedShowControlAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <see cref="TargetControl"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Control?> TargetControlProperty =
        AvaloniaProperty.Register<DelayedShowControlAction, Control?>(nameof(TargetControl));

    /// <summary>
    /// Identifies the <see cref="Delay"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<TimeSpan> DelayProperty =
        AvaloniaProperty.Register<DelayedShowControlAction, TimeSpan>(nameof(Delay), TimeSpan.FromMilliseconds(500));

    /// <summary>
    /// Gets or sets the target control. This is an avalonia property.
    /// </summary>
    [ResolveByName]
    public Control? TargetControl
    {
        get => GetValue(TargetControlProperty);
        set => SetValue(TargetControlProperty, value);
    }

    /// <summary>
    /// Gets or sets the delay before the control is shown.
    /// </summary>
    public TimeSpan Delay
    {
        get => GetValue(DelayProperty);
        set => SetValue(DelayProperty, value);
    }

    /// <inheritdoc />
    public override object Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        var control = TargetControl ?? sender as Control;
        if (control is null)
        {
            return false;
        }

        DispatcherTimer? timer = null;
        void OnTick(object? s, EventArgs e)
        {
            timer!.Tick -= OnTick;
            timer.Stop();
            control.SetCurrentValue(Visual.IsVisibleProperty, true);
        }

        timer = new DispatcherTimer { Interval = Delay };
        timer.Tick += OnTick;
        timer.Start();

        return true;
    }
}
