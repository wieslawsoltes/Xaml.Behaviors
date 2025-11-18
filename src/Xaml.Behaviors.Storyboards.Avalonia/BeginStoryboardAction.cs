// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;

namespace Avalonia.Xaml.Behaviors.Storyboards;

/// <summary>
/// Starts an Avalonia storyboard animation and optionally registers it for interactive control.
/// </summary>
public sealed class BeginStoryboardAction : StoryboardActionBase
{
    /// <summary>
    /// Identifies the <see cref="Storyboard"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<IAnimation?> StoryboardProperty =
        AvaloniaProperty.Register<BeginStoryboardAction, IAnimation?>(nameof(Storyboard));

    /// <summary>
    /// Identifies the <see cref="StoryboardResourceKey"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<object?> StoryboardResourceKeyProperty =
        AvaloniaProperty.Register<BeginStoryboardAction, object?>(nameof(StoryboardResourceKey));

    /// <summary>
    /// Identifies the <see cref="HandoffBehavior"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<StoryboardHandoffBehavior> HandoffBehaviorProperty =
        AvaloniaProperty.Register<BeginStoryboardAction, StoryboardHandoffBehavior>(
            nameof(StoryboardHandoffBehavior),
            defaultValue: StoryboardHandoffBehavior.SnapshotAndReplace);

    /// <summary>
    /// Identifies the <see cref="KeepAlive"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<bool> KeepAliveProperty =
        AvaloniaProperty.Register<BeginStoryboardAction, bool>(nameof(KeepAlive));

    /// <summary>
    /// Gets or sets the inline storyboard instance to run when the action executes.
    /// </summary>
    public IAnimation? Storyboard
    {
        get => GetValue(StoryboardProperty);
        set => SetValue(StoryboardProperty, value);
    }

    /// <summary>
    /// Gets or sets the resource key used when <see cref="Storyboard"/> is null.
    /// </summary>
    public object? StoryboardResourceKey
    {
        get => GetValue(StoryboardResourceKeyProperty);
        set => SetValue(StoryboardResourceKeyProperty, value);
    }

    /// <summary>
    /// Gets or sets the handoff behavior applied when registering the storyboard.
    /// </summary>
    public StoryboardHandoffBehavior HandoffBehavior
    {
        get => GetValue(HandoffBehaviorProperty);
        set => SetValue(HandoffBehaviorProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the storyboard instance should remain in the registry after completion.
    /// </summary>
    public bool KeepAlive
    {
        get => GetValue(KeepAliveProperty);
        set => SetValue(KeepAliveProperty, value);
    }

    /// <inheritdoc />
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Storyboard animations explicitly rely on Avalonia internals.")]
    protected override object? ExecuteCore(in StoryboardActionContext context)
    {
        return ExecuteInternal(context);
    }

    [RequiresUnreferencedCode("Storyboard animation helpers rely on Avalonia's animation internals.")]
    private object? ExecuteInternal(in StoryboardActionContext context)
    {
        if (context.AssociatedElement is not StyledElement host)
        {
            throw new InvalidOperationException("BeginStoryboardAction must be attached to a StyledElement.");
        }

        var storyboard = ResolveStoryboard(context);
        var animatable = ResolveStoryboardTarget(storyboard, context);
        var controller = new StoryboardPlaybackController(storyboard, animatable);
        controller.WatchForCompletion();

        var key = StoryboardKey;
        if (!string.IsNullOrWhiteSpace(key))
        {
            RegisterInstance(context, host, key!.Trim(), controller);
        }
        else
        {
            controller.Completed += (_, _) => controller.Dispose();
        }

        return null;
    }

    private void RegisterInstance(
        in StoryboardActionContext context,
        StyledElement host,
        string key,
        StoryboardPlaybackController controller)
    {
        var registry = context.Registry;
        StoryboardInstance? instance = null;
        var removed = false;

        void RemoveInstance()
        {
            if (instance is not null && !removed)
            {
                removed = true;
                registry.Remove(host, key, instance);
            }
        }

        instance = new StoryboardInstance(
            host,
            key,
            HandoffBehavior,
            KeepAlive,
            pauseAction: controller.Pause,
            resumeAction: controller.Resume,
            stopAction: () =>
            {
                controller.Stop();
                RemoveInstance();
            },
            removeAction: RemoveInstance,
            seekAction: controller.Seek,
            skipToFillAction: controller.SkipToFill,
            setSpeedRatioAction: controller.SetSpeedRatio,
            onDispose: controller.Dispose);

        controller.Completed += (_, _) =>
        {
            if (!KeepAlive)
            {
                RemoveInstance();
            }
        };

        registry.Register(host, key, instance);
    }

    private IAnimation ResolveStoryboard(in StoryboardActionContext context)
    {
        if (Storyboard is IAnimation inline)
        {
            return inline;
        }

        if (StoryboardResourceKey is null)
        {
            throw new InvalidOperationException("BeginStoryboardAction requires either Storyboard or StoryboardResourceKey to be set.");
        }

        if (context.AssociatedObject is IResourceHost resourceHost &&
            resourceHost.TryGetResource(StoryboardResourceKey, out var value) &&
            value is IAnimation hostAnimation)
        {
            return hostAnimation;
        }

        if (context.AssociatedElement is StyledElement styled &&
            styled.TryFindResource(StoryboardResourceKey, out var resolved) &&
            resolved is IAnimation resolvedAnimation)
        {
            return resolvedAnimation;
        }

        throw new InvalidOperationException($"Storyboard resource '{StoryboardResourceKey}' could not be found.");
    }

    [RequiresUnreferencedCode("Storyboard property path resolution relies on reflection.")]
    private Animatable ResolveStoryboardTarget(IAnimation storyboard, in StoryboardActionContext context)
    {
        var storyboardMetadata = storyboard is AvaloniaObject storyboardObject
            ? StoryboardService.GetTargetMetadata(storyboardObject)
            : default;

        var effectiveMetadata = MergeMetadata(storyboardMetadata, context.ActionTarget);
        var target = context.TargetResolver.ResolveTarget(effectiveMetadata);
        var originalTarget = target;

        if (effectiveMetadata.ParsedTargetProperty is { } propertyPath)
        {
            if (target is not AvaloniaObject avaloniaObject)
            {
                throw new InvalidOperationException("Storyboard targets must derive from AvaloniaObject when using TargetProperty.");
            }

            var resolved = StoryboardPropertyPathResolver.Resolve(avaloniaObject, propertyPath);
            ApplyTargetProperty(storyboard, resolved.Property);
            target = ShouldUseResolvedTarget(resolved.Property, originalTarget)
                ? resolved.Target
                : originalTarget;
        }

        if (target is not Animatable animatable)
        {
            throw new InvalidOperationException("Storyboard targets must derive from Animatable.");
        }

        return animatable;
    }

    private static StoryboardTargetMetadata MergeMetadata(
        StoryboardTargetMetadata primary,
        StoryboardTargetMetadata fallback)
    {
        var target = primary.HasTarget ? primary.Target : fallback.Target;
        var targetName = primary.HasTargetName ? primary.TargetName : fallback.TargetName;
        var targetProperty = primary.HasTargetProperty ? primary.TargetProperty : fallback.TargetProperty;
        var parsedTargetProperty = primary.ParsedTargetProperty ?? fallback.ParsedTargetProperty;

        return new StoryboardTargetMetadata(target, targetName, targetProperty, parsedTargetProperty);
    }

    private static void ApplyTargetProperty(IAnimation storyboard, AvaloniaProperty property)
    {
        if (storyboard is not Avalonia.Animation.Animation animation)
        {
            throw new InvalidOperationException("Storyboard.TargetProperty can only be used with Avalonia Animation instances.");
        }

        foreach (var keyFrame in animation.Children)
        {
            if (keyFrame.Setters.Count == 0)
            {
                throw new InvalidOperationException("KeyFrames must declare at least one Setter when Storyboard.TargetProperty is specified.");
            }

            foreach (var setter in keyFrame.Setters)
            {
                if (setter is not Setter styleSetter)
                {
                    throw new InvalidOperationException("Storyboard.TargetProperty currently supports Setter instances only.");
                }

                if (styleSetter.Property is null)
                {
                    styleSetter.Property = property;
                    continue;
                }

                if (!ReferenceEquals(styleSetter.Property, property))
                {
                    throw new InvalidOperationException("Setter.Property conflicts with the resolved Storyboard.TargetProperty.");
                }
            }
        }
    }

    private static bool ShouldUseResolvedTarget(
        AvaloniaProperty property,
        AvaloniaObject originalTarget)
    {
        if (property is null)
        {
            return true;
        }

        var ownerType = property.OwnerType;
        if (ownerType is null)
        {
            return true;
        }

        if (typeof(Transform).IsAssignableFrom(ownerType) && originalTarget is Visual)
        {
            return false;
        }

        return true;
    }
}
