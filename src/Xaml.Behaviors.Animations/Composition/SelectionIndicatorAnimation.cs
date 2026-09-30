// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Linq;
using System.Numerics;
#if UNO
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Media;
#else
using Avalonia.Animation.Easings;
using Avalonia.Controls.Primitives;
using Avalonia.Rendering.Composition;
using Avalonia.Rendering.Composition.Animations;
using Avalonia.VisualTree;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

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

        if (duration == TimeSpan.Zero ||
            newIndicator is null || oldIndicator is null || newSelection is null || oldSelection is null)
        {
            return false;
        }

#if UNO
        StopIndicatorAnimations(ElementComposition.GetElementVisual(oldIndicator));
#else
        ElementComposition.GetElementVisual(oldIndicator)?.ImplicitAnimations?.Clear();
#endif

        CompositionVisual? indicatorVisual = ElementComposition.GetElementVisual(newIndicator);
        CompositionVisual? newSelectionVisual = ElementComposition.GetElementVisual(newSelection);
        CompositionVisual? oldSelectionVisual = ElementComposition.GetElementVisual(oldSelection);
        if (indicatorVisual is null || newSelectionVisual is null || oldSelectionVisual is null)
        {
            return false;
        }

#if UNO
        // Uno Platform (Skia) implements neither implicit animations nor animation groups, and WinUI composition
        // offsets are relative to the arranged position: the movement from the previous container is started
        // explicitly on the indicator translation, measured between the containers in the visual tree.
        Windows.Foundation.Point oldOrigin = oldSelection.TransformToVisual(newSelection)
            .TransformPoint(default(Windows.Foundation.Point));
        Vector3 selectionOffset = new((float)oldOrigin.X, (float)oldOrigin.Y, 0f);
        bool isVerticalOffset = selectionOffset.Y != 0f;
        Compositor compositor = indicatorVisual.Compositor;
        // Overshooting cubic bezier in place of Avalonia's SpringEasing (no spring easing for key frames on WinUI).
        CubicBezierEasingFunction springEasing = compositor.CreateCubicBezierEasingFunction(
            new Vector2(0.34f, 1.56f),
            new Vector2(0.64f, 1f));

        StopIndicatorAnimations(indicatorVisual);
        ElementCompositionPreview.SetIsTranslationEnabled(newIndicator, true);

        Vector3KeyFrameAnimation translationAnimation = compositor.CreateVector3KeyFrameAnimation();
        translationAnimation.InsertKeyFrame(
            0f,
            isVerticalOffset ? new Vector3(0f, selectionOffset.Y, 0f) : new Vector3(selectionOffset.X, 0f, 0f));
        translationAnimation.InsertKeyFrame(1f, Vector3.Zero);
        translationAnimation.Duration = duration;

        Vector3KeyFrameAnimation scaleAnimation = compositor.CreateVector3KeyFrameAnimation();
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

        indicatorVisual.StartAnimation(TranslationProperty, translationAnimation);
        indicatorVisual.StartAnimation(ScaleProperty, scaleAnimation);
        return true;
#else
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
#endif
    }
#if UNO

    private const string TranslationProperty = "Translation";
    private const string ScaleProperty = "Scale";

    private static void StopIndicatorAnimations(CompositionVisual? visual)
    {
        if (visual is null)
        {
            return;
        }

        visual.StopAnimation(TranslationProperty);
        visual.StopAnimation(ScaleProperty);
        visual.Scale = Vector3.One;
        visual.Properties.InsertVector3(TranslationProperty, Vector3.Zero);
    }
#endif
}
