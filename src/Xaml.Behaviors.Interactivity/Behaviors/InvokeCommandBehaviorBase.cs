// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Reactive;

namespace Avalonia.Xaml.Interactivity;

/// <summary>
/// Invoke command behavior base class.
/// </summary>
public abstract class InvokeCommandBehaviorBase : StyledElementBehavior<Control>
{
    private readonly CommandCanExecuteObserver _commandCanExecuteObserver;
    private readonly CommandCanExecuteIsEnabledBinder _commandCanExecuteIsEnabledBinder;
    private bool _canExecuteCommand = true;

    /// <summary>
    /// Identifies the <seealso cref="Command"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<ICommand?> CommandProperty =
        AvaloniaProperty.Register<InvokeCommandBehaviorBase, ICommand?>(nameof(Command));

    /// <summary>
    /// Identifies the <seealso cref="CanExecuteCommand"/> avalonia property.
    /// </summary>
    public static readonly DirectProperty<InvokeCommandBehaviorBase, bool> CanExecuteCommandProperty =
        AvaloniaProperty.RegisterDirect<InvokeCommandBehaviorBase, bool>(nameof(CanExecuteCommand), behavior => behavior.CanExecuteCommand);

    /// <summary>
    /// Identifies the <seealso cref="UseCommandCanExecuteForIsEnabled"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<bool> UseCommandCanExecuteForIsEnabledProperty =
        AvaloniaProperty.Register<InvokeCommandBehaviorBase, bool>(nameof(UseCommandCanExecuteForIsEnabled));

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
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    /// <summary>
    /// Gets a value indicating whether <see cref="Command"/> can execute with the current <see cref="CommandParameter"/>.
    /// </summary>
    public bool CanExecuteCommand
    {
        get => _canExecuteCommand;
        private set => SetAndRaise(CanExecuteCommandProperty, ref _canExecuteCommand, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the associated control's
    /// <see cref="Avalonia.Input.InputElement.IsEnabled"/> property should follow <see cref="CanExecuteCommand"/>.
    /// </summary>
    public bool UseCommandCanExecuteForIsEnabled
    {
        get => GetValue(UseCommandCanExecuteForIsEnabledProperty);
        set => SetValue(UseCommandCanExecuteForIsEnabledProperty, value);
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

    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();

        _commandCanExecuteObserver.Start(Command, CommandParameter);
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

        if (change.Property == CommandProperty || change.Property == CommandParameterProperty)
        {
            _commandCanExecuteObserver.Update(Command, CommandParameter);
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

    private void SetCanExecuteCommand(bool value)
    {
        CanExecuteCommand = value;
    }

    private void UpdateCommandCanExecuteIsEnabledBinding()
    {
        _commandCanExecuteIsEnabledBinder.Update(
            AssociatedObject,
            UseCommandCanExecuteForIsEnabled,
            AvaloniaObjectExtensions.GetObservable(this, CanExecuteCommandProperty));
    }
}
