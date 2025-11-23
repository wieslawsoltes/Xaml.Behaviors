// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Microsoft.CodeAnalysis;

namespace Xaml.Behaviors.SourceGenerators
{
    public partial class XamlBehaviorsGenerator
    {
        internal static class AccessibilityHelper
        {
            public static bool HasInternalAccess(Compilation? compilation, IAssemblySymbol assemblySymbol)
            {
                if (compilation is null)
                {
                    return false;
                }

                return assemblySymbol.GivesAccessTo(compilation.Assembly);
            }

            public static bool IsSymbolAccessible(ISymbol symbol, Compilation? compilation)
            {
                return symbol.DeclaredAccessibility switch
                {
                    Accessibility.Public => true,
                    Accessibility.Internal => HasInternalAccess(compilation, symbol.ContainingAssembly),
                    _ => false
                };
            }

            public static Diagnostic? ValidateTypeAccessibility(INamedTypeSymbol symbol, Location? location, Compilation? compilation)
            {
                var current = symbol;
                while (current != null)
                {
                    if (current.DeclaredAccessibility is not (Accessibility.Public or Accessibility.Internal))
                    {
                        return Diagnostic.Create(MemberNotAccessibleDiagnostic, location ?? Location.None, current.Name, current.ToDisplayString());
                    }

                    if (current.DeclaredAccessibility == Accessibility.Internal && !HasInternalAccess(compilation, current.ContainingAssembly))
                    {
                        return Diagnostic.Create(MemberNotAccessibleDiagnostic, location ?? Location.None, current.Name, current.ToDisplayString());
                    }

                    current = current.ContainingType;
                }

                return null;
            }

            public static bool HasAccessibleSetter(IPropertySymbol propertySymbol, Compilation? compilation)
            {
                if (propertySymbol.SetMethod is null)
                {
                    return false;
                }

                return IsSymbolAccessible(propertySymbol.SetMethod, compilation);
            }

            public static bool IsPubliclyAccessibleType(ITypeSymbol typeSymbol)
            {
                if (typeSymbol is ITypeParameterSymbol)
                    return false;

                if (typeSymbol.SpecialType != SpecialType.None)
                    return true;

                if (typeSymbol is IArrayTypeSymbol array)
                    return IsPubliclyAccessibleType(array.ElementType);

                if (typeSymbol is IPointerTypeSymbol pointer)
                    return IsPubliclyAccessibleType(pointer.PointedAtType);

                if (typeSymbol is INamedTypeSymbol named)
                {
                    if (named.DeclaredAccessibility != Accessibility.Public)
                        return false;

                    foreach (var arg in named.TypeArguments)
                    {
                        if (!IsPubliclyAccessibleType(arg))
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
        }
    }
}
