namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Defines a validation rule.
/// </summary>
/// <typeparam name="T">Type of value to validate.</typeparam>
public interface IValidationRule<T>
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
