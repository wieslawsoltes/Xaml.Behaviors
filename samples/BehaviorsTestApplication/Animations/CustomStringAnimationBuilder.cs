using System;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Controls;
#else
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Styling;
#endif

namespace BehaviorsTestApplication.Animations;

#if UNO
/// <summary>
/// Builds a storyboard that progressively reveals the characters of a string (Avalonia: a custom string animator).
/// </summary>
/// <remarks>
/// WinUI storyboards have no custom animators; discrete object key frames reveal one more character each step.
/// </remarks>
public partial class CustomStringAnimationBuilder : AvaloniaObject, global::Xaml.Interactions.Custom.IAnimationBuilder
{
    private const string Value = "0123456789";

    /// <inheritdoc />
    public Storyboard Build(Control control)
    {
        var animation = new ObjectAnimationUsingKeyFrames
        {
            Duration = TimeSpan.FromSeconds(1),
        };

        for (var length = 1; length <= Value.Length; length++)
        {
            animation.KeyFrames.Add(new DiscreteObjectKeyFrame
            {
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds((length - 1) / (double)Value.Length)),
                Value = Value.Substring(0, length)
            });
        }

        // Avalonia sets TextBlock.Text on any control. A WinUI storyboard needs a property of the target (native
        // WinUI throws when it cannot resolve it): elements that are not text blocks get the text as their Tag.
        Storyboard.SetTarget(animation, control);
        Storyboard.SetTargetProperty(animation, control is TextBlock ? nameof(TextBlock.Text) : nameof(FrameworkElement.Tag));

        return new Storyboard
        {
            RepeatBehavior = RepeatBehavior.Forever,
            Children = { animation }
        };
    }
}
#else
/// <summary>
/// Builds an animation that uses <see cref="CustomStringAnimator"/>.
/// </summary>
public partial class CustomStringAnimationBuilder : AvaloniaObject, Avalonia.Xaml.Interactions.Custom.IAnimationBuilder
{
    /// <inheritdoc />
    public Animation Build(Control control)
    {
        return new Animation
        {
            Duration = TimeSpan.FromSeconds(1),
            IterationCount = IterationCount.Infinite,
            Children =
            {
                new KeyFrame
                {
                    Cue = new Cue(0d),
                    Setters =
                    {
                        CreateSetter(string.Empty)
                    }
                },
                new KeyFrame
                {
                    Cue = new Cue(1d),
                    Setters =
                    {
                        CreateSetter("0123456789")
                    }
                }
            }
        };
    }

    private static Setter CreateSetter(string value)
    {
        var setter = new Setter(TextBlock.TextProperty, value);
        Animation.SetAnimator(setter, new CustomStringAnimator());
        return setter;
    }
}
#endif
