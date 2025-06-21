// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Core;

/// <summary>
/// Executes a specified <see cref="System.Windows.Input.ICommand"/> when invoked. 
/// </summary>
public class InvokeCommandAction : InvokeCommandActionBase
{
    /// <summary>
    /// Executes the action.
    /// </summary>
    /// <param name="sender">The <see cref="object"/> that is passed to the action by the behavior. Generally this is <seealso cref="Avalonia.Xaml.Interactivity.IBehavior.AssociatedObject"/> or a target object.</param>
    /// <param name="parameter">The value of this parameter is determined by the caller.</param>
    /// <returns>True if the command is successfully executed; else false.</returns>
    public override object Execute(object? sender, object? parameter)
    {
        if (IsEnabled != true || Command is null)
        {
            return false;
        }

        var resolvedParameter = ResolveParameter(parameter);

        if (!Command.CanExecute(resolvedParameter))
        {
            return false;
        }

        Command.Execute(resolvedParameter);

        return true;
    }
}
