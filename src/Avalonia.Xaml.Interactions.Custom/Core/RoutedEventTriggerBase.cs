using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// 
/// </summary>
public abstract class RoutedEventTriggerBase : AttachedToVisualTreeTriggerBase<Visual>
{
    /// <summary>
    /// 
    /// </summary>
    public static readonly StyledProperty<RoutingStrategies> EventRoutingStrategyProperty =
        AvaloniaProperty.Register<RoutedEventTriggerBase, RoutingStrategies>(nameof(EventRoutingStrategy), defaultValue: RoutingStrategies.Direct);

    /// <summary>
    /// 
    /// </summary>
    public RoutingStrategies EventRoutingStrategy
    {
        get => GetValue(EventRoutingStrategyProperty);
        set => SetValue(EventRoutingStrategyProperty, value);
    }

    /// <summary>
    /// 
    /// </summary>
    public bool MarkAsHandled { get; set; }
}
