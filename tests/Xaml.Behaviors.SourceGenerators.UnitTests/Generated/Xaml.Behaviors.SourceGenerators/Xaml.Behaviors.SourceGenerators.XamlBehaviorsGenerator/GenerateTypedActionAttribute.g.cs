using System;

namespace Xaml.Behaviors.SourceGenerators
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Assembly, AllowMultiple = true)]
    internal class GenerateTypedActionAttribute : Attribute
    {
        public GenerateTypedActionAttribute()
        {
        }

        public GenerateTypedActionAttribute(Type targetType, string methodName)
        {
        }
    }
}