// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace Xaml.PortAnalyzers.PortMaps
{
    /// <summary>
    /// The result of classifying a symbol against a port map.
    /// </summary>
    internal readonly struct PortSymbolClassification
    {
        /// <summary>
        /// A classification for symbols that are not checked.
        /// </summary>
        public static readonly PortSymbolClassification Ignored = new(PortSymbolStatus.Ignored, string.Empty, null);

        /// <summary>
        /// A classification for symbols that are available on the port target.
        /// </summary>
        public static readonly PortSymbolClassification Mapped = new(PortSymbolStatus.Mapped, string.Empty, null);

        /// <summary>
        /// Initializes a new instance of the <see cref="PortSymbolClassification"/> struct.
        /// </summary>
        public PortSymbolClassification(PortSymbolStatus status, string name, string? renamedTo)
        {
            Status = status;
            Name = name;
            RenamedTo = renamedTo;
        }

        /// <summary>Gets the status of the symbol.</summary>
        public PortSymbolStatus Status { get; }

        /// <summary>Gets the port map name of the symbol.</summary>
        public string Name { get; }

        /// <summary>Gets the new namespace name of a renamed namespace.</summary>
        public string? RenamedTo { get; }

        /// <summary>
        /// Gets a value indicating whether the symbol must be guarded.
        /// </summary>
        public bool RequiresGuard => Status is PortSymbolStatus.NotMapped or PortSymbolStatus.Unsupported or PortSymbolStatus.Renamed;

        /// <summary>
        /// Gets the reason inserted into the XPORT002 message.
        /// </summary>
        public string Reason => Status switch
        {
            PortSymbolStatus.Unsupported => "is unsupported",
            PortSymbolStatus.Renamed => "is renamed to '" + RenamedTo + "'",
            _ => "is not mapped",
        };
    }
}
