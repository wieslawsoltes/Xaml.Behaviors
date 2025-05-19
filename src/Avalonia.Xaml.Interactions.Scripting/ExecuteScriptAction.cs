using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Threading;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Scripting;

/// <summary>
/// Executes a C# script using Roslyn scripting API.
/// </summary>
[RequiresUnreferencedCode("This functionality is not compatible with trimming.")]
public class ExecuteScriptAction : StyledElementAction
{
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

    /// <inheritdoc />
    public override object Execute(object? sender, object? parameter)
    {
        if (string.IsNullOrWhiteSpace(Script))
        {
            return false;
        }

        var globals = new ExecuteScriptActionGlobals(sender, parameter);

        _ = Dispatcher.UIThread.InvokeAsync(async () =>
        {
            try
            {
                _ = await CSharpScript.EvaluateAsync(Script, globals: globals);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Script execution failed: {ex.Message}");
            }
        });
        return true;
    }
}
