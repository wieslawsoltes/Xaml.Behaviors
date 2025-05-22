using Avalonia;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Action that invokes <see cref="IRenderTargetBitmapRenderHost.Render"/> on the specified target.
/// </summary>
public class RenderRenderTargetBitmapAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <see cref="Target"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<IRenderTargetBitmapRenderHost?> TargetProperty =
        AvaloniaProperty.Register<RenderRenderTargetBitmapAction, IRenderTargetBitmapRenderHost?>(nameof(Target));

    /// <summary>
    /// Gets or sets the render host. This is an avalonia property.
    /// </summary>
    [ResolveByName]
    public IRenderTargetBitmapRenderHost? Target
    {
        get => GetValue(TargetProperty);
        set => SetValue(TargetProperty, value);
    }

    /// <inheritdoc />
    public override object Execute(object? sender, object? parameter)
    {
        if (IsEnabled != true)
        {
            return false;
        }

        Target?.Render();
        return true;
    }
}
