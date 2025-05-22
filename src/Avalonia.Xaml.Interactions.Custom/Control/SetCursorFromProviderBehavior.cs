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
            AssociatedObject.Cursor = Provider.CreateCursor();
        }
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        AssociatedObject?.ClearValue(InputElement.CursorProperty);
    }
}
