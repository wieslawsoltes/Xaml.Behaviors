// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Xaml.Behaviors.Uno.Headless.XUnit;
using Xaml.Interactions.Custom;
using Xunit;

namespace Xaml.Behaviors.Animations.UnitTests;

public class UnoTransitionOperationsTests
{
    [UnoHeadlessFact]
    public void AddRemoveAndClear_ManageTransitions()
    {
        Border target = new();
        EntranceThemeTransition first = new();
        RepositionThemeTransition second = new();

        Assert.True(TransitionOperations.Add(target, first));
        Assert.True(TransitionOperations.Add(target, second));
        Assert.Equal(2, target.Transitions?.Count);
        Assert.True(TransitionOperations.Remove(target, first));
        Assert.Single(target.Transitions!);
        Assert.True(TransitionOperations.Clear(target));
        Assert.Empty(target.Transitions!);
    }

    [UnoHeadlessFact]
    public void Replace_ReturnsPreviousCollection()
    {
        Border target = new();
        TransitionCollection previous = new();
        TransitionCollection replacement = new();
        target.Transitions = previous;

        TransitionCollection? result = TransitionOperations.Replace(target, replacement);

        Assert.Same(previous, result);
        Assert.Same(replacement, target.Transitions);
    }

    [UnoHeadlessFact]
    public void Operations_ReturnFalseForMissingInputs()
    {
        Assert.False(TransitionOperations.Add(null, null));
        Assert.False(TransitionOperations.Add(new Border(), null));
        Assert.False(TransitionOperations.Remove(null, null));
        Assert.False(TransitionOperations.Remove(new Border(), new EntranceThemeTransition()));
        Assert.False(TransitionOperations.Clear(null));
    }

    [UnoHeadlessFact]
    public void Observe_ReportsCurrentAndReplacementCollectionsUntilDisposed()
    {
        Border target = new();
        TransitionCollection replacement = new();
        TransitionCollection afterDisposal = new();
        List<TransitionCollection?> observed = [];

        using (TransitionOperations.Observe(target, observed.Add))
        {
            target.Transitions = replacement;
#if WINUI
            // Native WinUI raises the change before the new collection can be read: it is reported once applied.
            UnoHeadlessSession.Current.RunJobs();
#endif
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

    [UnoHeadlessFact]
    public void Observe_RejectsMissingArguments()
    {
        Border target = new();

        Assert.Throws<ArgumentNullException>(() => TransitionOperations.Observe(null!, _ => { }));
        Assert.Throws<ArgumentNullException>(() => TransitionOperations.Observe(target, null!));
    }
}
