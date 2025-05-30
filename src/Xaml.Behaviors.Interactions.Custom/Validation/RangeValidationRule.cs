using System;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Validation rule that checks whether a value is within a specified range.
/// </summary>
/// <typeparam name="T">Type of value to validate.</typeparam>
public class RangeValidationRule<T> : IValidationRule<T>
    where T : IComparable<T>
{
    /// <summary>
    /// Gets or sets the minimum allowed value.
    /// </summary>
    public T? Minimum { get; set; }

    /// <summary>
    /// Gets or sets the maximum allowed value.
    /// </summary>
    public T? Maximum { get; set; }

    /// <inheritdoc />
    public string? ErrorMessage { get; set; } = "Value is out of range.";

    /// <inheritdoc />
    public bool Validate(T? value)
    {
        if (value is null)
        {
            return false;
        }

        if (Minimum is not null && value.CompareTo(Minimum) < 0)
        {
            return false;
        }

        if (Maximum is not null && value.CompareTo(Maximum) > 0)
        {
            return false;
        }

        return true;
    }
}
