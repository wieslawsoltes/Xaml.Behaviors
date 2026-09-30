// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Microsoft.UI.Xaml;

namespace Xaml.Interactivity;

/// <summary>
/// Receives change notifications for dependency properties registered through <see cref="AvaloniaProperty"/>.
/// </summary>
/// <remarks>
/// WinUI has no <c>OnPropertyChanged</c> virtual on <see cref="DependencyObject"/>. The Uno base classes
/// implement this interface and forward to their <c>OnPropertyChanged</c> virtual.
/// </remarks>
internal interface IDependencyPropertyChangedHandler
{
    /// <summary>
    /// Called after the value of a registered dependency property changed.
    /// </summary>
    /// <param name="e">The change details.</param>
    void OnDependencyPropertyChanged(DependencyPropertyChangedEventArgs e);
}
