// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Dispatching;
// WinUI describes how an element got focus with FocusState (Pointer, Keyboard, Programmatic).
using NavigationMethod = Microsoft.UI.Xaml.FocusState;
#else
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Base class for behaviors that manage focus on a control.
/// </summary>
public abstract partial class FocusBehaviorBase : AttachedToVisualTreeBehavior<Control>
{

    /// <summary>
    /// 
    /// </summary>
    [StyledProperty]
    public partial NavigationMethod NavigationMethod { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    [StyledProperty]
    public partial KeyModifiers KeyModifiers { get; set; }

    /// <summary>
    /// Sets focus on the associated control.
    /// </summary>
    /// <returns>True if the operation succeeds; otherwise, false.</returns>
    protected virtual bool Focus()
    {
        if (!IsEnabled)
        {
            return false;
        }

#if UNO
        // FocusState.Unfocused (the default) is not a valid focus request: focus programmatically.
        var focusState = NavigationMethod == NavigationMethod.Unfocused ? NavigationMethod.Programmatic : NavigationMethod;
        Dispatcher.UIThread.Post(() => AssociatedObject?.Focus(focusState));
#else
        Dispatcher.UIThread.Post(() => AssociatedObject?.Focus(NavigationMethod, KeyModifiers));
#endif

        return true;

    }
}
