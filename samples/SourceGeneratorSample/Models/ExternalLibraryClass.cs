using System;
using Xaml.Behaviors.SourceGenerators;

namespace SourceGeneratorSample.Models
{
    public class ExternalLibraryClass
    {
        [GenerateTypedTrigger]
        public event EventHandler? ExternalEvent;

        [GenerateTypedAction]
        public void ExternalMethod()
        {
            Console.WriteLine("ExternalMethod called!");
        }

        public void RaiseExternalEvent()
        {
            ExternalEvent?.Invoke(this, EventArgs.Empty);
        }

        [GenerateTypedChangePropertyAction]
        public string ExternalProperty { get; set; } = "Initial Value";
    }
}
