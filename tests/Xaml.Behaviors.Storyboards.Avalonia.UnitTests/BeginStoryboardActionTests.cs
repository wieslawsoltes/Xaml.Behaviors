using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using Xunit;
using AnimationTimeline = Avalonia.Animation.Animation;

namespace Avalonia.Xaml.Behaviors.Storyboards.UnitTests;

public class BeginStoryboardActionTests
{
    [AvaloniaFact]
    public async Task BeginStoryboardAction_RegistersInstanceForKey()
    {
        var host = new Border();
        var animation = CreateOpacityAnimation(TimeSpan.FromMilliseconds(50));
        var action = new BeginStoryboardAction
        {
            StoryboardKey = "Fade",
            Storyboard = animation
        };

        await RunOnUiAsync(() => action.Execute(host, parameter: null));

        var registry = StoryboardRegistryService.Instance;
        Assert.True(registry.TryGet(host, "Fade", out var instance));
        Assert.NotNull(instance);

        registry.CleanupHost(host);
    }

    [AvaloniaFact]
    public async Task PauseAndResumeStoryboardAction_ControlProgress()
    {
        var host = new Border();
        var action = new BeginStoryboardAction
        {
            StoryboardKey = "Fade",
            Storyboard = CreateOpacityAnimation(TimeSpan.FromSeconds(1))
        };

        await RunOnUiAsync(() => action.Execute(host, null));

        await Task.Delay(150);
        var beforePause = await GetOpacityAsync(host);

        await RunOnUiAsync(() =>
            new PauseStoryboardAction { StoryboardKey = "Fade" }.Execute(host, null));

        await Task.Delay(250);
        var pausedValue = await GetOpacityAsync(host);

        Assert.InRange(Math.Abs(pausedValue - beforePause), 0d, 0.02d);

        await RunOnUiAsync(() =>
            new ResumeStoryboardAction { StoryboardKey = "Fade" }.Execute(host, null));

        await Task.Delay(350);
        var afterResume = await GetOpacityAsync(host);
        Assert.True(afterResume > pausedValue);

        StoryboardRegistryService.Instance.CleanupHost(host);
    }

    [AvaloniaFact]
    public async Task BeginStoryboardAction_ComposeHandoff_RetainsPreviousInstance()
    {
        var host = new Border();
        var registry = StoryboardRegistryService.Instance;
        registry.CleanupHost(host);

        var first = new BeginStoryboardAction
        {
            StoryboardKey = "Fade",
            Storyboard = CreateOpacityAnimation(TimeSpan.FromMilliseconds(200))
        };

        var second = new BeginStoryboardAction
        {
            StoryboardKey = "Fade",
            Storyboard = CreateOpacityAnimation(TimeSpan.FromMilliseconds(200)),
            HandoffBehavior = StoryboardHandoffBehavior.Compose
        };

        await RunOnUiAsync(() => first.Execute(host, null));
        Assert.True(registry.TryGet(host, "Fade", out var firstInstance));

        await RunOnUiAsync(() => second.Execute(host, null));
        Assert.True(registry.TryGet(host, "Fade", out var secondInstance));
        Assert.NotSame(firstInstance, secondInstance);

        registry.Remove(host, "Fade", secondInstance);
        Assert.True(registry.TryGet(host, "Fade", out var remaining));
        Assert.Same(firstInstance, remaining);

        registry.CleanupHost(host);
    }

    [AvaloniaFact]
    public async Task BeginStoryboardAction_RemovesInstanceWhenAnimationCompletes()
    {
        var host = new Border();
        var action = new BeginStoryboardAction
        {
            StoryboardKey = "Fade",
            Storyboard = CreateOpacityAnimation(TimeSpan.FromMilliseconds(40))
        };

        await RunOnUiAsync(() => action.Execute(host, null));
        await Task.Delay(150);

        Assert.False(StoryboardRegistryService.Instance.TryGet(host, "Fade", out _));
    }

    [AvaloniaFact]
    public async Task BeginStoryboardAction_AnimatesNestedScaleTransformProperties()
    {
        var host = CreateInteractiveCard();
        var scaleXPath = "(Border.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)";
        var scaleYPath = "(Border.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)";

        var scaleXAction = CreateScaleStoryboard(host, ScaleTransform.ScaleXProperty, scaleXPath, 1.15);
        var scaleYAction = CreateScaleStoryboard(host, ScaleTransform.ScaleYProperty, scaleYPath, 1.15);

        await RunOnUiAsync(() =>
        {
            scaleXAction.Execute(host, null);
            scaleYAction.Execute(host, null);
        });

        await Task.Delay(250);

        var (scaleX, scaleY) = await GetScaleAsync(host);
        Assert.True(scaleX > 1.05, $"Expected ScaleX to increase, actual {scaleX:F3}");
        Assert.True(scaleY > 1.05, $"Expected ScaleY to increase, actual {scaleY:F3}");
    }

    private static AnimationTimeline CreateOpacityAnimation(TimeSpan duration)
    {
        return new AnimationTimeline
        {
            Duration = duration,
            Easing = new Avalonia.Animation.Easings.LinearEasing(),
            Children =
            {
                new KeyFrame
                {
                    Cue = new Cue(0d),
                    Setters = { new Setter(Border.OpacityProperty, 0d) }
                },
                new KeyFrame
                {
                    Cue = new Cue(1d),
                    Setters = { new Setter(Border.OpacityProperty, 1d) }
                }
            }
        };
    }

    private static BeginStoryboardAction CreateScaleStoryboard(
        Border host,
        AvaloniaProperty property,
        string targetPropertyPath,
        double toValue)
    {
        var animation = new AnimationTimeline
        {
            Duration = TimeSpan.FromMilliseconds(150),
            FillMode = FillMode.Forward,
            Children =
            {
                new KeyFrame
                {
                    Cue = new Cue(0d),
                    Setters = { new Setter(property, 1d) }
                },
                new KeyFrame
                {
                    Cue = new Cue(1d),
                    Setters = { new Setter(property, toValue) }
                }
            }
        };

        StoryboardService.SetTarget(animation, host);
        StoryboardService.SetTargetProperty(animation, targetPropertyPath);

        return new BeginStoryboardAction
        {
            Storyboard = animation
        };
    }

    private static Border CreateInteractiveCard()
    {
        var transforms = new TransformGroup();
        transforms.Children.Add(new ScaleTransform { ScaleX = 1, ScaleY = 1 });
        transforms.Children.Add(new SkewTransform());

        return new Border
        {
            RenderTransform = transforms,
            RenderTransformOrigin = new RelativePoint(0.5, 0.5, RelativeUnit.Relative)
        };
    }

    private static async Task<(double ScaleX, double ScaleY)> GetScaleAsync(Border host)
    {
        if (Dispatcher.UIThread.CheckAccess())
        {
            var scale = GetScaleTransform(host);
            return (scale.ScaleX, scale.ScaleY);
        }

        return await Dispatcher.UIThread.InvokeAsync(() =>
        {
            var scale = GetScaleTransform(host);
            return (scale.ScaleX, scale.ScaleY);
        });
    }

    private static ScaleTransform GetScaleTransform(Border host)
    {
        return host.RenderTransform switch
        {
            ScaleTransform scale => scale,
            TransformGroup group => group.Children.OfType<Transform>().OfType<ScaleTransform>().First(),
            _ => throw new InvalidOperationException("Host RenderTransform does not contain a ScaleTransform.")
        };
    }

    private static async ValueTask RunOnUiAsync(Action action)
    {
        if (Dispatcher.UIThread.CheckAccess())
        {
            action();
            return;
        }

        await Dispatcher.UIThread.InvokeAsync(action);
    }

    private static async Task<double> GetOpacityAsync(Border host)
    {
        if (Dispatcher.UIThread.CheckAccess())
        {
            return host.Opacity;
        }

        return await Dispatcher.UIThread.InvokeAsync(() => host.Opacity);
    }

}
