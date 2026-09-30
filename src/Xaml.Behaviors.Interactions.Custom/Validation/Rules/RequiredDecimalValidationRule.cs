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
/// Validation rule that requires a non-null decimal value.
/// </summary>
public partial class RequiredDecimalValidationRule : AvaloniaObject, IValidationRule<decimal?>
{

    /// <inheritdoc />
    [StyledProperty(DefaultValue = "Value is required.")]
    public partial string? ErrorMessage { get; set; }

    /// <inheritdoc />
    public bool Validate(decimal? value)
    {
        return value is { };
    }
}
