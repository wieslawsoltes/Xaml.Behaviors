namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Validation rule that checks that a numeric value is greater than or equal to a minimum value.
/// </summary>
public class MinValueValidationRule : IValidationRule<decimal?>
{
    /// <summary>
    /// Gets or sets the minimum value.
    /// </summary>
    public decimal MinValue { get; set; }

    /// <inheritdoc />
    public string? ErrorMessage { get; set; } = "Value is below minimum.";

    /// <inheritdoc />
    public bool Validate(decimal? value)
    {
        return value is { } v && v >= MinValue;
    }
}
