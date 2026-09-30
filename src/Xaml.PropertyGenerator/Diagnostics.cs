// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Microsoft.CodeAnalysis;

namespace Xaml.PropertyGenerator
{
    internal static class Diagnostics
    {
        private const string Category = "XamlPropertyGenerator";
        private const string HelpLinkBase = "https://github.com/wieslawsoltes/Xaml.Behaviors/blob/master/src/Xaml.PropertyGenerator/README.md#";

        public static readonly DiagnosticDescriptor PropertyMustBePartial = new(
            "XPG0001",
            "Generated property must be a partial property",
            "Property '{0}' must be a partial property declaration without accessor bodies",
            Category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            helpLinkUri: HelpLinkBase + "xpg0001");

        public static readonly DiagnosticDescriptor TypeMustBePartial = new(
            "XPG0002",
            "Containing type must be partial",
            "Type '{0}' and all its containing types must be partial",
            Category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            helpLinkUri: HelpLinkBase + "xpg0002");

        public static readonly DiagnosticDescriptor NoPlatform = new(
            "XPG0003",
            "No supported XAML platform was found",
            "Cannot generate '{0}': reference Avalonia or WinUI/Uno Platform, or set the XamlPropertyGeneratorPlatform MSBuild property",
            Category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            helpLinkUri: HelpLinkBase + "xpg0003");

        public static readonly DiagnosticDescriptor UnsupportedShape = new(
            "XPG0004",
            "Unsupported property shape",
            "Cannot generate '{0}'",
            Category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            helpLinkUri: HelpLinkBase + "xpg0004");

        public static readonly DiagnosticDescriptor UnsupportedOption = new(
            "XPG0005",
            "Option is not supported by the target platform",
            "{0}",
            Category,
            DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            helpLinkUri: HelpLinkBase + "xpg0005");

        public static DiagnosticDescriptor Get(string id) => id switch
        {
            "XPG0001" => PropertyMustBePartial,
            "XPG0002" => TypeMustBePartial,
            "XPG0003" => NoPlatform,
            "XPG0004" => UnsupportedShape,
            _ => UnsupportedOption,
        };
    }
}
