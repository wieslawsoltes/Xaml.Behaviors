// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
namespace Avalonia.Xaml.Interactions.Scripting;

/// <summary>
/// Provides global variables for script execution, including the sender and parameter.
/// </summary>
/// <param name="sender">The object that triggered the script execution, typically the sender of an event.</param>
/// <param name="parameter">An optional parameter passed to the script, providing additional context or data.</param>
public sealed class ExecuteScriptActionGlobals(object? sender, object? parameter)
{
    /// <summary>
    /// Gets the object that triggered the script execution, typically the sender of an event.
    /// </summary>
    public object? Sender { get; } = sender;

    /// <summary>
    /// Gets an optional parameter passed to the script, providing additional context or data.
    /// </summary>
    public object? Parameter { get; } = parameter;
}
