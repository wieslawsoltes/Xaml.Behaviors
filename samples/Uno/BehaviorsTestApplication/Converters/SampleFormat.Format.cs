// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Globalization;

namespace BehaviorsTestApplication.Converters;

/// <content>
/// Composite formatting for compiled bindings.
/// </content>
public static partial class SampleFormat
{
    /// <summary>
    /// Formats a value like the Avalonia binding <c>StringFormat='Text: {0}'</c>
    /// (<c>{x:Bind converters:SampleFormat.Format('Text: {0}', Source.Value), Mode=OneWay}</c>).
    /// </summary>
    /// <param name="format">The composite format string.</param>
    /// <param name="value">The value.</param>
    /// <returns>The formatted text.</returns>
    public static string Format(string format, object? value) => string.Format(CultureInfo.CurrentCulture, format, value);
}
