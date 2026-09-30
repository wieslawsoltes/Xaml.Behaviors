// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Microsoft.UI.Xaml;

namespace Xaml.Interactivity;

/// <summary>
/// Uno Platform counterpart of the Avalonia <c>Visual.IsVisible</c> property used by the shared sources.
/// </summary>
internal static class VisibilityCompatExtensions
{
    extension(UIElement element)
    {
        /// <summary>
        /// Gets or sets a value indicating whether the element is visible (<see cref="Visibility.Visible"/>) or
        /// collapsed (<see cref="Visibility.Collapsed"/>).
        /// </summary>
        public bool IsVisible
        {
            get => element.Visibility == Visibility.Visible;
            set => element.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
