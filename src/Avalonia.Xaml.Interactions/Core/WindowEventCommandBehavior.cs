using System;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Core;

/// <summary>
/// Executes a command for specified window events.
/// </summary>
public class WindowEventCommandBehavior : AttachedToVisualTreeBehavior<Window>
{
    /// <summary>
    /// Identifies the <seealso cref="Event"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<WindowEvent> EventProperty =
        AvaloniaProperty.Register<WindowEventCommandBehavior, WindowEvent>(nameof(Event));

    /// <summary>
    /// Identifies the <seealso cref="Command"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<ICommand?> CommandProperty =
        AvaloniaProperty.Register<WindowEventCommandBehavior, ICommand?>(nameof(Command));

    /// <summary>
    /// Identifies the <seealso cref="CommandParameter"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<object?> CommandParameterProperty =
        AvaloniaProperty.Register<WindowEventCommandBehavior, object?>(nameof(CommandParameter));

    /// <summary>
    /// Gets or sets the window event to listen for.
    /// </summary>
    public WindowEvent Event
    {
        get => GetValue(EventProperty);
        set => SetValue(EventProperty, value);
    }

    /// <summary>
    /// Gets or sets the command to execute.
    /// </summary>
    public ICommand? Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    /// <summary>
    /// Gets or sets the command parameter.
    /// </summary>
    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    /// <inheritdoc />
    protected override IDisposable OnAttachedToVisualTreeOverride()
    {
        if (AssociatedObject is not Window window)
        {
            return DisposableAction.Empty;
        }

        void Handler(object? sender, EventArgs e)
        {
            if (Command?.CanExecute(CommandParameter) == true)
            {
                Command.Execute(CommandParameter);
            }
        }

        switch (Event)
        {
            case WindowEvent.Opened:
                window.Opened += Handler;
                break;
            case WindowEvent.Closing:
                window.Closing += Handler;
                break;
            case WindowEvent.Closed:
                window.Closed += Handler;
                break;
            default:
                return DisposableAction.Empty;
        }

        return DisposableAction.Create(() =>
        {
            switch (Event)
            {
                case WindowEvent.Opened:
                    window.Opened -= Handler;
                    break;
                case WindowEvent.Closing:
                    window.Closing -= Handler;
                    break;
                case WindowEvent.Closed:
                    window.Closed -= Handler;
                    break;
            }
        });
    }
}

/// <summary>
/// Available window events for <see cref="WindowEventCommandBehavior"/>.
/// </summary>
public enum WindowEvent
{
    /// <summary>
    /// Window opened.
    /// </summary>
    Opened,
    /// <summary>
    /// Window closing.
    /// </summary>
    Closing,
    /// <summary>
    /// Window closed.
    /// </summary>
    Closed,
}
