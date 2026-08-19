// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Validation rule that requires a non-empty string value.
/// </summary>
public class RequiredTextValidationRule : AvaloniaObject, IValidationRule<string>
{
    /// <summary>
    /// Identifies the <see cref="ErrorMessage"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> ErrorMessageProperty =
        AvaloniaProperty.Register<RequiredTextValidationRule, string?>(
            nameof(ErrorMessage),
            defaultValue: "Value is required.");

    /// <inheritdoc />
    public string? ErrorMessage
    {
        get => GetValue(ErrorMessageProperty);
        set => SetValue(ErrorMessageProperty, value);
    }

    /// <inheritdoc />
    public bool Validate(string? value)
    {
        return !string.IsNullOrWhiteSpace(value);
    }
}
