// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
#if UNO
using Microsoft.UI.Dispatching;
using Xaml.Interactivity;
#else
using Avalonia.Reactive;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// A trigger that subscribes to an <see cref="IObservable{T}"/> and executes its actions whenever a new value is produced.
/// The emitted value is exposed through the <see cref="Value"/> property and passed to the actions as a parameter.
/// </summary>
/// <typeparam name="T">The type of the observable sequence.</typeparam>
public partial class ObservableTriggerBehavior<T> : StyledElementTrigger
{
#pragma warning restore AVP1002

    /// <summary>
    /// Identifies the <seealso cref="Value"/> avalonia property.
    /// </summary>
#if UNO
    // The generator does not emit properties typed by a type parameter; WinUI only needs the value type for XAML.
#pragma warning disable IL2087
    public static readonly AvaloniaProperty ValueProperty =
        AvaloniaProperty.Register(nameof(Value), typeof(T), typeof(ObservableTriggerBehavior<T>), new Microsoft.UI.Xaml.PropertyMetadata(default(T)));
#pragma warning restore IL2087
#else
    public static readonly StyledProperty<T?> ValueProperty =
#pragma warning disable AVP1002
        AvaloniaProperty.Register<ObservableTriggerBehavior<T>, T?>(nameof(Value));
#pragma warning restore AVP1002
#endif

    private IDisposable? _subscription;

    /// <summary>
    /// Gets or sets the observable sequence that triggers the actions. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial IObservable<T>? Observable { get; set; }

    /// <summary>
    /// Gets the last value received from the <see cref="Observable"/>. This is an avalonia property.
    /// </summary>
    public T? Value
    {
        get => (T?)GetValue(ValueProperty);
        private set => SetCurrentValue(ValueProperty, value);
    }

    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();
        Subscribe();
    }

    /// <inheritdoc />
    protected override void OnDetaching()
    {
        base.OnDetaching();
        _subscription?.Dispose();
        _subscription = null;
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ObservableProperty)
        {
            Subscribe();
        }
    }

    private void Subscribe()
    {
        _subscription?.Dispose();
        var observable = Observable;
        if (observable is not null)
        {
            _subscription = observable
                .Subscribe(new AnonymousObserver<T>(value =>
                {
                    if (_subscription is null)
                    {
                        return;
                    }
                    Dispatcher.UIThread.Invoke(() =>
                    {
                        Value = value;
                        Execute(value);
                    });
                }));
        }
    }

    private void Execute(object? parameter)
    {
        if (AssociatedObject is null || !IsEnabled)
        {
            return;
        }

        Interaction.ExecuteActions(AssociatedObject, Actions, parameter);
    }
}
