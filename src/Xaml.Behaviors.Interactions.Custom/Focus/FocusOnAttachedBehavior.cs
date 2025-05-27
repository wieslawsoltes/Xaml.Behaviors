// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Focuses the associated control when it is attached to the visual tree.
/// </summary>
public class FocusOnAttachedBehavior : FocusBehaviorBase
{
    /// <summary>
    /// Called when the behavior is attached.
    /// </summary>
    /// <returns>A disposable used to clean up the operation.</returns>
    protected override System.IDisposable OnAttachedToVisualTreeOverride()
    {
        Focus();

        return DisposableAction.Empty;
    }
}
