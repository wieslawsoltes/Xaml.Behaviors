// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Xaml.Interactions.Custom;

/// <summary>
/// Avalonia <c>ToolTip</c> attached property accessors mapped to the WinUI <see cref="ToolTipService"/>.
/// </summary>
internal static class ToolTipExtensions
{
    extension(ToolTip)
    {
        /// <summary>
        /// Sets the tooltip content of the element (Avalonia <c>ToolTip.SetTip</c>).
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="value">The tooltip content.</param>
        public static void SetTip(DependencyObject element, object? value) => ToolTipService.SetToolTip(element, value);

        /// <summary>
        /// Opens or closes the tooltip of the element (Avalonia <c>ToolTip.SetIsOpen</c>).
        /// </summary>
        /// <remarks>
        /// WinUI only opens <see cref="ToolTip"/> instances programmatically: other tooltip content is wrapped in a
        /// <see cref="ToolTip"/> first.
        /// </remarks>
        /// <param name="element">The element.</param>
        /// <param name="value"><c>true</c> to open the tooltip.</param>
        public static void SetIsOpen(DependencyObject element, bool value)
        {
            var tip = ToolTipService.GetToolTip(element);
            if (tip is null)
            {
                return;
            }

            if (tip is not ToolTip toolTip)
            {
                if (!value)
                {
                    return;
                }

                toolTip = new ToolTip { Content = tip };
                ToolTipService.SetToolTip(element, toolTip);
            }

            toolTip.IsOpen = value;
        }
    }
}
