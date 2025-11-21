using System;

namespace Xaml.Behaviors.SourceGenerators
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    internal sealed class GenerateTypedMultiDataTriggerAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    internal sealed class TriggerPropertyAttribute : Attribute
    {
    }
}