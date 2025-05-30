using System.Text.RegularExpressions;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Validation rule that checks value against a regular expression pattern.
/// </summary>
public class RegexValidationRule : IValidationRule<string?>
{
    /// <summary>
    /// Gets or sets the regex pattern.
    /// </summary>
    public string Pattern { get; set; } = string.Empty;

    /// <inheritdoc />
    public string? ErrorMessage { get; set; } = "Invalid format.";

    /// <inheritdoc />
    public bool Validate(string? value)
    {
        if (value is null)
        {
            return false;
        }

        return Regex.IsMatch(value, Pattern);
    }
}
