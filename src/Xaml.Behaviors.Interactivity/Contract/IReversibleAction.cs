// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
namespace Avalonia.Xaml.Interactivity;

/// <summary>
/// Interface for actions that can revert the effect of a previous <see cref="IAction.Execute"/> call.
/// </summary>
public interface IReversibleAction : IAction
{
    /// <summary>
    /// Executes the action while capturing the state required by <see cref="Revert"/>.
    /// </summary>
    /// <param name="sender">The <see cref="object"/> that is passed to the action by the behavior.</param>
    /// <param name="parameter">The value of this parameter is determined by the caller.</param>
    /// <returns>Returns the result of the reversible execution.</returns>
    object? ExecuteReversibly(object? sender, object? parameter);

    /// <summary>
    /// Reverts the previously applied action.
    /// </summary>
    /// <param name="sender">The <see cref="object"/> that is passed to the action by the behavior.</param>
    /// <param name="parameter">The value of this parameter is determined by the caller.</param>
    /// <returns>Returns the result of the revert operation.</returns>
    object? Revert(object? sender, object? parameter);
}
