using System;
using System.IO;
using System.Linq;
using Mono.Cecil;

namespace GenerateDocs;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 1 && args[0].Contains(' '))
        {
            args = args[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);
        }
        var root = args.Length > 0 ? args[0] : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        var configuration = args.Length > 1 ? args[1] : "Debug";

        var srcDir = Path.Combine(root, "src");
        var docsDir = Path.Combine(root, "docs");

        var assemblyPaths = Directory.GetFiles(srcDir, "Xaml.Behaviors.*.dll", SearchOption.AllDirectories)
            .Where(p => p.Contains(Path.Combine("bin", configuration)))
            .ToList();

        foreach (var asmPath in assemblyPaths)
        {
            AssemblyDefinition asm;
            try
            {
                asm = AssemblyDefinition.ReadAssembly(asmPath);
            }
            catch
            {
                continue;
            }

            foreach (var type in asm.MainModule.Types.Where(t => t.IsPublic && t.IsClass && !t.IsAbstract))
            {
                string? category = null;
                if (DerivesFrom(type, "Avalonia.Xaml.Interactivity.Behavior"))
                    category = "Behaviors";
                else if (ImplementsInterface(type, "Avalonia.Xaml.Interactivity.IAction"))
                    category = "Actions";
                else if (DerivesFrom(type, "Avalonia.Xaml.Interactivity.Trigger") || ImplementsInterface(type, "Avalonia.Xaml.Interactivity.ITrigger"))
                    category = "Triggers";

                if (category is null)
                    continue;

                var stubDir = Path.Combine(docsDir, category);
                Directory.CreateDirectory(stubDir);
                var file = Path.Combine(stubDir, $"{type.Name}.md");
                if (!File.Exists(file))
                {
                    File.WriteAllText(file, $"# {type.Name}\n\nTODO: Document the {type.Name}.\n");
                    Console.WriteLine($"Created {file}");
                }
            }
        }
    }

    static bool DerivesFrom(TypeDefinition type, string baseFullName)
    {
        var current = type;
        while (current != null)
        {
            if (current.FullName.StartsWith(baseFullName))
                return true;
            current = current.BaseType?.Resolve();
        }
        return false;
    }

    static bool ImplementsInterface(TypeDefinition type, string ifaceFullName)
    {
        return type.Interfaces.Any(i => i.InterfaceType.FullName.StartsWith(ifaceFullName));
    }
}
