using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// A behavior that performs actions when the bound data produces new value.
/// </summary>
public class ValueChangedTriggerBehavior : StyledElementTrigger
{
    /// <summary>
    /// Identifies the <seealso cref="Binding"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<object?> BindingProperty =
        AvaloniaProperty.Register<ValueChangedTriggerBehavior, object?>(nameof(Binding));

    /// <summary>
    /// Gets or sets the bound object that the <see cref="ValueChangedTriggerBehavior"/> will listen to. This is an avalonia property.
    /// </summary>
    public object? Binding
    {
        get => GetValue(BindingProperty);
        set => SetValue(BindingProperty, value);
    }

    
    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
                
        if (change.Property == BindingProperty)
        {
            OnValueChanged(change);
        }
    }

    private void OnValueChanged(AvaloniaPropertyChangedEventArgs args)
    {
        if (args.Sender is not ValueChangedTriggerBehavior behavior)
        {
            return;
        }

        Dispatcher.UIThread.Post(() =>
        {
            behavior.Execute(args);
        });
    }

    private void Execute(object? parameter)
    {
        if (AssociatedObject is null)
        {
            return;
        }

        if (!IsEnabled)
        {
            return;
        }

        var binding = Binding;
        if (binding is not null)
        {
            Interaction.ExecuteActions(AssociatedObject, Actions, parameter);
        }
    }
}
