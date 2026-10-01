// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;

namespace Xaml.Interactions.Custom;

/// <summary>
/// Notifies the validation behaviors that a validation rule changed, so the validated value is checked again.
/// </summary>
/// <remarks>
/// <para>
/// Uno Platform counterpart of the Avalonia <c>AvaloniaObject.PropertyChanged</c> event that the Avalonia
/// <see cref="PropertyValidationBehavior{TControl, TValue}"/> observes on its rules. WinUI has no event that reports
/// the changes of any dependency property of an object, so the behavior revalidates when a rule of its
/// <see cref="PropertyValidationBehavior{TControl, TValue}.Rules"/> implements this interface and raises
/// <see cref="Changed"/>.
/// </para>
/// <para>
/// The built-in rules implement it. A custom <see cref="IValidationRule{T}"/> implements it to revalidate when
/// one of its properties (for example a threshold) changes, typically by raising <see cref="Changed"/> from the
/// property changed callback of its dependency properties or from its property setters. Rules that do not
/// implement it are still evaluated whenever the validated property or the rules collection changes.
/// </para>
/// </remarks>
public interface IValidationRuleChanged
{
    /// <summary>
    /// Occurs when a property of the rule that affects its result or its error message changes.
    /// </summary>
    event EventHandler? Changed;
}
