namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Validation rule that requires a non-null decimal value.
/// </summary>
public class RequiredDecimalValidationRule : IValidationRule<decimal?>
{
    /// <inheritdoc />
    public string? ErrorMessage { get; set; } = "Value is required.";

    /// <inheritdoc />
    public bool Validate(decimal? value)
    {
        return value is { };
    }
}
