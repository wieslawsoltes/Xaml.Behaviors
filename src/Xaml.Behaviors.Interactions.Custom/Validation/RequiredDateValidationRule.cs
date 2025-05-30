using System;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Validation rule that requires a non-null date value.
/// </summary>
public class RequiredDateValidationRule : IValidationRule<DateTimeOffset?>
{
    /// <inheritdoc />
    public string? ErrorMessage { get; set; } = "Date is required.";

    /// <inheritdoc />
    public bool Validate(DateTimeOffset? value)
    {
        return value is not null;
    }
}
