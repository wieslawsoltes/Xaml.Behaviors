using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Metadata;
using Avalonia.Reactive;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Base behavior that validates a property value using a set of rules.
/// </summary>
/// <typeparam name="TControl">Associated control type.</typeparam>
/// <typeparam name="TValue">Property type.</typeparam>
public class PropertyValidationBehavior<TControl, TValue> : DisposingBehavior<TControl>
    where TControl : AvaloniaObject
{
    private AvaloniaList<IValidationRule<TValue>>? _rules;

    /// <summary>
    /// Identifies the <seealso cref="Property"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<AvaloniaProperty?> PropertyProperty =
        AvaloniaProperty.Register<PropertyValidationBehavior<TControl, TValue>, AvaloniaProperty?>(nameof(Property));

    /// <summary>
    /// Identifies the <seealso cref="IsValid"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<bool> IsValidProperty =
        AvaloniaProperty.Register<PropertyValidationBehavior<TControl, TValue>, bool>(nameof(IsValid),
            defaultValue: true, defaultBindingMode: BindingMode.TwoWay);

    /// <summary>
    /// Identifies the <seealso cref="Error"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> ErrorProperty =
        AvaloniaProperty.Register<PropertyValidationBehavior<TControl, TValue>, string?>(nameof(Error));

    /// <summary>
    /// Identifies the <seealso cref="Rules"/> avalonia property.
    /// </summary>
    public static readonly DirectProperty<PropertyValidationBehavior<TControl, TValue>, AvaloniaList<IValidationRule<TValue>>> RulesProperty =
        AvaloniaProperty.RegisterDirect<PropertyValidationBehavior<TControl, TValue>, AvaloniaList<IValidationRule<TValue>>>(nameof(Rules), b => b.Rules);

    /// <summary>
    /// Gets or sets the property to validate. This is an avalonia property.
    /// </summary>
    public AvaloniaProperty? Property
    {
        get => GetValue(PropertyProperty);
        set => SetValue(PropertyProperty, value);
    }

    /// <summary>
    /// Gets validation rules collection. This is an avalonia property.
    /// </summary>
    [Content]
    public AvaloniaList<IValidationRule<TValue>> Rules => _rules ??= [];

    /// <summary>
    /// Gets or sets value indicating whether the property value is valid. This is an avalonia property.
    /// </summary>
    public bool IsValid
    {
        get => GetValue(IsValidProperty);
        set => SetValue(IsValidProperty, value);
    }

    /// <summary>
    /// Gets or sets the validation error message. This is an avalonia property.
    /// </summary>
    public string? Error
    {
        get => GetValue(ErrorProperty);
        set => SetValue(ErrorProperty, value);
    }

    /// <inheritdoc />
    protected override IDisposable OnAttachedOverride()
    {
        if (AssociatedObject is null)
        {
            return DisposableAction.Empty;
        }

        if (Property is not AvaloniaProperty<TValue> property)
        {
            return DisposableAction.Empty;
        }

        void Handler(object? sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property == property)
            {
                Validate(e.GetNewValue<TValue>());
            }
        }

        AssociatedObject.PropertyChanged += Handler;

        // Validate initial value to ensure errors are shown when the control is first displayed
        Validate(AssociatedObject.GetValue(property));

        return DisposableAction.Create(() => AssociatedObject.PropertyChanged -= Handler);
    }

    private void Validate(TValue value)
    {
        var errors = new List<string>();
        var valid = true;

        foreach (var rule in Rules)
        {
            if (!rule.Validate(value))
            {
                valid = false;
                if (!string.IsNullOrEmpty(rule.ErrorMessage))
                {
                    errors.Add(rule.ErrorMessage!);
                }
            }
        }

        IsValid = valid;
        Error = errors.Count > 0 ? string.Join(Environment.NewLine, errors) : null;

        if (AssociatedObject is Control control)
        {
            DataValidationErrors.SetErrors(control, errors.Count > 0 ? errors : null);
        }
    }
}
