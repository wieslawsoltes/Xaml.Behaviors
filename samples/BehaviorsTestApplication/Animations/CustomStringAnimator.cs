using System;
using Avalonia.Animation;
using Avalonia.Animation.Animators;

namespace BehaviorsTestApplication.Animations;

/// <summary>
/// Animates a string by progressively revealing characters.
/// </summary>
public class CustomStringAnimator : InterpolatingAnimator<string>
{
    /// <inheritdoc />
    public override string Interpolate(double progress, string oldValue, string newValue)
    {
        if (string.IsNullOrEmpty(newValue))
        {
            return string.Empty;
        }

        var step = 1.0 / newValue.Length;
        var length = (int)(progress / step);
        return newValue.Substring(0, Math.Clamp(length + 1, 0, newValue.Length));
    }
}
