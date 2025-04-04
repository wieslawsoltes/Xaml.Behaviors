using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// 
/// </summary>
public class ButtonExecuteCommandOnKeyDownBehavior : ExecuteCommandOnKeyBehaviorBase
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected override System.IDisposable OnAttachedToVisualTreeOverride()
    {
        if (AssociatedObject?.GetVisualRoot() is InputElement inputRoot)
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
            Dispatcher.UIThread.Post(() => (TopLevel ?? AssociatedObject?.GetSelfAndLogicalAncestors().LastOrDefault() as TopLevel)?.Focus());
        }

        if (FocusControl is { } focusControl)
        {
            Dispatcher.UIThread.Post(() => focusControl.Focus());
        }
        
        button.Command.Execute(button.CommandParameter);
        return true;
    }
}
