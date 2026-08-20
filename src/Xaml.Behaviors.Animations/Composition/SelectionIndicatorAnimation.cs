// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Linq;
using System.Numerics;
using Avalonia.Animation.Easings;
using Avalonia.Controls.Primitives;
using Avalonia.Rendering.Composition;
using Avalonia.Rendering.Composition.Animations;
using Avalonia.VisualTree;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Creates and applies the composition animation used to move a selection indicator between item containers.
/// </summary>
public static class SelectionIndicatorAnimation
{
    private const string IndicatorPartName = "PART_SelectedPipe";

    /// <summary>
    /// Gets the default selection indicator animation duration.
    /// </summary>
    public static TimeSpan DefaultDuration { get; } = TimeSpan.FromMilliseconds(250);

    /// <summary>
    /// Finds the standard selection indicator parts and starts their movement animation.
    /// </summary>
    /// <param name="newSelection">The newly selected item container.</param>
    /// <param name="oldSelection">The previously selected item container.</param>
    /// <param name="duration">The animation duration.</param>
    /// <returns><c>true</c> when composition visuals were available and the animation was installed; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="duration"/> is negative.</exception>
    public static bool TryStart(
        TemplatedControl? newSelection,
        TemplatedControl? oldSelection,
        TimeSpan duration)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(duration, TimeSpan.Zero);

        if (newSelection is null || oldSelection is null)
        {
            return false;
        }

        Visual? newIndicator = newSelection.GetVisualDescendants()
            .FirstOrDefault(visual => visual.Name == IndicatorPartName);
        Visual? oldIndicator = oldSelection.GetVisualDescendants()
            .FirstOrDefault(visual => visual.Name == IndicatorPartName);

        return TryStart(
            newIndicator,
            oldIndicator,
            newSelection,
            oldSelection,
            duration);
    }

    /// <summary>
    /// Starts a selection indicator movement animation using explicitly supplied visuals.
    /// </summary>
    /// <param name="newIndicator">The indicator visual in the newly selected container.</param>
    /// <param name="oldIndicator">The indicator visual in the previously selected container.</param>
    /// <param name="newSelection">The newly selected container visual.</param>
    /// <param name="oldSelection">The previously selected container visual.</param>
    /// <param name="duration">The animation duration.</param>
    /// <returns><c>true</c> when composition visuals were available and the animation was installed; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="duration"/> is negative.</exception>
    public static bool TryStart(
        Visual? newIndicator,
        Visual? oldIndicator,
        Visual? newSelection,
        Visual? oldSelection,
        TimeSpan duration)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(duration, TimeSpan.Zero);

        if (newIndicator is null || oldIndicator is null || newSelection is null || oldSelection is null)
        {
            return false;
        }

        ElementComposition.GetElementVisual(oldIndicator)?.ImplicitAnimations?.Clear();

        CompositionVisual? indicatorVisual = ElementComposition.GetElementVisual(newIndicator);
        CompositionVisual? newSelectionVisual = ElementComposition.GetElementVisual(newSelection);
        CompositionVisual? oldSelectionVisual = ElementComposition.GetElementVisual(oldSelection);
        if (indicatorVisual is null || newSelectionVisual is null || oldSelectionVisual is null)
        {
            return false;
        }

        Vector3D selectionOffset = oldSelectionVisual.Offset - newSelectionVisual.Offset;
        bool isVerticalOffset = selectionOffset.Y != 0f;
        double offset = isVerticalOffset ? selectionOffset.Y : selectionOffset.X;
        Compositor compositor = indicatorVisual.Compositor;
        var springEasing = new SpringEasing();

        var offsetAnimation = compositor.CreateVector3KeyFrameAnimation();
        offsetAnimation.Target = "Offset";
        string expression = (offset > 0d ? "+" : "-") + Math.Abs(offset);
        offsetAnimation.InsertExpressionKeyFrame(
            0f,
            isVerticalOffset
                ? $"Vector3(this.FinalValue.X, this.FinalValue.Y{expression}, 0)"
                : $"Vector3(this.FinalValue.X{expression}, this.FinalValue.Y, 0)");
        offsetAnimation.InsertExpressionKeyFrame(1f, "this.FinalValue");
        offsetAnimation.Duration = duration;

        var scaleAnimation = compositor.CreateVector3KeyFrameAnimation();
        scaleAnimation.Target = "Scale";
        scaleAnimation.InsertKeyFrame(0f, Vector3.One, springEasing);
        scaleAnimation.InsertKeyFrame(
            0.5f,
            new Vector3(
                1f + (!isVerticalOffset ? 0.75f : 0f),
                1f + (isVerticalOffset ? 0.75f : 0f),
                1f),
            springEasing);
        scaleAnimation.InsertKeyFrame(1f, Vector3.One, springEasing);
        scaleAnimation.Duration = duration;

        CompositionAnimationGroup animationGroup = compositor.CreateAnimationGroup();
        animationGroup.Add(offsetAnimation);
        animationGroup.Add(scaleAnimation);

        ImplicitAnimationCollection implicitAnimations = compositor.CreateImplicitAnimationCollection();
        double currentOffset = isVerticalOffset ? indicatorVisual.Offset.Y : indicatorVisual.Offset.X;
        implicitAnimations[currentOffset == 0d ? "Offset" : "Visible"] = animationGroup;
        indicatorVisual.ImplicitAnimations = implicitAnimations;
        return true;
    }
}
