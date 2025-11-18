// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Avalonia.Xaml.Behaviors.Storyboards;

/// <summary>
/// Provides contextual information shared by storyboard actions during execution.
/// </summary>
public readonly struct StoryboardActionContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StoryboardActionContext"/> struct.
    /// </summary>
    /// <param name="associatedObject">The object that triggered the action.</param>
    /// <param name="parameter">The parameter supplied by the trigger.</param>
    /// <param name="nameScope">The resolved name scope for target lookups.</param>
    /// <param name="targetResolver">The resolver used to locate storyboard targets.</param>
    /// <param name="actionTarget">The targeting metadata defined on the action itself.</param>
    /// <param name="registry">The shared storyboard registry service.</param>
    public StoryboardActionContext(
        AvaloniaObject associatedObject,
        object? parameter,
        INameScope? nameScope,
        StoryboardTargetResolver targetResolver,
        StoryboardTargetMetadata actionTarget,
        StoryboardRegistryService registry)
    {
        AssociatedObject = associatedObject ?? throw new ArgumentNullException(nameof(associatedObject));
        Parameter = parameter;
        NameScope = nameScope;
        TargetResolver = targetResolver ?? throw new ArgumentNullException(nameof(targetResolver));
        ActionTarget = actionTarget;
        Registry = registry ?? throw new ArgumentNullException(nameof(registry));
    }

    /// <summary>
    /// Gets the object that triggered the action.
    /// </summary>
    public AvaloniaObject AssociatedObject { get; }

    /// <summary>
    /// Gets the action parameter supplied by the trigger.
    /// </summary>
    public object? Parameter { get; }

    /// <summary>
    /// Gets the name scope available to the action.
    /// </summary>
    public INameScope? NameScope { get; }

    /// <summary>
    /// Gets the resolver used to locate storyboard targets.
    /// </summary>
    public StoryboardTargetResolver TargetResolver { get; }

    /// <summary>
    /// Gets the targeting metadata defined directly on the action.
    /// </summary>
    public StoryboardTargetMetadata ActionTarget { get; }

    /// <summary>
    /// Gets the registry service used to store running storyboard instances.
    /// </summary>
    public StoryboardRegistryService Registry { get; }

    /// <summary>
    /// Gets the associated object cast to <see cref="StyledElement"/>, if possible.
    /// </summary>
    public StyledElement? AssociatedElement => AssociatedObject as StyledElement;
}
