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
    /// <remarks>
    /// On native WinUI the translation of the visual is enabled: layout owns <c>Visual.Offset</c> there, so the
    /// animations move elements with <c>Translation</c> (see <c>CompositionAnimationHelpers.SetOffset</c>).
    /// </remarks>
    public static CompositionVisual? GetElementVisual(UIElement element)
    {
#if WINUI
        var visual = ElementCompositionPreview.GetElementVisual(element);

        // Enabling the translation (again) resets it: only enable it when the visual has none yet.
        if (visual.Properties.TryGetVector3("Translation", out _) != Microsoft.UI.Composition.CompositionGetValueStatus.Succeeded)
        {
            ElementCompositionPreview.SetIsTranslationEnabled(element, true);
        }

        return visual;
#else
        return ElementCompositionPreview.GetElementVisual(element);
#endif
    }
}
