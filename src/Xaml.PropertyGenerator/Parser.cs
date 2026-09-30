// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Xaml.PropertyGenerator
{
    /// <summary>Builds generator models from annotated declarations.</summary>
    internal static class Parser
    {
        public const string AvaloniaObjectName = "Avalonia.AvaloniaObject";
        public const string WinUIDependencyObjectName = "Microsoft.UI.Xaml.DependencyObject";
        public const string WinUIChangedEventArgsName = "Microsoft.UI.Xaml.DependencyPropertyChangedEventArgs";

        private static readonly SymbolDisplayFormat s_typeFormat =
            SymbolDisplayFormat.FullyQualifiedFormat.WithMiscellaneousOptions(
                SymbolDisplayFormat.FullyQualifiedFormat.MiscellaneousOptions |
                SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);

        private static readonly SymbolDisplayFormat s_typeOfFormat = SymbolDisplayFormat.FullyQualifiedFormat;

        public static CandidateModel ParseProperty(GeneratorAttributeSyntaxContext context, PropertyKind kind, CancellationToken cancellationToken)
        {
            var syntax = (PropertyDeclarationSyntax)context.TargetNode;
            var symbol = (IPropertySymbol)context.TargetSymbol;
            var owner = symbol.ContainingType;
            var diagnostics = new List<DiagnosticModel>();

            if (!syntax.Modifiers.Any(SyntaxKind.PartialKeyword) ||
                syntax.AccessorList is null ||
                syntax.AccessorList.Accessors.Any(static a => a.Body is not null || a.ExpressionBody is not null) ||
                syntax.ExpressionBody is not null ||
                syntax.Initializer is not null)
            {
                diagnostics.Add(DiagnosticModel.Create("XPG0001", syntax.Identifier.Parent ?? syntax, symbol.Name));
                return new CandidateModel(null, diagnostics.ToEquatableArray());
            }

            if (!AreContainingTypesPartial(owner, cancellationToken))
            {
                diagnostics.Add(DiagnosticModel.Create("XPG0002", syntax, owner.Name));
                return new CandidateModel(null, diagnostics.ToEquatableArray());
            }

            if (symbol.IsStatic || symbol.IsIndexer || symbol.GetMethod is null)
            {
                diagnostics.Add(DiagnosticModel.Create("XPG0004", syntax, $"{symbol.Name}: generated properties must be non-static, non-indexer properties with a getter"));
                return new CandidateModel(null, diagnostics.ToEquatableArray());
            }

            var platform = DetectPlatform(context.SemanticModel.Compilation, owner);
            var attribute = context.Attributes[0];
            var args = ReadNamedArguments(attribute);

            var lazy = kind == PropertyKind.Direct && GetBool(args, "Lazy");
            if (lazy)
            {
                if (symbol.SetMethod is not null)
                {
                    diagnostics.Add(DiagnosticModel.Create("XPG0004", syntax, $"{symbol.Name}: lazy direct properties must be get-only"));
                    return new CandidateModel(null, diagnostics.ToEquatableArray());
                }

                if (symbol.Type is not INamedTypeSymbol { IsAbstract: false } lazyType ||
                    !lazyType.InstanceConstructors.Any(static c => c.Parameters.Length == 0 && c.DeclaredAccessibility == Accessibility.Public))
                {
                    diagnostics.Add(DiagnosticModel.Create("XPG0004", syntax, $"{symbol.Name}: lazy direct property type must have a public parameterless constructor"));
                    return new CandidateModel(null, diagnostics.ToEquatableArray());
                }
            }

            var defaultValue = ReadDefaultValue(args, syntax, symbol.Name, diagnostics);
            var setter = syntax.AccessorList.Accessors.FirstOrDefault(static a => a.IsKind(SyntaxKind.SetAccessorDeclaration) || a.IsKind(SyntaxKind.InitAccessorDeclaration));
            var getter = syntax.AccessorList.Accessors.First(static a => a.IsKind(SyntaxKind.GetAccessorDeclaration));

            var property = new PropertyModel(
                Kind: kind,
                Name: symbol.Name,
                Type: symbol.Type.ToDisplayString(s_typeFormat),
                TypeOfType: symbol.Type.WithNullableAnnotation(NullableAnnotation.NotAnnotated).ToDisplayString(s_typeOfFormat),
                IsValueType: symbol.Type.IsValueType,
                Modifiers: syntax.Modifiers.ToString(),
                FieldAccessibility: AccessibilityText(symbol.DeclaredAccessibility),
                GetterModifiers: ModifiersPrefix(getter),
                SetterModifiers: setter is null ? null : ModifiersPrefix(setter),
                SetterIsInit: setter is not null && setter.IsKind(SyntaxKind.InitAccessorDeclaration),
                DefaultValue: defaultValue,
                DefaultBindingMode: GetInt(args, "DefaultBindingMode"),
                Inherits: GetBool(args, "Inherits"),
                Content: GetBool(args, "Content"),
                ResolveByName: GetBool(args, "ResolveByName"),
                AssignBinding: GetBool(args, "AssignBinding"),
                Lazy: lazy,
                HasChangedHook: HasChangedHook(owner, symbol.Name, 2),
                HostType: string.Empty);

            return new CandidateModel(CreateTypeModel(platform, owner, syntax, property, context.SemanticModel.Compilation), diagnostics.ToEquatableArray());
        }

        public static IEnumerable<CandidateModel> ParseAttached(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken)
        {
            var syntax = (ClassDeclarationSyntax)context.TargetNode;
            var owner = (INamedTypeSymbol)context.TargetSymbol;
            var compilation = context.SemanticModel.Compilation;

            if (!AreContainingTypesPartial(owner, cancellationToken))
            {
                yield return new CandidateModel(null, new[] { DiagnosticModel.Create("XPG0002", syntax, owner.Name) }.ToEquatableArray());
                yield break;
            }

            var platform = DetectPlatform(compilation, owner);

            foreach (var attribute in context.Attributes)
            {
                if (attribute.ConstructorArguments.Length != 2 ||
                    attribute.ConstructorArguments[0].Value is not string name ||
                    attribute.ConstructorArguments[1].Value is not ITypeSymbol valueType)
                {
                    continue;
                }

                var diagnostics = new List<DiagnosticModel>();
                var args = ReadNamedArguments(attribute);
                var isNullable = GetBool(args, "IsNullable") && !valueType.IsValueType;
                var type = valueType.ToDisplayString(s_typeOfFormat) + (isNullable ? "?" : string.Empty);
                var hostType = args.TryGetValue("HostType", out var host) && host.Value is ITypeSymbol hostSymbol
                    ? hostSymbol.ToDisplayString(s_typeOfFormat)
                    : "global::" + (platform == TargetPlatform.WinUI ? WinUIDependencyObjectName : AvaloniaObjectName);
                var attributeSyntax = attribute.ApplicationSyntaxReference?.GetSyntax(cancellationToken) ?? syntax;

                if (platform == TargetPlatform.Avalonia && owner.IsStatic)
                {
                    diagnostics.Add(DiagnosticModel.Create("XPG0004", attributeSyntax, $"{name}: Avalonia attached properties cannot be owned by a static class"));
                    yield return new CandidateModel(null, diagnostics.ToEquatableArray());
                    continue;
                }

                var property = new PropertyModel(
                    Kind: PropertyKind.Attached,
                    Name: name,
                    Type: type,
                    TypeOfType: valueType.ToDisplayString(s_typeOfFormat),
                    IsValueType: valueType.IsValueType,
                    Modifiers: string.Empty,
                    FieldAccessibility: "public",
                    GetterModifiers: string.Empty,
                    SetterModifiers: string.Empty,
                    SetterIsInit: false,
                    DefaultValue: ReadDefaultValue(args, attributeSyntax, name, diagnostics),
                    DefaultBindingMode: GetInt(args, "DefaultBindingMode"),
                    Inherits: GetBool(args, "Inherits"),
                    Content: false,
                    ResolveByName: false,
                    AssignBinding: false,
                    Lazy: false,
                    HasChangedHook: HasChangedHook(owner, name, 3),
                    HostType: hostType);

                yield return new CandidateModel(CreateTypeModel(platform, owner, syntax, property, compilation), diagnostics.ToEquatableArray());
            }
        }

        public static TargetPlatform DetectPlatform(Compilation compilation, INamedTypeSymbol owner)
        {
            for (var type = owner.BaseType; type is not null; type = type.BaseType)
            {
                var name = type.ToDisplayString();
                if (name == AvaloniaObjectName)
                {
                    return TargetPlatform.Avalonia;
                }

                if (name == WinUIDependencyObjectName)
                {
                    return TargetPlatform.WinUI;
                }
            }

            foreach (var i in owner.AllInterfaces)
            {
                if (i.ToDisplayString() == WinUIDependencyObjectName)
                {
                    return TargetPlatform.WinUI;
                }
            }

            if (compilation.GetTypeByMetadataName(AvaloniaObjectName) is not null)
            {
                return TargetPlatform.Avalonia;
            }

            return compilation.GetTypeByMetadataName(WinUIDependencyObjectName) is not null
                ? TargetPlatform.WinUI
                : TargetPlatform.None;
        }

        private static TypeModel CreateTypeModel(TargetPlatform platform, INamedTypeSymbol owner, SyntaxNode syntax, PropertyModel property, Compilation compilation)
        {
            var containing = new List<TypeDeclarationModel>();
            for (var type = owner; type is not null; type = type.ContainingType)
            {
                var keyword = type.IsRecord
                    ? type.TypeKind == TypeKind.Struct ? "record struct" : "record"
                    : type.TypeKind == TypeKind.Struct ? "struct" : "class";
                if (type.IsStatic)
                {
                    keyword = "static " + keyword;
                }

                var typeParameters = type.TypeParameters.Length == 0
                    ? string.Empty
                    : "<" + string.Join(", ", type.TypeParameters.Select(static p => p.Name)) + ">";
                containing.Insert(0, new TypeDeclarationModel(keyword, type.Name, typeParameters));
            }

            var ns = owner.ContainingNamespace.IsGlobalNamespace ? string.Empty : owner.ContainingNamespace.ToDisplayString();
            var hintName = (ns.Length == 0 ? string.Empty : ns + ".") + string.Join(".", containing.Select(static c => c.Name + (c.TypeParameters.Length == 0 ? string.Empty : "`" + c.TypeParameters.Count(static ch => ch == ',').ToString(CultureInfo.InvariantCulture))));

            return new TypeModel(
                platform,
                hintName,
                ns,
                containing.ToEquatableArray(),
                owner.ToDisplayString(s_typeOfFormat),
                CollectUsings(syntax),
                FindChangedMethod(compilation, owner),
                new[] { property }.ToEquatableArray());
        }

        private static string CollectUsings(SyntaxNode syntax)
        {
            if (syntax.SyntaxTree.GetRoot() is not CompilationUnitSyntax unit)
            {
                return string.Empty;
            }

            var builder = new StringBuilder();
            foreach (var @using in unit.Usings)
            {
                if (@using.GlobalKeyword.IsKind(SyntaxKind.GlobalKeyword))
                {
                    continue;
                }

                builder.Append(@using.WithoutTrivia().ToFullString()).Append('\n');
            }

            return builder.ToString();
        }

        private static string? FindChangedMethod(Compilation compilation, INamedTypeSymbol owner)
        {
            for (var type = owner; type is not null; type = type.BaseType)
            {
                foreach (var member in type.GetMembers("OnPropertyChanged"))
                {
                    if (member is IMethodSymbol { IsStatic: false, Parameters.Length: 1 } method &&
                        method.Parameters[0].Type.ToDisplayString() == WinUIChangedEventArgsName &&
                        compilation.IsSymbolAccessibleWithin(method, owner))
                    {
                        return method.Name;
                    }
                }
            }

            return null;
        }

        private static bool HasChangedHook(INamedTypeSymbol owner, string name, int parameterCount)
        {
            foreach (var member in owner.GetMembers("On" + name + "Changed"))
            {
                if (member is IMethodSymbol method && method.Parameters.Length == parameterCount && method.ReturnsVoid)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool AreContainingTypesPartial(INamedTypeSymbol owner, CancellationToken cancellationToken)
        {
            for (var type = owner; type is not null; type = type.ContainingType)
            {
                var isPartial = false;
                foreach (var reference in type.DeclaringSyntaxReferences)
                {
                    if (reference.GetSyntax(cancellationToken) is TypeDeclarationSyntax declaration &&
                        declaration.Modifiers.Any(SyntaxKind.PartialKeyword))
                    {
                        isPartial = true;
                        break;
                    }
                }

                if (!isPartial)
                {
                    return false;
                }
            }

            return true;
        }

        private static Dictionary<string, TypedConstant> ReadNamedArguments(AttributeData attribute)
        {
            var result = new Dictionary<string, TypedConstant>();
            foreach (var pair in attribute.NamedArguments)
            {
                result[pair.Key] = pair.Value;
            }

            return result;
        }

        private static bool GetBool(Dictionary<string, TypedConstant> args, string name)
            => args.TryGetValue(name, out var value) && value.Value is bool b && b;

        private static int GetInt(Dictionary<string, TypedConstant> args, string name)
            => args.TryGetValue(name, out var value) && value.Value is int i ? i : 0;

        private static string? ReadDefaultValue(Dictionary<string, TypedConstant> args, SyntaxNode syntax, string name, List<DiagnosticModel> diagnostics)
        {
            if (args.TryGetValue("DefaultValueExpression", out var expression) && expression.Value is string text && text.Length > 0)
            {
                return text;
            }

            if (!args.TryGetValue("DefaultValue", out var constant))
            {
                return null;
            }

            var formatted = FormatConstant(constant);
            if (formatted is null)
            {
                diagnostics.Add(DiagnosticModel.Create("XPG0004", syntax, $"{name}: unsupported DefaultValue, use DefaultValueExpression"));
            }

            return formatted;
        }

        private static string? FormatConstant(TypedConstant constant)
        {
            if (constant.IsNull)
            {
                return "null";
            }

            switch (constant.Kind)
            {
                case TypedConstantKind.Enum:
                    return "(" + constant.Type!.ToDisplayString(s_typeOfFormat) + ")(" +
                           SymbolDisplay.FormatPrimitive(constant.Value!, quoteStrings: true, useHexadecimalNumbers: false) + ")";
                case TypedConstantKind.Type:
                    return "typeof(" + ((ITypeSymbol)constant.Value!).ToDisplayString(s_typeOfFormat) + ")";
                case TypedConstantKind.Primitive:
                    return constant.Value switch
                    {
                        double d => d.ToString("R", CultureInfo.InvariantCulture) + "d",
                        float f => f.ToString("R", CultureInfo.InvariantCulture) + "f",
                        decimal m => m.ToString(CultureInfo.InvariantCulture) + "m",
                        long l => l.ToString(CultureInfo.InvariantCulture) + "L",
                        ulong ul => ul.ToString(CultureInfo.InvariantCulture) + "UL",
                        uint ui => ui.ToString(CultureInfo.InvariantCulture) + "U",
                        _ => SymbolDisplay.FormatPrimitive(constant.Value!, quoteStrings: true, useHexadecimalNumbers: false),
                    };
                default:
                    return null;
            }
        }

        private static string ModifiersPrefix(AccessorDeclarationSyntax accessor)
            => accessor.Modifiers.Count == 0 ? string.Empty : accessor.Modifiers.ToString() + " ";

        private static string AccessibilityText(Accessibility accessibility) => accessibility switch
        {
            Accessibility.Public => "public",
            Accessibility.Internal => "internal",
            Accessibility.Protected => "protected",
            Accessibility.ProtectedOrInternal => "protected internal",
            Accessibility.ProtectedAndInternal => "private protected",
            _ => "private",
        };
    }
}
