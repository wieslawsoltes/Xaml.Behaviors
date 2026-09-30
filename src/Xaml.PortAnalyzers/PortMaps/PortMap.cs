// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;

namespace Xaml.PortAnalyzers.PortMaps
{
    /// <summary>
    /// Describes which symbols of the source platform (e.g. Avalonia) are available on the port target.
    /// </summary>
    internal sealed class PortMap
    {
        /// <summary>
        /// The namespace prefix used when a port map does not declare <c>sourceNamespacePrefixes</c>.
        /// </summary>
        public const string DefaultSourceNamespacePrefix = "Avalonia";

        /// <summary>
        /// Initializes a new instance of the <see cref="PortMap"/> class.
        /// </summary>
        public PortMap(
            IReadOnlyList<string> sourceNamespacePrefixes,
            PortMapSection mapped,
            PortMapSection unsupported,
            IReadOnlyDictionary<string, string> renamedNamespaces)
        {
            SourceNamespacePrefixes = sourceNamespacePrefixes;
            Mapped = mapped;
            Unsupported = unsupported;
            RenamedNamespaces = renamedNamespaces;
        }

        /// <summary>
        /// Gets the namespace prefixes of the source platform; only symbols from these namespaces are checked.
        /// </summary>
        public IReadOnlyList<string> SourceNamespacePrefixes { get; }

        /// <summary>
        /// Gets the symbols that exist (under the same name) on the port target.
        /// </summary>
        public PortMapSection Mapped { get; }

        /// <summary>
        /// Gets the symbols that are known not to exist on the port target.
        /// </summary>
        public PortMapSection Unsupported { get; }

        /// <summary>
        /// Gets namespaces whose content is ported under a different namespace name (old name to new name).
        /// </summary>
        public IReadOnlyDictionary<string, string> RenamedNamespaces { get; }

        /// <summary>
        /// Merges several port maps into one.
        /// </summary>
        public static PortMap Merge(IReadOnlyList<PortMap> maps)
        {
            if (maps.Count == 1)
            {
                return maps[0];
            }

            var prefixes = new List<string>();
            var prefixSet = new HashSet<string>(StringComparer.Ordinal);
            var mapped = new List<PortMapSection>();
            var unsupported = new List<PortMapSection>();
            var renamed = new Dictionary<string, string>(StringComparer.Ordinal);
            for (var i = 0; i < maps.Count; i++)
            {
                var map = maps[i];
                for (var j = 0; j < map.SourceNamespacePrefixes.Count; j++)
                {
                    if (prefixSet.Add(map.SourceNamespacePrefixes[j]))
                    {
                        prefixes.Add(map.SourceNamespacePrefixes[j]);
                    }
                }

                mapped.Add(map.Mapped);
                unsupported.Add(map.Unsupported);
                foreach (var pair in map.RenamedNamespaces)
                {
                    renamed[pair.Key] = pair.Value;
                }
            }

            return new PortMap(prefixes, PortMapSection.Merge(mapped), PortMapSection.Merge(unsupported), renamed);
        }

        /// <summary>
        /// Returns whether the namespace belongs to the source platform.
        /// </summary>
        public bool IsSourceNamespace(string namespaceName)
        {
            for (var i = 0; i < SourceNamespacePrefixes.Count; i++)
            {
                if (IsSameOrNested(namespaceName, SourceNamespacePrefixes[i]))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the new name of a renamed namespace (or of its nearest renamed ancestor), or <see langword="null"/>.
        /// </summary>
        public string? GetRenamedNamespace(string namespaceName)
        {
            var current = namespaceName;
            while (current.Length > 0)
            {
                if (RenamedNamespaces.TryGetValue(current, out var renamed))
                {
                    return renamed + namespaceName.Substring(current.Length);
                }

                var dot = current.LastIndexOf('.');
                current = dot < 0 ? string.Empty : current.Substring(0, dot);
            }

            return null;
        }

        /// <summary>
        /// Returns whether <paramref name="namespaceName"/> equals <paramref name="prefix"/> or is nested in it.
        /// </summary>
        public static bool IsSameOrNested(string namespaceName, string prefix)
        {
            if (prefix.Length == 0)
            {
                return true;
            }

            if (!namespaceName.StartsWith(prefix, StringComparison.Ordinal))
            {
                return false;
            }

            return namespaceName.Length == prefix.Length || namespaceName[prefix.Length] == '.';
        }
    }
}
