// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Avalonia.Controls;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Validation behavior for <see cref="DatePicker"/> selected date.
/// </summary>
public class DatePickerValidationBehavior : PropertyValidationBehavior<DatePicker, DateTimeOffset?>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DatePickerValidationBehavior"/> class.
    /// </summary>
    public DatePickerValidationBehavior()
    {
        Property = DatePicker.SelectedDateProperty;
    }
}
