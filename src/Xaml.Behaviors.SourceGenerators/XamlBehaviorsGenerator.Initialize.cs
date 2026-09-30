// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Microsoft.CodeAnalysis;
using Xaml.Behaviors.SourceGenerators.Platforms;

namespace Xaml.Behaviors.SourceGenerators
{
    public partial class XamlBehaviorsGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            RegisterAttributeSources(context);

            var platform = RegisterTargetPlatform(context);

            RegisterActionGeneration(context, platform);
            RegisterTriggerGeneration(context, platform);
            RegisterChangePropertyActionGeneration(context, platform);
            RegisterDataTriggerGeneration(context, platform);
            RegisterMultiDataTriggerGeneration(context, platform);
            RegisterInvokeCommandActionGeneration(context, platform);
            RegisterPropertyTriggerGeneration(context, platform);
            RegisterEventCommandGeneration(context, platform);
            RegisterEventArgsActionGeneration(context, platform);
            RegisterAsyncObservableTriggerGeneration(context, platform);
        }

        private static IncrementalValueProvider<TargetPlatform> RegisterTargetPlatform(IncrementalGeneratorInitializationContext context)
        {
            var detected = context.CompilationProvider
                .Select(static (compilation, _) => TargetPlatformDetector.Detect(compilation));

            var requested = context.AnalyzerConfigOptionsProvider
                .Select(static (options, _) => options.GlobalOptions.TryGetValue(TargetPlatformDetector.OverrideOptionName, out var value)
                    ? TargetPlatformDetector.ParseOverride(value)
                    : null);

            return detected
                .Combine(requested)
                .Select(static (pair, _) => TargetPlatformDetector.Resolve(pair.Left, pair.Right));
        }
    }
}
