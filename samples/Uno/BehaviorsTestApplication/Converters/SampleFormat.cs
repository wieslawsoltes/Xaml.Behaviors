// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Globalization;

namespace BehaviorsTestApplication.Converters;

/// <summary>
/// Formatting functions for compiled bindings (<c>{x:Bind converters:SampleFormat.Value(...)}</c>): WinUI bindings have
/// no <c>StringFormat</c>.
/// </summary>
public static partial class SampleFormat
{
    /// <summary>
    /// Formats a value like the Avalonia binding <c>StringFormat={} Value: {0}</c>.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The formatted text.</returns>
    public static string Value(int value) => string.Format(CultureInfo.CurrentCulture, " Value: {0}", value);
}
