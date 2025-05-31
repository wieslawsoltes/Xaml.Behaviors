namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Validation rule that requires a non-null value.
/// </summary>
/// <typeparam name="T">Type of value to validate.</typeparam>
public class NotNullValidationRule<T> : IValidationRule<T>
{
    /// <inheritdoc />
    public string? ErrorMessage { get; set; } = "Value is required.";

    /// <inheritdoc />
    public bool Validate(T value)
    {
        return value is not null;
    }
}
