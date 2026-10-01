// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.Foundation;
using Xaml.Behaviors.Uno.Headless;
using Xaml.Behaviors.Uno.Headless.XUnit;
using Xaml.Interactions.Events;
using Xaml.Interactivity;
using Xunit;

namespace Xaml.Interactions.DragAndDrop.UnitTests;

/// <summary>
/// Regression tests for https://github.com/wieslawsoltes/Xaml.Behaviors/issues/381: the drag event triggers observe
/// the drag events that a drop handler on the same element handles (WinUI drags driven with injected mouse input).
/// </summary>
public class DragEventTriggerTests
{
    private static UnoHeadlessSession Session => UnoHeadlessSession.Current;

    private sealed class RecordingAction : Action
    {
        public List<object?> Parameters { get; } = [];

        public override object? Execute(object? sender, object? parameter)
        {
            Parameters.Add(parameter);
            return null;
        }
    }

    private static Border CreateBox(double left) => new()
    {
        Width = 60,
        Height = 60,
        Margin = new Thickness(left, 0, 0, 0),
        Background = new SolidColorBrush(Colors.Red),
        HorizontalAlignment = HorizontalAlignment.Left,
        VerticalAlignment = VerticalAlignment.Top,
    };

    private static RecordingAction AddTrigger(UIElement target, InteractiveTriggerBase trigger)
    {
        var action = new RecordingAction();
        trigger.Actions!.Add(action);
        Interaction.GetBehaviors(target).Add(trigger);
        return action;
    }

    [UnoHeadlessFact]
    public async Task Drag_Event_Triggers_Fire_When_Drop_Handler_Handles_The_Events()
    {
        var input = MouseInput.TryCreate();
        Assert.SkipWhen(input is null, "Input injection is not available.");

        var source = CreateBox(0);
        var target = CreateBox(200);
        var dragHandler = new RecordingDragHandler();
        var dropHandler = new RecordingDropHandler();
        // The drop handlers do not handle DragLeave: a handler registered before the trigger does.
        target.AddHandler(UIElement.DragLeaveEvent, new DragEventHandler(static (_, e) => e.Handled = true), false);
        Interaction.GetBehaviors(source).Add(new ContextDragBehavior { Context = "payload", Handler = dragHandler });
        Interaction.GetBehaviors(target).Add(new ContextDropBehavior { Handler = dropHandler });
        var enter = AddTrigger(target, new DragEnterEventTrigger());
        var over = AddTrigger(target, new DragOverEventTrigger());
        var leave = AddTrigger(target, new DragLeaveEventTrigger());
        var drop = AddTrigger(target, new DropEventTrigger());
        await Session.ShowAsync(new Grid { Children = { source, target } });

        var targetCenter = MouseInput.Center(target);
        await input!.PressAndMoveAsync(MouseInput.Center(source), targetCenter, steps: 10);
        await input.MoveAsync(new Point(targetCenter.X + 100, targetCenter.Y));
        await input.MoveAsync(new Point(targetCenter.X + 50, targetCenter.Y));
        await input.MoveAsync(targetCenter);
        await input.UpAsync();
        for (var i = 0; i < 400 && dragHandler.Calls.Count < 2; i++)
        {
            await Task.Delay(5);
        }

        await Session.WaitForIdleAsync();

        Assert.Contains("Drop", dropHandler.Calls);
        Assert.NotEmpty(enter.Parameters);
        Assert.NotEmpty(over.Parameters);
        Assert.NotEmpty(leave.Parameters);
        Assert.Single(drop.Parameters);
    }
}
