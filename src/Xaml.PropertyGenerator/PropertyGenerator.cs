// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Xaml.PropertyGenerator
{
    /// <summary>
    /// Generates Avalonia or WinUI/Uno Platform properties from <c>[StyledProperty]</c>, <c>[DirectProperty]</c>
    /// and <c>[AttachedProperty]</c> annotations so the same source compiles for both frameworks.
    /// </summary>
    [Generator(LanguageNames.CSharp)]
    public sealed class PropertyGenerator : IIncrementalGenerator
    {
        /// <inheritdoc />
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            context.RegisterPostInitializationOutput(static ctx =>
            {
                ctx.AddEmbeddedAttributeDefinition();
                ctx.AddSource("Xaml.PropertyGenerator.Attributes.g.cs", SourceText.From(AttributeSources.Source, Encoding.UTF8));
            });

            var styled = context.SyntaxProvider.ForAttributeWithMetadataName(
                AttributeSources.StyledPropertyAttributeName,
                static (node, _) => node is PropertyDeclarationSyntax,
                static (ctx, ct) => Parser.ParseProperty(ctx, PropertyKind.Styled, ct));

            var direct = context.SyntaxProvider.ForAttributeWithMetadataName(
                AttributeSources.DirectPropertyAttributeName,
                static (node, _) => node is PropertyDeclarationSyntax,
                static (ctx, ct) => Parser.ParseProperty(ctx, PropertyKind.Direct, ct));

            var attached = context.SyntaxProvider.ForAttributeWithMetadataName(
                AttributeSources.AttachedPropertyAttributeName,
                static (node, _) => node is ClassDeclarationSyntax,
                static (ctx, ct) => Parser.ParseAttached(ctx, ct).ToEquatableArray());

            var platformOverride = context.AnalyzerConfigOptionsProvider.Select(static (options, _) =>
                options.GlobalOptions.TryGetValue("build_property.XamlPropertyGeneratorPlatform", out var value) ? value.Trim() : string.Empty);

            var candidates = styled.Collect()
                .Combine(direct.Collect())
                .Combine(attached.Collect())
                .Combine(platformOverride);

            context.RegisterSourceOutput(candidates, static (spc, input) =>
            {
                var (((styledItems, directItems), attachedItems), overrideText) = input;
                var platformOverrideValue = ParsePlatform(overrideText);
                var all = new List<CandidateModel>(styledItems.Length + directItems.Length);
                all.AddRange(styledItems);
                all.AddRange(directItems);
                foreach (var group in attachedItems)
                {
                    all.AddRange(group);
                }

                Execute(spc, all, platformOverrideValue);
            });
        }

        private static void Execute(SourceProductionContext context, List<CandidateModel> candidates, TargetPlatform platformOverride)
        {
            var types = new Dictionary<string, (TypeModel Type, List<PropertyModel> Properties)>(StringComparer.Ordinal);
            var order = new List<string>();

            foreach (var candidate in candidates)
            {
                foreach (var diagnostic in candidate.Diagnostics)
                {
                    context.ReportDiagnostic(diagnostic.ToDiagnostic());
                }

                if (candidate.Type is not { } type)
                {
                    continue;
                }

                if (!types.TryGetValue(type.HintName, out var entry))
                {
                    entry = (type, new List<PropertyModel>());
                    types.Add(type.HintName, entry);
                    order.Add(type.HintName);
                }

                entry.Properties.AddRange(type.Properties);
            }

            foreach (var hintName in order)
            {
                var (type, properties) = types[hintName];
                var platform = platformOverride != TargetPlatform.None ? platformOverride : type.Platform;
                if (platform == TargetPlatform.None)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Diagnostics.NoPlatform, Location.None, type.OwnerType));
                    continue;
                }

                if (platform == TargetPlatform.WinUI)
                {
                    foreach (var property in properties)
                    {
                        if (property.Inherits || property.DefaultBindingMode != 0 || property.ResolveByName || property.AssignBinding)
                        {
                            context.ReportDiagnostic(Diagnostic.Create(
                                Diagnostics.UnsupportedOption,
                                Location.None,
                                $"{type.OwnerType}.{property.Name}: Inherits, DefaultBindingMode, ResolveByName and AssignBinding are ignored on WinUI"));
                        }
                    }
                }

                var merged = type with
                {
                    Platform = platform,
                    Properties = properties.OrderBy(static p => p.Name, StringComparer.Ordinal).ToEquatableArray(),
                };

                context.AddSource(hintName + ".XamlProperties.g.cs", SourceText.From(Emitter.Emit(merged), Encoding.UTF8));
            }
        }

        private static TargetPlatform ParsePlatform(string value)
        {
            if (value.Equals("Avalonia", StringComparison.OrdinalIgnoreCase))
            {
                return TargetPlatform.Avalonia;
            }

            if (value.Equals("WinUI", StringComparison.OrdinalIgnoreCase) ||
                value.Equals("Uno", StringComparison.OrdinalIgnoreCase))
            {
                return TargetPlatform.WinUI;
            }

            return TargetPlatform.None;
        }
    }
}
