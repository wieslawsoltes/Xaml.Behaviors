// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using global::ReactiveUI;
using Microsoft.UI.Xaml.Controls;
using Splat;
using Xaml.Behaviors.Uno.Headless;
using Xaml.Behaviors.Uno.Headless.XUnit;
using Xaml.Interactivity;
using Xunit;

namespace Xaml.Interactions.ReactiveUI.UnitTests;

public sealed class TestScreen : IScreen
{
    public RoutingState Router { get; } = new(ImmediateScheduler.Instance);
}

public class FirstViewModel(IScreen screen) : ReactiveObject, IRoutableViewModel
{
    public FirstViewModel()
        : this(new TestScreen())
    {
    }

    public string? UrlPathSegment => "first";

    public IScreen HostScreen { get; } = screen;
}

public sealed class SecondViewModel(IScreen screen) : FirstViewModel(screen)
{
    public SecondViewModel()
        : this(new TestScreen())
    {
    }
}

public sealed class UnregisteredViewModel(IScreen screen) : FirstViewModel(screen)
{
}

public partial class RecordingAction : Xaml.Interactivity.Action
{
    public List<object?> Parameters { get; } = [];

    public override object? Execute(object? sender, object? parameter)
    {
        Parameters.Add(parameter);
        return null;
    }
}

public class ReactiveUIActionsTests
{
    private static UnoHeadlessSession Session => UnoHeadlessSession.Current;

    [UnoHeadlessFact]
    public void NavigateAction_NavigatesToTheViewModelOrTheParameter()
    {
        TestScreen screen = new();
        FirstViewModel first = new(screen);
        SecondViewModel second = new(screen);
        NavigateAction action = new() { Router = screen.Router, ViewModel = first };

        Assert.Equal(true, action.Execute(null, second));
        action.ViewModel = null;
        Assert.Equal(true, action.Execute(null, second));

        Assert.Equal([first, second], screen.Router.NavigationStack);
    }

    [UnoHeadlessFact]
    public void NavigateAction_ReturnsFalseWhenDisabledOrWithoutRouterOrViewModel()
    {
        TestScreen screen = new();
        FirstViewModel first = new(screen);

        Assert.Equal(false, new NavigateAction { ViewModel = first }.Execute(null, null));
        Assert.Equal(false, new NavigateAction { Router = screen.Router }.Execute(null, "not a view model"));
        Assert.Equal(false, new NavigateAction { Router = screen.Router, ViewModel = first, IsEnabled = false }.Execute(null, null));
        Assert.Empty(screen.Router.NavigationStack);
    }

    [UnoHeadlessFact]
    public void NavigateAndReset_ReplacesTheNavigationStack()
    {
        TestScreen screen = new();
        FirstViewModel first = new(screen);
        SecondViewModel second = new(screen);
        screen.Router.Navigate.Execute(first).Subscribe();

        Assert.Equal(true, new NavigateAndReset { Router = screen.Router, ViewModel = second }.Execute(null, null));

        Assert.Equal([second], screen.Router.NavigationStack);
        Assert.Equal(false, new NavigateAndReset { Router = screen.Router }.Execute(null, null));
    }

    [UnoHeadlessFact]
    public void NavigateBackAction_NavigatesBackWhenPossible()
    {
        TestScreen screen = new();
        FirstViewModel first = new(screen);
        SecondViewModel second = new(screen);
        NavigateBackAction action = new() { Router = screen.Router };

        Assert.Equal(false, action.Execute(null, null));

        screen.Router.Navigate.Execute(first).Subscribe();
        screen.Router.Navigate.Execute(second).Subscribe();
        Assert.Equal(true, action.Execute(null, null));

        Assert.Equal([first], screen.Router.NavigationStack);
        Assert.Equal(false, new NavigateBackAction().Execute(null, null));
    }

    [UnoHeadlessFact]
    public void ClearNavigationStackAction_ClearsTheStack()
    {
        TestScreen screen = new();
        screen.Router.Navigate.Execute(new FirstViewModel(screen)).Subscribe();

        Assert.Equal(true, new ClearNavigationStackAction { Router = screen.Router }.Execute(null, null));

        Assert.Empty(screen.Router.NavigationStack);
        Assert.Equal(false, new ClearNavigationStackAction().Execute(null, null));
        Assert.Equal(false, new ClearNavigationStackAction { Router = screen.Router, IsEnabled = false }.Execute(null, null));
    }

    [UnoHeadlessFact]
    public void NavigateToAction_UsesTheParameterOrTheServiceLocator()
    {
        TestScreen screen = new();
        SecondViewModel fromParameter = new(screen);
        SecondViewModel fromLocator = new(screen);
        NavigateToAction<SecondViewModel> action = new() { Router = screen.Router };

        Assert.Equal(true, action.Execute(null, fromParameter));
        Locator.CurrentMutable.RegisterConstant(fromLocator);
        try
        {
            Assert.Equal(true, action.Execute(null, null));
        }
        finally
        {
            Locator.CurrentMutable.UnregisterCurrent(typeof(SecondViewModel));
        }

        Assert.Equal<IRoutableViewModel>([fromParameter, fromLocator], screen.Router.NavigationStack);
        Assert.Equal(false, new NavigateToAction<UnregisteredViewModel> { Router = screen.Router }.Execute(null, null));
        Assert.Equal(false, new NavigateToAction<SecondViewModel>().Execute(null, fromParameter));
    }

    [UnoHeadlessFact]
    public void NavigateToAndResetAction_ResetsTheStack()
    {
        TestScreen screen = new();
        FirstViewModel first = new(screen);
        SecondViewModel second = new(screen);
        screen.Router.Navigate.Execute(first).Subscribe();
        NavigateToAndResetAction<SecondViewModel> action = new() { Router = screen.Router };

        Assert.Equal(true, action.Execute(null, second));

        Assert.Equal<IRoutableViewModel>([second], screen.Router.NavigationStack);
        Assert.Equal(false, new NavigateToAndResetAction<SecondViewModel>().Execute(null, second));
    }

    [UnoHeadlessFact]
    public async Task InteractionTriggerBehavior_ExecutesActionsWhileAttached()
    {
        Interaction<string, int> interaction = new(ImmediateScheduler.Instance);
        Border element = new();
        RecordingAction action = new();
        InteractionTriggerBehavior<string, int> trigger = new() { Interaction = interaction };
        trigger.Actions!.Add(action);
        Interaction.GetBehaviors(element).Add(trigger);
        await Session.ShowAsync(element);

        int output = await interaction.Handle("input");

        Assert.Equal(0, output);
        Assert.Equal(["input"], action.Parameters);

        Interaction.GetBehaviors(element).Remove(trigger);
        await Assert.ThrowsAsync<UnhandledInteractionException<string, int>>(async () => await interaction.Handle("again"));
        Assert.Single(action.Parameters);
    }
}
