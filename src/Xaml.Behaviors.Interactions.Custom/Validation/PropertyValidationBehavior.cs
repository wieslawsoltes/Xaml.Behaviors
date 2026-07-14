// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Metadata;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Base behavior that validates a property value using a set of rules.
/// </summary>
/// <typeparam name="TControl">Associated control type.</typeparam>
/// <typeparam name="TValue">Property type.</typeparam>
[SuppressMessage("AvaloniaProperty", "AVP1002:AvaloniaProperty objects should not be owned by a generic type")]
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

        var associatedObject = AssociatedObject;
        var rules = Rules;
        var subscribedRules = new HashSet<AvaloniaObject>();

        void RulePropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
        {
            Validate();
        }

        void AttachRule(IValidationRule<TValue> rule)
        {
            if (rule is AvaloniaObject avaloniaObject && subscribedRules.Add(avaloniaObject))
            {
                avaloniaObject.PropertyChanged += RulePropertyChanged;
            }
        }

        void RulesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (var subscribedRule in subscribedRules)
            {
                subscribedRule.PropertyChanged -= RulePropertyChanged;
            }

            subscribedRules.Clear();

            foreach (var rule in rules)
            {
                AttachRule(rule);
            }

            Validate();
        }

        void Handler(object? sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property == property)
            {
                Validate(e.GetNewValue<TValue>());
            }
        }

        associatedObject.PropertyChanged += Handler;
        rules.CollectionChanged += RulesCollectionChanged;

        foreach (var rule in rules)
        {
            AttachRule(rule);
        }

        return DisposableAction.Create(() =>
        {
            associatedObject.PropertyChanged -= Handler;
            rules.CollectionChanged -= RulesCollectionChanged;

            foreach (var subscribedRule in subscribedRules)
            {
                subscribedRule.PropertyChanged -= RulePropertyChanged;
            }

            subscribedRules.Clear();
        });
    }

    /// <inheritdoc />
    protected override void OnLoaded()
    {
        base.OnLoaded();

        Validate();
    }

    private void Validate()
    {
        if (AssociatedObject is not null && Property is AvaloniaProperty<TValue> property)
        {
            Validate(AssociatedObject.GetValue<TValue>(property));
        }
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
