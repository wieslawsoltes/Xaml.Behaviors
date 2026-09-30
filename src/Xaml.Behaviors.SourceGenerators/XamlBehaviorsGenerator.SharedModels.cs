// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Xaml.Behaviors.SourceGenerators.Platforms;

namespace Xaml.Behaviors.SourceGenerators
{
    public partial class XamlBehaviorsGenerator
    {
        /// <summary>
        /// The registered (<c>typeof</c>) type of <c>LastError</c> properties. WinUI registers dependency properties with
        /// a trimming-annotated <see cref="System.Type"/>; <c>System.Exception</c> would pull trim warnings
        /// (<c>Exception.TargetSite</c>) into user code, so the value is registered as <c>object</c> while the CLR
        /// accessor stays typed. Avalonia ignores this value.
        /// </summary>
        private const string ExceptionPropertyTypeOf = "object";

        private record TriggerPropertyInfo(string Name, string Type, string FieldName, bool RequiresInternal, string? TypeOf = null)
        {
            public PropertySpec ToPropertySpec() => new(Name, Type, TypeOf ?? TrimNullableAnnotation(Type));
        }

        /// <summary>
        /// Reports the diagnostics that only apply to the resolved platform.
        /// </summary>
        /// <returns><c>true</c> when an error was reported and no source must be generated.</returns>
        private static bool ReportPlatformDiagnostics(SourceProductionContext spc, PlatformDiagnostics? diagnostics, TargetPlatform platform)
        {
            if (diagnostics is null)
            {
                return false;
            }

            var platformDiagnostics = diagnostics.For(platform);
            foreach (var diagnostic in platformDiagnostics)
            {
                spc.ReportDiagnostic(diagnostic);
            }

            return platformDiagnostics.Any(d => d.Severity == DiagnosticSeverity.Error);
        }

        /// <summary>
        /// Creates the per platform diagnostics reported when a type derives from neither the Avalonia nor the WinUI base class.
        /// </summary>
        private static PlatformDiagnostics? ValidateInteractivityBaseType(
            INamedTypeSymbol symbol,
            Location? location,
            string avaloniaBaseType,
            string winUIBaseType,
            DiagnosticDescriptor avaloniaDescriptor,
            string attributeName)
        {
            if (InheritsFrom(symbol, avaloniaBaseType) || InheritsFrom(symbol, winUIBaseType))
            {
                return null;
            }

            var loc = location ?? Location.None;
            return new PlatformDiagnostics(
                ImmutableArray.Create(Diagnostic.Create(avaloniaDescriptor, loc, symbol.ToDisplayString())),
                ImmutableArray.Create(Diagnostic.Create(InvalidWinUIBaseTypeDiagnostic, loc, symbol.ToDisplayString(), winUIBaseType, attributeName)));
        }
    }
}
