using System;
using Avalonia;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace BehaviorsTestApplication.Triggers;

public class PointerPressedDisposingTrigger : DisposingTrigger<StyledElement>
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

        AssociatedObject.AddHandler(InputElement.PointerPressedEvent, Handler, RoutingStrategies.Tunnel);

        return DisposableAction.Create(() =>
        {
            AssociatedObject.RemoveHandler(InputElement.PointerPressedEvent, Handler);
        });
    }
}
