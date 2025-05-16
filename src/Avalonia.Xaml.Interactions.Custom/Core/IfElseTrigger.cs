using Avalonia.Metadata;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// A behavior that executes different collections of actions depending on the specified condition.
/// </summary>
public class IfElseTrigger : StyledElementTrigger
{
    /// <summary>
    /// Identifies the <seealso cref="Condition"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<bool> ConditionProperty =
        AvaloniaProperty.Register<IfElseTrigger, bool>(nameof(Condition));

    /// <summary>
    /// Identifies the <seealso cref="IfActions"/> avalonia property.
    /// </summary>
    public static readonly DirectProperty<IfElseTrigger, ActionCollection> IfActionsProperty =
        AvaloniaProperty.RegisterDirect<IfElseTrigger, ActionCollection>(nameof(IfActions), b => b.IfActions);

    /// <summary>
    /// Identifies the <seealso cref="ElseActions"/> avalonia property.
    /// </summary>
    public static readonly DirectProperty<IfElseTrigger, ActionCollection> ElseActionsProperty =
        AvaloniaProperty.RegisterDirect<IfElseTrigger, ActionCollection>(nameof(ElseActions), b => b.ElseActions);

    private ActionCollection? _ifActions;
    private ActionCollection? _elseActions;

    /// <summary>
    /// Gets or sets the condition that determines which actions are executed. This is an avalonia property.
    /// </summary>
    public bool Condition
    {
        get => GetValue(ConditionProperty);
        set => SetValue(ConditionProperty, value);
    }

    /// <summary>
    /// Gets the actions executed when <see cref="Condition"/> evaluates to <c>true</c>. This is an avalonia property.
    /// </summary>
    [Content]
    public ActionCollection IfActions => _ifActions ??= [];

    /// <summary>
    /// Gets the actions executed when <see cref="Condition"/> evaluates to <c>false</c>. This is an avalonia property.
    /// </summary>
    public ActionCollection ElseActions => _elseActions ??= [];

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ConditionProperty)
        {
            OnConditionChanged(change);
        }
    }

    /// <inheritdoc />
    protected override void OnInitializedEvent()
    {
        base.OnInitializedEvent();

        Execute(parameter: null);
    }

    private void OnConditionChanged(AvaloniaPropertyChangedEventArgs args)
    {
        if (args.Sender is not IfElseTrigger behavior)
        {
            return;
        }

        Dispatcher.UIThread.Post(() =>
        {
            behavior.Execute(parameter: args);
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

        var actions = Condition ? IfActions : ElseActions;
        Interaction.ExecuteActions(AssociatedObject, actions, parameter);
    }
}
