// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Behaviors.Storyboards;

/// <summary>
/// Provides shared targeting and registry plumbing for storyboard-oriented actions.
/// </summary>
public abstract class StoryboardActionBase : AvaloniaObject, IAction
{
    /// <summary>
    /// Identifies the <see cref="StoryboardKey"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> StoryboardKeyProperty =
        AvaloniaProperty.Register<StoryboardActionBase, string?>(nameof(StoryboardKey));

    /// <summary>
    /// Identifies the <see cref="Target"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<AvaloniaObject?> TargetObjectProperty =
        AvaloniaProperty.Register<StoryboardActionBase, AvaloniaObject?>(nameof(Target));

    /// <summary>
    /// Identifies the <see cref="TargetName"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> TargetNameProperty =
        AvaloniaProperty.Register<StoryboardActionBase, string?>(nameof(TargetName));

    /// <summary>
    /// Identifies the <see cref="TargetProperty"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> TargetPropertyProperty =
        AvaloniaProperty.Register<StoryboardActionBase, string?>(nameof(TargetProperty));

    /// <summary>
    /// Gets or sets the registry key used to store a running storyboard.
    /// </summary>
    public string? StoryboardKey
    {
        get => GetValue(StoryboardKeyProperty);
        set => SetValue(StoryboardKeyProperty, value);
    }

    /// <summary>
    /// Gets or sets an explicit storyboard target instance.
    /// </summary>
    public AvaloniaObject? Target
    {
        get => GetValue(TargetObjectProperty);
        set => SetValue(TargetObjectProperty, value);
    }

    /// <summary>
    /// Gets or sets the storyboard target name.
    /// </summary>
    public string? TargetName
    {
        get => GetValue(TargetNameProperty);
        set => SetValue(TargetNameProperty, value);
    }

    /// <summary>
    /// Gets or sets the storyboard target property path.
    /// </summary>
    public string? TargetProperty
    {
        get => GetValue(TargetPropertyProperty);
        set => SetValue(TargetPropertyProperty, value);
    }

    /// <inheritdoc />
    public object? Execute(object? sender, object? parameter)
    {
        if (sender is null)
        {
            throw new ArgumentNullException(nameof(sender));
        }

        if (sender is not AvaloniaObject associatedObject)
        {
            throw new InvalidOperationException("Storyboard actions must be invoked with an AvaloniaObject sender.");
        }

        var context = CreateContext(associatedObject, parameter);
        return ExecuteCore(context);
    }

    /// <summary>
    /// Executes the action-specific logic.
    /// </summary>
    /// <param name="context">The computed action context.</param>
    /// <returns>The action result.</returns>
    protected abstract object? ExecuteCore(in StoryboardActionContext context);

    /// <summary>
    /// Gets the registry service used to store storyboard instances.
    /// </summary>
    protected virtual StoryboardRegistryService Registry => StoryboardRegistryService.Instance;

    private StoryboardActionContext CreateContext(AvaloniaObject associatedObject, object? parameter)
    {
        var styledElement = associatedObject as StyledElement;
        var nameScope = styledElement is not null ? NameScope.GetNameScope(styledElement) : null;
        var isStyleContext = associatedObject is Style;
        var defaultTarget = Target ?? styledElement ?? associatedObject;

        var resolver = new StoryboardTargetResolver(
            styledElement ?? associatedObject,
            defaultTarget,
            nameScope,
            isStyleContext);

        var metadata = CreateTargetMetadata();

        return new StoryboardActionContext(
            associatedObject,
            parameter,
            nameScope,
            resolver,
            metadata,
            Registry);
    }

    private StoryboardTargetMetadata CreateTargetMetadata()
    {
        StoryboardPropertyPath? propertyPath = null;
        if (!string.IsNullOrWhiteSpace(TargetProperty))
        {
            propertyPath = StoryboardPropertyPath.Parse(TargetProperty!);
        }

        return new StoryboardTargetMetadata(
            Target,
            TargetName,
            TargetProperty,
            propertyPath);
    }
}
