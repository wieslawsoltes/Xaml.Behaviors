// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Xaml.Behaviors.Uno.Headless;
using Xaml.Behaviors.Uno.Headless.XUnit;
using Xaml.Interactivity;
using Xunit;

namespace Xaml.Interactions.Custom.Animations.UnitTests;

public class AnimationBehaviorsTests
{
    private static UnoHeadlessSession Session => UnoHeadlessSession.Current;

    [UnoHeadlessFact]
    public async Task PlayAnimationBehavior_PlaysTheStoryboardWhenAttached()
    {
        Border target = new() { Width = 10d, Height = 10d };
        Interaction.GetBehaviors(target).Add(new PlayAnimationBehavior { Animation = TestSupport.CreateOpacityStoryboard(0.4d) });

        await Session.ShowAsync(target);

        await TestSupport.WaitUntilAsync(() => Math.Abs(target.Opacity - 0.4d) < 0.001d, "the storyboard completed");
    }

    [UnoHeadlessFact]
    public async Task AnimateOnAttachedBehavior_PrefersTheExplicitStoryboard()
    {
        Border target = new() { Width = 10d, Height = 10d };
        OpacityAnimationBuilder builder = new(0.2d);
        Interaction.GetBehaviors(target).Add(new AnimateOnAttachedBehavior
        {
            Animation = TestSupport.CreateOpacityStoryboard(0.6d),
            AnimationBuilder = builder,
        });

        await Session.ShowAsync(target);

        await TestSupport.WaitUntilAsync(() => Math.Abs(target.Opacity - 0.6d) < 0.001d, "the storyboard completed");
        Assert.Null(builder.Control);
    }

    [UnoHeadlessFact]
    public async Task AnimateOnAttachedBehavior_BuildsTheStoryboardForTheAssociatedControl()
    {
        Border target = new() { Width = 10d, Height = 10d };
        OpacityAnimationBuilder builder = new(0.3d);
        Interaction.GetBehaviors(target).Add(new AnimateOnAttachedBehavior { AnimationBuilder = builder });

        await Session.ShowAsync(target);

        Assert.Same(target, builder.Control);
        await TestSupport.WaitUntilAsync(() => Math.Abs(target.Opacity - 0.3d) < 0.001d, "the built storyboard completed");
    }

    [UnoHeadlessFact]
    public async Task FadeInBehavior_HoldsTheControlTransparentDuringTheDelay()
    {
        Border target = new() { Width = 10d, Height = 10d };
        FadeInBehavior behavior = new();
        Assert.Equal(TimeSpan.FromMilliseconds(500), behavior.InitialDelay);
        Assert.Equal(TimeSpan.FromMilliseconds(250), behavior.Duration);
        behavior.InitialDelay = TimeSpan.FromMilliseconds(300);
        behavior.Duration = TimeSpan.FromMilliseconds(50);
        Interaction.GetBehaviors(target).Add(behavior);

        await Session.ShowAsync(target);
        await Task.Delay(50);

        Assert.Equal(0d, target.Opacity);
        await TestSupport.WaitUntilAsync(() => target.Opacity == 1d, "the fade completed");
    }

    [UnoHeadlessFact]
    public async Task BeginAnimationAction_RunsOnTheTargetControlOrTheSender()
    {
        Border sender = new() { Width = 10d, Height = 10d };
        Border target = new() { Width = 10d, Height = 10d };
        await Session.ShowAsync(new StackPanel { Children = { sender, target } });
        BeginAnimationAction action = new() { Animation = TestSupport.CreateOpacityStoryboard(0.5d) };

        Assert.Equal(true, action.Execute(sender, null));
        await TestSupport.WaitUntilAsync(() => Math.Abs(sender.Opacity - 0.5d) < 0.001d, "the sender was animated");

        action.Animation.Stop();
        action.TargetControl = target;
        Assert.Equal(true, action.Execute(sender, null));
        await TestSupport.WaitUntilAsync(() => Math.Abs(target.Opacity - 0.5d) < 0.001d, "the target was animated");

        action.IsEnabled = false;
        Assert.Equal(false, action.Execute(sender, null));
    }

    [UnoHeadlessFact]
    public void BeginAnimationAction_ReturnsFalseWithoutStoryboardOrTarget()
    {
        Assert.Equal(false, new BeginAnimationAction().Execute(new Border(), null));
        Assert.Equal(false, new BeginAnimationAction { Animation = new Storyboard() }.Execute(null, null));
    }

    [UnoHeadlessFact]
    public async Task StartAnimationAction_RunsOnTheSender()
    {
        Border sender = new() { Width = 10d, Height = 10d };
        await Session.ShowAsync(sender);
        StartAnimationAction action = new() { Animation = TestSupport.CreateOpacityStoryboard(0.7d) };

        Assert.Equal(true, action.Execute(sender, null));
        Assert.Equal(false, action.Execute(null, null));
        await TestSupport.WaitUntilAsync(() => Math.Abs(sender.Opacity - 0.7d) < 0.001d, "the sender was animated");
    }

    [UnoHeadlessFact]
    public async Task StartBuiltAnimationAction_BuildsForTheSender()
    {
        Border sender = new() { Width = 10d, Height = 10d };
        await Session.ShowAsync(sender);
        OpacityAnimationBuilder builder = new(0.35d);
        StartBuiltAnimationAction action = new() { AnimationBuilder = builder };

        Assert.Equal(true, action.Execute(sender, null));
        Assert.Same(sender, builder.Control);
        Assert.Equal(false, new StartBuiltAnimationAction().Execute(sender, null));
        await TestSupport.WaitUntilAsync(() => Math.Abs(sender.Opacity - 0.35d) < 0.001d, "the sender was animated");
    }

    [UnoHeadlessFact]
    public async Task AnimationCompletedTrigger_ExecutesActionsAfterTheStoryboard()
    {
        Border target = new() { Width = 10d, Height = 10d };
        RecordingAction action = new();
        AnimationCompletedTrigger trigger = new() { Animation = TestSupport.CreateOpacityStoryboard(0.5d, 60) };
        trigger.Actions!.Add(action);
        Interaction.GetBehaviors(target).Add(trigger);

        await Session.ShowAsync(target);

        Assert.Empty(action.Senders);
        await TestSupport.WaitUntilAsync(() => action.Senders.Count == 1, "the actions ran");
        Assert.Same(target, action.Senders[0]);
        Assert.Equal(0.5d, target.Opacity, 3);
    }

    [UnoHeadlessFact]
    public async Task AnimationCompletedTrigger_ExecutesImmediatelyWithoutStoryboard()
    {
        Border target = new();
        RecordingAction action = new();
        AnimationCompletedTrigger trigger = new();
        trigger.Actions!.Add(action);
        Interaction.GetBehaviors(target).Add(trigger);

        await Session.ShowAsync(target);

        Assert.Single(action.Senders);
    }

    [UnoHeadlessFact]
    public async Task RunAnimationTrigger_ExecutesActionsAfterTheBuiltStoryboard()
    {
        Border target = new() { Width = 10d, Height = 10d };
        RecordingAction action = new();
        OpacityAnimationBuilder builder = new(0.45d);
        RunAnimationTrigger trigger = new() { AnimationBuilder = builder };
        trigger.Actions!.Add(action);
        Interaction.GetBehaviors(target).Add(trigger);

        await Session.ShowAsync(target);

        Assert.Same(target, builder.Control);
        await TestSupport.WaitUntilAsync(() => action.Senders.Count == 1, "the actions ran");
        Assert.Equal(0.45d, target.Opacity, 3);
    }

    [UnoHeadlessFact]
    public async Task RunAnimationTrigger_DoesNothingWithoutStoryboard()
    {
        Border target = new();
        RecordingAction action = new();
        RunAnimationTrigger trigger = new();
        trigger.Actions!.Add(action);
        Interaction.GetBehaviors(target).Add(trigger);

        await Session.ShowAsync(target);
        await Session.WaitForIdleAsync();

        Assert.Empty(action.Senders);
    }
}
