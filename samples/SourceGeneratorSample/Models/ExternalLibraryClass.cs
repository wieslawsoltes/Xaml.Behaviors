using System;

namespace SourceGeneratorSample.Models
{
    // Simulates a class from an external library that we cannot modify.
    public class ExternalLibraryClass
    {
        public event EventHandler? ExternalEvent;

        public void ExternalMethod()
        {
            Console.WriteLine("ExternalMethod called!");
        }

        public void RaiseExternalEvent()
        {
            ExternalEvent?.Invoke(this, EventArgs.Empty);
        }

        public string ExternalProperty { get; set; } = "Initial Value";
    }
}
