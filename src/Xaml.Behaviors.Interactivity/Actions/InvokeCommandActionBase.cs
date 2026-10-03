// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Windows.Input;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
#else
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Reactive;
#endif

#if UNO
namespace Xaml.Interactivity;
#else
namespace Avalonia.Xaml.Interactivity;
#endif

/// <summary>
/// Command action base class.
/// </summary>
public abstract partial class InvokeCommandActionBase : StyledElementAction, IActionLogicalTreeLifecycle
{
    private readonly CommandCanExecuteObserver _commandCanExecuteObserver;
    private readonly CommandCanExecuteIsEnabledBinder _commandCanExecuteIsEnabledBinder;
    private bool _passEventArgsToCommand;

    /// <summary>
    /// Initializes a new instance of the <see cref="InvokeCommandActionBase"/> class.
    /// </summary>
    protected InvokeCommandActionBase()
    {
        _commandCanExecuteObserver = new CommandCanExecuteObserver(SetCanExecuteCommand);
        _commandCanExecuteIsEnabledBinder = new CommandCanExecuteIsEnabledBinder();
    }

    /// <summary>
    /// Gets or sets the command this action should invoke. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial ICommand? Command { get; set; }

    /// <summary>
    /// Gets a value indicating whether <see cref="Command"/> can execute with the current can-execute parameter.
    /// </summary>
    [DirectProperty(DefaultValue = true)]
    public partial bool CanExecuteCommand { get; private set; }

    /// <summary>
    /// Gets or sets the parameter that is passed to <see cref="ICommand.CanExecute(object)"/>.
    /// When this property is not set, <see cref="CommandParameter"/> is used if it is set.
    /// This property does not change the parameter passed to <see cref="ICommand.Execute(object)"/>.
    /// </summary>
    [StyledProperty]
    public partial object? CanExecuteCommandParameter { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the control associated with the hosting trigger should have its
    /// <c>IsEnabled</c> property follow <see cref="CanExecuteCommand"/>.
    /// </summary>
    [StyledProperty]
    public partial bool UseCommandCanExecuteForIsEnabled { get; set; }
  
    /// <summary>
    /// Gets or sets the parameter that is passed to <see cref="System.Windows.Input.ICommand.Execute(object)"/>.
    /// If this is not set, the parameter from the <seealso cref="IAction.Execute(object, object)"/> method will be used.
    /// This is an optional avalonia property.
    /// </summary>
    [StyledProperty]
    public partial object? CommandParameter { get; set; }
  
    /// <summary>
    /// Gets or sets the converter that is run on the parameter from the <seealso cref="IAction.Execute(object, object)"/> method.
    /// This is an optional avalonia property.
    /// </summary>
    [StyledProperty]
    public partial IValueConverter? InputConverter { get; set; }

    /// <summary>
    /// Gets or sets the parameter that is passed to the <see cref="IValueConverter.Convert"/>
    /// method of <see cref="InputConverter"/>.
    /// This is an optional avalonia property.
    /// </summary>
    [StyledProperty]
    public partial object? InputConverterParameter { get; set; }
    
    /// <summary>
    /// Gets or sets the language that is passed to the <see cref="IValueConverter.Convert"/>
    /// method of <see cref="InputConverter"/>.
    /// This is an optional avalonia property.
    /// </summary>
    [StyledProperty(DefaultValue = "")]
    public partial string? InputConverterLanguage { get; set; }

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

    void IActionLogicalTreeLifecycle.AttachedToActionLogicalTree()
    {
        _commandCanExecuteObserver.Start(Command, ResolveCanExecuteParameter(), IsCanExecuteParameterKnown());
        UpdateCommandCanExecuteIsEnabledBinding();
    }

    void IActionLogicalTreeLifecycle.DetachedFromActionLogicalTree()
    {
        _commandCanExecuteIsEnabledBinder.Stop();
        _commandCanExecuteObserver.Stop();
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
    /// Resolves the command parameter that will be passed to the
    /// associated <see cref="System.Windows.Input.ICommand"/>.
    /// </summary>
    /// <param name="parameter">The original parameter supplied by the caller.</param>
    /// <returns>The value that will be provided to the command.</returns>
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
#if UNO
                InputConverterLanguage ?? string.Empty);
#else
                InputConverterLanguage is not null
                    ? 
                    new System.Globalization.CultureInfo(InputConverterLanguage)
                    : System.Globalization.CultureInfo.CurrentCulture);
#endif
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
            ResolveCommandCanExecuteIsEnabledTarget(),
            UseCommandCanExecuteForIsEnabled,
            AvaloniaObjectExtensions.GetObservable<bool>(this, CanExecuteCommandProperty));
    }

    private InputElement? ResolveCommandCanExecuteIsEnabledTarget()
    {
#if UNO
        // WinUI has no logical tree: the owning trigger publishes its associated object as the host.
        return Host as InputElement;
#else
        foreach (var logical in this.GetSelfAndLogicalAncestors())
        {
            if (logical is IBehavior { AssociatedObject: InputElement associatedInputElement })
            {
                return associatedInputElement;
            }

            if (logical is InputElement inputElement)
            {
                return inputElement;
            }
        }

        return null;
#endif
    }
}
