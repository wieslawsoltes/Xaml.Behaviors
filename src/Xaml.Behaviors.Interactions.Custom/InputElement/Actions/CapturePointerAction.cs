// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Captures the pointer.
/// </summary>
public partial class CapturePointerAction : StyledElementAction
{

    /// <summary>
    /// Gets or sets the target control. This is an avalonia property.
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial Control? TargetControl { get; set; }

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

        if (pointerEventArgs.Source is not IInputElement inputElement)
        {
            return null;
        }

        var control = TargetControl ?? inputElement;

        pointerEventArgs.Pointer.Capture(control);

        return null;
    }
}
