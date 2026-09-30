// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
#else
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

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
