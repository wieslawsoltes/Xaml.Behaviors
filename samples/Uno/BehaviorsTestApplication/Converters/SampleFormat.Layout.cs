// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Globalization;

namespace BehaviorsTestApplication.Converters;

/// <content>
/// Formatting functions for the counter, bounds and state samples.
/// </content>
public static partial class SampleFormat
{
    /// <summary>
    /// Formats a counter like the Avalonia binding <c>StringFormat={}Count: {0}</c>.
    /// </summary>
    /// <param name="count">The counter value.</param>
    /// <returns>The formatted text.</returns>
    public static string Count(int count) => string.Format(CultureInfo.CurrentCulture, "Count: {0}", count);

    /// <summary>
    /// Formats a width like the Avalonia binding <c>StringFormat={}Width: {0}</c>.
    /// </summary>
    /// <param name="width">The width.</param>
    /// <returns>The formatted text.</returns>
    public static string Width(double width) => string.Format(CultureInfo.CurrentCulture, "Width: {0}", width);

    /// <summary>
    /// Formats a height like the Avalonia binding <c>StringFormat={}Height: {0}</c>.
    /// </summary>
    /// <param name="height">The height.</param>
    /// <returns>The formatted text.</returns>
    public static string Height(double height) => string.Format(CultureInfo.CurrentCulture, "Height: {0}", height);

    /// <summary>
    /// Formats a number like an Avalonia binding of a <see cref="double"/> to <c>TextBlock.Text</c> (no format).
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The formatted text.</returns>
    public static string Number(double value) => value.ToString(CultureInfo.CurrentCulture);

    /// <summary>
    /// Formats a flag like an Avalonia binding of a <see cref="bool"/> to <c>TextBlock.Text</c> (<c>True</c>/<c>False</c>).
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The formatted text.</returns>
    public static string Boolean(bool value) => value.ToString(CultureInfo.CurrentCulture);
}
