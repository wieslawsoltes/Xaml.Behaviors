// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace Xaml.PortAnalyzers
{
    /// <summary>
    /// Identifiers of the diagnostics reported by the Xaml.PortAnalyzers package.
    /// </summary>
    internal static class DiagnosticIds
    {
        /// <summary>
        /// A type deriving directly from a dependency object must be partial.
        /// </summary>
        public const string PartialDependencyObject = "XPORT001";

        /// <summary>
        /// A platform-specific API is not guarded for the port target.
        /// </summary>
        public const string UnguardedPlatformApi = "XPORT002";

        /// <summary>
        /// An unknown preprocessor symbol is used in a platform condition.
        /// </summary>
        public const string UnknownPreprocessorSymbol = "XPORT003";

        /// <summary>
        /// A dependency property getter must cast the result of <c>GetValue</c>.
        /// </summary>
        public const string GetValueCast = "XPORT004";

        /// <summary>
        /// A port map file could not be read.
        /// </summary>
        public const string InvalidPortMap = "XPORT005";
    }
}
