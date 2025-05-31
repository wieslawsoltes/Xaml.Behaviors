namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Validation rule that checks that a numeric value is less than or equal to a maximum value.
/// </summary>
public class NumericUpDownMaxValueValidationRule : IValidationRule<decimal?>
{
    /// <summary>
    /// Gets or sets the maximum value.
    /// </summary>
    public decimal? MaxValue { get; set; }

    /// <inheritdoc />
    public string? ErrorMessage { get; set; } = "Value is above maximum.";

    /// <inheritdoc />
    public bool Validate(decimal? value)
    {
        return value is { } v && v <= MaxValue;
    }
}
