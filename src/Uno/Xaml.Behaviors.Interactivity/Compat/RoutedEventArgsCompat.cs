// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace Xaml.Interactivity;

/// <summary>
/// Uno Platform counterpart of the Avalonia <c>RoutedEventArgs.Handled</c> property used by the shared sources.
/// </summary>
/// <remarks>
/// WinUI declares <c>Handled</c> on each handleable event argument type, not on <see cref="RoutedEventArgs"/>.
/// Arguments that cannot be handled (for example <c>GotFocus</c>/<c>LostFocus</c>) report <c>false</c> and ignore
/// assignments, like WinUI does.
/// </remarks>
internal static class RoutedEventArgsCompatExtensions
{
    extension(RoutedEventArgs e)
    {
        /// <summary>
        /// Gets the element that raised the event (Avalonia <c>RoutedEventArgs.Source</c>).
        /// </summary>
        public object? Source => e.OriginalSource;

        /// <summary>
        /// Gets or sets a value indicating whether the routed event was handled.
        /// </summary>
        public bool Handled
        {
            get => e switch
            {
                PointerRoutedEventArgs args => args.Handled,
                KeyRoutedEventArgs args => args.Handled,
                TappedRoutedEventArgs args => args.Handled,
                DoubleTappedRoutedEventArgs args => args.Handled,
                RightTappedRoutedEventArgs args => args.Handled,
                HoldingRoutedEventArgs args => args.Handled,
                CharacterReceivedRoutedEventArgs args => args.Handled,
                DragEventArgs args => args.Handled,
                ManipulationStartingRoutedEventArgs args => args.Handled,
                ManipulationStartedRoutedEventArgs args => args.Handled,
                ManipulationDeltaRoutedEventArgs args => args.Handled,
                ManipulationInertiaStartingRoutedEventArgs args => args.Handled,
                ManipulationCompletedRoutedEventArgs args => args.Handled,
                ContextRequestedEventArgs args => args.Handled,
                GettingFocusEventArgs args => args.Handled,
                LosingFocusEventArgs args => args.Handled,
                BringIntoViewRequestedEventArgs args => args.Handled,
                _ => false,
            };
            set
            {
                switch (e)
                {
                    case PointerRoutedEventArgs args:
                        args.Handled = value;
                        break;
                    case KeyRoutedEventArgs args:
                        args.Handled = value;
                        break;
                    case TappedRoutedEventArgs args:
                        args.Handled = value;
                        break;
                    case DoubleTappedRoutedEventArgs args:
                        args.Handled = value;
                        break;
                    case RightTappedRoutedEventArgs args:
                        args.Handled = value;
                        break;
                    case HoldingRoutedEventArgs args:
                        args.Handled = value;
                        break;
                    case CharacterReceivedRoutedEventArgs args:
                        args.Handled = value;
                        break;
                    case DragEventArgs args:
                        args.Handled = value;
                        break;
                    case ManipulationStartingRoutedEventArgs args:
                        args.Handled = value;
                        break;
                    case ManipulationStartedRoutedEventArgs args:
                        args.Handled = value;
                        break;
                    case ManipulationDeltaRoutedEventArgs args:
                        args.Handled = value;
                        break;
                    case ManipulationInertiaStartingRoutedEventArgs args:
                        args.Handled = value;
                        break;
                    case ManipulationCompletedRoutedEventArgs args:
                        args.Handled = value;
                        break;
                    case ContextRequestedEventArgs args:
                        args.Handled = value;
                        break;
                    case GettingFocusEventArgs args:
                        args.Handled = value;
                        break;
                    case LosingFocusEventArgs args:
                        args.Handled = value;
                        break;
                    case BringIntoViewRequestedEventArgs args:
                        args.Handled = value;
                        break;
                }
            }
        }
    }
}
