// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactions.Custom;
#else
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Xaml.Interactions.Custom;
#endif
using Xunit;

namespace Xaml.Behaviors.Animations.UnitTests;

public class TransitionOperationsTests
{
    [AvaloniaFact]
    public void AddRemoveAndClear_ManageTransitions()
    {
        var target = new Border();
#if UNO
        // WinUI has no property transitions (DoubleTransition); its transitions are theme transitions.
        var first = new EntranceThemeTransition();
        var second = new RepositionThemeTransition();
#else
        var first = new DoubleTransition { Property = Visual.OpacityProperty };
        var second = new DoubleTransition { Property = Border.WidthProperty };
#endif

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

    [AvaloniaFact]
    public void Observe_ReportsCurrentAndReplacementCollectionsUntilDisposed()
    {
        var target = new Border();
        var replacement = new Transitions();
        var afterDisposal = new Transitions();
        var observed = new List<Transitions?>();

        using (TransitionOperations.Observe(target, observed.Add))
        {
            target.Transitions = replacement;
        }

        target.Transitions = afterDisposal;

        Assert.Collection(
            observed,
            #if WINUI
            // Native WinUI creates an empty transition collection on first access (Uno Platform returns null).
            transitions => Assert.True(transitions is null || transitions.Count == 0),
#else
            transitions => Assert.Null(transitions),
#endif
            transitions => Assert.Same(replacement, transitions));
    }

    [AvaloniaFact]
    public void Observe_RejectsMissingArguments()
    {
        var target = new Border();

        Assert.Throws<ArgumentNullException>(() => TransitionOperations.Observe(null!, _ => { }));
        Assert.Throws<ArgumentNullException>(() => TransitionOperations.Observe(target, null!));
    }
}
