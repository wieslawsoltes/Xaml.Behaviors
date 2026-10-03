// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;

namespace Xaml.PortAnalyzers.PortMaps
{
    /// <summary>
    /// A set of fully qualified type, namespace and member names of a port map section.
    /// </summary>
    internal sealed class PortMapSection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PortMapSection"/> class.
        /// </summary>
        public PortMapSection(HashSet<string> types, HashSet<string> namespaces, HashSet<string> members)
        {
            Types = types;
            Namespaces = namespaces;
            Members = members;
        }

        /// <summary>
        /// Gets the fully qualified type names (without type arguments, optionally with a <c>`N</c> arity suffix).
        /// </summary>
        public HashSet<string> Types { get; }

        /// <summary>
        /// Gets the namespaces; a namespace also covers its nested namespaces.
        /// </summary>
        public HashSet<string> Namespaces { get; }

        /// <summary>
        /// Gets the member names (<c>Namespace.Type.Member</c> or <c>Namespace.Type.*</c>).
        /// </summary>
        public HashSet<string> Members { get; }

        /// <summary>
        /// Creates an empty section.
        /// </summary>
        public static PortMapSection CreateEmpty() => new(
            new HashSet<string>(StringComparer.Ordinal),
            new HashSet<string>(StringComparer.Ordinal),
            new HashSet<string>(StringComparer.Ordinal));

        /// <summary>
        /// Merges sections into a new one.
        /// </summary>
        public static PortMapSection Merge(IReadOnlyList<PortMapSection> sections)
        {
            var result = CreateEmpty();
            for (var i = 0; i < sections.Count; i++)
            {
                result.Types.UnionWith(sections[i].Types);
                result.Namespaces.UnionWith(sections[i].Namespaces);
                result.Members.UnionWith(sections[i].Members);
            }

            return result;
        }

        /// <summary>
        /// Returns whether the namespace, or one of its ancestors, is listed.
        /// </summary>
        public bool ContainsNamespace(string namespaceName)
        {
            if (Namespaces.Count == 0)
            {
                return false;
            }

            var current = namespaceName;
            while (current.Length > 0)
            {
                if (Namespaces.Contains(current))
                {
                    return true;
                }

                var dot = current.LastIndexOf('.');
                current = dot < 0 ? string.Empty : current.Substring(0, dot);
            }

            return false;
        }
    }
}
