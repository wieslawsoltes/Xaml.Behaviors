using System;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Controls.Selection;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Core;

/// <summary>
/// Executes a command when the DataGrid selection changes.
/// </summary>
public class DataGridSelectionChangedCommandBehavior : DisposingBehavior<DataGrid>
{
    /// <summary>
    /// Identifies the <seealso cref="Command"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<ICommand?> CommandProperty =
        AvaloniaProperty.Register<DataGridSelectionChangedCommandBehavior, ICommand?>(nameof(Command));

    /// <summary>
    /// Identifies the <seealso cref="CommandParameter"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<object?> CommandParameterProperty =
        AvaloniaProperty.Register<DataGridSelectionChangedCommandBehavior, object?>(nameof(CommandParameter));

    /// <summary>
    /// Command to execute on selection change.
    /// </summary>
    public ICommand? Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    /// <summary>
    /// Optional command parameter.
    /// </summary>
    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    /// <inheritdoc />
    protected override IDisposable OnAttachedOverride()
    {
        if (AssociatedObject is not { } grid)
        {
            return DisposableAction.Empty;
        }

        void Handler(object? s, SelectionChangedEventArgs e)
        {
            if (Command?.CanExecute(CommandParameter) == true)
            {
                Command.Execute(CommandParameter);
            }
        }

        grid.SelectionChanged += Handler;
        return DisposableAction.Create(() => grid.SelectionChanged -= Handler);
    }
}
