// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Microsoft.CodeAnalysis;

namespace Xaml.PortAnalyzers
{
    /// <summary>
    /// Descriptors of the diagnostics reported by the Xaml.PortAnalyzers package.
    /// </summary>
    public static class PortDiagnosticDescriptors
    {
        /// <summary>
        /// The diagnostic category shared by all port analyzers.
        /// </summary>
        public const string Category = "Portability";

        private const string HelpLinkBase = "https://github.com/wieslawsoltes/Xaml.Behaviors/blob/master/src/Xaml.PortAnalyzers/README.md#";

        /// <summary>
        /// XPORT001: Type deriving directly from a dependency object must be partial.
        /// </summary>
        public static readonly DiagnosticDescriptor PartialDependencyObject = new(
            id: DiagnosticIds.PartialDependencyObject,
            title: "Type deriving directly from a dependency object must be partial",
            messageFormat: "Type '{0}' derives directly from '{1}' and is shared with port target '{2}'; declare '{3}' partial so the port's dependency object generator can extend it",
            category: Category,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "On Uno Platform, DependencyObject is an interface implemented by a source generator which emits a partial declaration of every type deriving directly from it. The type and all of its containing types must therefore be partial.",
            helpLinkUri: HelpLinkBase + "xport001");

        /// <summary>
        /// XPORT002: Platform-specific API is not guarded for the port target.
        /// </summary>
        public static readonly DiagnosticDescriptor UnguardedPlatformApi = new(
            id: DiagnosticIds.UnguardedPlatformApi,
            title: "Platform-specific API is not guarded for the port target",
            messageFormat: "'{0}' {1} on port target '{2}'; guard it with '#if !{2}' (or the '#else' branch of '#if {2}') or add it to the port map",
            category: Category,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Shared sources are compiled for the port target too. Symbols from the source platform that the port map does not declare as mapped (or declares as unsupported) must only be used in code that is excluded from the port build by preprocessor directives.",
            helpLinkUri: HelpLinkBase + "xport002");

        /// <summary>
        /// XPORT003: Unknown preprocessor symbol in platform condition.
        /// </summary>
        public static readonly DiagnosticDescriptor UnknownPreprocessorSymbol = new(
            id: DiagnosticIds.UnknownPreprocessorSymbol,
            title: "Unknown preprocessor symbol in platform condition",
            messageFormat: "Preprocessor symbol '{0}' is not defined and is not a known platform symbol; check it for typos or add it to 'XamlPortKnownSymbols'",
            category: Category,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "A misspelled symbol in an '#if' or '#elif' condition silently evaluates to false, which usually excludes code from one of the platforms.",
            helpLinkUri: HelpLinkBase + "xport003");

        /// <summary>
        /// XPORT004: Dependency property getter must cast GetValue result.
        /// </summary>
        public static readonly DiagnosticDescriptor GetValueCast = new(
            id: DiagnosticIds.GetValueCast,
            title: "Dependency property getter must cast GetValue result",
            messageFormat: "Getter of property '{0}' returns the result of 'GetValue' without a cast; cast it to '{1}' because 'GetValue' returns 'object' on the port target",
            category: Category,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Avalonia's GetValue is generic and returns the property type, while WinUI's GetValue returns object. Shared getters must cast the value explicitly.",
            helpLinkUri: HelpLinkBase + "xport004");

        /// <summary>
        /// XPORT005: Port map file could not be read.
        /// </summary>
        public static readonly DiagnosticDescriptor InvalidPortMap = new(
            id: DiagnosticIds.InvalidPortMap,
            title: "Port map file could not be read",
            messageFormat: "Port map '{0}' is invalid and was ignored: {1}",
            category: Category,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "A '*.xamlport.json' additional file is not valid JSON or does not follow the port map schema.",
            helpLinkUri: HelpLinkBase + "xport005");
    }
}
