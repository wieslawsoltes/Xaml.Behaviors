// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Avalonia;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Validation rule that requires a non-null date value.
/// </summary>
public class RequiredDateValidationRule : AvaloniaObject, IValidationRule<DateTimeOffset?>
{
    /// <summary>
    /// Identifies the <see cref="ErrorMessage"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> ErrorMessageProperty =
        AvaloniaProperty.Register<RequiredDateValidationRule, string?>(
            nameof(ErrorMessage),
            defaultValue: "Date is required.");

    /// <inheritdoc />
    public string? ErrorMessage
    {
        get => GetValue(ErrorMessageProperty);
        set => SetValue(ErrorMessageProperty, value);
    }

    /// <inheritdoc />
    public bool Validate(DateTimeOffset? value)
    {
        return value is { };
    }
}
