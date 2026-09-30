// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
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
/// Validation rule that requires a string with a minimal length.
/// </summary>
public partial class MinLengthValidationRule : AvaloniaObject, IValidationRule<string>
{

    /// <summary>
    /// Gets or sets the minimal allowed length.
    /// </summary>
    [StyledProperty]
    public partial int Length { get; set; }

    /// <inheritdoc />
    [StyledProperty(DefaultValue = "Value is too short.")]
    public partial string? ErrorMessage { get; set; }

    /// <inheritdoc />
    public bool Validate(string? value)
    {
        return value is not null && value.Length >= Length;
    }
}
