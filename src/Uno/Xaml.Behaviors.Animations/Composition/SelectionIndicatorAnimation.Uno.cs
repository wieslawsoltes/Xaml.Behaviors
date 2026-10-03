// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Numerics;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Hosting;

namespace Xaml.Interactions.Custom;

/// <summary>
/// Uno Platform implementation of the selection indicator movement.
/// </summary>
public static partial class SelectionIndicatorAnimation
{
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

    private static bool StartIndicatorAnimation(
        CompositionVisual indicatorVisual,
        UIElement newIndicator,
        UIElement newSelection,
        UIElement oldSelection,
        TimeSpan duration)
    {
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
    }
}
