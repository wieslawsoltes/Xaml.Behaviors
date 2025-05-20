using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Scripting;

/// <summary>
/// Executes C# code that is generated at build time from a script.
/// </summary>
/// <remarks>
/// A source generator or Fody weaver is expected to replace the body of
/// <see cref="ExecuteCompiledCode"/> with code produced from the provided script.
/// </remarks>
public partial class ExecuteCompiledScriptAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <see cref="Script"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> ScriptProperty =
        AvaloniaProperty.Register<ExecuteCompiledScriptAction, string?>(nameof(Script));

    /// <summary>
    /// Gets or sets the C# script which will be processed at build time.
    /// This is an avalonia property.
    /// </summary>
    public string? Script
    {
        get => GetValue(ScriptProperty);
        set => SetValue(ScriptProperty, value);
    }

    /// <inheritdoc />
    public override object Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        ExecuteCompiledCode(sender, parameter);
        return true;
    }

    /// <summary>
    /// Contains the code generated from <see cref="Script"/>.
    /// This method is populated by a source generator or Fody weaver at build time.
    /// </summary>
    /// <param name="sender">The object that invoked the action.</param>
    /// <param name="parameter">Additional data passed to the action.</param>
    partial void ExecuteCompiledCode(object? sender, object? parameter);
}
