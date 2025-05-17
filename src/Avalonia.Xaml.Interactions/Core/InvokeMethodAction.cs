using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Core;

/// <summary>
/// An action that invokes a generated method on a specified object.
/// If the target object does not implement <see cref="IInvokeMethodGenerated"/>,
/// the action falls back to <see cref="CallMethodAction"/>.
/// </summary>
public class InvokeMethodAction : CallMethodAction
{
    /// <inheritdoc />
    public override object Execute(object? sender, object? parameter)
    {
        var target = GetValue(TargetObjectProperty) ?? sender;
        if (IsEnabled &&
            target is IInvokeMethodGenerated generated &&
            !string.IsNullOrEmpty(MethodName))
        {
            return generated.InvokeGenerated(MethodName!, sender, parameter);
        }

        return base.Execute(sender, parameter);
    }
}
