// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Numerics;
using Microsoft.UI.Composition;

// Visual is a global alias of UIElement in the shared tests: the composition visual is fully qualified.

namespace Xaml.Behaviors.Uno.TestCompat;

/// <summary>
/// Reads the composition offset the behaviors give an element, relative to its layout position.
/// </summary>
internal static class CompositionTestCompat
{
    /// <summary>
    /// Gets the offset of an element visual relative to the layout position of the element.
    /// </summary>
    /// <param name="visual">The element visual.</param>
    /// <returns>
    /// <c>Visual.Offset</c> on Uno Platform; the <c>Translation</c> of the visual on native WinUI, where layout owns
    /// <c>Visual.Offset</c> and the behaviors move elements with their translation.
    /// </returns>
    /// <remarks>
    /// On native WinUI composition animations run in the compositor: the value is the last one set, not the animated
    /// value.
    /// </remarks>
    public static Vector3 GetLayoutRelativeOffset(this Microsoft.UI.Composition.Visual visual)
    {
#if WINUI
        return visual.Properties.TryGetVector3("Translation", out Vector3 translation) == CompositionGetValueStatus.Succeeded
            ? translation
            : Vector3.Zero;
#else
        return visual.Offset;
#endif
    }
}
