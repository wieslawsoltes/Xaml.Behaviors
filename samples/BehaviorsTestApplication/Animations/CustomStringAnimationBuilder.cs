using System;
using Avalonia.Animation;
using Avalonia.Controls;

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
                        new Setter(TextBlock.TextProperty, string.Empty)
                        {
                            Animator = new CustomStringAnimator()
                        }
                    }
                },
                new KeyFrame
                {
                    Cue = new Cue(1d),
                    Setters =
                    {
                        new Setter(TextBlock.TextProperty, "0123456789")
                        {
                            Animator = new CustomStringAnimator()
                        }
                    }
                }
            }
        };
    }
}
