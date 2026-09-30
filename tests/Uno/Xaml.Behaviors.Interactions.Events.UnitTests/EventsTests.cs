// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Input.Preview.Injection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;
using Xaml.Behaviors.Uno.Headless;
using Xaml.Behaviors.Uno.Headless.XUnit;
using Xaml.Interactions.Events;
using Xaml.Interactivity;
using Xunit;

namespace Xaml.Interactions.Events.UnitTests;

public partial class RecordingAction : Xaml.Interactivity.Action
{
    public List<object?> Parameters { get; } = [];

    public override object? Execute(object? sender, object? parameter)
    {
        Parameters.Add(parameter);
        return null;
    }
}

public class EventsTests
{
    private static UnoHeadlessSession Session => UnoHeadlessSession.Current;

    private static async Task<(Button Button, TextBox Other)> ShowFocusablesAsync()
    {
        var button = new Button { Content = "b" };
        var other = new TextBox();
        await Session.ShowAsync(new StackPanel { Children = { button, other } });
        return (button, other);
    }

    [UnoHeadlessFact]
    public async Task GotFocus_And_LostFocus_Triggers_Execute()
    {
        var (button, other) = await ShowFocusablesAsync();
        var got = new GotFocusEventTrigger();
        var lost = new LostFocusEventTrigger();
        var gotAction = new RecordingAction();
        var lostAction = new RecordingAction();
        got.Actions!.Add(gotAction);
        lost.Actions!.Add(lostAction);
        Interaction.GetBehaviors(button).Add(got);
        Interaction.GetBehaviors(button).Add(lost);

        button.Focus(FocusState.Programmatic);
        await Session.WaitForIdleAsync();
        other.Focus(FocusState.Programmatic);
        await Session.WaitForIdleAsync();

        Assert.Single(gotAction.Parameters);
        Assert.Single(lostAction.Parameters);
    }

    [UnoHeadlessFact]
    public async Task Detached_Trigger_Stops_Receiving_Events()
    {
        var (button, other) = await ShowFocusablesAsync();
        var got = new GotFocusEventTrigger();
        var action = new RecordingAction();
        got.Actions!.Add(action);
        Interaction.GetBehaviors(button).Add(got);

        Interaction.GetBehaviors(button).Remove(got);
        button.Focus(FocusState.Programmatic);
        await Session.WaitForIdleAsync();

        Assert.Empty(action.Parameters);
        other.Focus(FocusState.Programmatic);
    }

    [UnoHeadlessFact]
    public void Per_Type_RoutingStrategies_Default_Is_Applied()
    {
        var pressed = new PointerPressedEventTrigger();
        var tapped = new TappedEventTrigger();

        Assert.Equal(RoutingStrategies.Tunnel | RoutingStrategies.Bubble, pressed.RoutingStrategies);
        Assert.Equal(RoutingStrategies.Bubble, tapped.RoutingStrategies & RoutingStrategies.Bubble);
    }

    [UnoHeadlessFact]
    public async Task PointerPressed_Trigger_Receives_Injected_Input()
    {
        var injector = InputInjector.TryCreate();
        Assert.SkipWhen(injector is null, "Input injection is not available.");

        var border = new Border { Width = 100, Height = 100, Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Red) };
        var trigger = new PointerPressedEventTrigger();
        var action = new RecordingAction();
        trigger.Actions!.Add(action);
        Interaction.GetBehaviors(border).Add(trigger);
        await Session.ShowAsync(border);

        var position = border.TransformToVisual(null).TransformPoint(new Point(50, 50));
        injector!.InitializeTouchInjection(InjectedInputVisualizationMode.None);
        injector.InjectMouseInput([new InjectedInputMouseInfo
        {
            DeltaX = (int)position.X,
            DeltaY = (int)position.Y,
            MouseOptions = InjectedInputMouseOptions.Absolute | InjectedInputMouseOptions.Move,
        }]);
        injector.InjectMouseInput([new InjectedInputMouseInfo { MouseOptions = InjectedInputMouseOptions.LeftDown }]);
        injector.InjectMouseInput([new InjectedInputMouseInfo { MouseOptions = InjectedInputMouseOptions.LeftUp }]);
        await Session.WaitForIdleAsync();

        Assert.NotEmpty(action.Parameters);
    }
}
