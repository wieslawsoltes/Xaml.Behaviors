// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.UI;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using Windows.Foundation;
using Windows.UI.Input.Preview.Injection;
using Xaml.Behaviors.Uno.Headless;
using Xaml.Behaviors.Uno.Headless.XUnit;
using Xaml.Interactivity;
using Xunit;

namespace Xaml.Interactions.Custom.Animations.UnitTests;

public class CompositionBehaviorsTests
{
    private static UnoHeadlessSession Session => UnoHeadlessSession.Current;

    [UnoHeadlessFact]
    public void Behaviors_HaveTheAvaloniaDefaults()
    {
        Assert.Equal(5d, new TiltEffectBehavior().TiltStrength);
        Assert.Equal(0.5d, new OrbitEffectBehavior().Sensitivity);
        Assert.Equal(0.2d, new ParallaxBehavior().ParallaxRatio);
    }

    [UnoHeadlessFact]
    public async Task ParallaxBehavior_MovesWithTheParentScrollViewer()
    {
        Border target = new() { Width = 50d, Height = 50d };
        ScrollViewer scrollViewer = CreateScrollViewer(target);
        ParallaxBehavior behavior = new() { ParallaxRatio = 0.5d };
        Interaction.GetBehaviors(target).Add(behavior);
        await Session.ShowAsync(scrollViewer);

        Assert.Same(scrollViewer, behavior.SourceScrollViewer);

        scrollViewer.ChangeView(null, 100d, null, disableAnimation: true);

        Visual visual = ElementCompositionPreview.GetElementVisual(target);
        await TestSupport.WaitUntilAsync(() => visual.Offset == new Vector3(0f, 50f, 0f), "the parallax offset is applied");
    }

    [UnoHeadlessFact]
    public async Task ParallaxBehavior_StopsFollowingWhenDetached()
    {
        Border target = new() { Width = 50d, Height = 50d };
        ScrollViewer scrollViewer = CreateScrollViewer(target);
        ParallaxBehavior behavior = new() { ParallaxRatio = 0.5d, SourceScrollViewer = scrollViewer };
        Interaction.GetBehaviors(target).Add(behavior);
        await Session.ShowAsync(scrollViewer);
        Interaction.GetBehaviors(target).Remove(behavior);

        scrollViewer.ChangeView(null, 100d, null, disableAnimation: true);
        await TestSupport.WaitUntilAsync(() => scrollViewer.VerticalOffset == 100d, "the view scrolled");
        await Session.WaitForIdleAsync();

        Assert.Equal(Vector3.Zero, ElementCompositionPreview.GetElementVisual(target).Offset);
    }

    [UnoHeadlessFact]
    public async Task TiltEffectBehavior_TiltsTowardsThePointerAndResetsOnExit()
    {
        InputInjector? injector = InputInjector.TryCreate();
        Assert.SkipWhen(injector is null, "Input injection is not available.");

        Border target = new() { Width = 100d, Height = 80d, Background = new SolidColorBrush(Colors.Red) };
        Interaction.GetBehaviors(target).Add(new TiltEffectBehavior());
        await Session.ShowAsync(new Grid { Children = { target } });
        Visual visual = ElementCompositionPreview.GetElementVisual(target);

        injector!.InitializeTouchInjection(InjectedInputVisualizationMode.None);
        MoveMouse(injector, target, new Point(99d, 40d));
        await Session.WaitForIdleAsync();

        Assert.Equal(new Vector3(50f, 40f, 0f), visual.CenterPoint);
        Assert.True(Vector3.Distance(Vector3.UnitY, visual.RotationAxis) < 0.05f, $"Unexpected axis {visual.RotationAxis}.");
        await TestSupport.WaitUntilAsync(() => visual.RotationAngle > 0.05f, "the element tilted");

        MoveMouse(injector, target, new Point(300d, 300d));
        await TestSupport.WaitUntilAsync(() => visual.RotationAngle == 0f, "the tilt was reset");
    }

    [UnoHeadlessFact]
    public async Task OrbitEffectBehavior_RotatesWhileThePointerIsPressed()
    {
        InputInjector? injector = InputInjector.TryCreate();
        Assert.SkipWhen(injector is null, "Input injection is not available.");

        Border target = new() { Width = 100d, Height = 100d, Background = new SolidColorBrush(Colors.Red) };
        Interaction.GetBehaviors(target).Add(new OrbitEffectBehavior { Sensitivity = 1d });
        await Session.ShowAsync(new Grid { Children = { target } });
        Visual visual = ElementCompositionPreview.GetElementVisual(target);

        injector!.InitializeTouchInjection(InjectedInputVisualizationMode.None);
        MoveMouse(injector, target, new Point(50d, 50d));
        MoveMouseBy(injector, 10, 0);
        await Session.WaitForIdleAsync();
        Assert.Equal(0f, visual.RotationAngle);

        injector.InjectMouseInput([new InjectedInputMouseInfo { MouseOptions = InjectedInputMouseOptions.LeftDown }]);
        MoveMouseBy(injector, 20, 0);
        // A horizontal drag of 20 pixels rotates around the vertical axis by 20 * sensitivity * 0.01 radians.
        await TestSupport.WaitUntilAsync(() => MathF.Abs(visual.RotationAngle - 0.2f) < 0.0001f, "the element rotated");
        TestAxis(Vector3.UnitY, visual.RotationAxis);

        injector.InjectMouseInput([new InjectedInputMouseInfo { MouseOptions = InjectedInputMouseOptions.LeftUp }]);
        MoveMouseBy(injector, -20, 0);
        await Session.WaitForIdleAsync();
        await Task.Delay(20);
        Assert.Equal(0.2f, visual.RotationAngle, 4);
    }

    private static ScrollViewer CreateScrollViewer(UIElement target)
    {
        // The headless test application has no theme resources: give the scroll viewer a minimal template.
        ScrollViewer scrollViewer = (ScrollViewer)XamlReader.Load(
            """
            <ScrollViewer xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                          xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                          Height="100">
              <ScrollViewer.Template>
                <ControlTemplate TargetType="ScrollViewer">
                  <ScrollContentPresenter x:Name="ScrollContentPresenter" />
                </ControlTemplate>
              </ScrollViewer.Template>
            </ScrollViewer>
            """);
        scrollViewer.Content = new StackPanel { Children = { target, new Border { Height = 1_000d } } };
        return scrollViewer;
    }

    private static void TestAxis(Vector3 expected, Vector3 actual)
        => Assert.True(Vector3.Distance(expected, actual) < 0.05f, $"Expected axis {expected} but was {actual}.");

    // Uno Platform applies absolute moves relative to the current position while a button is pressed:
    // position the pointer once, then move it by deltas.
    private static void MoveMouseBy(InputInjector injector, int deltaX, int deltaY)
        => injector.InjectMouseInput([new InjectedInputMouseInfo
        {
            DeltaX = deltaX,
            DeltaY = deltaY,
            MouseOptions = InjectedInputMouseOptions.Move,
        }]);

    private static void MoveMouse(InputInjector injector, UIElement relativeTo, Point point)
    {
        Point position = relativeTo.TransformToVisual(null).TransformPoint(point);
        injector.InjectMouseInput([new InjectedInputMouseInfo
        {
            DeltaX = (int)position.X,
            DeltaY = (int)position.Y,
            MouseOptions = InjectedInputMouseOptions.Absolute | InjectedInputMouseOptions.Move,
        }]);
    }
}
