// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Globalization;

namespace BehaviorsTestApplication.Converters;

/// <summary>
/// Path formatting functions for compiled bindings (<c>{x:Bind converters:SamplePathFormat.TestFile(...)}</c>): WinUI
/// bindings have no <c>StringFormat</c> (see <see cref="SampleFormat"/>).
/// </summary>
public static class SamplePathFormat
{
    /// <summary>
    /// Formats the path of the sample text file in a directory, like the Avalonia binding
    /// <c>StringFormat='{}{0}/test.txt'</c>.
    /// </summary>
    /// <param name="directory">The directory path.</param>
    /// <returns>The path of <c>test.txt</c> in <paramref name="directory"/>.</returns>
    public static string TestFile(string? directory) => string.Format(CultureInfo.InvariantCulture, "{0}/test.txt", directory);
}
