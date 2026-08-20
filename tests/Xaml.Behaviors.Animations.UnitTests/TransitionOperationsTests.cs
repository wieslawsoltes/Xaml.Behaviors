// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Xaml.Interactions.Custom;
using Xunit;

namespace Xaml.Behaviors.Animations.UnitTests;

public class TransitionOperationsTests
{
    [AvaloniaFact]
    public void AddRemoveAndClear_ManageTransitions()
    {
        var target = new Border();
        var first = new DoubleTransition { Property = Visual.OpacityProperty };
        var second = new DoubleTransition { Property = Border.WidthProperty };

        Assert.True(TransitionOperations.Add(target, first));
        Assert.True(TransitionOperations.Add(target, second));
        Assert.Equal(2, target.Transitions?.Count);
        Assert.True(TransitionOperations.Remove(target, first));
        Assert.Single(target.Transitions!);
        Assert.True(TransitionOperations.Clear(target));
        Assert.Empty(target.Transitions!);
    }

    [AvaloniaFact]
    public void Replace_ReturnsPreviousCollection()
    {
        var target = new Border();
        var previous = new Transitions();
        var replacement = new Transitions();
        target.Transitions = previous;

        Transitions? result = TransitionOperations.Replace(target, replacement);

        Assert.Same(previous, result);
        Assert.Same(replacement, target.Transitions);
    }

    [AvaloniaFact]
    public void Operations_ReturnFalseForMissingInputs()
    {
        Assert.False(TransitionOperations.Add(null, null));
        Assert.False(TransitionOperations.Remove(null, null));
        Assert.False(TransitionOperations.Clear(null));
    }
}
