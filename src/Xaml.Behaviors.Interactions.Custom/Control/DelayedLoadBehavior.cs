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
/// Delays the visibility of the associated control when it is attached to the visual tree.
/// </summary>
public sealed partial class DelayedLoadBehavior : AttachedToVisualTreeBehavior<Control>
{

    private DispatcherTimer? _timer;

    /// <summary>
    /// Gets or sets the delay before the control becomes visible.
    /// </summary>
    [StyledProperty(DefaultValueExpression = "TimeSpan.FromMilliseconds(500)")]
    public partial TimeSpan Delay { get; set; }

    /// <inheritdoc />
    protected override IDisposable OnAttachedToVisualTreeOverride()
    {
        if (AssociatedObject is null)
        {
            return DisposableAction.Empty;
        }

        AssociatedObject.IsVisible = false;

        _timer = new DispatcherTimer { Interval = Delay };
        _timer.Tick += OnTick;
        _timer.Start();

        return new DisposableAction(DisposeTimer);
    }

    private void OnTick(object? sender, EventArgs e)
    {
        if (AssociatedObject is not null)
        {
            AssociatedObject.SetCurrentValue(Visual.IsVisibleProperty, true);
        }
        DisposeTimer();
    }

    private void DisposeTimer()
    {
        if (_timer is not null)
        {
            _timer.Tick -= OnTick;
            _timer.Stop();
            _timer = null;
        }
    }
}
