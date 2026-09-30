// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
#if UNO
using Microsoft.UI.Dispatching;
using Xaml.Interactivity;
#else
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;
#endif
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

#if UNO
namespace Xaml.Interactions.Scripting;
#else
namespace Avalonia.Xaml.Interactions.Scripting;
#endif

/// <summary>
/// Executes a C# script using Roslyn scripting API.
/// </summary>
[RequiresUnreferencedCode("This functionality is not compatible with trimming.")]
public partial class ExecuteScriptAction : StyledElementAction
{
    private static string[] s_imports = [
        "System",
        "System.Collections.Generic",
        "System.Linq",
#if UNO
        // The WinUI namespaces matching the Avalonia imports below.
        "System.Collections.ObjectModel",
        "Microsoft.UI.Xaml",
        "Microsoft.UI.Xaml.Controls",
        "Microsoft.UI.Xaml.Media",
        "Microsoft.UI.Xaml.Input",
        "Microsoft.UI.Xaml.Markup"
#else
        "Avalonia",
        "Avalonia.Collections",
        "Avalonia.Controls",
        "Avalonia.Interactivity",
        "Avalonia.Metadata",
        "Avalonia.LogicalTree",
        "Avalonia.Reactive",
        "Avalonia.Input",
        "Avalonia.Markup.Xaml"
#endif
    ];

    /// <summary>
    /// Gets or sets the C# script to execute. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial string? Script { get; set; }

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
#pragma warning disable IL3000
            .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
#pragma warning restore IL3000
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
