// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Xaml.Interactivity;
#else
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Sets the cursor for the associated control when attached.
/// </summary>
public partial class SetCursorBehavior : StyledElementBehavior<InputElement>
{

    /// <summary>
    /// Gets or sets the cursor to apply.
    /// </summary>
    [StyledProperty]
    public partial Cursor? Cursor { get; set; }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        if (AssociatedObject is not null && Cursor is not null)
        {
            AssociatedObject.SetCurrentValue(InputElement.CursorProperty, Cursor);
        }
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        AssociatedObject?.ClearValue(InputElement.CursorProperty);
    }
}
