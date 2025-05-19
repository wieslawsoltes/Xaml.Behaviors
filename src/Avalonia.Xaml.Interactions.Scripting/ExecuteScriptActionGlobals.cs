namespace Avalonia.Xaml.Interactions.Scripting;

/// <summary>
/// Globals for the script execution.
/// </summary>
/// <param name="sender"></param>
/// <param name="parameter"></param>
public sealed class ExecuteScriptActionGlobals(object? sender, object? parameter)
{
    /// <summary>
    /// 
    /// </summary>
    public object? Sender { get; } = sender;

    /// <summary>
    /// 
    /// </summary>
    public object? Parameter { get; } = parameter;
}
