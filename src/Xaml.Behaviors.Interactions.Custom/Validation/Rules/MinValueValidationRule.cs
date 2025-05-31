using System.Collections.Generic;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Validation rule that checks that a numeric value is greater than or equal to a minimum value.
/// </summary>
/// <typeparam name="T">Type of value to validate.</typeparam>
public class MinValueValidationRule<T> : IValidationRule<T>
{
    /// <summary>
    /// Gets or sets the minimum value.
    /// </summary>
    public T? MinValue { get; set; }

    /// <inheritdoc />
    public string? ErrorMessage { get; set; } = "Value is below minimum.";

    /// <inheritdoc />
    public bool Validate(T? value)
    {
        if (value is null || MinValue is null)
        {
            return false;
        }
        
        if (Comparer<T>.Default.Compare(value, MinValue) < 0)
        {
            return false;
        }

        return true;
    }
}
