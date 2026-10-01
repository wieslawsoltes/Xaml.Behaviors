using System;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;
#endif

namespace BehaviorsTestApplication.Triggers;

public class PointerPressedDisposingTrigger : DisposingTrigger<Control>
{
    protected override IDisposable OnAttachedOverride()
    {
        if (AssociatedObject is null)
        {
            return DisposableAction.Empty;
        }

        void Handler(object? sender, PointerPressedEventArgs e)
        {
            Interaction.ExecuteActions(AssociatedObject, Actions, e);
        }

#if UNO
        // WinUI: a pointer handler that also receives handled events (Avalonia: the tunneling PointerPressed).
        var handler = new PointerEventHandler(Handler);
        AssociatedObject.AddHandler(UIElement.PointerPressedEvent, handler, handledEventsToo: true);
#else
        AssociatedObject.AddHandler(InputElement.PointerPressedEvent, Handler, RoutingStrategies.Tunnel);
#endif

        return DisposableAction.Create(() =>
        {
#if UNO
            AssociatedObject.RemoveHandler(UIElement.PointerPressedEvent, handler);
#else
            AssociatedObject.RemoveHandler(InputElement.PointerPressedEvent, Handler);
#endif
        });
    }
}
