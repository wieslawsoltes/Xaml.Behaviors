// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PlatformAnimation = Microsoft.UI.Xaml.Media.Animation.Storyboard;
#else
using Avalonia.Animation;
using Avalonia.Controls;
using PlatformAnimation = Avalonia.Animation.Animation;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Provides a way to build <see cref="Animation"/> instances in code.
/// </summary>
public interface IAnimationBuilder
{
    /// <summary>
    /// Creates an animation for the specified control.
    /// </summary>
    /// <param name="control">The control that will run the animation.</param>
    /// <returns>The created animation or <c>null</c>.</returns>
    PlatformAnimation? Build(Control control);
}
