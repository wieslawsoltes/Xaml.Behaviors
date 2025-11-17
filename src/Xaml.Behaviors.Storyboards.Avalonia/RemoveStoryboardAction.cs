// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace Avalonia.Xaml.Behaviors.Storyboards;

/// <summary>
/// Removes a storyboard registration without altering its playback state.
/// </summary>
public sealed class RemoveStoryboardAction : StoryboardRegistryActionBase
{
    /// <inheritdoc />
    protected override object? ExecuteCore(in StoryboardActionContext context)
    {
        if (!TryGetInstance(context, out var instance))
        {
            return null;
        }

        instance!.Remove();
        return null;
    }
}
