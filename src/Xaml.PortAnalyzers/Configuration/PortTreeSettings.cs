// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace Xaml.PortAnalyzers.Configuration
{
    /// <summary>
    /// Port settings resolved for a single syntax tree.
    /// </summary>
    internal sealed class PortTreeSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PortTreeSettings"/> class.
        /// </summary>
        public PortTreeSettings(string targetSymbol, SymbolPatternSet knownSymbols, bool isShared)
        {
            TargetSymbol = targetSymbol;
            KnownSymbols = knownSymbols;
            IsShared = isShared;
        }

        /// <summary>
        /// Gets the preprocessor symbol that marks the port target.
        /// </summary>
        public string TargetSymbol { get; }

        /// <summary>
        /// Gets the known preprocessor symbols (defaults, target symbol and configured symbols).
        /// </summary>
        public SymbolPatternSet KnownSymbols { get; }

        /// <summary>
        /// Gets a value indicating whether the file is shared with (compiled for) the port target.
        /// </summary>
        public bool IsShared { get; }
    }
}
