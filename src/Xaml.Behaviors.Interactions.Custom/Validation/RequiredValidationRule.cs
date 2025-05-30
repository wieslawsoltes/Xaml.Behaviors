namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Validation rule that requires a non-empty string value.
/// </summary>
public class RequiredValidationRule : IValidationRule<string?>
{
    /// <inheritdoc />
    public string? ErrorMessage { get; set; } = "Value is required.";

    /// <inheritdoc />
    public bool Validate(string? value)
    {
        return !string.IsNullOrWhiteSpace(value);
    }
}
