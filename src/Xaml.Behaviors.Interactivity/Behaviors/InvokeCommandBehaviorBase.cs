// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Windows.Input;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
#else
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Reactive;
#endif

#if UNO
namespace Xaml.Interactivity;
#else
namespace Avalonia.Xaml.Interactivity;
#endif

/// <summary>
/// Invoke command behavior base class.
/// </summary>
public abstract class InvokeCommandBehaviorBase : StyledElementBehavior<Control>
{
    private readonly CommandCanExecuteObserver _commandCanExecuteObserver;
    private readonly CommandCanExecuteIsEnabledBinder _commandCanExecuteIsEnabledBinder;
    private bool _canExecuteCommand = true;
    private bool _passEventArgsToCommand;

    /// <summary>
    /// Identifies the <seealso cref="Command"/> avalonia property.
    /// </summary>
#if UNO
    public static readonly DependencyProperty CommandProperty =
#else
    public static readonly StyledProperty<ICommand?> CommandProperty =
#endif
        AvaloniaProperty.Register<InvokeCommandBehaviorBase, ICommand?>(nameof(Command));

    /// <summary>
    /// Identifies the <seealso cref="CanExecuteCommand"/> avalonia property.
    /// </summary>
#if UNO
    public static readonly DependencyProperty CanExecuteCommandProperty =
#else
    public static readonly DirectProperty<InvokeCommandBehaviorBase, bool> CanExecuteCommandProperty =
#endif
        AvaloniaProperty.RegisterDirect<InvokeCommandBehaviorBase, bool>(nameof(CanExecuteCommand), behavior => behavior.CanExecuteCommand);

    /// <summary>
    /// Identifies the <seealso cref="CanExecuteCommandParameter"/> avalonia property.
    /// </summary>
#if UNO
    public static readonly DependencyProperty CanExecuteCommandParameterProperty =
#else
    public static readonly StyledProperty<object?> CanExecuteCommandParameterProperty =
#endif
        AvaloniaProperty.Register<InvokeCommandBehaviorBase, object?>(nameof(CanExecuteCommandParameter));

    /// <summary>
    /// Identifies the <seealso cref="UseCommandCanExecuteForIsEnabled"/> avalonia property.
    /// </summary>
#if UNO
    public static readonly DependencyProperty UseCommandCanExecuteForIsEnabledProperty =
#else
    public static readonly StyledProperty<bool> UseCommandCanExecuteForIsEnabledProperty =
#endif
        AvaloniaProperty.Register<InvokeCommandBehaviorBase, bool>(nameof(UseCommandCanExecuteForIsEnabled));

    /// <summary>
    /// Identifies the <seealso cref="CommandParameter"/> avalonia property.
    /// </summary>
#if UNO
    public static readonly DependencyProperty CommandParameterProperty =
#else
    public static readonly StyledProperty<object?> CommandParameterProperty =
#endif
        AvaloniaProperty.Register<InvokeCommandBehaviorBase, object?>(nameof(CommandParameter));

    /// <summary>
    /// Identifies the <seealso cref="InputConverter"/> avalonia property.
    /// </summary>
#if UNO
    public static readonly DependencyProperty InputConverterProperty =
#else
    public static readonly StyledProperty<IValueConverter?> InputConverterProperty =
#endif
        AvaloniaProperty.Register<InvokeCommandBehaviorBase, IValueConverter?>(nameof(InputConverter));

    /// <summary>
    /// Identifies the <seealso cref="InputConverterParameter"/> avalonia property.
    /// </summary>
#if UNO
    public static readonly DependencyProperty InputConverterParameterProperty =
#else
    public static readonly StyledProperty<object?> InputConverterParameterProperty =
#endif
        AvaloniaProperty.Register<InvokeCommandBehaviorBase, object?>(nameof(InputConverterParameter));

    /// <summary>
    /// Identifies the <seealso cref="InputConverterLanguage"/> avalonia property.
    /// </summary>
    /// <remarks>The string.Empty used for default value string means the invariant culture.</remarks>
#if UNO
    public static readonly DependencyProperty InputConverterLanguageProperty =
#else
    public static readonly StyledProperty<string?> InputConverterLanguageProperty =
#endif
        AvaloniaProperty.Register<InvokeCommandBehaviorBase, string?>(nameof(InputConverterLanguage), string.Empty);

    /// <summary>
    /// Initializes a new instance of the <see cref="InvokeCommandBehaviorBase"/> class.
    /// </summary>
    protected InvokeCommandBehaviorBase()
    {
        _commandCanExecuteObserver = new CommandCanExecuteObserver(SetCanExecuteCommand);
        _commandCanExecuteIsEnabledBinder = new CommandCanExecuteIsEnabledBinder();
    }

    /// <summary>
    /// Gets or sets the command this action should invoke. This is an avalonia property.
    /// </summary>
    public ICommand? Command
    {
        get => (ICommand?)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    /// <summary>
    /// Gets a value indicating whether <see cref="Command"/> can execute with the current can-execute parameter.
    /// </summary>
    public bool CanExecuteCommand
    {
        get => _canExecuteCommand;
        private set => SetAndRaise(CanExecuteCommandProperty, ref _canExecuteCommand, value);
    }

    /// <summary>
    /// Gets or sets the parameter that is passed to <see cref="ICommand.CanExecute(object)"/>.
    /// When this property is not set, <see cref="CommandParameter"/> is used if it is set.
    /// This property does not change the parameter passed to <see cref="ICommand.Execute(object)"/>.
    /// </summary>
    public object? CanExecuteCommandParameter
    {
        get => (object?)GetValue(CanExecuteCommandParameterProperty);
        set => SetValue(CanExecuteCommandParameterProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the associated control's
    /// <see cref="Avalonia.Input.InputElement.IsEnabled"/> property should follow <see cref="CanExecuteCommand"/>.
    /// </summary>
    public bool UseCommandCanExecuteForIsEnabled
    {
        get => (bool)GetValue(UseCommandCanExecuteForIsEnabledProperty);
        set => SetValue(UseCommandCanExecuteForIsEnabledProperty, value);
    }
  
    /// <summary>
    /// Gets or sets the parameter that is passed to <see cref="System.Windows.Input.ICommand.Execute(object)"/>.
    /// If this is not set, the parameter from the <seealso cref="IAction.Execute(object, object)"/> method will be used.
    /// This is an optional avalonia property.
    /// </summary>
    public object? CommandParameter
    {
        get => (object?)GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }
  
    /// <summary>
    /// Gets or sets the converter that is run on the parameter from the <seealso cref="IAction.Execute(object, object)"/> method.
    /// This is an optional avalonia property.
    /// </summary>
    public IValueConverter? InputConverter
    {
        get => (IValueConverter?)GetValue(InputConverterProperty);
        set => SetValue(InputConverterProperty, value);
    }

    /// <summary>
    /// Gets or sets the parameter that is passed to the <see cref="IValueConverter.Convert"/>
    /// method of <see cref="InputConverter"/>.
    /// This is an optional avalonia property.
    /// </summary>
    public object? InputConverterParameter
    {
        get => (object?)GetValue(InputConverterParameterProperty);
        set => SetValue(InputConverterParameterProperty, value);
    }
    
    /// <summary>
    /// Gets or sets the language that is passed to the <see cref="IValueConverter.Convert"/>
    /// method of <see cref="InputConverter"/>.
    /// This is an optional avalonia property.
    /// </summary>
    public string? InputConverterLanguage
    {
        get => (string?)GetValue(InputConverterLanguageProperty);
        set => SetValue(InputConverterLanguageProperty, value);
    }

    /// <summary>
    /// Specifies whether the EventArgs of the event that triggered this action should be passed to the Command as a parameter.
    /// </summary>
    public bool PassEventArgsToCommand
    {
        get => _passEventArgsToCommand;
        set
        {
            if (_passEventArgsToCommand == value)
            {
                return;
            }

            _passEventArgsToCommand = value;
            _commandCanExecuteObserver.Update(Command, ResolveCanExecuteParameter(), IsCanExecuteParameterKnown());
        }
    }

    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();

        _commandCanExecuteObserver.Start(Command, ResolveCanExecuteParameter(), IsCanExecuteParameterKnown());
        UpdateCommandCanExecuteIsEnabledBinding();
    }

    /// <inheritdoc />
    protected override void OnDetaching()
    {
        _commandCanExecuteIsEnabledBinder.Stop();
        _commandCanExecuteObserver.Stop();

        base.OnDetaching();
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == CommandProperty ||
            change.Property == CommandParameterProperty ||
            change.Property == CanExecuteCommandParameterProperty ||
            change.Property == InputConverterProperty)
        {
            _commandCanExecuteObserver.Update(Command, ResolveCanExecuteParameter(), IsCanExecuteParameterKnown());
        }
        else if (change.Property == UseCommandCanExecuteForIsEnabledProperty)
        {
            UpdateCommandCanExecuteIsEnabledBinding();
        }
    }

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

    private object? ResolveCanExecuteParameter()
    {
        if (IsSet(CanExecuteCommandParameterProperty))
        {
            return CanExecuteCommandParameter;
        }

        return IsSet(CommandParameterProperty) ? CommandParameter : null;
    }

    private bool IsCanExecuteParameterKnown()
    {
        return IsSet(CanExecuteCommandParameterProperty) ||
               IsSet(CommandParameterProperty) ||
               (InputConverter is null && !PassEventArgsToCommand);
    }

    private void SetCanExecuteCommand(bool value)
    {
        CanExecuteCommand = value;
    }

    private void UpdateCommandCanExecuteIsEnabledBinding()
    {
        _commandCanExecuteIsEnabledBinder.Update(
            AssociatedObject,
            UseCommandCanExecuteForIsEnabled,
            AvaloniaObjectExtensions.GetObservable<bool>(this, CanExecuteCommandProperty));
    }
}
