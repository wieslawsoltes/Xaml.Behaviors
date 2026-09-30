// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Xaml.Behaviors.Uno.Headless;
using Xaml.Behaviors.Uno.Headless.XUnit;
using Xaml.Interactivity;
using Xunit;

namespace Xaml.Interactions.Custom.Animations.UnitTests;

public class TransitionsTests
{
    private static UnoHeadlessSession Session => UnoHeadlessSession.Current;

    [UnoHeadlessFact]
    public void TransitionActions_ManageTheSenderTransitions()
    {
        Border sender = new();
        EntranceThemeTransition transition = new();

        Assert.Equal(true, new AddTransitionAction { Transition = transition }.Execute(sender, null));
        Assert.Same(transition, Assert.Single(sender.Transitions!));
        Assert.Equal(true, new RemoveTransitionAction { Transition = transition }.Execute(sender, null));
        Assert.Empty(sender.Transitions!);
        Assert.Equal(true, new AddTransitionAction { Transition = transition }.Execute(sender, null));
        Assert.Equal(true, new ClearTransitionsAction().Execute(sender, null));
        Assert.Empty(sender.Transitions!);
    }

    [UnoHeadlessFact]
    public void TransitionActions_PreferTheExplicitElement()
    {
        Border sender = new();
        Border target = new();
        RepositionThemeTransition transition = new();

        Assert.Equal(true, new AddTransitionAction { Transition = transition, StyledElement = target }.Execute(sender, null));

        Assert.Null(sender.Transitions);
        Assert.Same(transition, Assert.Single(target.Transitions!));
        Assert.Equal(true, new ClearTransitionsAction { StyledElement = target }.Execute(sender, null));
        Assert.Empty(target.Transitions!);
    }

    [UnoHeadlessFact]
    public void TransitionActions_ReturnFalseWhenDisabledOrWithoutInputs()
    {
        Border sender = new();

        Assert.Equal(false, new AddTransitionAction { Transition = new EntranceThemeTransition(), IsEnabled = false }.Execute(sender, null));
        Assert.Equal(false, new AddTransitionAction().Execute(sender, null));
        Assert.Equal(false, new RemoveTransitionAction { Transition = new EntranceThemeTransition() }.Execute(sender, null));
        Assert.Equal(false, new ClearTransitionsAction().Execute(sender, null));
        Assert.Equal(false, new ClearTransitionsAction { IsEnabled = false }.Execute(sender, null));
        Assert.Null(sender.Transitions);
    }

    [UnoHeadlessFact]
    public async Task TransitionsBehavior_ReplacesAndRestoresTheTransitions()
    {
        TransitionCollection original = [new EntranceThemeTransition()];
        TransitionCollection source = [new RepositionThemeTransition()];
        Border target = new() { Transitions = original };
        TransitionsBehavior behavior = new() { TransitionsSource = source };
        Interaction.GetBehaviors(target).Add(behavior);

        await Session.ShowAsync(target);

        Assert.Same(source, target.Transitions);

        Interaction.GetBehaviors(target).Remove(behavior);

        Assert.Same(original, target.Transitions);
    }

    [UnoHeadlessFact]
    public async Task TransitionsChangedTrigger_ExecutesActionsWhenTheCollectionIsReplaced()
    {
        Border target = new();
        RecordingAction action = new();
        TransitionsChangedTrigger trigger = new();
        trigger.Actions.Add(action);
        Interaction.GetBehaviors(target).Add(trigger);
        await Session.ShowAsync(target);
        await Session.WaitForIdleAsync();
        int initial = action.Senders.Count;

        target.Transitions = [new EntranceThemeTransition()];

        await TestSupport.WaitUntilAsync(() => action.Senders.Count == initial + 1, "the trigger ran");
        Assert.Same(target, action.Senders[^1]);
    }
}
