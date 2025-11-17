// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using Avalonia;

namespace Avalonia.Xaml.Behaviors.Storyboards;

/// <summary>
/// Provides attached properties that mirror WPF storyboard targeting semantics.
/// </summary>
public static class StoryboardService
{
    /// <summary>
    /// Identifies the <see cref="TargetProperty"/> attached property.
    /// </summary>
    public static readonly AttachedProperty<AvaloniaObject?> TargetProperty =
        AvaloniaProperty.RegisterAttached<AvaloniaObject, AvaloniaObject?>(
            "Target",
            typeof(StoryboardService));

    /// <summary>
    /// Identifies the <see cref="TargetNameProperty"/> attached property.
    /// </summary>
    public static readonly AttachedProperty<string?> TargetNameProperty =
        AvaloniaProperty.RegisterAttached<AvaloniaObject, string?>(
            "TargetName",
            typeof(StoryboardService));

    /// <summary>
    /// Identifies the <see cref="TargetPropertyProperty"/> attached property.
    /// </summary>
    public static readonly AttachedProperty<string?> TargetPropertyProperty =
        AvaloniaProperty.RegisterAttached<AvaloniaObject, string?>(
            "TargetProperty",
            typeof(StoryboardService));

    /// <summary>
    /// Sets the explicit target associated with a storyboard timeline or animation.
    /// </summary>
    /// <param name="element">The element that hosts the storyboard metadata.</param>
    /// <param name="value">The explicit target object.</param>
    public static void SetTarget(AvaloniaObject element, AvaloniaObject? value) =>
        element.SetValue(TargetProperty, value);

    /// <summary>
    /// Gets the explicit target associated with a storyboard timeline or animation.
    /// </summary>
    /// <param name="element">The element that hosts the storyboard metadata.</param>
    /// <returns>The explicit target object, if any.</returns>
    public static AvaloniaObject? GetTarget(AvaloniaObject element) =>
        element.GetValue(TargetProperty);

    /// <summary>
    /// Sets the target name associated with a storyboard timeline or animation.
    /// </summary>
    /// <param name="element">The element that hosts the storyboard metadata.</param>
    /// <param name="value">The target name.</param>
    public static void SetTargetName(AvaloniaObject element, string? value) =>
        element.SetValue(TargetNameProperty, value);

    /// <summary>
    /// Gets the target name associated with a storyboard timeline or animation.
    /// </summary>
    /// <param name="element">The element that hosts the storyboard metadata.</param>
    /// <returns>The target name, if set.</returns>
    public static string? GetTargetName(AvaloniaObject element) =>
        element.GetValue(TargetNameProperty);

    /// <summary>
    /// Sets the target property path associated with a storyboard timeline or animation.
    /// </summary>
    /// <param name="element">The element that hosts the storyboard metadata.</param>
    /// <param name="value">The property path string.</param>
    public static void SetTargetProperty(AvaloniaObject element, string? value) =>
        element.SetValue(TargetPropertyProperty, value);

    /// <summary>
    /// Gets the target property path associated with a storyboard timeline or animation.
    /// </summary>
    /// <param name="element">The element that hosts the storyboard metadata.</param>
    /// <returns>The property path string, if set.</returns>
    public static string? GetTargetProperty(AvaloniaObject element) =>
        element.GetValue(TargetPropertyProperty);

    /// <summary>
    /// Attempts to parse the target property path stored on the specified element.
    /// </summary>
    /// <param name="element">The element that hosts the storyboard metadata.</param>
    /// <param name="propertyPath">The parsed property path.</param>
    /// <returns><c>true</c> if the property path was parsed successfully; otherwise, <c>false</c>.</returns>
    public static bool TryGetTargetPropertyPath(AvaloniaObject element, out StoryboardPropertyPath? propertyPath)
    {
        if (element is null)
        {
            throw new ArgumentNullException(nameof(element));
        }

        var rawPath = GetTargetProperty(element);
        if (string.IsNullOrWhiteSpace(rawPath))
        {
            propertyPath = null;
            return false;
        }

        propertyPath = StoryboardPropertyPath.Parse(rawPath!);
        return true;
    }

    /// <summary>
    /// Collects the storyboard targeting metadata defined on the specified element.
    /// </summary>
    /// <param name="element">The element that hosts the storyboard metadata.</param>
    /// <returns>The aggregated targeting metadata.</returns>
    public static StoryboardTargetMetadata GetTargetMetadata(AvaloniaObject element)
    {
        if (element is null)
        {
            throw new ArgumentNullException(nameof(element));
        }

        StoryboardPropertyPath? propertyPath = null;
        if (TryGetTargetPropertyPath(element, out var parsedPath))
        {
            propertyPath = parsedPath;
        }

        return new StoryboardTargetMetadata(
            GetTarget(element),
            GetTargetName(element),
            GetTargetProperty(element),
            propertyPath);
    }
}

/// <summary>
/// Represents the storyboard targeting metadata resolved from attached properties.
/// </summary>
public readonly struct StoryboardTargetMetadata
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StoryboardTargetMetadata"/> struct.
    /// </summary>
    /// <param name="target">The explicit target, if provided.</param>
    /// <param name="targetName">The target name, if provided.</param>
    /// <param name="targetProperty">The raw target property string.</param>
    /// <param name="parsedTargetProperty">The parsed target property path.</param>
    public StoryboardTargetMetadata(
        AvaloniaObject? target,
        string? targetName,
        string? targetProperty,
        StoryboardPropertyPath? parsedTargetProperty)
    {
        Target = target;
        TargetName = targetName;
        TargetProperty = targetProperty;
        ParsedTargetProperty = parsedTargetProperty;
    }

    /// <summary>
    /// Gets the explicit target, if provided.
    /// </summary>
    public AvaloniaObject? Target { get; }

    /// <summary>
    /// Gets the target name, if provided.
    /// </summary>
    public string? TargetName { get; }

    /// <summary>
    /// Gets the raw target property string.
    /// </summary>
    public string? TargetProperty { get; }

    /// <summary>
    /// Gets the parsed target property path.
    /// </summary>
    public StoryboardPropertyPath? ParsedTargetProperty { get; }

    /// <summary>
    /// Gets a value indicating whether this metadata defines an explicit target.
    /// </summary>
    public bool HasTarget => Target is not null;

    /// <summary>
    /// Gets a value indicating whether this metadata defines a target name.
    /// </summary>
    public bool HasTargetName => !string.IsNullOrWhiteSpace(TargetName);

    /// <summary>
    /// Gets a value indicating whether this metadata defines a target property string.
    /// </summary>
    public bool HasTargetProperty => !string.IsNullOrWhiteSpace(TargetProperty);
}
