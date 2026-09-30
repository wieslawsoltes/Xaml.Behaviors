// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Microsoft.UI.Xaml;

namespace Xaml.Interactions.Custom;

/// <content>
/// Uno Platform change notification (the generated properties route their changes to <c>OnPropertyChanged</c>).
/// </content>
public partial class MaxValueValidationRule<T> : IValidationRuleChanged
{
    private EventHandler? _changed;

    event EventHandler? IValidationRuleChanged.Changed
    {
        add => _changed += value;
        remove => _changed -= value;
    }

    private void OnPropertyChanged(DependencyPropertyChangedEventArgs change) => _changed?.Invoke(this, EventArgs.Empty);
}

/// <content>
/// Uno Platform change notification (the generated properties route their changes to <c>OnPropertyChanged</c>).
/// </content>
public partial class MinValueValidationRule<T> : IValidationRuleChanged
{
    private EventHandler? _changed;

    event EventHandler? IValidationRuleChanged.Changed
    {
        add => _changed += value;
        remove => _changed -= value;
    }

    private void OnPropertyChanged(DependencyPropertyChangedEventArgs change) => _changed?.Invoke(this, EventArgs.Empty);
}

/// <content>
/// Uno Platform change notification (the generated properties route their changes to <c>OnPropertyChanged</c>).
/// </content>
public partial class NotNullValidationRule<T> : IValidationRuleChanged
{
    private EventHandler? _changed;

    event EventHandler? IValidationRuleChanged.Changed
    {
        add => _changed += value;
        remove => _changed -= value;
    }

    private void OnPropertyChanged(DependencyPropertyChangedEventArgs change) => _changed?.Invoke(this, EventArgs.Empty);
}

/// <content>
/// Uno Platform change notification (the generated properties route their changes to <c>OnPropertyChanged</c>).
/// </content>
public partial class RangeValidationRule<T> : IValidationRuleChanged
{
    private EventHandler? _changed;

    event EventHandler? IValidationRuleChanged.Changed
    {
        add => _changed += value;
        remove => _changed -= value;
    }

    private void OnPropertyChanged(DependencyPropertyChangedEventArgs change) => _changed?.Invoke(this, EventArgs.Empty);
}

/// <content>
/// Uno Platform change notification (the generated properties route their changes to <c>OnPropertyChanged</c>).
/// </content>
public partial class MinLengthValidationRule : IValidationRuleChanged
{
    private EventHandler? _changed;

    event EventHandler? IValidationRuleChanged.Changed
    {
        add => _changed += value;
        remove => _changed -= value;
    }

    private void OnPropertyChanged(DependencyPropertyChangedEventArgs change) => _changed?.Invoke(this, EventArgs.Empty);
}

/// <content>
/// Uno Platform change notification (the generated properties route their changes to <c>OnPropertyChanged</c>).
/// </content>
public partial class RegexValidationRule : IValidationRuleChanged
{
    private EventHandler? _changed;

    event EventHandler? IValidationRuleChanged.Changed
    {
        add => _changed += value;
        remove => _changed -= value;
    }

    private void OnPropertyChanged(DependencyPropertyChangedEventArgs change) => _changed?.Invoke(this, EventArgs.Empty);
}

/// <content>
/// Uno Platform change notification (the generated properties route their changes to <c>OnPropertyChanged</c>).
/// </content>
public partial class RequiredDateValidationRule : IValidationRuleChanged
{
    private EventHandler? _changed;

    event EventHandler? IValidationRuleChanged.Changed
    {
        add => _changed += value;
        remove => _changed -= value;
    }

    private void OnPropertyChanged(DependencyPropertyChangedEventArgs change) => _changed?.Invoke(this, EventArgs.Empty);
}

/// <content>
/// Uno Platform change notification (the generated properties route their changes to <c>OnPropertyChanged</c>).
/// </content>
public partial class RequiredDecimalValidationRule : IValidationRuleChanged
{
    private EventHandler? _changed;

    event EventHandler? IValidationRuleChanged.Changed
    {
        add => _changed += value;
        remove => _changed -= value;
    }

    private void OnPropertyChanged(DependencyPropertyChangedEventArgs change) => _changed?.Invoke(this, EventArgs.Empty);
}

/// <content>
/// Uno Platform change notification (the generated properties route their changes to <c>OnPropertyChanged</c>).
/// </content>
public partial class RequiredTextValidationRule : IValidationRuleChanged
{
    private EventHandler? _changed;

    event EventHandler? IValidationRuleChanged.Changed
    {
        add => _changed += value;
        remove => _changed -= value;
    }

    private void OnPropertyChanged(DependencyPropertyChangedEventArgs change) => _changed?.Invoke(this, EventArgs.Empty);
}
