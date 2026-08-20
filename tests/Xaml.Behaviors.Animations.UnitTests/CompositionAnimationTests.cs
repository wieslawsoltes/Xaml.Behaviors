// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Numerics;
using Avalonia;
using Avalonia.Headless.XUnit;
using Avalonia.Xaml.Interactions.Custom;
using Xunit;

namespace Xaml.Behaviors.Animations.UnitTests;

public class CompositionAnimationTests
{
    [AvaloniaFact]
    public void ParallaxAnimation_CalculatesProportionalOffset()
    {
        Vector3 offset = ParallaxAnimation.CalculateOffset(new Avalonia.Vector(20d, 50d), 0.25d);

        Assert.Equal(new Vector3(5f, 12.5f, 0f), offset);
    }

    [AvaloniaFact]
    public void OrbitAnimation_CalculatesNormalizedOrientation()
    {
        Quaternion orientation = OrbitAnimation.CalculateOrientation(
            Quaternion.Identity,
            new Avalonia.Vector(10d, 5d),
            0.5d);

        Assert.NotEqual(Quaternion.Identity, orientation);
        Assert.InRange(orientation.Length(), 0.9999f, 1.0001f);
    }

    [AvaloniaFact]
    public void TiltAnimation_ReturnsIdentityAtCenterAndForEmptySize()
    {
        Quaternion center = TiltAnimation.CalculateOrientation(new Size(100d, 80d), new Point(50d, 40d), 5d);
        Quaternion empty = TiltAnimation.CalculateOrientation(default, new Point(10d, 10d), 5d);

        Assert.Equal(Quaternion.Identity, center);
        Assert.Equal(Quaternion.Identity, empty);
    }

    [AvaloniaFact]
    public void TiltAnimation_CalculatesOrientationAwayFromCenter()
    {
        Quaternion orientation = TiltAnimation.CalculateOrientation(
            new Size(100d, 100d),
            new Point(100d, 50d),
            5d);

        Assert.NotEqual(Quaternion.Identity, orientation);
    }

    [AvaloniaFact]
    public void TiltAnimation_DoesNotCalculateOrientationAtCenterOrForEmptySize()
    {
        bool center = TiltAnimation.TryCalculateOrientation(
            new Size(100d, 80d),
            new Point(50d, 40d),
            5d,
            out Quaternion centerOrientation);
        bool empty = TiltAnimation.TryCalculateOrientation(
            default,
            new Point(10d, 10d),
            5d,
            out Quaternion emptyOrientation);

        Assert.False(center);
        Assert.False(empty);
        Assert.Equal(Quaternion.Identity, centerOrientation);
        Assert.Equal(Quaternion.Identity, emptyOrientation);
    }

    [AvaloniaFact]
    public void CompositionEffects_ReturnFalseWithoutTargetVisuals()
    {
        var orbit = new OrbitAnimation();

        Assert.False(ParallaxAnimation.Apply(null, default, 0.25d));
        Assert.False(orbit.Rotate(null, new Avalonia.Vector(10d, 5d), 0.5d));
        Assert.False(TiltAnimation.Apply(null, default, 5d));
        Assert.False(TiltAnimation.Reset(null));
        Assert.Equal(Quaternion.Identity, orbit.Orientation);
    }
}
