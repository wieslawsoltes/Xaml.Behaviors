using System;
using System.Linq;

#if UNO
namespace Xaml.Behaviors.SourceGenerators.UnitTests;
#else
namespace Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests;
#endif

public static class GeneratedTypeHelper
{
    public static object CreateInstance(string baseName, string? @namespace = null)
    {
        var type = FindGeneratedType(baseName, @namespace);
        return Activator.CreateInstance(type) ?? throw new InvalidOperationException($"Could not create instance of {type}.");
    }

    public static Type FindGeneratedType(string baseName, string? @namespace = null)
    {
#if UNO
        // The shared tests compile in the Xaml.* namespaces on Uno Platform (see build/UnoPort/uno_share.py).
        if (@namespace is not null && @namespace.StartsWith("Avalonia.Xaml.", StringComparison.Ordinal))
        {
            @namespace = @namespace.Substring("Avalonia.".Length);
        }
#endif
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
