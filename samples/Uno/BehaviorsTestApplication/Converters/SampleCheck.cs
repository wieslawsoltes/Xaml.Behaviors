// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Microsoft.UI.Xaml;

namespace BehaviorsTestApplication.Converters;

/// <summary>
/// Check state functions for compiled bindings (<c>{x:Bind converters:SampleCheck.IsTrue(Box.IsChecked)}</c>): the
/// WinUI <c>ToggleButton.IsChecked</c> is a nullable <see cref="bool"/>, which Avalonia bindings convert implicitly.
/// </summary>
public static class SampleCheck
{
    /// <summary>
    /// Returns whether a check state is checked (Avalonia binds <c>IsChecked</c> to <see cref="bool"/> properties).
    /// </summary>
    /// <param name="isChecked">The check state.</param>
    /// <returns><c>true</c> when <paramref name="isChecked"/> is <c>true</c>.</returns>
    public static bool IsTrue(bool? isChecked) => isChecked == true;

    /// <summary>
    /// Returns the visibility of an element shown while a check box is checked (Avalonia binds <c>IsVisible</c>).
    /// </summary>
    /// <param name="isChecked">The check state.</param>
    /// <returns><see cref="Visibility.Visible"/> when <paramref name="isChecked"/> is <c>true</c>.</returns>
    public static Visibility Visible(bool? isChecked) => isChecked == true ? Visibility.Visible : Visibility.Collapsed;
}
