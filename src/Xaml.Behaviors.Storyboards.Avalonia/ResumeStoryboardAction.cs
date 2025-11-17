// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace Avalonia.Xaml.Behaviors.Storyboards;

/// <summary>
/// Resumes a paused storyboard stored in the registry.
/// </summary>
public sealed class ResumeStoryboardAction : StoryboardRegistryActionBase
{
    /// <inheritdoc />
    protected override object? ExecuteCore(in StoryboardActionContext context)
    {
        if (!TryGetInstance(context, out var instance))
        {
            return null;
        }

        instance!.Resume();
        return null;
    }
}
