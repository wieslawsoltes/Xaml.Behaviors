// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Dispatching;
using Xaml.Interactivity;
#else
using Avalonia.Metadata;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// A behavior that executes different collections of actions depending on the specified condition.
/// </summary>
public partial class IfElseTrigger : StyledElementTrigger
{

    /// <summary>
    /// Gets or sets the condition that determines which actions are executed. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial bool Condition { get; set; }

    /// <summary>
    /// Gets the actions executed when <see cref="Condition"/> evaluates to <c>true</c>. This is an avalonia property.
    /// </summary>
    [DirectProperty(Lazy = true, Content = true)]
    public partial ActionCollection IfActions { get; }

    /// <summary>
    /// Gets the actions executed when <see cref="Condition"/> evaluates to <c>false</c>. This is an avalonia property.
    /// </summary>
    [DirectProperty(Lazy = true)]
    public partial ActionCollection ElseActions { get; }

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
