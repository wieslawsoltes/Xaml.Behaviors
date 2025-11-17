// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Styling;

namespace Avalonia.Xaml.Behaviors.Storyboards;

/// <summary>
/// Resolves storyboard targets using the same precedence order as WPF (explicit target → name scope → associated object).
/// </summary>
public sealed class StoryboardTargetResolver
{
    private readonly AvaloniaObject? _associatedObject;
    private readonly AvaloniaObject? _defaultTarget;
    private readonly INameScope? _nameScope;
    private readonly bool _isStyleContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="StoryboardTargetResolver"/> class.
    /// </summary>
    /// <param name="associatedObject">The object that hosts the storyboard (usually the behavior's associated object).</param>
    /// <param name="defaultTarget">An optional default target when no explicit target metadata is provided.</param>
    /// <param name="nameScope">An optional name scope (e.g. template scope) that should be queried before walking the logical tree.</param>
    /// <param name="isStyleContext">Indicates whether the resolver operates inside a style, where target names are prohibited.</param>
    public StoryboardTargetResolver(
        AvaloniaObject? associatedObject,
        AvaloniaObject? defaultTarget = null,
        INameScope? nameScope = null,
        bool isStyleContext = false)
    {
        _associatedObject = associatedObject;
        _defaultTarget = defaultTarget ?? associatedObject;
        _nameScope = nameScope;
        _isStyleContext = isStyleContext;
    }

    /// <summary>
    /// Resolves a storyboard target using metadata collected from <see cref="StoryboardService"/>.
    /// </summary>
    /// <param name="metadata">The targeting metadata.</param>
    /// <returns>The resolved target.</returns>
    public AvaloniaObject ResolveTarget(StoryboardTargetMetadata metadata) =>
        ResolveTarget(metadata.Target, metadata.TargetName);

    /// <summary>
    /// Resolves a storyboard target using explicit target values.
    /// </summary>
    /// <param name="target">The explicit target object.</param>
    /// <param name="targetName">The target name.</param>
    /// <returns>The resolved target.</returns>
    public AvaloniaObject ResolveTarget(AvaloniaObject? target, string? targetName)
    {
        if (target is not null)
        {
            return target;
        }

        if (!string.IsNullOrWhiteSpace(targetName))
        {
            EnsureTargetNamesAllowed();

            var resolved = ResolveByName(targetName!);
            if (resolved is not null)
            {
                return resolved;
            }

            throw new InvalidOperationException($"Storyboard target named '{targetName}' could not be found in the current name scope.");
        }

        if (_defaultTarget is not null)
        {
            return _defaultTarget;
        }

        throw new InvalidOperationException("Storyboard target could not be resolved because no Target or TargetName was specified and no associated object was supplied.");
    }

    private void EnsureTargetNamesAllowed()
    {
        if (_isStyleContext)
        {
            throw new InvalidOperationException("Storyboard.TargetName cannot be used within a Style. Only the styled element may be targeted.");
        }
    }

    private AvaloniaObject? ResolveByName(string targetName)
    {
        targetName = targetName.Trim();
        if (targetName.Length == 0)
        {
            return null;
        }

        if (_nameScope?.Find(targetName) is AvaloniaObject scopedTarget)
        {
            return scopedTarget;
        }

        if (_associatedObject is INameScope localScope &&
            localScope.Find(targetName) is AvaloniaObject localTarget)
        {
            return localTarget;
        }

        if (_associatedObject is AvaloniaObject owner)
        {
            return FindInLogicalAncestors(owner, targetName);
        }

        return null;
    }

    private static AvaloniaObject? FindInLogicalAncestors(AvaloniaObject owner, string targetName)
    {
        if (owner is not ILogical logical)
        {
            return null;
        }

        foreach (var node in logical.GetSelfAndLogicalAncestors())
        {
            if (node is not StyledElement styledElement)
            {
                continue;
            }

            if (NameScope.GetNameScope(styledElement) is { } scope &&
                scope.Find(targetName) is AvaloniaObject found)
            {
                return found;
            }
        }

        return null;
    }
}
