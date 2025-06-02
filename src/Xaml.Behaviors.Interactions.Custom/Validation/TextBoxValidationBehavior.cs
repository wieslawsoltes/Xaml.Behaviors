// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Controls;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Validation behavior for <see cref="TextBox"/> text.
/// </summary>
public class TextBoxValidationBehavior : PropertyValidationBehavior<TextBox, string?>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TextBoxValidationBehavior"/> class.
    /// </summary>
    public TextBoxValidationBehavior()
    {
        Property = TextBox.TextProperty;
    }
}
