// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Linq;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Dispatching;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Executes a command when a key is pressed while the button has focus.
/// </summary>
public class ButtonExecuteCommandOnKeyDownBehavior : ExecuteCommandOnKeyBehaviorBase
{
    /// <summary>
    /// Called when the behavior is attached to the visual tree.
    /// </summary>
    /// <returns>A disposable used to detach the key handler.</returns>
    protected override System.IDisposable OnAttachedToVisualTreeOverride()
    {
#if UNO
        // WinUI: key events bubble to the root element of the XAML island hosting the button.
        if (AssociatedObject?.XamlRoot?.Content is InputElement inputRoot)
#else
        if (TopLevel.GetTopLevel(AssociatedObject) is InputElement inputRoot)
#endif
        {
            return inputRoot.AddDisposableHandler(InputElement.KeyDownEvent, RootDefaultKeyDown);
        }
        
        return DisposableAction.Empty;
    }

    private void RootDefaultKeyDown(object? sender, KeyEventArgs e)
    {
        var haveKey = Key is not null && e.Key == Key;
        var haveGesture = Gesture is not null && Gesture.Matches(e);

        if (!haveKey && !haveGesture)
        {
            return;
        }

        if (AssociatedObject is Button button)
        {
            ExecuteCommand(button);
        }
    }

    private bool ExecuteCommand(Button button)
    {
        if (!IsEnabled)
        {
            return false;
        }

        if (button is not { IsVisible: true, IsEnabled: true })
        {
            return false;
        }

        if (button.Command?.CanExecute(button.CommandParameter) != true)
        {
            return false;
        }

        if (FocusTopLevel)
        {
#if UNO
            Dispatcher.UIThread.Post(() => (TopLevel ?? AssociatedObject?.XamlRoot?.Content)?.Focus());
#else
            Dispatcher.UIThread.Post(() => (TopLevel ?? AssociatedObject?.GetSelfAndLogicalAncestors().LastOrDefault() as TopLevel)?.Focus());
#endif
        }

        if (FocusControl is { } focusControl)
        {
            Dispatcher.UIThread.Post(() => focusControl.Focus());
        }
        
        button.Command.Execute(button.CommandParameter);
        return true;
    }
}
