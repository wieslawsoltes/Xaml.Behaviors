// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Xaml.Behaviors.Uno.Headless;
using Xaml.Behaviors.Uno.Headless.XUnit;
using Xaml.Interactions.Core;
using Xaml.Interactions.Custom;
using Xaml.Interactions.DragAndDrop;
using Xaml.Interactions.Draggable;
using Xaml.Interactions.Events;
using Xaml.Interactions.Responsive;
using Xaml.Interactivity;
using Xunit;

namespace Xaml.Behaviors.UnitTests;

public partial class RecordingAction : Xaml.Interactivity.Action
{
    public List<object?> Parameters { get; } = [];

    public override object? Execute(object? sender, object? parameter)
    {
        Parameters.Add(parameter);
        return null;
    }
}

/// <summary>
/// Smoke tests of the single assembly build (Xaml.Behaviors.Uno): the components compile into one assembly and work
/// together. The behaviors themselves are covered by the tests of the individual packages.
/// </summary>
public class CombinedAssemblyTests
{
    private static UnoHeadlessSession Session => UnoHeadlessSession.Current;

    [Theory]
    [InlineData(typeof(Behavior))]
    [InlineData(typeof(EventTriggerBehavior))]
    [InlineData(typeof(HideControlAction))]
    [InlineData(typeof(FluidMoveAnimation))]
    [InlineData(typeof(ContextDragBehavior))]
    [InlineData(typeof(CanvasDragBehavior))]
    [InlineData(typeof(GotFocusEventTrigger))]
    [InlineData(typeof(AdaptiveBehavior))]
    public void Components_Are_Compiled_Into_One_Assembly(Type type)
    {
        Assert.Equal("Xaml.Behaviors.Uno", type.Assembly.GetName().Name);
    }

    [UnoHeadlessFact]
    public async Task Events_Trigger_Runs_Interactions_And_Custom_Actions()
    {
        var button = new Button { Content = "b" };
        var other = new TextBox();
        var target = new Border();
        await Session.ShowAsync(new StackPanel { Children = { button, other, target } });

        var trigger = new GotFocusEventTrigger();
        var recording = new RecordingAction();
        trigger.Actions!.Add(recording);
        trigger.Actions.Add(new HideControlAction { TargetControl = target });
        Interaction.GetBehaviors(button).Add(trigger);

        button.Focus(FocusState.Programmatic);
        await Session.WaitForIdleAsync();

        Assert.Single(recording.Parameters);
        Assert.Equal(Visibility.Collapsed, target.Visibility);
        other.Focus(FocusState.Programmatic);
    }

    [UnoHeadlessFact]
    public async Task Custom_Behavior_Uses_Animations_Component()
    {
        var element = new Border { Width = 10, Height = 10 };
        await Session.ShowAsync(element);

        Assert.True(FluidMoveAnimation.TryRun(element, 20, 30, TimeSpan.FromMilliseconds(1)));
        Assert.IsType<TranslateTransform>(element.RenderTransform);
    }
}
