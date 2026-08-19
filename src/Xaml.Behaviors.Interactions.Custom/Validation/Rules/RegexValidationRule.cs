// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Text.RegularExpressions;
using Avalonia;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Validation rule that checks value against a regular expression pattern.
/// </summary>
public class RegexValidationRule : AvaloniaObject, IValidationRule<string>
{
    /// <summary>
    /// Identifies the <see cref="Pattern"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string> PatternProperty =
        AvaloniaProperty.Register<RegexValidationRule, string>(
            nameof(Pattern),
            defaultValue: string.Empty);

    /// <summary>
    /// Identifies the <see cref="ErrorMessage"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> ErrorMessageProperty =
        AvaloniaProperty.Register<RegexValidationRule, string?>(
            nameof(ErrorMessage),
            defaultValue: "Invalid format.");

    /// <summary>
    /// Gets or sets the regex pattern.
    /// </summary>
    public string Pattern
    {
        get => GetValue(PatternProperty);
        set => SetValue(PatternProperty, value);
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
        if (value is null)
        {
            return false;
        }

        return Regex.IsMatch(value, Pattern);
    }
}
