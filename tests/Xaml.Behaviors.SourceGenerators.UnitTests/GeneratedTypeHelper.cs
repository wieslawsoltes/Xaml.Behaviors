using System;
using System.Linq;

namespace Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests;

public static class GeneratedTypeHelper
{
    public static object CreateInstance(string baseName, string? @namespace = null)
    {
        var type = FindGeneratedType(baseName, @namespace);
        return Activator.CreateInstance(type) ?? throw new InvalidOperationException($"Could not create instance of {type}.");
    }

    public static Type FindGeneratedType(string baseName, string? @namespace = null)
    {
        var assembly = typeof(TestControl).Assembly;
        var type = assembly
            .GetTypes()
            .FirstOrDefault(t =>
                string.Equals(t.Name, baseName, StringComparison.Ordinal) &&
                (string.IsNullOrEmpty(@namespace) || string.Equals(t.Namespace, @namespace, StringComparison.Ordinal)));
        if (type == null)
        {
            var nsInfo = string.IsNullOrEmpty(@namespace) ? string.Empty : $" in namespace '{@namespace}'";
            throw new InvalidOperationException($"Generated type '{baseName}'{nsInfo} was not found.");
        }
        return type;
    }
}
