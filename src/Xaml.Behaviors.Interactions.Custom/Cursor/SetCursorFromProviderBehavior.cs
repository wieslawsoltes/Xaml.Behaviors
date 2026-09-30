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
/// Sets the cursor provided by an <see cref="ICursorProvider"/> when attached.
/// </summary>
public partial class SetCursorFromProviderBehavior : StyledElementBehavior<InputElement>
{

    /// <summary>
    /// Gets or sets the <see cref="ICursorProvider"/> that supplies the cursor.
    /// </summary>
    [StyledProperty]
    public partial ICursorProvider? Provider { get; set; }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        if (AssociatedObject is not null && Provider is not null)
        {
            var cursor = Provider.CreateCursor();
            AssociatedObject.SetCurrentValue(InputElement.CursorProperty, cursor);

        }
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        AssociatedObject?.ClearValue(InputElement.CursorProperty);
    }
}
