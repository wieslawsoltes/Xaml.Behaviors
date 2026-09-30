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
/// report their changes through <see cref="IValidationRuleChanged"/>.
/// </remarks>
public partial class PropertyValidationBehavior<TControl, TValue>
{
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

/// <summary>
/// Raised by the validation rules when one of their properties changes (Uno Platform counterpart of the Avalonia
/// <c>AvaloniaObject.PropertyChanged</c> event used to revalidate).
/// </summary>
internal interface IValidationRuleChanged
{
    /// <summary>
    /// Occurs when a property of the rule changes.
    /// </summary>
    event EventHandler? Changed;
}
