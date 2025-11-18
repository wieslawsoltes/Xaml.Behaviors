// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace Avalonia.Xaml.Behaviors.Storyboards;

/// <summary>
/// Skips the storyboard to its fill position.
/// </summary>
public sealed class SkipToFillStoryboardAction : StoryboardRegistryActionBase
{
    /// <inheritdoc />
    protected override object? ExecuteCore(in StoryboardActionContext context)
    {
        if (!TryGetInstance(context, out var instance))
        {
            return null;
        }

        instance!.SkipToFill();
        return null;
    }
}
