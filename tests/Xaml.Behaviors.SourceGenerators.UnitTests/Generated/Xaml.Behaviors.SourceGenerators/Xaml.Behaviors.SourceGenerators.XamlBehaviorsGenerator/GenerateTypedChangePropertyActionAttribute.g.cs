using System;

namespace Xaml.Behaviors.SourceGenerators
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Assembly, AllowMultiple = true)]
    internal class GenerateTypedChangePropertyActionAttribute : Attribute
    {
        public GenerateTypedChangePropertyActionAttribute()
        {
        }

        public GenerateTypedChangePropertyActionAttribute(Type targetType, string propertyName)
        {
        }
    }
}