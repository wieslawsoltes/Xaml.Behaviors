// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace Xaml.Behaviors.SourceGenerators.Platforms
{
    /// <summary>
    /// Maps a <see cref="TargetPlatform"/> to its (immutable) emitter strategy.
    /// </summary>
    internal static class XamlPlatforms
    {
        public static IXamlPlatform Get(TargetPlatform platform)
        {
            return platform == TargetPlatform.WinUI
                ? WinUIPlatform.Instance
                : AvaloniaPlatform.Instance;
        }
    }
}
