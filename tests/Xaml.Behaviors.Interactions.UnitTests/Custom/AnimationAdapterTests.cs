// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Composition;
using Microsoft.UI.Dispatching;
using Xaml.Interactions.Custom;
using Xaml.Interactivity;
#else
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Rendering.Composition;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.Xaml.Interactions.Custom;
using Avalonia.Xaml.Interactivity;
#endif
using Xunit;

#if UNO
namespace Xaml.Interactions.UnitTests.Custom;
#else
namespace Avalonia.Xaml.Interactions.UnitTests.Custom;
#endif

#if UNO
// Uno's dependency object generator requires the declaring types of dependency objects to be partial.
public partial class AnimationAdapterTests
#else
public class AnimationAdapterTests
#endif
{
    private sealed class RecordingAnimationBuilder : IAnimationBuilder
    {
        public int BuildCount { get; private set; }

#if UNO
        public Storyboard? Build(Control control)
        {
            BuildCount++;
            return new Storyboard { Duration = new Duration(TimeSpan.Zero) };
        }
#else
        public Avalonia.Animation.Animation? Build(Control control)
        {
            BuildCount++;
            return new Avalonia.Animation.Animation { Duration = TimeSpan.Zero };
        }
#endif
    }

#if UNO
    private sealed partial class RecordingAction : AvaloniaObject, IAction
#else
    private sealed class RecordingAction : AvaloniaObject, IAction
#endif
    {
        public int ExecutionCount { get; private set; }

        public object Execute(object? sender, object? parameter)
        {
            ExecutionCount++;
            return true;
        }
    }

    [AvaloniaFact]
    public void TransitionsBehavior_AppliesAndRestoresTransitions()
    {
        var original = new Transitions();
        var replacement = new Transitions();
        var target = new Border { Transitions = original };
#if WINUI
        // WinUI Panel cannot be created directly (Uno Platform and Avalonia allow it).
        var panel = new Grid { Children = { target } };
#else
        var panel = new Panel { Children = { target } };
#endif
        var behavior = new TransitionsBehavior();
        behavior.SetValue(TransitionsBehavior.TransitionsSourceProperty, replacement);
        Assert.Same(replacement, behavior.TransitionsSource);
        Interaction.GetBehaviors(target).Add(behavior);
        var window = new Window { Content = panel };

        window.Show();

        Assert.Same(replacement, target.Transitions);

        panel.Children.Remove(target);

        Assert.Same(original, target.Transitions);
        window.Close();
    }

#if !UNO
    // Avalonia runtime XAML loader (AvaloniaRuntimeXamlLoader); Uno Platform compiles XAML at build time.
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
#endif

    [AvaloniaFact]
    public void TransitionsChangedTrigger_ExecutesForSharedTransitionObservation()
    {
        var target = new Border();
        var action = new RecordingAction();
        var trigger = new TransitionsChangedTrigger();
        trigger.Actions!.Add(action);
        Interaction.GetBehaviors(target).Add(trigger);
        Dispatcher.UIThread.RunJobs();
        int initialCount = action.ExecutionCount;

        target.Transitions = new Transitions();
        Dispatcher.UIThread.RunJobs();

        Assert.Equal(initialCount + 1, action.ExecutionCount);
    }

    [AvaloniaFact]
    public void FluidMoveBehavior_AnimatesChangedChildPositionThroughSharedPrimitive()
    {
        var child = new Border { Width = 40d, Height = 40d };
        Canvas.SetLeft(child, 0d);
        var canvas = new Canvas { Width = 200d, Height = 100d, Children = { child } };
        var behavior = new FluidMoveBehavior
        {
            AppliesTo = FluidMoveScope.Children,
            Duration = TimeSpan.Zero
        };
        Interaction.GetBehaviors(canvas).Add(behavior);
        var window = new Window { Content = canvas };
        window.Show();
        Dispatcher.UIThread.RunJobs();

        Canvas.SetLeft(child, 80d);
        Dispatcher.UIThread.RunJobs();

        Assert.IsType<TranslateTransform>(child.RenderTransform);
        window.Close();
    }

    [AvaloniaFact]
    public void ParallaxBehavior_TracksScrollOffsetThroughSharedPrimitive()
    {
        var target = new Border { Width = 100d, Height = 100d };
        Canvas.SetLeft(target, 30d);
        Canvas.SetTop(target, 40d);
        var content = new Canvas { Width = 500d, Height = 1000d, Children = { target } };
        var scrollViewer = new ScrollViewer
        {
            Width = 300d,
            Height = 300d,
            Content = content
        };
        var behavior = new ParallaxBehavior
        {
            SourceScrollViewer = scrollViewer,
            ParallaxRatio = 0.25d
        };
        Interaction.GetBehaviors(target).Add(behavior);
        var window = new Window { Content = scrollViewer };
        window.Show();
        Dispatcher.UIThread.RunJobs();

#if UNO
        // WinUI scrolls with ChangeView (once the extent is measured); composition offsets are relative to the layout
        // position on Uno Skia.
        window.CaptureRenderedFrame();
        scrollViewer.ChangeView(20d, 100d, null, disableAnimation: true);
        window.CaptureRenderedFrame();

        Assert.Equal(
            new System.Numerics.Vector3(5f, 25f, 0f),
            Microsoft.UI.Xaml.Hosting.ElementCompositionPreview.GetElementVisual(target)?.GetLayoutRelativeOffset());
#else
        scrollViewer.Offset = new Vector(20d, 100d);
        Dispatcher.UIThread.RunJobs();

        Assert.Equal(
            new System.Numerics.Vector3(35f, 65f, 0f),
            ElementComposition.GetElementVisual(target)?.Offset);
#endif
        window.Close();
    }

    [AvaloniaFact]
    public void AnimationActions_PreserveTargetsResultsAndBuilderPrecedence()
    {
        var target = new Border();
#if UNO
        var animation = new Storyboard { Duration = new Duration(TimeSpan.Zero) };
#else
        var animation = new Avalonia.Animation.Animation { Duration = TimeSpan.Zero };
#endif

        var start = new StartAnimationAction { Animation = animation };
        Assert.False(Assert.IsType<bool>(start.Execute(null, null)));
        Assert.True(Assert.IsType<bool>(start.Execute(target, null)));

        var begin = new BeginAnimationAction { Animation = animation, TargetControl = target };
        Assert.True(Assert.IsType<bool>(begin.Execute(sender: null, parameter: null)));
        begin.IsEnabled = false;
        Assert.False(Assert.IsType<bool>(begin.Execute(target, null)));

        var builder = new RecordingAnimationBuilder();
        var built = new StartBuiltAnimationAction { AnimationBuilder = builder };
        Assert.True(Assert.IsType<bool>(built.Execute(target, null)));
        Assert.Equal(1, builder.BuildCount);

        built.Animation = animation;
        Assert.True(Assert.IsType<bool>(built.Execute(target, null)));
        Assert.Equal(1, builder.BuildCount);
    }

    [AvaloniaFact]
    public void AnimateOnAttachedBehavior_UsesBuilderWhenAnimationIsMissing()
    {
        var target = new Border();
        var builder = new RecordingAnimationBuilder();
        Interaction.GetBehaviors(target).Add(new AnimateOnAttachedBehavior
        {
            AnimationBuilder = builder
        });
        var window = new Window { Content = target };

        window.Show();
        Dispatcher.UIThread.RunJobs();

        Assert.Equal(1, builder.BuildCount);
        window.Close();
    }

    [AvaloniaFact]
    public void PlayAnimationBehavior_RunsConfiguredAnimationOnAssociatedVisual()
    {
        var target = new Border();
#if UNO
        // A WinUI storyboard animating the opacity to 0.25 and holding the end value.
        var opacity = new DoubleAnimation
        {
            To = 0.25d,
            Duration = new Duration(TimeSpan.Zero),
            FillBehavior = FillBehavior.HoldEnd
        };
        Storyboard.SetTargetProperty(opacity, nameof(UIElement.Opacity));
        var animation = new Storyboard { Children = { opacity } };
#else
        var animation = new Avalonia.Animation.Animation
        {
            Duration = TimeSpan.Zero,
            FillMode = FillMode.Forward,
            Children =
            {
                new KeyFrame
                {
                    Cue = new Cue(1d),
                    Setters = { new Setter(Visual.OpacityProperty, 0.25d) }
                }
            }
        };
#endif
        Interaction.GetBehaviors(target).Add(new PlayAnimationBehavior { Animation = animation });
        var window = new Window { Content = target };

        window.Show();
        Dispatcher.UIThread.RunJobs();

        Assert.Equal(0.25d, target.Opacity);
        window.Close();
    }

    [AvaloniaFact]
    public void AnimationCompletedTrigger_ExecutesImmediatelyWithoutAnimation()
    {
        var target = new Border();
        var action = new RecordingAction();
        var trigger = new AnimationCompletedTrigger();
        trigger.Actions!.Add(action);
        Interaction.GetBehaviors(target).Add(trigger);
        var window = new Window { Content = target };

        window.Show();
        Dispatcher.UIThread.RunJobs();

        Assert.Equal(1, action.ExecutionCount);
        window.Close();
    }

    [AvaloniaFact]
    public async Task RunAnimationTrigger_UsesBuilderAndExecutesAfterCompletion()
    {
        var target = new Border();
        var builder = new RecordingAnimationBuilder();
        var action = new RecordingAction();
        var trigger = new RunAnimationTrigger { AnimationBuilder = builder };
        trigger.Actions!.Add(action);
        Interaction.GetBehaviors(target).Add(trigger);
        var window = new Window { Content = target };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            await Task.Delay(20);
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(1, builder.BuildCount);
            Assert.Equal(1, action.ExecutionCount);
        }
        finally
        {
            window.Close();
        }
    }
}
