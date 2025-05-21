using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Core;

/// <summary>
/// Invoke command behavior base class.
/// </summary>
public abstract class InvokeCommandBehaviorBase : StyledElementBehavior<Control>
{
    /// <summary>
    /// Identifies the <seealso cref="Command"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<ICommand?> CommandProperty =
        AvaloniaProperty.Register<InvokeCommandBehaviorBase, ICommand?>(nameof(Command));

    /// <summary>
    /// Identifies the <seealso cref="CommandParameter"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<object?> CommandParameterProperty =
        AvaloniaProperty.Register<InvokeCommandBehaviorBase, object?>(nameof(CommandParameter));

    /// <summary>
    /// Identifies the <seealso cref="InputConverter"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<IValueConverter?> InputConverterProperty =
        AvaloniaProperty.Register<InvokeCommandBehaviorBase, IValueConverter?>(nameof(InputConverter));

    /// <summary>
    /// Identifies the <seealso cref="InputConverterParameter"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<object?> InputConverterParameterProperty =
        AvaloniaProperty.Register<InvokeCommandBehaviorBase, object?>(nameof(InputConverterParameter));

    /// <summary>
    /// Identifies the <seealso cref="InputConverterLanguage"/> avalonia property.
    /// </summary>
    /// <remarks>The string.Empty used for default value string means the invariant culture.</remarks>
    public static readonly StyledProperty<string?> InputConverterLanguageProperty =
        AvaloniaProperty.Register<InvokeCommandBehaviorBase, string?>(nameof(InputConverterLanguage), string.Empty);

    /// <summary>
    /// Gets or sets the command this action should invoke. This is an avalonia property.
    /// </summary>
    public ICommand? Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }
  
    /// <summary>
    /// Gets or sets the parameter that is passed to <see cref="System.Windows.Input.ICommand.Execute(object)"/>.
    /// If this is not set, the parameter from the <seealso cref="IAction.Execute(object, object)"/> method will be used.
    /// This is an optional avalonia property.
    /// </summary>
    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }
  
    /// <summary>
    /// Gets or sets the converter that is run on the parameter from the <seealso cref="IAction.Execute(object, object)"/> method.
    /// This is an optional avalonia property.
    /// </summary>
    public IValueConverter? InputConverter
    {
        get => GetValue(InputConverterProperty);
        set => SetValue(InputConverterProperty, value);
    }

    /// <summary>
    /// Gets or sets the parameter that is passed to the <see cref="IValueConverter.Convert"/>
    /// method of <see cref="InputConverter"/>.
    /// This is an optional avalonia property.
    /// </summary>
    public object? InputConverterParameter
    {
        get => GetValue(InputConverterParameterProperty);
        set => SetValue(InputConverterParameterProperty, value);
    }
    
    /// <summary>
    /// Gets or sets the language that is passed to the <see cref="IValueConverter.Convert"/>
    /// method of <see cref="InputConverter"/>.
    /// This is an optional avalonia property.
    /// </summary>
    public string? InputConverterLanguage
    {
        get => GetValue(InputConverterLanguageProperty);
        set => SetValue(InputConverterLanguageProperty, value);
    }

    /// <summary>
    /// Specifies whether the EventArgs of the event that triggered this action should be passed to the Command as a parameter.
    /// </summary>
    public bool PassEventArgsToCommand { get; set; }

    /// <summary>
    /// Resolves the command parameter that will be supplied to the
    /// <see cref="System.Windows.Input.ICommand"/> implementation.
    /// </summary>
    /// <param name="parameter">The original parameter provided by the trigger.</param>
    /// <returns>The resolved parameter value.</returns>
    protected object? ResolveParameter(object? parameter)
    {
        object? resolvedParameter = null;
        if (IsSet(CommandParameterProperty))
        {
            resolvedParameter = CommandParameter;
        }
        else if (InputConverter is not null)
        {
            resolvedParameter = InputConverter.Convert(
                parameter,
                typeof(object),
                InputConverterParameter,
                InputConverterLanguage is not null
                    ? 
                    new System.Globalization.CultureInfo(InputConverterLanguage)
                    : System.Globalization.CultureInfo.CurrentCulture);
        }
        else
        {
            if (PassEventArgsToCommand)
            {
                resolvedParameter = parameter;
            }
        }

        return resolvedParameter;
    }
}
