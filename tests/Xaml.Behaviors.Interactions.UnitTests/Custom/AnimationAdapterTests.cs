// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Diagnostics.CodeAnalysis;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Markup.Xaml;
using Avalonia.Xaml.Interactions.Custom;
using Avalonia.Xaml.Interactivity;
using Xunit;

namespace Avalonia.Xaml.Interactions.UnitTests.Custom;

public class AnimationAdapterTests
{
    [AvaloniaFact]
    public void TransitionsBehavior_AppliesAndRestoresTransitions()
    {
        var original = new Transitions();
        var replacement = new Transitions();
        var target = new Border { Transitions = original };
        var panel = new Panel { Children = { target } };
        var behavior = new TransitionsBehavior { TransitionsSource = replacement };
        Interaction.GetBehaviors(target).Add(behavior);
        var window = new Window { Content = panel };

        window.Show();

        Assert.Same(replacement, target.Transitions);

        panel.Children.Remove(target);

        Assert.Same(original, target.Transitions);
        window.Close();
    }

    [AvaloniaFact]
    [UnconditionalSuppressMessage(
        "Trimming",
        "IL2026",
        Justification = "This test intentionally exercises Avalonia's runtime XAML compiler.")]
    public void RuntimeLoader_ResolvesStandaloneAnimationsAndBehaviorAdapters()
    {
        const string xaml = """
            <StackPanel
                xmlns="https://github.com/avaloniaui"
                xmlns:i="clr-namespace:Avalonia.Xaml.Interactivity;assembly=Xaml.Behaviors.Interactivity"
                xmlns:animations="clr-namespace:Avalonia.Xaml.Interactions.Custom;assembly=Xaml.Behaviors.Animations"
                xmlns:legacy="clr-namespace:Avalonia.Xaml.Interactions.Custom;assembly=Xaml.Behaviors.Interactions.Custom">
              <Border animations:EntranceAnimations.FadeIn="0" />
              <Border legacy:EntranceAnimations.FadeIn="0">
                <i:Interaction.Behaviors>
                  <legacy:FadeInBehavior InitialDelay="0:0:0" Duration="0:0:0" />
                </i:Interaction.Behaviors>
              </Border>
            </StackPanel>
            """;

        object result = AvaloniaRuntimeXamlLoader.Load(
            xaml,
            typeof(AnimationAdapterTests).Assembly,
            designMode: true);

        var panel = Assert.IsType<StackPanel>(result);
        Assert.Equal(2, panel.Children.Count);
        var border = Assert.IsType<Border>(panel.Children[1]);
        Assert.IsType<FadeInBehavior>(Assert.Single(Interaction.GetBehaviors(border)));
    }
}
