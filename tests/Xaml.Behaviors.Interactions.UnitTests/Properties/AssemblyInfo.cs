using System.Reflection;
using Avalonia.Metadata;
using Xunit;

[assembly: AssemblyTitle("Xaml.Behaviors.Interactions.UnitTests")]
[assembly: XmlnsDefinition("https://unit.test/features", "Avalonia.Xaml.Interactions.UnitTests.Custom")]

[assembly: CollectionBehavior(DisableTestParallelization = true)]
