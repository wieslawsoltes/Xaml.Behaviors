using System;
using System.Linq;

namespace Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests;

public static class GeneratedTypeHelper
{
    public static object CreateInstance(string baseName)
    {
        var type = FindGeneratedType(baseName);
        return Activator.CreateInstance(type) ?? throw new InvalidOperationException($"Could not create instance of {type}.");
    }

    public static Type FindGeneratedType(string baseName)
    {
        var assembly = typeof(TestControl).Assembly;
        var type = assembly.GetTypes().FirstOrDefault(t => t.Name.StartsWith(baseName, StringComparison.Ordinal));
        if (type == null)
        {
            throw new InvalidOperationException($"Generated type starting with '{baseName}' was not found.");
        }
        return type;
    }
}
