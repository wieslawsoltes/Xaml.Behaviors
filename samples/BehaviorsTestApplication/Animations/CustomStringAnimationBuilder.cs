using System;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Styling;

namespace BehaviorsTestApplication.Animations;

/// <summary>
/// Builds an animation that uses <see cref="CustomStringAnimator"/>.
/// </summary>
public class CustomStringAnimationBuilder : AvaloniaObject, Avalonia.Xaml.Interactions.Custom.IAnimationBuilder
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
