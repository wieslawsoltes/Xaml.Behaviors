// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Text.RegularExpressions;
#if UNO
using Microsoft.UI.Xaml;
#else
using Avalonia;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Validation rule that checks value against a regular expression pattern.
/// </summary>
public partial class RegexValidationRule : AvaloniaObject, IValidationRule<string>
{

    /// <summary>
    /// Gets or sets the regex pattern.
    /// </summary>
    [StyledProperty(DefaultValue = "")]
    public partial string Pattern { get; set; }

    /// <inheritdoc />
    [StyledProperty(DefaultValue = "Invalid format.")]
    public partial string? ErrorMessage { get; set; }

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
