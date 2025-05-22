using Avalonia;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Trigger that calls <see cref="IRenderTargetBitmapRenderHost.Render"/> when the specified event occurs.
/// </summary>
public class RenderTargetBitmapRenderTrigger : EventTriggerBehavior
{
    /// <summary>
    /// Identifies the <see cref="Target"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<IRenderTargetBitmapRenderHost?> TargetProperty =
        AvaloniaProperty.Register<RenderTargetBitmapRenderTrigger, IRenderTargetBitmapRenderHost?>(nameof(Target));

    /// <summary>
    /// Gets or sets the render host that should be rendered. This is an avalonia property.
    /// </summary>
    [ResolveByName]
    public IRenderTargetBitmapRenderHost? Target
    {
        get => GetValue(TargetProperty);
        set => SetValue(TargetProperty, value);
    }

    /// <inheritdoc />
    protected override void OnEvent(object? eventArgs)
    {
        Target?.Render();
        base.OnEvent(eventArgs);
    }
}
