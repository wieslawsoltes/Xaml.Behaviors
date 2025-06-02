// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Validation rule that requires a non-empty string value.
/// </summary>
public class RequiredTextValidationRule : IValidationRule<string>
{
    /// <inheritdoc />
    public string? ErrorMessage { get; set; } = "Value is required.";

    /// <inheritdoc />
    public bool Validate(string? value)
    {
        return !string.IsNullOrWhiteSpace(value);
    }
}
