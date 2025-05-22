using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Core;

/// <summary>
/// Toggles between two visual states based on a conditional statement.
/// </summary>
[RequiresUnreferencedCode("This functionality is not compatible with trimming.")]
public class DataStateBehavior : StyledElementBehavior<Control>
{
    /// <summary>
    /// Identifies the <seealso cref="Binding"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<object?> BindingProperty =
        AvaloniaProperty.Register<DataStateBehavior, object?>(nameof(Binding));

    /// <summary>
    /// Identifies the <seealso cref="Value"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<object?> ValueProperty =
        AvaloniaProperty.Register<DataStateBehavior, object?>(nameof(Value));

    /// <summary>
    /// Identifies the <seealso cref="TrueState"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> TrueStateProperty =
        AvaloniaProperty.Register<DataStateBehavior, string?>(nameof(TrueState));

    /// <summary>
    /// Identifies the <seealso cref="FalseState"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> FalseStateProperty =
        AvaloniaProperty.Register<DataStateBehavior, string?>(nameof(FalseState));

    /// <summary>
    /// Gets or sets the bound object that the <see cref="DataStateBehavior"/> listens to. This is an avalonia property.
    /// </summary>
    public object? Binding
    {
        get => GetValue(BindingProperty);
        set => SetValue(BindingProperty, value);
    }

    /// <summary>
    /// Gets or sets the value to be compared with the <see cref="Binding"/>. This is an avalonia property.
    /// </summary>
    public object? Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>
    /// Gets or sets the name of the visual state to transition to when the condition is met. This is an avalonia property.
    /// </summary>
    public string? TrueState
    {
        get => GetValue(TrueStateProperty);
        set => SetValue(TrueStateProperty, value);
    }

    /// <summary>
    /// Gets or sets the name of the visual state to transition to when the condition is not met. This is an avalonia property.
    /// </summary>
    public string? FalseState
    {
        get => GetValue(FalseStateProperty);
        set => SetValue(FalseStateProperty, value);
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == BindingProperty ||
            change.Property == ValueProperty ||
            change.Property == TrueStateProperty ||
            change.Property == FalseStateProperty)
        {
            OnValueChanged(change);
        }
    }

    /// <inheritdoc />
    protected override void OnInitializedEvent()
    {
        base.OnInitializedEvent();

        Evaluate();
    }

    private void OnValueChanged(AvaloniaPropertyChangedEventArgs args)
    {
        if (args.Sender is not DataStateBehavior behavior)
        {
            return;
        }

        Dispatcher.UIThread.Post(behavior.Evaluate);
    }

    private void Evaluate()
    {
        if (AssociatedObject is not Control control)
        {
            return;
        }

        var isTrue = ComparisonConditionTypeHelper.Compare(Binding, ComparisonConditionType.Equal, Value);
        var stateName = isTrue ? TrueState : FalseState;

        if (!string.IsNullOrEmpty(stateName))
        {
            VisualStateManager.GoToState(control, stateName);
        }
    }
}
