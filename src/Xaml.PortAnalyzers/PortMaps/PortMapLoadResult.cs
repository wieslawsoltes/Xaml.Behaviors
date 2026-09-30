// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;

namespace Xaml.PortAnalyzers.PortMaps
{
    /// <summary>
    /// The result of loading the port maps of a compilation.
    /// </summary>
    internal sealed class PortMapLoadResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PortMapLoadResult"/> class.
        /// </summary>
        public PortMapLoadResult(PortMap? map, PortMapError[] errors)
        {
            Map = map;
            Errors = errors;
        }

        /// <summary>
        /// Gets the merged port map, or <see langword="null"/> when no valid port map exists.
        /// </summary>
        public PortMap? Map { get; }

        /// <summary>
        /// Gets the errors of invalid port map files.
        /// </summary>
        public IReadOnlyList<PortMapError> Errors { get; }
    }
}
