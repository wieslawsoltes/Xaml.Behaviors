// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace Xaml.PortAnalyzers.Configuration
{
    /// <summary>
    /// Names of the analyzer configuration keys read by the port analyzers.
    /// </summary>
    internal static class PortOptionNames
    {
        /// <summary>MSBuild property: the preprocessor symbol that marks the port target.</summary>
        public const string TargetSymbolProperty = "build_property.XamlPortTargetSymbol";

        /// <summary>MSBuild property: <c>;</c>-separated globs of files that are not shared with the port.</summary>
        public const string SharedSourceExcludesProperty = "build_property.XamlPortSharedSourceExcludes";

        /// <summary>MSBuild property: extra known preprocessor symbols for XPORT003.</summary>
        public const string KnownSymbolsProperty = "build_property.XamlPortKnownSymbols";

        /// <summary>MSBuild property: the directory of the project being compiled.</summary>
        public const string ProjectDirectoryProperty = "build_property.MSBuildProjectDirectory";

        /// <summary>MSBuild property exposed by the .NET SDK: the directory of the project being compiled.</summary>
        public const string ProjectDirProperty = "build_property.ProjectDir";

        /// <summary>.editorconfig key overriding the port target symbol.</summary>
        public const string TargetSymbolKey = "xaml_port.target_symbol";

        /// <summary>.editorconfig key adding known preprocessor symbols for XPORT003.</summary>
        public const string KnownSymbolsKey = "xaml_port.known_symbols";

        /// <summary>.editorconfig key forcing a file to be treated as shared (<c>true</c>) or not shared (<c>false</c>).</summary>
        public const string SharedKey = "xaml_port.shared";
    }
}
