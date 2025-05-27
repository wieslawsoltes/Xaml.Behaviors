// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
namespace Avalonia.Xaml.Interactivity;

/// <summary>
/// Represents an <see cref="System.IDisposable"/> that executes a specified action when disposed.
/// </summary>
/// <param name="dispose">The action to execute when disposed.</param>
public class DisposableAction(System.Action dispose) : System.IDisposable
{
    /// <summary>
    /// An empty disposable that does nothing when disposed.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static readonly System.IDisposable Empty = new DisposableAction(() => { });

    /// <summary>
    /// Creates a new <see cref="DisposableAction"/> that executes the specified action when disposed.
    /// </summary>
    /// <param name="dispose">The action to execute when disposed.</param>
    /// <returns>A new instance of <see cref="DisposableAction"/>.</returns>
    public static System.IDisposable Create(System.Action dispose) 
        => new DisposableAction(dispose);

    /// <summary>
    /// Executes the dispose action.
    /// </summary>
    void System.IDisposable.Dispose() => dispose();
}
