// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Diagnostics.CodeAnalysis;
#if UNO
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Dispatching;
using Xaml.Interactivity;
#else
using Avalonia.Data;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// A behavior that performs actions when the bound data meets a specified condition.
/// </summary>
[RequiresUnreferencedCode("This functionality is not compatible with trimming.")]
public partial class BindingTriggerBehavior : StyledElementTrigger
{

    private IDisposable? _dispose;

#if UNO
    /// <summary>
    /// Gets or sets the value that the <see cref="BindingTriggerBehavior"/> will listen to. This is a dependency property.
    /// </summary>
    /// <remarks>
    /// WinUI applies a binding written in XAML to this property (there is no <c>[AssignBinding]</c>), so its value is
    /// the bound value that is compared, for example <c>Binding="{x:Bind Slider.Value, Mode=OneWay}"</c>. A
    /// <see cref="BindingBase"/> value (assigned in code) is evaluated like on Avalonia and its value is compared.
    /// </remarks>
    [StyledProperty]
    public partial object? Binding { get; set; }
#else
    /// <summary>
    /// Gets or sets the bound object that the <see cref="BindingTriggerBehavior"/> will listen to. This is an avalonia property.
    /// </summary>
    [StyledProperty(AssignBinding = true)]
    public partial BindingBase? Binding { get; set; }
#endif

    /// <summary>
    /// Gets or sets the type of comparison to be performed between <see cref="BindingTriggerBehavior.Binding"/> and <see cref="BindingTriggerBehavior.Value"/>. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial ComparisonConditionType ComparisonCondition { get; set; }

    /// <summary>
    /// Gets or sets the value to be compared with the value of <see cref="BindingTriggerBehavior.Binding"/>. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial object? Value { get; set; }

    [StyledProperty]
    private partial object? BindingValue { get; set; }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        
        if (change.Property == ComparisonConditionProperty ||
            change.Property == ValueProperty ||
            change.Property == BindingValueProperty)
        {
            OnValueChanged(change);
        }

        if (change.Property == BindingProperty)
        {
            _dispose?.Dispose();
            _dispose = null;

#if UNO
            if (change.NewValue is BindingBase newValue)
            {
                _dispose = this.Bind(BindingValueProperty, newValue);
            }
            else
            {
                // The value of a binding applied by WinUI (XAML) is the compared value.
                BindingValue = change.NewValue;
            }
#else
            var newValue = change.GetNewValue<BindingBase?>();
            if (newValue is not null)
            {
                _dispose = this.Bind(BindingValueProperty, newValue);
            }
#endif
        }
    }

    /// <inheritdoc />
    protected override void OnDetaching()
    {
        base.OnDetaching();

        _dispose?.Dispose();
    }

    /// <inheritdoc />
    protected override void OnInitializedEvent()
    {
        base.OnInitializedEvent();

        Execute(parameter: null);
    }

    private void OnValueChanged(AvaloniaPropertyChangedEventArgs args)
    {
        // Property changes of this behavior are always raised on this instance.
        Dispatcher.UIThread.Post(() =>
        {
            Execute(parameter: args);
        });
    }

    private void Execute(object? parameter)
    {
        if (AssociatedObject is null)
        {
            return;
        }

        if (!IsEnabled)
        {
            return;
        }

        // NOTE: In UWP version binding null check is not present but Avalonia throws exception as Bindings are null when first initialized.
        var binding = BindingValue;
        if (binding is not null)
        {
            // Some value has changed--either the binding value, reference value, or the comparison condition. Re-evaluate the equation.
            if (ComparisonConditionTypeHelper.Compare(BindingValue, ComparisonCondition,
                    Value))
            {
                Interaction.ExecuteActions(AssociatedObject, Actions, parameter);
            }
        }
    }
}
