// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Hosting;

namespace Xaml.Interactions.Custom;

/// <summary>
/// Uno Platform counterpart of <c>Avalonia.Rendering.Composition.ElementComposition</c> used by the shared sources.
/// </summary>
/// <remarks>
/// Unlike Avalonia, the element visual always exists on WinUI, and its <c>Offset</c> is applied on top of the
/// arranged position of the element (see <see cref="CompositionAnimationHelpers.GetLayoutOffset(FrameworkElement)"/>).
/// </remarks>
internal static class ElementComposition
{
    /// <summary>
    /// Gets the composition visual that renders an element.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns>The element visual.</returns>
    public static CompositionVisual? GetElementVisual(UIElement element)
        => ElementCompositionPreview.GetElementVisual(element);
}
