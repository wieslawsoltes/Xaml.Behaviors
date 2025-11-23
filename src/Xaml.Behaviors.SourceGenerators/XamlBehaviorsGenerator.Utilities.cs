// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Xaml.Behaviors.SourceGenerators
{
    public partial class XamlBehaviorsGenerator
    {
        private static readonly SymbolDisplayFormat FullyQualifiedNullableFormat =
            SymbolDisplayFormat.FullyQualifiedFormat.WithMiscellaneousOptions(
                SymbolDisplayFormat.FullyQualifiedFormat.MiscellaneousOptions | SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);

        private static string ToDisplayStringWithNullable(ITypeSymbol typeSymbol)
        {
            return typeSymbol.ToDisplayString(FullyQualifiedNullableFormat);
        }

        private static string EscapeIdentifier(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return "arg";
            }

            var kind = SyntaxFacts.GetKeywordKind(name);
            return kind != SyntaxKind.None ? "@" + name : name;
        }

        private static string FormatParameters(ImmutableArray<TriggerParameter> parameters)
        {
            return string.Join(", ", parameters.Select(FormatParameter));
        }

        private static string FormatParameter(TriggerParameter parameter)
        {
            return $"{FormatRefKind(parameter.RefKind)}{parameter.Type} {parameter.Name}";
        }

        private static string FormatArguments(ImmutableArray<TriggerParameter> parameters)
        {
            return string.Join(", ", parameters.Select(FormatArgument));
        }

        private static string FormatArgument(TriggerParameter parameter)
        {
            return $"{FormatRefKind(parameter.RefKind)}{parameter.Name}";
        }

        private static string FormatRefKind(RefKind refKind)
        {
            return refKind switch
            {
                RefKind.Ref => "ref ",
                RefKind.Out => "out ",
                RefKind.In => "in ",
                _ => string.Empty
            };
        }

        private static string FormatRefKindKeyword(RefKind refKind)
        {
            return refKind switch
            {
                RefKind.Ref => "ref",
                RefKind.Out => "out",
                RefKind.In => "in",
                _ => string.Empty
            };
        }

        private static bool IsObjectType(string typeName)
        {
            return typeName == "object" || typeName == "System.Object" || typeName == "global::System.Object";
        }

        private static string TrimNullableAnnotation(string typeName)
        {
            if (typeName.EndsWith("?", StringComparison.Ordinal) && typeName.Length > 0)
            {
                return typeName.Substring(0, typeName.Length - 1);
            }

            return typeName;
        }

        private static string MakeUniqueName(string baseName, INamedTypeSymbol scope)
        {
            var fullyQualified = scope.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            return MakeUniqueName(baseName, fullyQualified);
        }

        private static string MakeUniqueName(string baseName, string scope)
        {
            return $"{baseName}_{ComputeHash(scope)}";
        }

        private static bool IsAssemblyAttribute(SyntaxNode node, string attributeName)
        {
            if (node is not AttributeSyntax attributeSyntax)
                return false;

            if (!attributeSyntax.Name.ToString().Contains(attributeName, StringComparison.Ordinal))
                return false;

            if (attributeSyntax.Parent is AttributeListSyntax list && list.Target?.Identifier.IsKind(SyntaxKind.AssemblyKeyword) == true)
                return true;

            return false;
        }

        private static bool IsAttribute(AttributeData attributeData, string metadataName)
        {
            var attributeClass = attributeData.AttributeClass;
            if (attributeClass == null)
                return false;

            var displayName = attributeClass.ToDisplayString();
            if (displayName == metadataName)
                return true;

            var fullyQualified = attributeClass.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            if (fullyQualified.StartsWith("global::", StringComparison.Ordinal))
            {
                fullyQualified = fullyQualified.Substring("global::".Length);
            }

            if (fullyQualified == metadataName)
                return true;

            return attributeClass.Name == metadataName.Split('.').Last();
        }

        private static bool ContainsTypeParameter(ITypeSymbol? typeSymbol)
        {
            if (typeSymbol == null)
            {
                return false;
            }

            if (typeSymbol.TypeKind == TypeKind.TypeParameter)
            {
                return true;
            }

            return typeSymbol switch
            {
                IArrayTypeSymbol array => ContainsTypeParameter(array.ElementType),
                IPointerTypeSymbol pointer => ContainsTypeParameter(pointer.PointedAtType),
                INamedTypeSymbol named => named.IsUnboundGenericType || named.TypeArguments.Any(ContainsTypeParameter),
                _ => false
            };
        }

        private static string CreateSafeIdentifier(string value)
        {
            var sb = new StringBuilder(value.Length);
            foreach (var c in value)
            {
                if (char.IsLetterOrDigit(c) || c == '_')
                {
                    sb.Append(c);
                }
                else
                {
                    sb.Append('_');
                }
            }

            var result = sb.ToString().TrimStart('_');
            if (string.IsNullOrEmpty(result) || char.IsDigit(result[0]))
            {
                result = "_" + result;
            }

            return result;
        }

        private static bool NameMatchesPattern(string name, string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
            {
                return false;
            }

            if (pattern == "*")
            {
                return true;
            }

            var regexMetaIndex = pattern.IndexOfAny(new[] { '.', '$', '^', '[', ']', '(', ')', '+', '?', '{', '}', '|', '\\' });

            if (pattern.Contains('*') && regexMetaIndex == -1)
            {
                var regexPattern = "^" + Regex.Escape(pattern).Replace("\\*", ".*") + "$";
                return RegexIsMatch(name, regexPattern);
            }

            if (string.Equals(name, pattern, StringComparison.Ordinal))
            {
                return true;
            }

            if (regexMetaIndex != -1)
            {
                return RegexIsMatch(name, pattern);
            }

            return false;
        }

        private static bool RegexIsMatch(string input, string pattern)
        {
            try
            {
                return Regex.IsMatch(input, pattern);
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        private static Diagnostic? ValidateStyledElementTriggerType(INamedTypeSymbol symbol, Location? location)
        {
            if (!InheritsFrom(symbol, "Avalonia.Xaml.Interactivity.StyledElementTrigger"))
            {
                return Diagnostic.Create(InvalidMultiDataTriggerTargetDiagnostic, location ?? Location.None, symbol.ToDisplayString());
            }

            return null;
        }

        private static string GetTypeNamePrefix(INamedTypeSymbol typeSymbol)
        {
            var name = typeSymbol.Name;
            var tickIndex = name.IndexOf('`');
            if (tickIndex >= 0)
            {
                name = name.Substring(0, tickIndex);
            }

            if (typeSymbol.ContainingType != null)
            {
                return GetTypeNamePrefix(typeSymbol.ContainingType) + name;
            }

            return name;
        }

        private static Diagnostic? ValidateStyledElementActionType(INamedTypeSymbol symbol, Location? location)
        {
            if (!InheritsFrom(symbol, "Avalonia.Xaml.Interactivity.StyledElementAction"))
            {
                return Diagnostic.Create(InvalidInvokeCommandTargetDiagnostic, location ?? Location.None, symbol.ToDisplayString());
            }

            return null;
        }

        private static Diagnostic? ValidateEvaluateMethod(INamedTypeSymbol symbol, Location? location)
        {
            var candidates = symbol.GetMembers("Evaluate").OfType<IMethodSymbol>().ToList();
            var match = candidates.FirstOrDefault(m =>
                !m.IsStatic &&
                m.Parameters.Length == 0 &&
                m.ReturnType.SpecialType == SpecialType.System_Boolean);

            if (match == null)
            {
                return Diagnostic.Create(MultiDataTriggerEvaluateMissingDiagnostic, location ?? Location.None, symbol.ToDisplayString());
            }

            return null;
        }

        private static bool InheritsFrom(INamedTypeSymbol symbol, string fullName)
        {
            var current = symbol;
            while (current != null)
            {
                if (string.Equals(current.ToDisplayString(), fullName, StringComparison.Ordinal))
                {
                    return true;
                }
                current = current.BaseType;
            }
            return false;
        }

        private static bool IsPartial(INamedTypeSymbol symbol)
        {
            foreach (var syntaxRef in symbol.DeclaringSyntaxReferences)
            {
                if (syntaxRef.GetSyntax() is TypeDeclarationSyntax typeDecl &&
                    typeDecl.Modifiers.Any(SyntaxKind.PartialKeyword))
                {
                    return true;
                }
            }

            return false;
        }

        private static Diagnostic? EnsurePartial(INamedTypeSymbol symbol, Location location, string attributeName)
        {
            if (!IsPartial(symbol))
            {
                return Diagnostic.Create(PartialTypeRequiredDiagnostic, location, symbol.ToDisplayString(), attributeName);
            }

            return null;
        }

        private static bool IsAccessibleToGenerator(ISymbol symbol)
        {
            return symbol.DeclaredAccessibility is Accessibility.Public
                or Accessibility.Internal;
        }

        private static bool IsAccessibleType(ITypeSymbol typeSymbol)
        {
            if (typeSymbol is ITypeParameterSymbol)
                return false;

            if (typeSymbol.SpecialType != SpecialType.None)
                return true;

            if (typeSymbol is ITypeParameterSymbol)
                return false;

            if (typeSymbol is IArrayTypeSymbol array)
                return IsAccessibleType(array.ElementType);

            if (typeSymbol is IPointerTypeSymbol pointer)
                return IsAccessibleType(pointer.PointedAtType);

            if (typeSymbol is INamedTypeSymbol named)
            {
                if (named.DeclaredAccessibility != Accessibility.Public)
                    return false;

                foreach (var arg in named.TypeArguments)
                {
                    if (!IsAccessibleType(arg))
                        return false;
                }

                var current = named.ContainingType;
                while (current != null)
                {
                    if (current.DeclaredAccessibility != Accessibility.Public)
                        return false;
                    current = current.ContainingType;
                }

                return true;
            }

            return false;
        }

        private static bool HasAccessibleSetter(IPropertySymbol propertySymbol)
        {
            if (propertySymbol.SetMethod is null)
            {
                return false;
            }

            return IsAccessibleToGenerator(propertySymbol.SetMethod);
        }

        private static string CreateHintName(string? @namespace, string className)
        {
            var nsPart = string.IsNullOrEmpty(@namespace) ? "global" : @namespace!.Replace('.', '_');
            return $"{nsPart}.{className}.g.cs";
        }

        private static string GetAccessibilityKeyword(INamedTypeSymbol symbol)
        {
            return symbol.DeclaredAccessibility switch
            {
                Accessibility.Public => "public",
                Accessibility.Internal => "internal",
                Accessibility.ProtectedOrInternal => "protected internal",
                Accessibility.ProtectedAndInternal => "private protected",
                _ => "public"
            };
        }

        private static Diagnostic? ValidateTypeAccessibility(INamedTypeSymbol symbol, Location? location)
        {
            var current = symbol;
            while (current != null)
            {
                if (current.DeclaredAccessibility is not (Accessibility.Public or Accessibility.Internal))
                {
                    return Diagnostic.Create(MemberNotAccessibleDiagnostic, location ?? Location.None, current.Name, current.ToDisplayString());
                }

                current = current.ContainingType;
            }

            return null;
        }

        private static Diagnostic? EnsureNotNested(INamedTypeSymbol symbol, Location? location)
        {
            if (symbol.ContainingType != null)
            {
                return Diagnostic.Create(NestedTypeNotSupportedDiagnostic, location ?? Location.None, symbol.ToDisplayString());
            }

            return null;
        }

        private static string CreateMethodSignatureKey(IMethodSymbol method)
        {
            var sb = new StringBuilder();
            sb.Append(method.Name);
            sb.Append('(');
            foreach (var parameter in method.Parameters)
            {
                sb.Append(parameter.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
                sb.Append(';');
            }
            sb.Append(')');
            return sb.ToString();
        }

        private static ImmutableArray<IEventSymbol> FindMatchingEvents(INamedTypeSymbol? type, string pattern)
        {
            var builder = ImmutableArray.CreateBuilder<IEventSymbol>();
            var seenNames = new HashSet<string>(StringComparer.Ordinal);
            var current = type;
            while (current != null)
            {
                foreach (var evt in current.GetMembers().OfType<IEventSymbol>())
                {
                    if (!NameMatchesPattern(evt.Name, pattern))
                        continue;

                    if (seenNames.Add(evt.Name))
                    {
                        builder.Add(evt);
                    }
                }

                current = current.BaseType;
            }

            return builder.ToImmutable();
        }

        private static ImmutableArray<IPropertySymbol> FindMatchingProperties(INamedTypeSymbol? type, string pattern)
        {
            var builder = ImmutableArray.CreateBuilder<IPropertySymbol>();
            var seenNames = new HashSet<string>(StringComparer.Ordinal);
            var current = type;
            while (current != null)
            {
                foreach (var prop in current.GetMembers().OfType<IPropertySymbol>())
                {
                    if (!NameMatchesPattern(prop.Name, pattern))
                        continue;

                    if (seenNames.Add(prop.Name))
                    {
                        builder.Add(prop);
                    }
                }

                current = current.BaseType;
            }

            return builder.ToImmutable();
        }

        private static ImmutableArray<IMethodSymbol> FindMatchingMethods(
            INamedTypeSymbol? type,
            string pattern,
            bool requirePublicAccessibility = true,
            bool requireAccessibleTypes = true)
        {
            var builder = ImmutableArray.CreateBuilder<IMethodSymbol>();
            var seenSignatures = new HashSet<string>(StringComparer.Ordinal);
            var current = type;
            while (current != null)
            {
                foreach (var method in current.GetMembers().OfType<IMethodSymbol>())
                {
                    if (method.MethodKind != MethodKind.Ordinary)
                        continue;

                    if (requirePublicAccessibility && method.DeclaredAccessibility != Accessibility.Public)
                        continue;

                    if (!NameMatchesPattern(method.Name, pattern))
                        continue;

                    if (requireAccessibleTypes && (!IsAccessibleType(method.ReturnType) || method.Parameters.Any(p => !IsAccessibleType(p.Type))))
                        continue;

                    var signature = CreateMethodSignatureKey(method);
                    if (seenSignatures.Add(signature))
                    {
                        builder.Add(method);
                    }
                }

                current = current.BaseType;
            }

            return builder.ToImmutable();
        }

        private static IEventSymbol? FindEvent(INamedTypeSymbol? type, string name)
        {
            while (type != null)
            {
                var evt = type.GetMembers(name).OfType<IEventSymbol>().FirstOrDefault();
                if (evt != null) return evt;

                evt = type.GetMembers().OfType<IEventSymbol>().FirstOrDefault(e => e.Name == name);
                if (evt != null) return evt;

                type = type.BaseType;
            }
            return null;
        }

        private static IPropertySymbol? FindProperty(INamedTypeSymbol? type, string name)
        {
            while (type != null)
            {
                var prop = type.GetMembers(name).OfType<IPropertySymbol>().FirstOrDefault();
                if (prop != null) return prop;

                prop = type.GetMembers().OfType<IPropertySymbol>().FirstOrDefault(p => p.Name == name);
                if (prop != null) return prop;

                var getMethod = type.GetMembers($"get_{name}").OfType<IMethodSymbol>().FirstOrDefault();
                if (getMethod != null && getMethod.AssociatedSymbol is IPropertySymbol associatedProp)
                {
                    return associatedProp;
                }

                type = type.BaseType;
            }
            return null;
        }

        private static ITypeSymbol? FindPropertyType(INamedTypeSymbol? type, string name)
        {
            while (type != null)
            {
                var prop = type.GetMembers(name).OfType<IPropertySymbol>().FirstOrDefault();
                if (prop != null) return prop.Type;

                var getMethod = type.GetMembers($"get_{name}").OfType<IMethodSymbol>().FirstOrDefault();
                if (getMethod != null) return getMethod.ReturnType;

                type = type.BaseType;
            }
            return null;
        }

        private static string ComputeHash(string input)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sb = new StringBuilder(8);
            for (int i = 0; i < 4 && i < bytes.Length; i++)
            {
                sb.Append(bytes[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }
}
