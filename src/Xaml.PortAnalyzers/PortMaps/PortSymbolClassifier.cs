// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Concurrent;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Xaml.PortAnalyzers.PortMaps
{
    /// <summary>
    /// Classifies referenced symbols against a <see cref="PortMap"/>. Results are cached per symbol.
    /// </summary>
    internal sealed class PortSymbolClassifier
    {
        private readonly PortMap _map;
        private readonly Func<SyntaxTree, bool> _isSharedTree;
        private readonly ConcurrentDictionary<ISymbol, PortSymbolClassification> _cache = new(SymbolEqualityComparer.Default);

        /// <summary>
        /// Initializes a new instance of the <see cref="PortSymbolClassifier"/> class.
        /// </summary>
        /// <param name="map">The port map.</param>
        /// <param name="isSharedTree">Returns whether a source file of the compilation is shared with the port.</param>
        public PortSymbolClassifier(PortMap map, Func<SyntaxTree, bool> isSharedTree)
        {
            _map = map;
            _isSharedTree = isSharedTree;
        }

        /// <summary>
        /// Classifies a referenced type or member. Namespaces, locals, parameters and similar symbols are ignored.
        /// </summary>
        public PortSymbolClassification Classify(ISymbol symbol)
        {
            var normalized = Normalize(symbol);
            if (normalized is null || normalized.Kind == SymbolKind.Namespace)
            {
                return PortSymbolClassification.Ignored;
            }

            if (_cache.TryGetValue(normalized, out var cached))
            {
                return cached;
            }

            var result = normalized is INamedTypeSymbol type ? ClassifyType(type) : ClassifyMember(normalized);
            return _cache.GetOrAdd(normalized, result);
        }

        /// <summary>
        /// Classifies a namespace referenced by a <c>using</c> directive or used as a qualifier.
        /// </summary>
        public PortSymbolClassification ClassifyNamespace(INamespaceSymbol namespaceSymbol)
        {
            if (namespaceSymbol.IsGlobalNamespace)
            {
                return PortSymbolClassification.Ignored;
            }

            if (_cache.TryGetValue(namespaceSymbol, out var cached))
            {
                return cached;
            }

            var name = GetNamespaceName(namespaceSymbol);
            PortSymbolClassification result;
            if (!_map.IsSourceNamespace(name))
            {
                result = PortSymbolClassification.Ignored;
            }
            else if (_map.Unsupported.ContainsNamespace(name))
            {
                result = new PortSymbolClassification(PortSymbolStatus.Unsupported, name, null);
            }
            else if (_map.Mapped.ContainsNamespace(name))
            {
                result = PortSymbolClassification.Mapped;
            }
            else
            {
                var renamed = _map.GetRenamedNamespace(name);
                result = renamed is null
                    ? new PortSymbolClassification(PortSymbolStatus.NotMapped, name, null)
                    : new PortSymbolClassification(PortSymbolStatus.Renamed, name, renamed);
            }

            return _cache.GetOrAdd(namespaceSymbol, result);
        }

        /// <summary>
        /// Gets the port map name of a type: namespace, containing types and name, without type arguments.
        /// </summary>
        public static string GetTypeName(INamedTypeSymbol type)
        {
            var builder = new StringBuilder();
            AppendTypeName(builder, type);
            return builder.ToString();
        }

        /// <summary>
        /// Gets the fully qualified name of a namespace.
        /// </summary>
        public static string GetNamespaceName(INamespaceSymbol? namespaceSymbol)
        {
            if (namespaceSymbol is null || namespaceSymbol.IsGlobalNamespace)
            {
                return string.Empty;
            }

            var parent = GetNamespaceName(namespaceSymbol.ContainingNamespace);
            return parent.Length == 0 ? namespaceSymbol.Name : parent + "." + namespaceSymbol.Name;
        }

        private static ISymbol? Normalize(ISymbol? symbol)
        {
            while (true)
            {
                switch (symbol)
                {
                    case null:
                        return null;
                    case IAliasSymbol alias:
                        symbol = alias.Target;
                        continue;
                    case IArrayTypeSymbol array:
                        symbol = array.ElementType;
                        continue;
                    case IPointerTypeSymbol pointer:
                        symbol = pointer.PointedAtType;
                        continue;
                    case INamespaceSymbol:
                        return symbol;
                    case INamedTypeSymbol type:
                        return type.TypeKind == TypeKind.Error ? null : type.OriginalDefinition;
                    case IMethodSymbol method:
                        switch (method.MethodKind)
                        {
                            case MethodKind.Constructor:
                            case MethodKind.StaticConstructor:
                            case MethodKind.Destructor:
                                symbol = method.ContainingType;
                                continue;
                            case MethodKind.PropertyGet:
                            case MethodKind.PropertySet:
                            case MethodKind.EventAdd:
                            case MethodKind.EventRemove:
                            case MethodKind.EventRaise:
                                symbol = method.AssociatedSymbol;
                                continue;
                            case MethodKind.AnonymousFunction:
                            case MethodKind.LocalFunction:
                                return null;
                        }

                        return (method.ReducedFrom ?? method).OriginalDefinition;
                    case IPropertySymbol:
                    case IFieldSymbol:
                    case IEventSymbol:
                        return symbol.ContainingType is null ? null : symbol.OriginalDefinition;
                    default:
                        return null;
                }
            }
        }

        private PortSymbolClassification ClassifyType(INamedTypeSymbol type)
        {
            var namespaceName = GetNamespaceName(type.ContainingNamespace);
            if (!_map.IsSourceNamespace(namespaceName))
            {
                return PortSymbolClassification.Ignored;
            }

            if (IsDeclaredInSharedSource(type))
            {
                return PortSymbolClassification.Mapped;
            }

            if (IsTypeListed(_map.Unsupported, type, includeContainingTypes: true) || _map.Unsupported.ContainsNamespace(namespaceName))
            {
                return new PortSymbolClassification(PortSymbolStatus.Unsupported, GetTypeName(type), null);
            }

            if (IsTypeListed(_map.Mapped, type, includeContainingTypes: false)
                || _map.Mapped.ContainsNamespace(namespaceName)
                || _map.GetRenamedNamespace(namespaceName) is not null)
            {
                return PortSymbolClassification.Mapped;
            }

            return new PortSymbolClassification(PortSymbolStatus.NotMapped, GetTypeName(type), null);
        }

        private PortSymbolClassification ClassifyMember(ISymbol member)
        {
            var type = member.ContainingType?.OriginalDefinition;
            if (type is null)
            {
                return PortSymbolClassification.Ignored;
            }

            var namespaceName = GetNamespaceName(type.ContainingNamespace);
            if (!_map.IsSourceNamespace(namespaceName))
            {
                return PortSymbolClassification.Ignored;
            }

            if (member.DeclaringSyntaxReferences.Length > 0 ? IsDeclaredInSharedSource(member) : IsDeclaredInSharedSource(type))
            {
                return PortSymbolClassification.Mapped;
            }

            var typeName = GetTypeName(type);
            var arityTypeName = type.Arity > 0 ? typeName + "`" + type.Arity : null;
            var memberName = typeName + "." + member.Name;

            if (IsTypeListed(_map.Unsupported, type, includeContainingTypes: true)
                || _map.Unsupported.ContainsNamespace(namespaceName)
                || IsMemberListed(_map.Unsupported, typeName, arityTypeName, member.Name))
            {
                return new PortSymbolClassification(PortSymbolStatus.Unsupported, memberName, null);
            }

            if (IsMemberListed(_map.Mapped, typeName, arityTypeName, member.Name)
                || _map.Mapped.ContainsNamespace(namespaceName)
                || _map.GetRenamedNamespace(namespaceName) is not null
                || (type.TypeKind == TypeKind.Enum && IsTypeListed(_map.Mapped, type, includeContainingTypes: false)))
            {
                return PortSymbolClassification.Mapped;
            }

            return new PortSymbolClassification(PortSymbolStatus.NotMapped, memberName, null);
        }

        private bool IsDeclaredInSharedSource(ISymbol symbol)
        {
            var references = symbol.DeclaringSyntaxReferences;
            for (var i = 0; i < references.Length; i++)
            {
                if (_isSharedTree(references[i].SyntaxTree))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsTypeListed(PortMapSection section, INamedTypeSymbol type, bool includeContainingTypes)
        {
            if (section.Types.Count == 0)
            {
                return false;
            }

            for (INamedTypeSymbol? current = type; current is not null; current = includeContainingTypes ? current.ContainingType : null)
            {
                var name = GetTypeName(current);
                if (section.Types.Contains(name) || (current.Arity > 0 && section.Types.Contains(name + "`" + current.Arity)))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsMemberListed(PortMapSection section, string typeName, string? arityTypeName, string memberName)
        {
            if (section.Members.Count == 0)
            {
                return false;
            }

            if (section.Members.Contains(typeName + "." + memberName) || section.Members.Contains(typeName + ".*"))
            {
                return true;
            }

            return arityTypeName is not null
                && (section.Members.Contains(arityTypeName + "." + memberName) || section.Members.Contains(arityTypeName + ".*"));
        }

        private static void AppendTypeName(StringBuilder builder, INamedTypeSymbol type)
        {
            if (type.ContainingType is not null)
            {
                AppendTypeName(builder, type.ContainingType);
            }
            else
            {
                builder.Append(GetNamespaceName(type.ContainingNamespace));
            }

            if (builder.Length > 0)
            {
                builder.Append('.');
            }

            builder.Append(type.Name);
        }
    }
}
