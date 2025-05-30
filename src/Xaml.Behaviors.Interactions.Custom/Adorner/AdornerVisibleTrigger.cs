using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Trigger invoked when the target <see cref="AdornerVisibilityBehavior"/> becomes visible.
/// </summary>
public class AdornerVisibleTrigger : StyledElementTrigger<Control>
{
    /// <summary>
    /// Identifies the <see cref="TargetBehavior"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<AdornerVisibilityBehavior?> TargetBehaviorProperty =
        AvaloniaProperty.Register<AdornerVisibleTrigger, AdornerVisibilityBehavior?>(nameof(TargetBehavior));

    /// <summary>
    /// Gets or sets the target adorner behavior. This is an avalonia property.
    /// </summary>
    [ResolveByName]
    public AdornerVisibilityBehavior? TargetBehavior
    {
        get => GetValue(TargetBehaviorProperty);
        set => SetValue(TargetBehaviorProperty, value);
    }

    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();
        var behavior = TargetBehavior ?? FindBehavior(AssociatedObject as StyledElement);
        if (behavior is not null)
        {
            behavior.PropertyChanged += OnBehaviorPropertyChanged;
        }
    }

    /// <inheritdoc />
    protected override void OnDetaching()
    {
        var behavior = TargetBehavior ?? FindBehavior(AssociatedObject as StyledElement);
        if (behavior is not null)
        {
            behavior.PropertyChanged -= OnBehaviorPropertyChanged;
        }
        base.OnDetaching();
    }

    private void OnBehaviorPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == AdornerVisibilityBehavior.IsVisibleProperty && e.NewValue is bool value && value)
        {
            Execute(e);
        }
    }

    private void Execute(object? parameter)
    {
        if (!IsEnabled)
        {
            return;
        }

        Interaction.ExecuteActions(AssociatedObject, Actions, parameter);
    }

    private static AdornerVisibilityBehavior? FindBehavior(StyledElement? element)
    {
        if (element is null)
        {
            return null;
        }

        var behaviors = Interaction.GetBehaviors(element);
        foreach (var behavior in behaviors)
        {
            if (behavior is AdornerVisibilityBehavior avb)
            {
                return avb;
            }
        }

        return null;
    }
}
