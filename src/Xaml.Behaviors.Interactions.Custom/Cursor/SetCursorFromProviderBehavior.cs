// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Sets the cursor provided by an <see cref="ICursorProvider"/> when attached.
/// </summary>
public class SetCursorFromProviderBehavior : StyledElementBehavior<InputElement>
{
    /// <summary>
    /// Identifies the <see cref="Provider"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<ICursorProvider?> ProviderProperty =
        AvaloniaProperty.Register<SetCursorFromProviderBehavior, ICursorProvider?>(nameof(Provider));

    /// <summary>
    /// Gets or sets the <see cref="ICursorProvider"/> that supplies the cursor.
    /// </summary>
    public ICursorProvider? Provider
    {
        get => GetValue(ProviderProperty);
        set => SetValue(ProviderProperty, value);
    }

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
