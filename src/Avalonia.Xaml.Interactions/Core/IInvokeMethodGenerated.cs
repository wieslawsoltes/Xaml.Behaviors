namespace Avalonia.Xaml.Interactions.Core;

/// <summary>
/// Implemented by classes that support generated method invocation.
/// </summary>
public interface IInvokeMethodGenerated
{
    /// <summary>
    /// Invokes a generated method.
    /// </summary>
    /// <param name="methodName">Name of the method.</param>
    /// <param name="sender">Action sender.</param>
    /// <param name="parameter">Action parameter.</param>
    /// <returns>True if the method was invoked; otherwise false.</returns>
    bool InvokeGenerated(string methodName, object? sender, object? parameter);
}
