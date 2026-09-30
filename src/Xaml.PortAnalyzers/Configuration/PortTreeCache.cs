// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.CodeAnalysis;
using Xaml.PortAnalyzers.Preprocessor;

namespace Xaml.PortAnalyzers.Configuration
{
    /// <summary>
    /// Per-compilation cache of tree settings and port activity maps.
    /// </summary>
    internal sealed class PortTreeCache
    {
        private readonly PortConfiguration _configuration;
        private readonly ConcurrentDictionary<SyntaxTree, PortTreeSettings> _settings = new();
        private readonly ConcurrentDictionary<SyntaxTree, PortActivityMap> _maps = new();
        private readonly Func<SyntaxTree, PortTreeSettings> _createSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="PortTreeCache"/> class.
        /// </summary>
        public PortTreeCache(PortConfiguration configuration)
        {
            _configuration = configuration;
            _createSettings = configuration.GetTreeSettings;
        }

        /// <summary>
        /// Gets the configuration the cache was created for.
        /// </summary>
        public PortConfiguration Configuration => _configuration;

        /// <summary>
        /// Gets the settings of a syntax tree.
        /// </summary>
        public PortTreeSettings GetSettings(SyntaxTree tree) => _settings.GetOrAdd(tree, _createSettings);

        /// <summary>
        /// Returns whether the tree is shared with the port target.
        /// </summary>
        public bool IsShared(SyntaxTree tree) => GetSettings(tree).IsShared;

        /// <summary>
        /// Returns whether code at the position of a shared tree is compiled for the port target.
        /// </summary>
        public bool IsActiveForPort(SyntaxTree tree, int position, CancellationToken cancellationToken)
            => GetActivityMap(tree, cancellationToken).IsActiveForPort(position);

        /// <summary>
        /// Gets the port activity map of a syntax tree.
        /// </summary>
        public PortActivityMap GetActivityMap(SyntaxTree tree, CancellationToken cancellationToken)
        {
            if (_maps.TryGetValue(tree, out var map))
            {
                return map;
            }

            map = PortActivityMap.Create(tree, GetSettings(tree).TargetSymbol, cancellationToken);
            return _maps.GetOrAdd(tree, map);
        }
    }
}
