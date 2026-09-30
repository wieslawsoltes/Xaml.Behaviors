// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using Xaml.PortAnalyzers.Json;

namespace Xaml.PortAnalyzers.PortMaps
{
    /// <summary>
    /// Reads a port map (<c>*.xamlport.json</c>) document.
    /// </summary>
    internal static class PortMapParser
    {
        /// <summary>
        /// Parses a port map document.
        /// </summary>
        /// <exception cref="JsonParseException">The document is not valid JSON or does not follow the schema.</exception>
        public static PortMap Parse(string text)
        {
            var root = JsonParser.Parse(text);
            if (root.Kind != JsonValueKind.Object)
            {
                throw new JsonParseException("The port map must be a JSON object", root.Position);
            }

            List<string>? prefixes = null;
            var mapped = PortMapSection.CreateEmpty();
            var unsupported = PortMapSection.CreateEmpty();
            var renamed = new Dictionary<string, string>(StringComparer.Ordinal);

            for (var i = 0; i < root.Properties.Count; i++)
            {
                var property = root.Properties[i];
                switch (property.Key)
                {
                    case "$schema":
                        break;
                    case "sourceNamespacePrefixes":
                        prefixes ??= new List<string>();
                        ReadStrings(property.Value, property.Key, prefixes);
                        break;
                    case "mapped":
                        ReadSection(property.Value, property.Key, mapped);
                        break;
                    case "unsupported":
                        ReadSection(property.Value, property.Key, unsupported);
                        break;
                    case "renamedNamespaces":
                        ReadRenamed(property.Value, renamed);
                        break;
                    default:
                        throw new JsonParseException("Unknown property '" + property.Key + "'", property.Value.Position);
                }
            }

            if (prefixes is null || prefixes.Count == 0)
            {
                prefixes = [PortMap.DefaultSourceNamespacePrefix];
            }

            return new PortMap(prefixes, mapped, unsupported, renamed);
        }

        private static void ReadSection(JsonValue value, string name, PortMapSection section)
        {
            if (value.Kind != JsonValueKind.Object)
            {
                throw new JsonParseException("'" + name + "' must be an object", value.Position);
            }

            var items = new List<string>();
            for (var i = 0; i < value.Properties.Count; i++)
            {
                var property = value.Properties[i];
                HashSet<string> target = property.Key switch
                {
                    "types" => section.Types,
                    "namespaces" => section.Namespaces,
                    "members" => section.Members,
                    _ => throw new JsonParseException("Unknown property '" + name + "." + property.Key + "'", property.Value.Position),
                };

                items.Clear();
                ReadStrings(property.Value, name + "." + property.Key, items);
                target.UnionWith(items);
            }
        }

        private static void ReadRenamed(JsonValue value, Dictionary<string, string> renamed)
        {
            if (value.Kind != JsonValueKind.Object)
            {
                throw new JsonParseException("'renamedNamespaces' must be an object", value.Position);
            }

            for (var i = 0; i < value.Properties.Count; i++)
            {
                var property = value.Properties[i];
                if (property.Value.Kind != JsonValueKind.String || string.IsNullOrWhiteSpace(property.Value.Text) || string.IsNullOrWhiteSpace(property.Key))
                {
                    throw new JsonParseException("'renamedNamespaces." + property.Key + "' must be a non-empty string", property.Value.Position);
                }

                renamed[property.Key.Trim()] = property.Value.Text!.Trim();
            }
        }

        private static void ReadStrings(JsonValue value, string name, List<string> target)
        {
            if (value.Kind != JsonValueKind.Array)
            {
                throw new JsonParseException("'" + name + "' must be an array of strings", value.Position);
            }

            for (var i = 0; i < value.Items.Count; i++)
            {
                var item = value.Items[i];
                if (item.Kind != JsonValueKind.String || string.IsNullOrWhiteSpace(item.Text))
                {
                    throw new JsonParseException("'" + name + "' must only contain non-empty strings", item.Position);
                }

                target.Add(item.Text!.Trim());
            }
        }
    }
}
