using System;
using System.ComponentModel;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Core;

/// <summary>
/// Executes a command when the associated control's DataContext reports validation errors.
/// </summary>
public class NotifyErrorsCommandBehavior : DisposingBehavior<Control>
{
    /// <summary>
    /// Identifies the <seealso cref="Command"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<ICommand?> CommandProperty =
        AvaloniaProperty.Register<NotifyErrorsCommandBehavior, ICommand?>(nameof(Command));

    /// <summary>
    /// Identifies the <seealso cref="CommandParameter"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<object?> CommandParameterProperty =
        AvaloniaProperty.Register<NotifyErrorsCommandBehavior, object?>(nameof(CommandParameter));

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
    protected override IDisposable OnAttachedOverride()
    {
        if (AssociatedObject?.DataContext is not INotifyDataErrorInfo indei)
        {
            return DisposableAction.Empty;
        }

        void Handler(object? s, DataErrorsChangedEventArgs e)
        {
            if (Command?.CanExecute(CommandParameter) == true)
            {
                Command.Execute(CommandParameter);
            }
        }

        indei.ErrorsChanged += Handler;

        return DisposableAction.Create(() => indei.ErrorsChanged -= Handler);
    }
}
