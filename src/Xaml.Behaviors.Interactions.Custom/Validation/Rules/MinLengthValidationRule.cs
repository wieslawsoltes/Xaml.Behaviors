namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Validation rule that requires a string with a minimal length.
/// </summary>
public class MinLengthValidationRule : IValidationRule<string>
{
    /// <summary>
    /// Gets or sets the minimal allowed length.
    /// </summary>
    public int Length { get; set; }

    /// <inheritdoc />
    public string? ErrorMessage { get; set; } = "Value is too short.";

    /// <inheritdoc />
    public bool Validate(string? value)
    {
        return value is not null && value.Length >= Length;
    }
}
