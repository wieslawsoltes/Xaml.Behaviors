// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using Microsoft.CodeAnalysis;

namespace Xaml.Behaviors.SourceGenerators.Platforms
{
    /// <summary>
    /// The XAML framework the generated actions and triggers are emitted for.
    /// </summary>
    internal enum TargetPlatform
    {
        /// <summary>Avalonia (<c>Avalonia.Xaml.Interactivity</c>).</summary>
        Avalonia,

        /// <summary>WinUI / Uno Platform (<c>Xaml.Interactivity</c> from <c>Xaml.Behaviors.Uno.Interactivity</c>).</summary>
        WinUI,
    }

    /// <summary>
    /// Resolves the <see cref="TargetPlatform"/> of a compilation.
    /// </summary>
    internal static class TargetPlatformDetector
    {
        /// <summary>
        /// The MSBuild property (surfaced through <c>CompilerVisibleProperty</c>) that overrides the detection.
        /// </summary>
        public const string OverrideOptionName = "build_property.XamlBehaviorsSourceGeneratorPlatform";

        private const string AvaloniaObjectMetadataName = "Avalonia.AvaloniaObject";
        private const string WinUIDependencyObjectMetadataName = "Microsoft.UI.Xaml.DependencyObject";

        /// <summary>
        /// Detects the platform from the referenced assemblies: Avalonia when <c>Avalonia.AvaloniaObject</c> is
        /// referenced, WinUI when only <c>Microsoft.UI.Xaml.DependencyObject</c> is referenced, and Avalonia otherwise.
        /// </summary>
        public static TargetPlatform Detect(Compilation compilation)
        {
            if (compilation.GetTypeByMetadataName(AvaloniaObjectMetadataName) is not null)
            {
                return TargetPlatform.Avalonia;
            }

            return compilation.GetTypeByMetadataName(WinUIDependencyObjectMetadataName) is not null
                ? TargetPlatform.WinUI
                : TargetPlatform.Avalonia;
        }

        /// <summary>
        /// Parses the value of the <c>XamlBehaviorsSourceGeneratorPlatform</c> MSBuild property.
        /// </summary>
        /// <returns>The requested platform, or <c>null</c> when the value is empty or not recognized.</returns>
        public static TargetPlatform? ParseOverride(string? value)
        {
            if (value is null)
            {
                return null;
            }

            var trimmed = value.Trim();
            if (string.Equals(trimmed, "Avalonia", StringComparison.OrdinalIgnoreCase))
            {
                return TargetPlatform.Avalonia;
            }

            if (string.Equals(trimmed, "WinUI", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(trimmed, "Uno", StringComparison.OrdinalIgnoreCase))
            {
                return TargetPlatform.WinUI;
            }

            return null;
        }

        /// <summary>
        /// Combines the detected platform with the optional MSBuild override (the override wins).
        /// </summary>
        public static TargetPlatform Resolve(TargetPlatform detected, TargetPlatform? overrideValue)
        {
            return overrideValue ?? detected;
        }
    }
}
