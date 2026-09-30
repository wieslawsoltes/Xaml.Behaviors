// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Xaml.Interactions.Custom;
#else
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactions.Custom;
#endif

namespace AnimationsTestApplication.Controls;

/// <summary>
/// Demonstrates direct transition collection operations.
/// </summary>
public class TransitionOperationsDemoControl : ContentControl
{
    private IDisposable? _transitionSubscription;
#if UNO
    private TransitionBase? _transition;
#else
    private DoubleTransition? _transition;
    private int _transitionChangeCount;
#endif
    private bool _dimmed;

#if UNO
    /// <summary>
    /// Identifies the <see cref="TransitionChangeCount"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TransitionChangeCountProperty =
        DependencyProperty.Register(
            nameof(TransitionChangeCount),
            typeof(int),
            typeof(TransitionOperationsDemoControl),
            new PropertyMetadata(0));

    /// <summary>
    /// Gets the number of transition collection values reported by the active observation.
    /// </summary>
    public int TransitionChangeCount
    {
        get => (int)GetValue(TransitionChangeCountProperty);
        private set => SetValue(TransitionChangeCountProperty, value);
    }

    public static readonly DependencyProperty DurationProperty =
        DependencyProperty.Register(
            nameof(Duration),
            typeof(TimeSpan),
            typeof(TransitionOperationsDemoControl),
            new PropertyMetadata(TimeSpan.FromMilliseconds(300)));

    public TransitionOperationsDemoControl()
    {
        // WinUI has no visual tree attachment notifications: Loaded/Unloaded are raised for the live tree.
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }
#else
    /// <summary>
    /// Identifies the <see cref="TransitionChangeCount"/> direct property.
    /// </summary>
    public static readonly DirectProperty<TransitionOperationsDemoControl, int> TransitionChangeCountProperty =
        AvaloniaProperty.RegisterDirect<TransitionOperationsDemoControl, int>(
            nameof(TransitionChangeCount),
            control => control.TransitionChangeCount);

    /// <summary>
    /// Gets the number of transition collection values reported by the active observation.
    /// </summary>
    public int TransitionChangeCount
    {
        get => _transitionChangeCount;
        private set => SetAndRaise(TransitionChangeCountProperty, ref _transitionChangeCount, value);
    }

    public static readonly StyledProperty<TimeSpan> DurationProperty =
        AvaloniaProperty.Register<TransitionOperationsDemoControl, TimeSpan>(
            nameof(Duration),
            TimeSpan.FromMilliseconds(300));
#endif

    public TimeSpan Duration
    {
        get => (TimeSpan)GetValue(DurationProperty);
        set => SetValue(DurationProperty, value);
    }

#if UNO
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _transitionSubscription?.Dispose();
        TransitionChangeCount = 0;
        _transitionSubscription = TransitionOperations.Observe(
            this,
            _ => TransitionChangeCount++);
        // WinUI transition collections hold theme transitions; there is no property (DoubleTransition) transition.
        // The collection operations are shown with a theme transition and the opacity change is animated explicitly.
        _transition = new EntranceThemeTransition();
        TransitionOperations.Add(this, _transition);
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        TransitionOperations.Remove(this, _transition);
        _transitionSubscription?.Dispose();
        _transitionSubscription = null;
        _transition = null;
    }
#else
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        TransitionChangeCount = 0;
        _transitionSubscription = TransitionOperations.Observe(
            this,
            _ => TransitionChangeCount++);
        _transition = new DoubleTransition
        {
            Property = OpacityProperty,
            Duration = Duration
        };
        TransitionOperations.Add(this, _transition);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        TransitionOperations.Remove(this, _transition);
        _transitionSubscription?.Dispose();
        _transitionSubscription = null;
        _transition = null;
        base.OnDetachedFromVisualTree(e);
    }
#endif

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        _dimmed = !_dimmed;
#if UNO
        double from = Opacity;
        Opacity = _dimmed ? 0.25d : 1d;
        AnimateOpacity(from, Opacity);
#else
        Opacity = _dimmed ? 0.25d : 1d;
#endif
        e.Handled = true;
    }
#if UNO

    private void AnimateOpacity(double from, double to)
    {
        // UIElement.OpacityTransition (the WinUI counterpart of an opacity DoubleTransition) is not implemented on
        // Uno Platform: a storyboard animates from the previous opacity to the new local value over Duration.
        DoubleAnimation animation = new()
        {
            From = from,
            To = to,
            Duration = new Duration(Duration),
            FillBehavior = FillBehavior.Stop
        };
        Storyboard.SetTargetProperty(animation, nameof(Opacity));
        Storyboard storyboard = new();
        storyboard.Children.Add(animation);
        AnimationRunner.TryRun(storyboard, this);
    }
#endif
}
