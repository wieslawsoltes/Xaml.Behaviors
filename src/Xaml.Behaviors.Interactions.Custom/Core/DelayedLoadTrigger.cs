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
/// Invokes its actions after the associated control is attached to the visual tree and a delay elapses.
/// </summary>
public sealed partial class DelayedLoadTrigger : StyledElementTrigger<Control>
{

    private DispatcherTimer? _timer;

    /// <summary>
    /// Gets or sets the delay before actions are executed.
    /// </summary>
    [StyledProperty(DefaultValueExpression = "TimeSpan.FromMilliseconds(500)")]
    public partial TimeSpan Delay { get; set; }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        _timer = new DispatcherTimer { Interval = Delay };
        _timer.Tick += OnTick;
        _timer.Start();
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        DisposeTimer();
    }

    private void OnTick(object? sender, object e)
    {
        Execute(parameter: null);
        DisposeTimer();
    }

    private void Execute(object? parameter)
    {
        if (!IsEnabled)
        {
            return;
        }

        Interaction.ExecuteActions(AssociatedObject, Actions, parameter);
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
