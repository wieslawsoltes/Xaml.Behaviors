using Avalonia.Animation;
using Avalonia.Xaml.Interactivity;
using Avalonia.Styling;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Sets the <see cref="StyledElement.Transitions"/> collection on the associated control when attached.
/// </summary>
public class TransitionsBehavior : AttachedToVisualTreeBehavior<StyledElement>
{
    /// <summary>
    /// Identifies the <seealso cref="Transitions"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Transitions?> TransitionsProperty =
        AvaloniaProperty.Register<TransitionsBehavior, Transitions?>(nameof(Transitions));

    private Transitions? _oldTransitions;

    /// <summary>
    /// Gets or sets the transitions collection to apply. This is an avalonia property.
    /// </summary>
    public Transitions? Transitions
    {
        get => GetValue(TransitionsProperty);
        set => SetValue(TransitionsProperty, value);
    }

    /// <inheritdoc />
    protected override System.IDisposable OnAttachedToVisualTreeOverride()
    {
        if (AssociatedObject is null)
        {
            return DisposableAction.Empty;
        }

        _oldTransitions = AssociatedObject.Transitions;
        AssociatedObject.Transitions = Transitions;

        return DisposableAction.Create(() =>
        {
            if (AssociatedObject is not null)
            {
                AssociatedObject.Transitions = _oldTransitions;
            }
        });
    }
}
