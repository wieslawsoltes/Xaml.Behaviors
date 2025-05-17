using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Avalonia;
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
    [RequiresUnreferencedCode("This functionality is not compatible with trimming.")]
    public override object? Execute(object? sender, object? parameter)
    {
        if (string.IsNullOrWhiteSpace(Script))
        {
            return null;
        }

        var globals = new Globals(sender, parameter);
        var task = CSharpScript.EvaluateAsync<object?>(Script!, globals: globals);
        return task.GetAwaiter().GetResult();
    }

    private sealed class Globals
    {
        public object? Sender { get; }
        public object? Parameter { get; }

        public Globals(object? sender, object? parameter)
        {
            Sender = sender;
            Parameter = parameter;
        }
    }
}
