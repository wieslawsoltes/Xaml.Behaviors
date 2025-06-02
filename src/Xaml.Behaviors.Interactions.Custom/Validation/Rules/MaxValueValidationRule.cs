using System.Collections.Generic;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Validation rule that checks that a numeric value is less than or equal to a maximum value.
/// </summary>
/// <typeparam name="T">Type of value to validate.</typeparam>
public class MaxValueValidationRule<T> : IValidationRule<T>
{
    /// <summary>
    /// Gets or sets the maximum value.
    /// </summary>
    public T? MaxValue { get; set; }

    /// <inheritdoc />
    public string? ErrorMessage { get; set; } = "Value is above maximum.";

    /// <inheritdoc />
    public bool Validate(T? value)
    {
        if (value is null || MaxValue is null)
        {
            return false;
        }
        
        if (Comparer<T>.Default.Compare(value, MaxValue) > 0)
        {
            return false;
        }

        return true;
    }
}
