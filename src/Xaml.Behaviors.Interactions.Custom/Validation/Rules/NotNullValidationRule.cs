// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Diagnostics.CodeAnalysis;
using Avalonia;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Validation rule that requires a non-null value.
/// </summary>
/// <typeparam name="T">Type of value to validate.</typeparam>
[SuppressMessage("AvaloniaProperty", "AVP1002:AvaloniaProperty objects should not be owned by a generic type")]
public class NotNullValidationRule<T> : AvaloniaObject, IValidationRule<T>
{
    /// <summary>
    /// Identifies the <see cref="ErrorMessage"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> ErrorMessageProperty =
        AvaloniaProperty.Register<NotNullValidationRule<T>, string?>(
            nameof(ErrorMessage),
            defaultValue: "Value is required.");

    /// <inheritdoc />
    public string? ErrorMessage
    {
        get => GetValue(ErrorMessageProperty);
        set => SetValue(ErrorMessageProperty, value);
    }

    /// <inheritdoc />
    public bool Validate(T? value)
    {
        return value is not null;
    }
}
