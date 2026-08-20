// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Xaml.Interactions.Custom;
using Xunit;

namespace Xaml.Behaviors.Animations.UnitTests;

public class SelectingItemsControlAnimationTests
{
    [AvaloniaFact]
    public void EnableSelectionAnimation_AttachedPropertyRoundTrips()
    {
        var target = new ListBox();

        SelectingItemsControlBehavior.SetEnableSelectionAnimation(target, true);

        Assert.True(SelectingItemsControlBehavior.GetEnableSelectionAnimation(target));
    }
}
