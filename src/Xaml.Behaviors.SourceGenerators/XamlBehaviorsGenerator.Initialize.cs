// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Microsoft.CodeAnalysis;

namespace Xaml.Behaviors.SourceGenerators
{
    public partial class XamlBehaviorsGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            RegisterAttributeSources(context);

            RegisterActionGeneration(context);
            RegisterTriggerGeneration(context);
            RegisterChangePropertyActionGeneration(context);
            RegisterDataTriggerGeneration(context);
            RegisterMultiDataTriggerGeneration(context);
            RegisterInvokeCommandActionGeneration(context);
            RegisterPropertyTriggerGeneration(context);
            RegisterEventCommandGeneration(context);
            RegisterEventArgsActionGeneration(context);
            RegisterAsyncObservableTriggerGeneration(context);
        }
    }
}
