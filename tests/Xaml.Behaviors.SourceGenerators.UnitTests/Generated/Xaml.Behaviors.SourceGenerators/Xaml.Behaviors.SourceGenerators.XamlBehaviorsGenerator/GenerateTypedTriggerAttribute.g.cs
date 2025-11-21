using System;

namespace Xaml.Behaviors.SourceGenerators
{
    [AttributeUsage(AttributeTargets.Event | AttributeTargets.Assembly, AllowMultiple = true)]
    internal class GenerateTypedTriggerAttribute : Attribute
    {
        public GenerateTypedTriggerAttribute()
        {
        }

        public GenerateTypedTriggerAttribute(Type targetType, string eventName)
        {
        }
    }
}