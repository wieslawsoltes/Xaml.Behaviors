using System;
using Avalonia.Reactive;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// A trigger that subscribes to an <see cref="IObservable{T}"/> and executes its actions whenever a new value is produced.
/// The emitted value is exposed through the <see cref="Value"/> property and passed to the actions as a parameter.
/// </summary>
/// <typeparam name="T">The type of the observable sequence.</typeparam>
public class ObservableTriggerBehavior<T> : StyledElementTrigger
{
    /// <summary>
    /// Identifies the <seealso cref="Observable"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<IObservable<T>?> ObservableProperty =
        AvaloniaProperty.Register<ObservableTriggerBehavior<T>, IObservable<T>?>(nameof(Observable));

    /// <summary>
    /// Identifies the <seealso cref="Value"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<T?> ValueProperty =
        AvaloniaProperty.Register<ObservableTriggerBehavior<T>, T?>(nameof(Value));

    private IDisposable? _subscription;

    /// <summary>
    /// Gets or sets the observable sequence that triggers the actions. This is an avalonia property.
    /// </summary>
    public IObservable<T>? Observable
    {
        get => GetValue(ObservableProperty);
        set => SetValue(ObservableProperty, value);
    }

    /// <summary>
    /// Gets the last value received from the <see cref="Observable"/>. This is an avalonia property.
    /// </summary>
    public T? Value
    {
        get => GetValue(ValueProperty);
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
