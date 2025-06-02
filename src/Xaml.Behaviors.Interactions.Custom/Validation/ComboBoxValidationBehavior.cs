// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Validation behavior for <see cref="ComboBox"/> selected item.
/// </summary>
public class ComboBoxValidationBehavior : PropertyValidationBehavior<ComboBox, object?>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ComboBoxValidationBehavior"/> class.
    /// </summary>
    public ComboBoxValidationBehavior()
    {
        Property = SelectingItemsControl.SelectedItemProperty;
    }
}
