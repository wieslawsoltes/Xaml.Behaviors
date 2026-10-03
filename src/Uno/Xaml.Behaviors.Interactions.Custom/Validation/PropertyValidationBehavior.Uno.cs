// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Microsoft.UI.Xaml;

namespace Xaml.Interactions.Custom;

/// <content>
/// Uno Platform change tracking of the validated property and of the rules.
/// </content>
/// <remarks>
/// Avalonia observes <c>AvaloniaObject.PropertyChanged</c> of the associated object and of the rules. WinUI
/// observes a single property with <see cref="DependencyObject.RegisterPropertyChangedCallback"/>; the rules
/// (built-in or custom) report their changes through the public <see cref="IValidationRuleChanged"/> interface.
/// Adding, removing or replacing rules in <see cref="Rules"/> also revalidates.
/// </remarks>
public partial class PropertyValidationBehavior<TControl, TValue>
{
    private IDisposable? _propertySubscription;
    private bool _isObservingProperty;

    /// <inheritdoc />
    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        // WinUI applies x:Bind values (for example Property="{x:Bind ...}") after the behavior was attached while the
        // XAML was loaded: observe the new property and validate its current value.
        if (e.Property == PropertyProperty && _isObservingProperty)
        {
            SubscribeToProperty();
            Validate();
        }
    }

    private void SubscribeToProperty()
    {
        _propertySubscription?.Dispose();
        _propertySubscription = AssociatedObject is { } associatedObject && Property is { } property
            ? SubscribeToChanges(associatedObject, property)
            : null;
        _isObservingProperty = true;
    }

    private void UnsubscribeFromProperty()
    {
        _propertySubscription?.Dispose();
        _propertySubscription = null;
        _isObservingProperty = false;
    }

    private IDisposable SubscribeToChanges(TControl associatedObject, DependencyProperty property)
    {
        var rules = Rules;
        var subscribedRules = new HashSet<IValidationRuleChanged>();

        void RuleChanged(object? sender, EventArgs e) => Validate();

        void AttachRule(IValidationRule<TValue> rule)
        {
            if (rule is IValidationRuleChanged notifier && subscribedRules.Add(notifier))
            {
                notifier.Changed += RuleChanged;
            }
        }

        void DetachRules()
        {
            foreach (var subscribedRule in subscribedRules)
            {
                subscribedRule.Changed -= RuleChanged;
            }

            subscribedRules.Clear();
        }

        void RulesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            DetachRules();

            foreach (var rule in rules)
            {
                AttachRule(rule);
            }

            Validate();
        }

        void PropertyChangedCallback(DependencyObject sender, DependencyProperty dp)
            => Validate(sender.GetValue(dp) is TValue value ? value : default!);

        var token = associatedObject.RegisterPropertyChangedCallback(property, PropertyChangedCallback);
        rules.CollectionChanged += RulesCollectionChanged;

        foreach (var rule in rules)
        {
            AttachRule(rule);
        }

        return DisposableAction.Create(() =>
        {
            associatedObject.UnregisterPropertyChangedCallback(property, token);
            rules.CollectionChanged -= RulesCollectionChanged;
            DetachRules();
        });
    }
}
