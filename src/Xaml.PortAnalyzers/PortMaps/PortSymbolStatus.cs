// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace Xaml.PortAnalyzers.PortMaps
{
    /// <summary>
    /// The availability of a symbol on the port target.
    /// </summary>
    internal enum PortSymbolStatus
    {
        /// <summary>The symbol does not belong to the source platform and is not checked.</summary>
        Ignored,

        /// <summary>The symbol is available on the port target.</summary>
        Mapped,

        /// <summary>The symbol belongs to the source platform and is not declared as mapped.</summary>
        NotMapped,

        /// <summary>The symbol is declared as unsupported on the port target.</summary>
        Unsupported,

        /// <summary>The namespace exists on the port target under a different name.</summary>
        Renamed,
    }
}
