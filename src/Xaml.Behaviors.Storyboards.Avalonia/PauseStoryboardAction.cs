// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace Avalonia.Xaml.Behaviors.Storyboards;

/// <summary>
/// Pauses a running storyboard stored in the registry.
/// </summary>
public sealed class PauseStoryboardAction : StoryboardRegistryActionBase
{
    /// <inheritdoc />
    protected override object? ExecuteCore(in StoryboardActionContext context)
    {
        if (!TryGetInstance(context, out var instance))
        {
            return null;
        }

        instance!.Pause();
        return null;
    }
}
