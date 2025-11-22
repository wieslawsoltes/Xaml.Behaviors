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
        }
    }
}
