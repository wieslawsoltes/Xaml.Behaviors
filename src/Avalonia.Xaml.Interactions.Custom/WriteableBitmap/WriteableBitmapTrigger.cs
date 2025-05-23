using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// A trigger that executes its actions when <see cref="Trigger"/> is called, passing a <see cref="WriteableBitmap"/> as parameter.
/// </summary>
public class WriteableBitmapTrigger : StyledElementTrigger
{
    /// <summary>
    /// Identifies the <see cref="Bitmap"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<WriteableBitmap?> BitmapProperty =
        AvaloniaProperty.Register<WriteableBitmapTrigger, WriteableBitmap?>(nameof(Bitmap));

    /// <summary>
    /// Gets or sets the bitmap passed to actions. This is an avalonia property.
    /// </summary>
    [ResolveByName]
    public WriteableBitmap? Bitmap
    {
        get => GetValue(BitmapProperty);
        set => SetValue(BitmapProperty, value);
    }

    /// <summary>
    /// Manually invokes the trigger.
    /// </summary>
    public void Trigger()
    {
        if (!IsEnabled)
        {
            return;
        }

        Interaction.ExecuteActions(AssociatedObject, Actions, Bitmap);
    }
}
