// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
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
/// Validation rule that requires a non-null date value.
/// </summary>
public partial class RequiredDateValidationRule : AvaloniaObject, IValidationRule<DateTimeOffset?>
{

    /// <inheritdoc />
    [StyledProperty(DefaultValue = "Date is required.")]
    public partial string? ErrorMessage { get; set; }

    /// <inheritdoc />
    public bool Validate(DateTimeOffset? value)
    {
        return value is { };
    }
}
