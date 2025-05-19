using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Avalonia.Threading;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Avalonia.Xaml.Interactivity;
using Microsoft.CodeAnalysis.Scripting;

namespace Avalonia.Xaml.Interactions.Scripting;

/// <summary>
/// Executes a C# script using Roslyn scripting API.
/// </summary>
[RequiresUnreferencedCode("This functionality is not compatible with trimming.")]
public class ExecuteScriptAction : StyledElementAction
{
    private static string[] s_imports = [
        "System",
        "System.Collections.Generic",
        "System.Linq",
        "Avalonia",
        "Avalonia.Collections",
        "Avalonia.Controls",
        "Avalonia.Interactivity",
        "Avalonia.Metadata",
        "Avalonia.LogicalTree",
        "Avalonia.Reactive",
        "Avalonia.Input",
        "Avalonia.Markup.Xaml"
    ];
    
    /// <summary>
    /// Identifies the <see cref="Script"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> ScriptProperty =
        AvaloniaProperty.Register<ExecuteScriptAction, string?>(nameof(Script));

    /// <summary>
    /// Gets or sets the C# script to execute. This is an avalonia property.
    /// </summary>
    public string? Script
    {
        get => GetValue(ScriptProperty);
        set => SetValue(ScriptProperty, value);
    }

    /// <summary>
    /// Run script using Dispatcher.UIThread.InvokeAsync instead Task.Run.
    /// </summary>
    public bool UseDispatcher { get; set; } = true;

    /// <inheritdoc />
    public override object Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(Script))
        {
            return false;
        }

        var script = Script;
        var globals = new ExecuteScriptActionGlobals(sender, parameter);
        var loadedAssemblies = AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
            .ToArray();

        if (UseDispatcher)
        {
            _ = Dispatcher.UIThread.InvokeAsync(async () =>
            {
                await Run(loadedAssemblies, script, s_imports, globals);
            });
        }
        else
        {
            _ = Task.Run(async () =>
            {
                await Run(loadedAssemblies, script, s_imports, globals);
            });
        }

        return true;
    }

    private static async Task Run(
        Assembly[] loadedAssemblies, 
        string? script, 
        string[] imports, 
        ExecuteScriptActionGlobals globals)
    {
        try
        {
            var options = ScriptOptions.Default.WithImports(imports).WithReferences(loadedAssemblies);
            _ = await CSharpScript.RunAsync(script, options, globals);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Script execution failed: {ex.Message}");
        }
    }
}
