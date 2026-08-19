// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Collections.Generic;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Core;

internal static class ReversibleActionExecution
{
    public static List<IReversibleAction> Execute(object? sender, ActionCollection? actions, object? parameter)
    {
        var appliedActions = new List<IReversibleAction>();
        if (actions is null)
        {
            return appliedActions;
        }

        try
        {
            foreach (var item in actions)
            {
                if (item is IReversibleAction reversibleAction)
                {
                    var result = reversibleAction.ExecuteReversibly(sender, parameter);
                    if (result is not false)
                    {
                        appliedActions.Add(reversibleAction);
                    }
                }
                else if (item is IAction action)
                {
                    action.Execute(sender, parameter);
                }
            }
        }
        catch
        {
            for (var index = appliedActions.Count - 1; index >= 0; index--)
            {
                try
                {
                    appliedActions[index].Revert(sender, parameter);
                }
                catch
                {
                    // Preserve the exception raised while applying the action sequence.
                }
            }

            throw;
        }

        return appliedActions;
    }
}
