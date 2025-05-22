using Avalonia.Animation;
using Avalonia.Controls;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Provides a way to build <see cref="Animation.Animation"/> instances in code.
/// </summary>
public interface IAnimationBuilder
{
    /// <summary>
    /// Creates an animation for the specified control.
    /// </summary>
    /// <param name="control">The control that will run the animation.</param>
    /// <returns>The created animation or <c>null</c>.</returns>
    Animation.Animation? Build(Control control);
}
