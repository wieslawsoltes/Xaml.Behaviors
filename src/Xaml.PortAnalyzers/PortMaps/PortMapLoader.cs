// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Xaml.PortAnalyzers.Json;

namespace Xaml.PortAnalyzers.PortMaps
{
    /// <summary>
    /// Loads and merges the port maps passed to the compiler as <c>AdditionalFiles</c> named <c>*.xamlport.json</c>.
    /// </summary>
    internal static class PortMapLoader
    {
        /// <summary>
        /// The file name suffix of port map files.
        /// </summary>
        public const string FileSuffix = ".xamlport.json";

        /// <summary>
        /// Returns whether an additional file is a port map.
        /// </summary>
        public static bool IsPortMapFile(string? path)
            => path is not null && path.EndsWith(FileSuffix, StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Loads all port maps of a compilation.
        /// </summary>
        public static PortMapLoadResult Load(ImmutableArray<AdditionalText> additionalFiles, CancellationToken cancellationToken)
        {
            List<PortMap>? maps = null;
            List<PortMapError>? errors = null;
            foreach (var file in additionalFiles)
            {
                if (!IsPortMapFile(file.Path))
                {
                    continue;
                }

                var text = file.GetText(cancellationToken);
                if (text is null)
                {
                    continue;
                }

                try
                {
                    (maps ??= new List<PortMap>()).Add(PortMapParser.Parse(text.ToString()));
                }
                catch (JsonParseException exception)
                {
                    var position = Math.Max(0, Math.Min(exception.Position, text.Length));
                    var span = new TextSpan(position, position < text.Length ? 1 : 0);
                    (errors ??= new List<PortMapError>()).Add(new PortMapError(file, exception.Message, span, text.Lines.GetLinePositionSpan(span)));
                }
            }

            return new PortMapLoadResult(
                maps is null ? null : PortMap.Merge(maps),
                errors is null ? [] : errors.ToArray());
        }
    }
}
