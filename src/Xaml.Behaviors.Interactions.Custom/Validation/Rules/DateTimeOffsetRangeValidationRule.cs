using System;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Validation rule that checks whether a value is within a specified range.
/// </summary>
/// <typeparam name="T">Type of value to validate.</typeparam>
public class DateTimeOffsetRangeValidationRule : IValidationRule<DateTimeOffset?>
{
    /// <summary>
    /// Gets or sets the minimum allowed value.
    /// </summary>
    public DateTimeOffset Minimum { get; set; }

    /// <summary>
    /// Gets or sets the maximum allowed value.
    /// </summary>
    public DateTimeOffset Maximum { get; set; }

    /// <inheritdoc />
    public string? ErrorMessage { get; set; } = "Value is out of range.";

    /// <inheritdoc />
    public bool Validate(DateTimeOffset? value)
    {
        if (!(value is { }))
        {
            return false;
        }
        
        if (value < Minimum)
        {
            return false;
        }

        if (value > Maximum)
        {
            return false;
        }

        return true;
    }
}
