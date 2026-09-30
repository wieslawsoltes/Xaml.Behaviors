// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Xaml.Behaviors.Uno.Headless;
using Xaml.Behaviors.Uno.Headless.XUnit;
using Xaml.Interactivity;
using Xunit;

namespace Xaml.Interactions.Custom.General.UnitTests;

public class CoreTests
{
    private static UnoHeadlessSession Session => UnoHeadlessSession.Current;

    [UnoHeadlessFact]
    public async Task LoadedTrigger_And_UnloadedTrigger_Follow_The_Live_Tree()
    {
        var border = new Border();
        var loaded = new LoadedTrigger();
        var unloaded = new UnloadedTrigger();
        var loadedAction = new RecordingAction();
        var unloadedAction = new RecordingAction();
        loaded.Actions!.Add(loadedAction);
        unloaded.Actions!.Add(unloadedAction);
        Interaction.GetBehaviors(border).Add(loaded);
        Interaction.GetBehaviors(border).Add(unloaded);

        var host = new StackPanel { Children = { border } };
        await Session.ShowAsync(host);
        await Session.WaitForIdleAsync();
        Assert.Single(loadedAction.Parameters);
        Assert.Empty(unloadedAction.Parameters);

        host.Children.Remove(border);
        await Session.WaitForIdleAsync();
        Assert.Single(unloadedAction.Parameters);
    }

    [UnoHeadlessFact]
    public async Task Initialized_And_AttachedToVisualTree_Triggers_Execute_Once_Loaded()
    {
        var border = new Border();
        var initialized = new InitializedTrigger();
        var attached = new AttachedToVisualTreeTrigger();
        var logical = new AttachedToLogicalTreeTrigger();
        var initializedAction = new RecordingAction();
        var attachedAction = new RecordingAction();
        var logicalAction = new RecordingAction();
        initialized.Actions!.Add(initializedAction);
        attached.Actions!.Add(attachedAction);
        logical.Actions!.Add(logicalAction);
        Interaction.GetBehaviors(border).Add(initialized);
        Interaction.GetBehaviors(border).Add(attached);
        Interaction.GetBehaviors(border).Add(logical);

        await Session.ShowAsync(border);
        await Session.WaitForIdleAsync();

        Assert.Single(initializedAction.Parameters);
        Assert.Single(attachedAction.Parameters);
        Assert.Single(logicalAction.Parameters);
    }

    [UnoHeadlessFact]
    public async Task DataContextChangedTrigger_Executes_On_DataContext_Change()
    {
        var border = new Border();
        var trigger = new DataContextChangedTrigger();
        var action = new RecordingAction();
        trigger.Actions!.Add(action);
        Interaction.GetBehaviors(border).Add(trigger);
        await Session.ShowAsync(border);
        await Session.WaitForIdleAsync();
        action.Parameters.Clear();

        border.DataContext = new TestViewModel();
        await Session.WaitForIdleAsync();

        Assert.NotEmpty(action.Parameters);
    }

    [UnoHeadlessFact]
    public async Task SizeChangedTrigger_Passes_SizeChangedEventArgs()
    {
        var border = new Border { Width = 10, Height = 10 };
        var trigger = new SizeChangedTrigger();
        var action = new RecordingAction();
        trigger.Actions!.Add(action);
        Interaction.GetBehaviors(border).Add(trigger);
        await Session.ShowAsync(border);
        await Session.WaitForIdleAsync();
        action.Parameters.Clear();

        border.Width = 40;
        await Session.WaitForIdleAsync();

        Assert.Contains(action.Parameters, p => p is SizeChangedEventArgs);
    }

    [UnoHeadlessFact]
    public async Task DelayedLoadTrigger_Executes_After_The_Delay()
    {
        var border = new Border();
        var trigger = new DelayedLoadTrigger { Delay = TimeSpan.FromMilliseconds(50) };
        var action = new RecordingAction();
        trigger.Actions!.Add(action);
        Interaction.GetBehaviors(border).Add(trigger);
        await Session.ShowAsync(border);

        await TestInput.WaitUntilAsync(() => action.Parameters.Count > 0);

        Assert.Single(action.Parameters);
    }

    [UnoHeadlessFact]
    public async Task IfElseTrigger_Executes_The_Branch_Matching_The_Condition()
    {
        var border = new Border();
        var trigger = new IfElseTrigger();
        var ifAction = new RecordingAction();
        var elseAction = new RecordingAction();
        trigger.IfActions.Add(ifAction);
        trigger.ElseActions.Add(elseAction);
        Interaction.GetBehaviors(border).Add(trigger);
        await Session.ShowAsync(border);
        await Session.WaitForIdleAsync();
        Assert.Single(elseAction.Parameters);

        trigger.Condition = true;
        await Session.WaitForIdleAsync();

        Assert.Single(ifAction.Parameters);
        Assert.Single(elseAction.Parameters);
    }

    [UnoHeadlessFact]
    public async Task PropertyChangedTrigger_And_ValueChangedTriggerBehavior_Execute_On_Binding_Change()
    {
        var vm = new TestViewModel();
        var border = new Border { DataContext = vm };
        var propertyChanged = new PropertyChangedTrigger();
        var valueChanged = new ValueChangedTriggerBehavior();
        var propertyAction = new RecordingAction();
        var valueAction = new RecordingAction();
        propertyChanged.Actions!.Add(propertyAction);
        valueChanged.Actions!.Add(valueAction);
        BindingOperations.SetBinding(propertyChanged, PropertyChangedTrigger.BindingProperty, new Binding { Path = new PropertyPath(nameof(TestViewModel.Count)) });
        BindingOperations.SetBinding(valueChanged, ValueChangedTriggerBehavior.BindingProperty, new Binding { Path = new PropertyPath(nameof(TestViewModel.Count)) });
        Interaction.GetBehaviors(border).Add(propertyChanged);
        Interaction.GetBehaviors(border).Add(valueChanged);
        await Session.ShowAsync(border);
        await Session.WaitForIdleAsync();
        propertyAction.Parameters.Clear();
        valueAction.Parameters.Clear();

        vm.Count = 5;
        await Session.WaitForIdleAsync();

        Assert.Single(propertyAction.Parameters);
        Assert.Single(valueAction.Parameters);
    }

    [UnoHeadlessFact]
    public async Task BindingTriggerBehavior_Evaluates_Its_Binding()
    {
        var vm = new TestViewModel();
        var border = new Border { DataContext = vm };
        var trigger = new BindingTriggerBehavior
        {
            Binding = new Binding { Path = new PropertyPath(nameof(TestViewModel.Count)) },
            ComparisonCondition = ComparisonConditionType.Equal,
            Value = 3,
        };
        var action = new RecordingAction();
        trigger.Actions!.Add(action);
        Interaction.GetBehaviors(border).Add(trigger);
        await Session.ShowAsync(border);
        await Session.WaitForIdleAsync();
        action.Parameters.Clear();

        vm.Count = 1;
        await Session.WaitForIdleAsync();
        Assert.Empty(action.Parameters);

        vm.Count = 3;
        await Session.WaitForIdleAsync();
        Assert.NotEmpty(action.Parameters);
    }

    [UnoHeadlessFact]
    public async Task BindingBehavior_Applies_And_Clears_The_Binding()
    {
        var vm = new TestViewModel { Name = "bound" };
        var target = new TextBlock();
        var border = new Border { DataContext = vm, Child = target };
        var behavior = new BindingBehavior
        {
            TargetObject = target,
            TargetProperty = TextBlock.TextProperty,
            Binding = new Binding { Source = vm, Path = new PropertyPath(nameof(TestViewModel.Name)) },
        };
        Interaction.GetBehaviors(border).Add(behavior);
        var host = new StackPanel { Children = { border } };
        await Session.ShowAsync(host);
        await Session.WaitForIdleAsync();

        Assert.Equal("bound", target.Text);

        Interaction.GetBehaviors(border).Remove(behavior);
        vm.Name = "changed";
        await Session.WaitForIdleAsync();
        Assert.NotEqual("changed", target.Text);
    }

    [UnoHeadlessFact]
    public async Task ObservableTriggerBehavior_Executes_With_The_Produced_Value()
    {
        var observable = new TestObservable<int>();
        var border = new Border();
        var trigger = new ObservableTriggerBehavior<int> { Observable = observable };
        var action = new RecordingAction();
        trigger.Actions!.Add(action);
        Interaction.GetBehaviors(border).Add(trigger);
        await Session.ShowAsync(border);

        observable.OnNext(7);
        await Session.WaitForIdleAsync();

        Assert.Equal([7], action.Parameters);
        Assert.Equal(7, trigger.Value);

        Interaction.GetBehaviors(border).Remove(trigger);
        Assert.Equal(0, observable.SubscriberCount);
    }

    [UnoHeadlessFact]
    public async Task RoutedEventTriggerBehavior_Listens_To_The_Routed_Event()
    {
        var button = new Button { Content = "b" };
        var other = new TextBox();
        var trigger = new RoutedEventTriggerBehavior { RoutedEvent = UIElement.KeyDownEvent };
        var action = new RecordingAction();
        trigger.Actions!.Add(action);
        Interaction.GetBehaviors(button).Add(trigger);
        await Session.ShowAsync(new StackPanel { Children = { button, other } });

        await TestInput.PressKeyAsync(button, Windows.System.VirtualKey.A);

        Assert.NotEmpty(action.Parameters);
    }

    [UnoHeadlessFact]
    public async Task Typed_RoutedEventTrigger_Receives_Focus_Events()
    {
        var button = new Button { Content = "b" };
        var other = new TextBox();
        var got = new GotFocusTrigger();
        var lost = new LostFocusTrigger();
        var gotAction = new RecordingAction();
        var lostAction = new RecordingAction();
        got.Actions!.Add(gotAction);
        lost.Actions!.Add(lostAction);
        Interaction.GetBehaviors(button).Add(got);
        Interaction.GetBehaviors(button).Add(lost);
        await Session.ShowAsync(new StackPanel { Children = { button, other } });

        button.Focus(FocusState.Programmatic);
        await Session.WaitForIdleAsync();
        other.Focus(FocusState.Programmatic);
        await Session.WaitForIdleAsync();

        Assert.Single(gotAction.Parameters);
        Assert.Single(lostAction.Parameters);
    }

    [UnoHeadlessFact]
    public void RoutedEvent_Of_T_Converts_WinUI_Identifiers()
    {
        RoutedEvent<Microsoft.UI.Xaml.Input.KeyRoutedEventArgs> keyDown = UIElement.KeyDownEvent;

        Assert.Same(UIElement.KeyDownEvent, keyDown.Event);
    }

    [UnoHeadlessFact]
    public async Task AsyncLoadBehavior_Invokes_The_Method_When_Loaded()
    {
        var target = new AsyncTarget();
        var border = new Border();
        Interaction.GetBehaviors(border).Add(new AsyncLoadBehavior { TargetObject = target, MethodName = nameof(AsyncTarget.LoadAsync) });

        await Session.ShowAsync(border);
        await TestInput.WaitUntilAsync(() => target.Calls > 0);

        Assert.Equal(1, target.Calls);
    }

    public sealed class AsyncTarget
    {
        public int Calls { get; private set; }

        public Task LoadAsync()
        {
            Calls++;
            return Task.CompletedTask;
        }
    }
}
