// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Diagnostics.CodeAnalysis;
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
/// Validation rule that requires a non-null value.
/// </summary>
/// <typeparam name="T">Type of value to validate.</typeparam>
[SuppressMessage("AvaloniaProperty", "AVP1002:AvaloniaProperty objects should not be owned by a generic type")]
#if UNO
// Uno's dependency object generator does not support generic types deriving directly from DependencyObject.
public partial class NotNullValidationRule<T> : ValidationRuleBase, IValidationRule<T>
#else
public partial class NotNullValidationRule<T> : AvaloniaObject, IValidationRule<T>
#endif
{

    /// <inheritdoc />
    [StyledProperty(DefaultValue = "Value is required.")]
    public partial string? ErrorMessage { get; set; }

    /// <inheritdoc />
    public bool Validate(T? value)
    {
        return value is not null;
    }
}
