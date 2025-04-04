using Avalonia.Input;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Releases the pointer capture.
/// </summary>
public class ReleasePointerCaptureAction : StyledElementAction
{
    /// <summary>
    /// Executes the action.
    /// </summary>
    /// <param name="sender">The <see cref="object"/> that is passed to the action by the behavior. Generally this is <seealso cref="IBehavior.AssociatedObject"/> or a target object.</param>
    /// <param name="parameter">The value of this parameter is determined by the caller.</param>
    /// <returns>Returns null after executed.</returns>
    public override object? Execute(object? sender, object? parameter)
    {
        if (parameter is not PointerEventArgs pointerEventArgs)
        {
            return null;
        }

        if (pointerEventArgs.Source is not IInputElement)
        {
            return null;
        }

        pointerEventArgs.Pointer.Capture(control: null);

        return null;
    }
}
