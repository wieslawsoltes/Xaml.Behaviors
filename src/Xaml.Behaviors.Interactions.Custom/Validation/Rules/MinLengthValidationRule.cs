// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Validation rule that requires a string with a minimal length.
/// </summary>
public class MinLengthValidationRule : IValidationRule<string>
{
    /// <summary>
    /// Gets or sets the minimal allowed length.
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public int Length { get; set; }

    /// <inheritdoc />
    public string? ErrorMessage { get; set; } = "Value is too short.";

    /// <inheritdoc />
    public bool Validate(string? value)
    {
        return value is not null && value.Length >= Length;
    }
}
