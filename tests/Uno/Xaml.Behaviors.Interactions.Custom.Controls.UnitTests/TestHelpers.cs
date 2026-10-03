// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Xaml.Behaviors.Uno.Headless;
using Xaml.Interactivity;

namespace Xaml.Interactions.Custom.Controls.UnitTests;

public partial class RecordingAction : Xaml.Interactivity.Action
{
    public List<object?> Parameters { get; } = [];

    public List<object?> Senders { get; } = [];

    public override object? Execute(object? sender, object? parameter)
    {
        Senders.Add(sender);
        Parameters.Add(parameter);
        return null;
    }
}

internal static class TestHelpers
{
    public static UnoHeadlessSession Session => UnoHeadlessSession.Current;

    /// <summary>
    /// Attaches the behavior (or trigger) to the element.
    /// </summary>
    public static T AttachTo<T>(this T behavior, DependencyObject element)
        where T : DependencyObject
    {
        Interaction.GetBehaviors(element).Add(behavior);
        return behavior;
    }

    /// <summary>
    /// Adds a recording action to the trigger.
    /// </summary>
    public static RecordingAction Record(this StyledElementTrigger trigger)
    {
        var action = new RecordingAction();
        trigger.Actions!.Add(action);
        return action;
    }

    /// <summary>
    /// Waits until the condition holds, processing the UI thread queue in between.
    /// </summary>
    public static async Task<bool> WaitUntilAsync(Func<bool> condition, int timeoutMilliseconds = 3000)
    {
        var deadline = DateTime.UtcNow.AddMilliseconds(timeoutMilliseconds);
        while (!condition())
        {
            if (DateTime.UtcNow > deadline)
            {
                return false;
            }

            await Session.WaitForIdleAsync();
            await Task.Delay(10);
        }

        return true;
    }
}
