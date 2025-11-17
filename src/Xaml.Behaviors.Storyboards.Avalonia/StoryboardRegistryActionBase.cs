// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using Avalonia.Controls;
using Avalonia.LogicalTree;

namespace Avalonia.Xaml.Behaviors.Storyboards;

/// <summary>
/// Base class for storyboard control actions that operate on registered instances.
/// </summary>
public abstract class StoryboardRegistryActionBase : StoryboardActionBase
{
    /// <summary>
    /// Resolves the storyboard instance using the current host and <see cref="StoryboardActionBase.StoryboardKey"/>.
    /// </summary>
    /// <param name="context">The execution context.</param>
    /// <returns>The resolved instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the host or key cannot be resolved.</exception>
    protected StoryboardInstance GetRequiredInstance(in StoryboardActionContext context)
    {
        if (context.AssociatedElement is not StyledElement host)
        {
            throw new InvalidOperationException("Storyboard control actions must be attached to a StyledElement.");
        }

        var key = StoryboardKey;
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new InvalidOperationException("StoryboardKey must be specified when using interactive storyboard actions.");
        }

        var normalizedKey = key.Trim();

        if (TryResolveInstance(host, normalizedKey, context.Registry, out var instance))
        {
            return instance!;
        }

        throw new InvalidOperationException($"No storyboard with key '{normalizedKey}' is currently registered on '{host.Name ?? host.ToString()}'.");
    }

    /// <summary>
    /// Attempts to resolve a storyboard instance without throwing when it cannot be found.
    /// </summary>
    /// <param name="context">The execution context.</param>
    /// <param name="instance">When successful, the resolved instance.</param>
    /// <returns><c>true</c> if an instance was found; otherwise, <c>false</c>.</returns>
    protected bool TryGetInstance(in StoryboardActionContext context, out StoryboardInstance? instance)
    {
        instance = null;

        if (context.AssociatedElement is not StyledElement host)
        {
            return false;
        }

        var key = StoryboardKey;
        if (string.IsNullOrWhiteSpace(key))
        {
            return false;
        }

        return TryResolveInstance(host, key.Trim(), context.Registry, out instance);
    }

    private static bool TryResolveInstance(
        StyledElement host,
        string normalizedKey,
        StoryboardRegistryService registry,
        out StoryboardInstance? instance)
    {
        foreach (var candidate in host.GetSelfAndLogicalAncestors())
        {
            if (candidate is not StyledElement styledCandidate)
            {
                continue;
            }

            if (registry.TryGet(styledCandidate, normalizedKey, out instance) && instance is not null)
            {
                return true;
            }
        }

        instance = null;
        return false;
    }
}
