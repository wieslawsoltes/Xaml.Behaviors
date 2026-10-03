// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Xaml.PortAnalyzers.PortMaps
{
    /// <summary>
    /// An error found while reading a port map file.
    /// </summary>
    internal sealed class PortMapError
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PortMapError"/> class.
        /// </summary>
        public PortMapError(AdditionalText file, string message, TextSpan span, LinePositionSpan lineSpan)
        {
            File = file;
            Message = message;
            Span = span;
            LineSpan = lineSpan;
        }

        /// <summary>Gets the invalid file.</summary>
        public AdditionalText File { get; }

        /// <summary>Gets the error message.</summary>
        public string Message { get; }

        /// <summary>Gets the span of the error.</summary>
        public TextSpan Span { get; }

        /// <summary>Gets the line span of the error.</summary>
        public LinePositionSpan LineSpan { get; }

        /// <summary>
        /// Creates the location of the error.
        /// </summary>
        public Location GetLocation() => Location.Create(File.Path, Span, LineSpan);
    }
}
