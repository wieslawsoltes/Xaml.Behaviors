using System;
using System.Threading.Tasks;
using Avalonia.Xaml.Interactivity;
using ReactiveUI;

namespace Avalonia.Xaml.Interactions.ReactiveUI;

/// <summary>
/// A behavior that registers a handler for a <see cref="Interaction{TInput,TOutput}"/> and executes its actions when the interaction is triggered.
/// </summary>
public class InteractionTriggerBehavior : StyledElementTrigger<Visual>
{
    private IDisposable? _disposable;

    /// <summary>
    /// Identifies the <see cref="Interaction"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Interaction<object?, object?>?> InteractionProperty =
        AvaloniaProperty.Register<InteractionTriggerBehavior, Interaction<object?, object?>?>(nameof(Interaction));

    /// <summary>
    /// Gets or sets the interaction to register the handler for. This is an avalonia property.
    /// </summary>
    public Interaction<object?, object?>? Interaction
    {
        get => GetValue(InteractionProperty);
        set => SetValue(InteractionProperty, value);
    }

    /// <inheritdoc/>
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == InteractionProperty)
        {
            
        }
    }

    /// <inheritdoc/>
    protected override void OnAttachedToVisualTree()
    {
        base.OnAttachedToVisualTree();
        
        if (Interaction is null)
        {
            return;
        }

        _disposable = Interaction.RegisterHandler(context =>
        {
            Avalonia.Xaml.Interactivity.Interaction.ExecuteActions(AssociatedObject, Actions, context.Input);
            context.SetOutput(null);
            return Task.CompletedTask;
        });
    }

    /// <inheritdoc/>
    protected override void OnDetachedFromVisualTree()
    {
        base.OnDetachedFromVisualTree();
        
        _disposable?.Dispose();
    }
}
