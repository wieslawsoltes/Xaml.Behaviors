using System;
using Avalonia.Controls;
using Avalonia.Reactive;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Executes actions when the window state matches the specified value.
/// </summary>
public class WindowStateTrigger : AttachedToVisualTreeTriggerBase<Window>
{
    /// <summary>
    /// Identifies the <see cref="State"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<WindowState> StateProperty =
        AvaloniaProperty.Register<WindowStateTrigger, WindowState>(nameof(State), WindowState.Normal);

    /// <summary>
    /// Gets or sets the window state to trigger on. This is an avalonia property.
    /// </summary>
    public WindowState State
    {
        get => GetValue(StateProperty);
        set => SetValue(StateProperty, value);
    }

    private IDisposable? _subscription;

    /// <inheritdoc />
    protected override IDisposable OnAttachedToVisualTreeOverride()
    {
        if (AssociatedObject is { } window)
        {
            _subscription = window.GetObservable(Window.WindowStateProperty)
                .Subscribe(new AnonymousObserver<WindowState>(OnStateChanged));
            OnStateChanged(window.WindowState);
        }

        return DisposableAction.Create(() => _subscription?.Dispose());
    }

    private void OnStateChanged(WindowState state)
    {
        if (!IsEnabled || state != State)
        {
            return;
        }

        Dispatcher.UIThread.Post(() => Interaction.ExecuteActions(AssociatedObject, Actions, state));
    }
}
