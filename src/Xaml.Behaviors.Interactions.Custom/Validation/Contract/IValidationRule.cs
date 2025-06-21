// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Defines a validation rule.
/// </summary>
/// <typeparam name="T">Type of value to validate.</typeparam>
public interface IValidationRule<in T>
{
    /// <summary>
    /// Gets or sets the error message for failed validation.
    /// </summary>
    string? ErrorMessage { get; set; }

    /// <summary>
    /// Validates the specified value.
    /// </summary>
    /// <param name="value">Value to validate.</param>
    /// <returns><c>true</c> if value is valid.</returns>
    bool Validate(T? value);
}
