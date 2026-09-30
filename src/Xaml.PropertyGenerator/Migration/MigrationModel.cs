// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

namespace Xaml.PropertyGenerator.Migration
{
    /// <summary>
    /// Describes a hand-written Avalonia property that can be converted to a generated property.
    /// </summary>
    /// <remarks>
    /// A candidate is only produced when the conversion is semantics preserving: the registration only uses options
    /// supported by the generator, the CLR accessors only forward to the property system and backing fields are not
    /// used anywhere else.
    /// </remarks>
    internal sealed class MigrationModel
    {
        private const string AvaloniaPropertyName = "Avalonia.AvaloniaProperty";
        private const string StyledPropertyName = "Avalonia.StyledProperty<TValue>";
        private const string DirectPropertyName = "Avalonia.DirectProperty<TOwner, TValue>";
        private const string ContentAttributeName = "Avalonia.Metadata.ContentAttribute";
        private const string ResolveByNameAttributeName = "Avalonia.Controls.ResolveByNameAttribute";
        private const string AssignBindingAttributeName = "Avalonia.Data.AssignBindingAttribute";

        private MigrationModel(
            FieldDeclarationSyntax registration,
            PropertyDeclarationSyntax property,
            bool isDirect,
            IReadOnlyList<string> attributeArguments,
            IReadOnlyList<AttributeSyntax> removedAttributes,
            FieldDeclarationSyntax? backingField,
            string? getterModifiers,
            string? setterModifiers,
            bool setterIsInit)
        {
            Registration = registration;
            Property = property;
            IsDirect = isDirect;
            AttributeArguments = attributeArguments;
            RemovedAttributes = removedAttributes;
            BackingField = backingField;
            GetterModifiers = getterModifiers;
            SetterModifiers = setterModifiers;
            SetterIsInit = setterIsInit;
        }

        /// <summary>Gets the <c>{Name}Property</c> field declaration.</summary>
        public FieldDeclarationSyntax Registration { get; }

        /// <summary>Gets the CLR property.</summary>
        public PropertyDeclarationSyntax Property { get; }

        /// <summary>Gets a value indicating whether the property is a direct property.</summary>
        public bool IsDirect { get; }

        /// <summary>Gets the named arguments of the generated attribute.</summary>
        public IReadOnlyList<string> AttributeArguments { get; }

        /// <summary>Gets the Avalonia attributes folded into the generated attribute.</summary>
        public IReadOnlyList<AttributeSyntax> RemovedAttributes { get; }

        /// <summary>Gets the backing field of a direct property.</summary>
        public FieldDeclarationSyntax? BackingField { get; }

        /// <summary>Gets the getter modifiers (with trailing space) or an empty string.</summary>
        public string? GetterModifiers { get; }

        /// <summary>Gets the setter modifiers (with trailing space), an empty string, or <c>null</c> without setter.</summary>
        public string? SetterModifiers { get; }

        /// <summary>Gets a value indicating whether the setter is an init accessor.</summary>
        public bool SetterIsInit { get; }

        /// <summary>Gets the generated attribute name.</summary>
        public string AttributeName => IsDirect ? "DirectProperty" : "StyledProperty";

        public static MigrationModel? TryCreate(FieldDeclarationSyntax field, SemanticModel model, CancellationToken cancellationToken)
        {
            if (field.Declaration.Variables.Count != 1 ||
                !field.Modifiers.Any(SyntaxKind.StaticKeyword) ||
                !field.Modifiers.Any(SyntaxKind.ReadOnlyKeyword) ||
                field.Declaration.Variables[0].Initializer?.Value is not InvocationExpressionSyntax invocation ||
                field.Parent is not TypeDeclarationSyntax typeDeclaration)
            {
                return null;
            }

            if (model.GetDeclaredSymbol(field.Declaration.Variables[0], cancellationToken) is not IFieldSymbol fieldSymbol ||
                fieldSymbol.Type is not INamedTypeSymbol fieldType ||
                !fieldSymbol.Name.EndsWith("Property", System.StringComparison.Ordinal))
            {
                return null;
            }

            var fieldTypeName = fieldType.OriginalDefinition.ToDisplayString();
            var isDirect = fieldTypeName == DirectPropertyName;
            if (!isDirect && fieldTypeName != StyledPropertyName)
            {
                return null;
            }

            if (model.GetOperation(invocation, cancellationToken) is not IInvocationOperation operation ||
                operation.TargetMethod.ContainingType.ToDisplayString() != AvaloniaPropertyName ||
                operation.TargetMethod.Name != (isDirect ? "RegisterDirect" : "Register") ||
                operation.TargetMethod.TypeArguments.Length != 2)
            {
                return null;
            }

            var owner = fieldSymbol.ContainingType;
            if (!SymbolEqualityComparer.Default.Equals(operation.TargetMethod.TypeArguments[0].OriginalDefinition, owner.OriginalDefinition))
            {
                return null;
            }

            var propertyName = fieldSymbol.Name.Substring(0, fieldSymbol.Name.Length - "Property".Length);
            var property = typeDeclaration.Members.OfType<PropertyDeclarationSyntax>().FirstOrDefault(p => p.Identifier.ValueText == propertyName);
            if (property is null ||
                model.GetDeclaredSymbol(property, cancellationToken) is not { } propertySymbol ||
                propertySymbol.IsStatic ||
                propertySymbol.IsAbstract ||
                propertySymbol.IsExtern ||
                property.Modifiers.Any(SyntaxKind.PartialKeyword) ||
                propertySymbol.DeclaredAccessibility != fieldSymbol.DeclaredAccessibility ||
                !SymbolEqualityComparer.Default.Equals(propertySymbol.Type, operation.TargetMethod.TypeArguments[1]))
            {
                return null;
            }

            var arguments = new List<string>();
            string? nameArgument = null;
            foreach (var argument in operation.Arguments)
            {
                var explicitArgument = argument.ArgumentKind == ArgumentKind.Explicit;
                switch (argument.Parameter?.Name)
                {
                    case "name":
                        nameArgument = argument.Value.ConstantValue.HasValue ? argument.Value.ConstantValue.Value as string : null;
                        break;
                    case "defaultValue" when explicitArgument && !isDirect:
                    {
                        var defaultValue = FormatDefaultValue(argument.Value);
                        if (defaultValue is not null)
                        {
                            arguments.Add(defaultValue);
                        }

                        break;
                    }

                    case "unsetValue" when explicitArgument:
                        // Direct properties with a custom unset value are not supported by the generator.
                        return null;
                    case "inherits" when explicitArgument:
                        if (argument.Value.ConstantValue is not { HasValue: true, Value: bool inherits })
                        {
                            return null;
                        }

                        if (inherits)
                        {
                            arguments.Add("Inherits = true");
                        }

                        break;
                    case "defaultBindingMode" when explicitArgument:
                        if (argument.Value.ConstantValue is not { HasValue: true, Value: int mode } || mode < 0 || mode > 3)
                        {
                            return null;
                        }

                        if (mode != 0)
                        {
                            arguments.Add("DefaultBindingMode = PropertyBindingMode." + BindingModeName(mode));
                        }

                        break;
                    case "enableDataValidation" when explicitArgument:
                        if (argument.Value.ConstantValue is not { HasValue: true, Value: false })
                        {
                            return null;
                        }

                        break;
                    case "getter":
                    case "setter":
                        break;
                    default:
                        if (explicitArgument)
                        {
                            // validate, coerce, notifying and other options are not supported by the generator.
                            return null;
                        }

                        break;
                }
            }

            if (nameArgument != propertyName)
            {
                return null;
            }

            var removedAttributes = new List<AttributeSyntax>();
            foreach (var attribute in property.AttributeLists.SelectMany(static l => l.Attributes))
            {
                var attributeType = model.GetTypeInfo(attribute, cancellationToken).Type?.ToDisplayString();
                switch (attributeType)
                {
                    case ContentAttributeName:
                        arguments.Add("Content = true");
                        removedAttributes.Add(attribute);
                        break;
                    case ResolveByNameAttributeName when !isDirect:
                        arguments.Add("ResolveByName = true");
                        removedAttributes.Add(attribute);
                        break;
                    case AssignBindingAttributeName when !isDirect:
                        arguments.Add("AssignBinding = true");
                        removedAttributes.Add(attribute);
                        break;
                }
            }

            return isDirect
                ? CreateDirect(field, property, propertySymbol, fieldSymbol, operation, arguments, removedAttributes, model, cancellationToken)
                : CreateStyled(field, property, fieldSymbol, arguments, removedAttributes, model, cancellationToken);
        }

        private static MigrationModel? CreateStyled(
            FieldDeclarationSyntax field,
            PropertyDeclarationSyntax property,
            IFieldSymbol fieldSymbol,
            List<string> arguments,
            List<AttributeSyntax> removedAttributes,
            SemanticModel model,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax? getterExpression;
            AccessorDeclarationSyntax? getter = null;
            AccessorDeclarationSyntax? setter = null;

            if (property.ExpressionBody is not null)
            {
                getterExpression = property.ExpressionBody.Expression;
            }
            else if (property.AccessorList is not null)
            {
                foreach (var accessor in property.AccessorList.Accessors)
                {
                    if (accessor.AttributeLists.Count > 0)
                    {
                        return null;
                    }

                    if (accessor.IsKind(SyntaxKind.GetAccessorDeclaration))
                    {
                        getter = accessor;
                    }
                    else if (accessor.IsKind(SyntaxKind.SetAccessorDeclaration) || accessor.IsKind(SyntaxKind.InitAccessorDeclaration))
                    {
                        setter = accessor;
                    }
                    else
                    {
                        return null;
                    }
                }

                getterExpression = getter is null ? null : SingleExpression(getter, expectReturn: true);
            }
            else
            {
                return null;
            }

            if (getterExpression is null || !IsInvocationOf(StripCast(getterExpression), "GetValue", fieldSymbol, model, cancellationToken, out var getArguments) || getArguments.Count != 1)
            {
                return null;
            }

            if (setter is not null)
            {
                var setterExpression = SingleExpression(setter, expectReturn: false);
                if (setterExpression is null ||
                    !IsInvocationOf(setterExpression, "SetValue", fieldSymbol, model, cancellationToken, out var setArguments) ||
                    setArguments.Count != 2 ||
                    setArguments[1].Expression is not IdentifierNameSyntax { Identifier.ValueText: "value" })
                {
                    return null;
                }
            }

            return new MigrationModel(
                field,
                property,
                isDirect: false,
                arguments,
                removedAttributes,
                backingField: null,
                getterModifiers: getter is null ? string.Empty : Prefix(getter.Modifiers),
                setterModifiers: setter is null ? null : Prefix(setter.Modifiers),
                setterIsInit: setter?.IsKind(SyntaxKind.InitAccessorDeclaration) == true);
        }

        private static MigrationModel? CreateDirect(
            FieldDeclarationSyntax field,
            PropertyDeclarationSyntax property,
            IPropertySymbol propertySymbol,
            IFieldSymbol fieldSymbol,
            IInvocationOperation registration,
            List<string> arguments,
            List<AttributeSyntax> removedAttributes,
            SemanticModel model,
            CancellationToken cancellationToken)
        {
            var hasSetterDelegate = registration.Arguments.Any(static a => a.Parameter?.Name == "setter" && a.ArgumentKind == ArgumentKind.Explicit && !a.Value.ConstantValue.HasValue);

            ExpressionSyntax? getterExpression;
            AccessorDeclarationSyntax? getter = null;
            AccessorDeclarationSyntax? setter = null;
            if (property.ExpressionBody is not null)
            {
                getterExpression = property.ExpressionBody.Expression;
            }
            else if (property.AccessorList is not null)
            {
                foreach (var accessor in property.AccessorList.Accessors)
                {
                    if (accessor.AttributeLists.Count > 0)
                    {
                        return null;
                    }

                    if (accessor.IsKind(SyntaxKind.GetAccessorDeclaration))
                    {
                        getter = accessor;
                    }
                    else if (accessor.IsKind(SyntaxKind.SetAccessorDeclaration))
                    {
                        setter = accessor;
                    }
                    else
                    {
                        return null;
                    }
                }

                getterExpression = getter is null ? null : SingleExpression(getter, expectReturn: true);
            }
            else
            {
                return null;
            }

            if (getterExpression is null)
            {
                return null;
            }

            IFieldSymbol? backingSymbol;
            var lazy = false;
            if (getterExpression is AssignmentExpressionSyntax { RawKind: (int)SyntaxKind.CoalesceAssignmentExpression } coalesce)
            {
                // get => _field ??= new T();  or  ??= [];
                if (setter is not null || hasSetterDelegate || !IsParameterlessCreation(coalesce.Right))
                {
                    return null;
                }

                backingSymbol = model.GetSymbolInfo(coalesce.Left, cancellationToken).Symbol as IFieldSymbol;
                lazy = true;
            }
            else
            {
                backingSymbol = model.GetSymbolInfo(getterExpression, cancellationToken).Symbol as IFieldSymbol;
                if (setter is null)
                {
                    return null;
                }

                var setterExpression = SingleExpression(setter, expectReturn: false);
                if (setterExpression is null ||
                    !IsInvocationOf(setterExpression, "SetAndRaise", fieldSymbol, model, cancellationToken, out var setArguments) ||
                    setArguments.Count != 3 ||
                    !setArguments[1].RefKindKeyword.IsKind(SyntaxKind.RefKeyword) ||
                    !SymbolEqualityComparer.Default.Equals(model.GetSymbolInfo(setArguments[1].Expression, cancellationToken).Symbol, backingSymbol) ||
                    setArguments[2].Expression is not IdentifierNameSyntax { Identifier.ValueText: "value" })
                {
                    return null;
                }

                // Avalonia only exposes a setter delegate when the CLR setter is accessible to XAML.
                var setterIsPublic = setter.Modifiers.Count == 0;
                if (setterIsPublic != hasSetterDelegate)
                {
                    return null;
                }
            }

            if (backingSymbol is null ||
                backingSymbol.IsStatic ||
                !SymbolEqualityComparer.Default.Equals(backingSymbol.ContainingType, propertySymbol.ContainingType) ||
                !SymbolEqualityComparer.Default.Equals(backingSymbol.Type.WithNullableAnnotation(NullableAnnotation.NotAnnotated), propertySymbol.Type.WithNullableAnnotation(NullableAnnotation.NotAnnotated)) ||
                backingSymbol.DeclaringSyntaxReferences.Length != 1 ||
                backingSymbol.DeclaringSyntaxReferences[0].GetSyntax(cancellationToken) is not VariableDeclaratorSyntax { Parent: VariableDeclarationSyntax { Variables.Count: 1, Parent: FieldDeclarationSyntax backingField } } backingDeclarator)
            {
                return null;
            }

            if (IsReferencedOutside(backingSymbol, property, backingField, propertySymbol.ContainingType, model.Compilation, cancellationToken))
            {
                return null;
            }

            if (lazy)
            {
                if (backingDeclarator.Initializer is not null)
                {
                    return null;
                }

                arguments.Insert(0, "Lazy = true");
            }
            else if (backingDeclarator.Initializer is { } initializer)
            {
                var backingModel = model.Compilation.GetSemanticModel(initializer.SyntaxTree);
                var operation = backingModel.GetOperation(initializer.Value, cancellationToken);
                if (operation is null)
                {
                    return null;
                }

                var defaultValue = FormatDefaultValue(operation);
                if (defaultValue is not null)
                {
                    arguments.Insert(0, defaultValue);
                }
            }

            return new MigrationModel(
                field,
                property,
                isDirect: true,
                arguments,
                removedAttributes,
                backingField,
                getterModifiers: getter is null ? string.Empty : Prefix(getter.Modifiers),
                setterModifiers: setter is null ? null : Prefix(setter.Modifiers),
                setterIsInit: false);
        }

        private static bool IsReferencedOutside(
            IFieldSymbol backingSymbol,
            PropertyDeclarationSyntax property,
            FieldDeclarationSyntax backingField,
            INamedTypeSymbol type,
            Compilation compilation,
            CancellationToken cancellationToken)
        {
            foreach (var reference in type.DeclaringSyntaxReferences)
            {
                var node = reference.GetSyntax(cancellationToken);
                var model = compilation.GetSemanticModel(node.SyntaxTree);
                foreach (var identifier in node.DescendantNodes().OfType<IdentifierNameSyntax>())
                {
                    if (identifier.Identifier.ValueText != backingSymbol.Name ||
                        property.Span.Contains(identifier.Span) && property.SyntaxTree == identifier.SyntaxTree ||
                        backingField.Span.Contains(identifier.Span) && backingField.SyntaxTree == identifier.SyntaxTree)
                    {
                        continue;
                    }

                    if (SymbolEqualityComparer.Default.Equals(model.GetSymbolInfo(identifier, cancellationToken).Symbol, backingSymbol))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool IsParameterlessCreation(ExpressionSyntax expression) => expression switch
        {
            CollectionExpressionSyntax { Elements.Count: 0 } => true,
            ImplicitObjectCreationExpressionSyntax { ArgumentList.Arguments.Count: 0, Initializer: null } => true,
            ObjectCreationExpressionSyntax { Initializer: null } creation => creation.ArgumentList is null || creation.ArgumentList.Arguments.Count == 0,
            _ => false,
        };

        private static ExpressionSyntax? SingleExpression(AccessorDeclarationSyntax accessor, bool expectReturn)
        {
            if (accessor.ExpressionBody is not null)
            {
                return accessor.ExpressionBody.Expression;
            }

            if (accessor.Body is not { Statements.Count: 1 } body)
            {
                return null;
            }

            return body.Statements[0] switch
            {
                ReturnStatementSyntax { Expression: { } returned } when expectReturn => returned,
                ExpressionStatementSyntax { Expression: var expression } when !expectReturn => expression,
                _ => null,
            };
        }

        private static ExpressionSyntax StripCast(ExpressionSyntax expression)
        {
            while (true)
            {
                switch (expression)
                {
                    case CastExpressionSyntax cast:
                        expression = cast.Expression;
                        continue;
                    case ParenthesizedExpressionSyntax parenthesized:
                        expression = parenthesized.Expression;
                        continue;
                    case PostfixUnaryExpressionSyntax { RawKind: (int)SyntaxKind.SuppressNullableWarningExpression } suppress:
                        expression = suppress.Operand;
                        continue;
                    default:
                        return expression;
                }
            }
        }

        private static bool IsInvocationOf(
            ExpressionSyntax expression,
            string methodName,
            IFieldSymbol propertyField,
            SemanticModel model,
            CancellationToken cancellationToken,
            out SeparatedSyntaxList<ArgumentSyntax> arguments)
        {
            arguments = default;
            if (expression is not InvocationExpressionSyntax invocation)
            {
                return false;
            }

            var name = invocation.Expression switch
            {
                IdentifierNameSyntax identifier => identifier.Identifier.ValueText,
                GenericNameSyntax generic => generic.Identifier.ValueText,
                MemberAccessExpressionSyntax { Expression: ThisExpressionSyntax, Name: var member } => member.Identifier.ValueText,
                _ => null,
            };

            if (name != methodName || invocation.ArgumentList.Arguments.Count == 0)
            {
                return false;
            }

            arguments = invocation.ArgumentList.Arguments;
            return SymbolEqualityComparer.Default.Equals(model.GetSymbolInfo(arguments[0].Expression, cancellationToken).Symbol, propertyField);
        }

        private static string? FormatDefaultValue(IOperation value)
        {
            // Unwrap implicit conversions to the property type; the generator casts the value again.
            while (value is IConversionOperation { IsImplicit: true } conversion)
            {
                value = conversion.Operand;
            }

            var syntax = value.Syntax;
            if (value is IDefaultValueOperation || syntax.IsKind(SyntaxKind.DefaultLiteralExpression))
            {
                return null;
            }

            if (value is IFieldReferenceOperation { Field: { Name: "Empty", ContainingType.SpecialType: SpecialType.System_String } })
            {
                return "DefaultValue = \"\"";
            }

            if (value is ITypeOfOperation || value.ConstantValue.HasValue)
            {
                // Enum and constant expressions are valid attribute arguments as they are written.
                return "DefaultValue = " + syntax.ToString();
            }

            return "DefaultValueExpression = " + SymbolDisplay.FormatLiteral(syntax.ToString(), quote: true);
        }

        private static string BindingModeName(int mode) => mode switch
        {
            1 => "OneWay",
            2 => "TwoWay",
            3 => "OneTime",
            4 => "OneWayToSource",
            _ => "Default",
        };

        private static string Prefix(SyntaxTokenList modifiers) => modifiers.Count == 0 ? string.Empty : modifiers.ToString() + " ";
    }
}
