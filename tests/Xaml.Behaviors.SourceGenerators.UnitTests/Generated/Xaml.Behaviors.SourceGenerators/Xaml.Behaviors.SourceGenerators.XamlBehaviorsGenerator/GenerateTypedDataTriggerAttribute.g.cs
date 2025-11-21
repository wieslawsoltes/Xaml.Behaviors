using System;

namespace Xaml.Behaviors.SourceGenerators
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    internal class GenerateTypedDataTriggerAttribute : Attribute
    {
        public GenerateTypedDataTriggerAttribute(Type type)
        {
        }
    }
}