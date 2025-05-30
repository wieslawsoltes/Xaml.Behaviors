using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Executes actions when the <see cref="Image.Source"/> property is set.
/// </summary>
public class ImageLoadedTrigger : StyledElementTrigger<Image>
{
    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject?.PropertyChanged += OnImagePropertyChanged;
    }

    /// <inheritdoc />
    protected override void OnDetaching()
    {
        if (AssociatedObject is not null)
        {
            AssociatedObject.PropertyChanged -= OnImagePropertyChanged;
        }
        base.OnDetaching();
    }

    private void OnImagePropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == Image.SourceProperty && e.NewValue is IBitmap)
        {
            Execute(parameter: e);
        }
    }

    private void Execute(object? parameter)
    {
        if (!IsEnabled || AssociatedObject is null)
        {
            return;
        }

        Interaction.ExecuteActions(AssociatedObject, Actions, parameter);
    }
}
