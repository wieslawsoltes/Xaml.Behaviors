using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// A behavior that listens for a <see cref="RoutedEvent"/> event on its source and executes its actions when that event is fired.
/// </summary>
public class RoutedEventTriggerBehavior : StyledElementTrigger<Interactive>
{
    /// <summary>
    /// Identifies the <seealso cref="RoutedEvent"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<RoutedEvent?> RoutedEventProperty =
        AvaloniaProperty.Register<RoutedEventTriggerBehavior, RoutedEvent?>(nameof(RoutedEvent));

    /// <summary>
    /// Identifies the <seealso cref="RoutingStrategies"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<RoutingStrategies> RoutingStrategiesProperty =
        AvaloniaProperty.Register<RoutedEventTriggerBehavior, RoutingStrategies>(nameof(RoutingStrategies),
            RoutingStrategies.Direct | RoutingStrategies.Bubble);

    /// <summary>
    /// Identifies the <seealso cref="SourceInteractive"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Interactive?> SourceInteractiveProperty =
        AvaloniaProperty.Register<RoutedEventTriggerBehavior, Interactive?>(nameof(SourceInteractive));

    private bool _isInitialized;
    private bool _isAttached;

    /// <summary>
    /// Gets or sets routing event to listen for. This is an avalonia property.
    /// </summary>
    public RoutedEvent? RoutedEvent
    {
        get => GetValue(RoutedEventProperty);
        set => SetValue(RoutedEventProperty, value);
    }

    /// <summary>
    /// Gets or sets the routing event <see cref="RoutingStrategies"/>. This is an avalonia property.
    /// </summary>
    public RoutingStrategies RoutingStrategies
    {
        get => GetValue(RoutingStrategiesProperty);
        set => SetValue(RoutingStrategiesProperty, value);
    }

    /// <summary>
    /// Gets or sets the source object from which this behavior listens for events.
    /// If <seealso cref="SourceInteractive"/> is not set, the source will default to <seealso cref="IBehavior.AssociatedObject"/>. This is an avalonia property.
    /// </summary>
    [ResolveByName]
    public Interactive? SourceInteractive
    {
        get => GetValue(SourceInteractiveProperty);
        set => SetValue(SourceInteractiveProperty, value);
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
                
        if (change.Property == RoutedEventProperty)
        {
            OnValueChanged(change);
        }

        if (change.Property == RoutingStrategiesProperty)
        {
            OnValueChanged(change);
        }

        if (change.Property == SourceInteractiveProperty)
        {
            OnValueChanged(change);
        }
    }

    private void OnValueChanged(AvaloniaPropertyChangedEventArgs args)
    {
        if (args.Sender is not RoutedEventTriggerBehavior behavior || behavior.AssociatedObject is null)
        {
            return;
        }

        if (behavior._isInitialized && behavior._isAttached)
        {
            behavior.RemoveHandler();
            behavior.AddHandler();
        }
    }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        _isAttached = true;
        AddHandler();
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        _isAttached = false;
        RemoveHandler();
    }

    private void AddHandler()
    {
        var interactive = ComputeResolvedSourceInteractive();
        if (interactive is not null && RoutedEvent is not null)
        {
            interactive.AddHandler(RoutedEvent, Handler, RoutingStrategies);
            _isInitialized = true;
        }
    }

    private void RemoveHandler()
    {
        var interactive = ComputeResolvedSourceInteractive();
        if (interactive is not null && RoutedEvent is not null && _isInitialized)
        {
            interactive.RemoveHandler(RoutedEvent, Handler);
            _isInitialized = false;
        }
    }

    private Interactive? ComputeResolvedSourceInteractive()
    {
        return GetValue(SourceInteractiveProperty) is not null ? SourceInteractive : AssociatedObject;
    }

    private void Handler(object? sender, RoutedEventArgs e)
    {
        Execute(e);
    }

    private void Execute(object? parameter)
    {
        if (!IsEnabled)
        {
            return;
        }

        var interactive = ComputeResolvedSourceInteractive();
        if (interactive is not null)
        {
            Interaction.ExecuteActions(interactive, Actions, parameter);
        }
    }
}
