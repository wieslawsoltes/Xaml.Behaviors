// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Avalonia.Controls;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

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
