// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Validation rule that requires a string with a minimal length.
/// </summary>
public class MinLengthValidationRule : AvaloniaObject, IValidationRule<string>
{
    /// <summary>
    /// Identifies the <see cref="Length"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<int> LengthProperty =
        AvaloniaProperty.Register<MinLengthValidationRule, int>(nameof(Length));

    /// <summary>
    /// Identifies the <see cref="ErrorMessage"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> ErrorMessageProperty =
        AvaloniaProperty.Register<MinLengthValidationRule, string?>(
            nameof(ErrorMessage),
            defaultValue: "Value is too short.");

    /// <summary>
    /// Gets or sets the minimal allowed length.
    /// </summary>
    public int Length
    {
        get => GetValue(LengthProperty);
        set => SetValue(LengthProperty, value);
    }

    /// <inheritdoc />
    public string? ErrorMessage
    {
        get => GetValue(ErrorMessageProperty);
        set => SetValue(ErrorMessageProperty, value);
    }

    /// <inheritdoc />
    public bool Validate(string? value)
    {
        return value is not null && value.Length >= Length;
    }
}
