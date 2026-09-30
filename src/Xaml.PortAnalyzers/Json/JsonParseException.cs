// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;

namespace Xaml.PortAnalyzers.Json
{
    /// <summary>
    /// Thrown when JSON text is malformed or does not follow an expected schema.
    /// </summary>
    internal sealed class JsonParseException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonParseException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="position">The offset of the error in the source text.</param>
        public JsonParseException(string message, int position)
            : base(message)
        {
            Position = position;
        }

        /// <summary>
        /// Gets the offset of the error in the source text.
        /// </summary>
        public int Position { get; }
    }
}
