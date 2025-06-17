// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Avalonia.Controls;
using Avalonia.Reactive;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Executes actions when the window state matches the specified value.
/// </summary>
public class WindowStateTrigger : AttachedToVisualTreeTriggerBase<Control>
{
    /// <summary>
    /// Identifies the <see cref="Window"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Window?> WindowProperty =
        AvaloniaProperty.Register<WindowStateTrigger, Window?>(nameof(Window));

    /// <summary>
    /// Identifies the <see cref="State"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<WindowState> StateProperty =
        AvaloniaProperty.Register<WindowStateTrigger, WindowState>(nameof(State));

    /// <summary>
    /// Gets or sets the window. If not set, the visual root window is used.
    /// </summary>
    [ResolveByName]
    public Window? Window
    {
        get => GetValue(WindowProperty);
        set => SetValue(WindowProperty, value);
    }

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
        var window = Window ?? AssociatedObject?.GetVisualRoot() as Window;
        if (window is null)
        {
            return DisposableAction.Empty;
        }

        _subscription = window.GetObservable(Window.WindowStateProperty)
            .Subscribe(new AnonymousObserver<WindowState>(OnStateChanged));
        OnStateChanged(window.WindowState);

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
