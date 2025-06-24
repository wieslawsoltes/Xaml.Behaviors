using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace BehaviorsTestApplication.Behaviors;

public class DisposingMessageBehavior : DisposingBehavior<Control>
{
    public static readonly StyledProperty<TextBlock?> TargetProperty =
        AvaloniaProperty.Register<DisposingMessageBehavior, TextBlock?>(nameof(Target));

    [ResolveByName]
    public TextBlock? Target
    {
        get => GetValue(TargetProperty);
        set => SetValue(TargetProperty, value);
    }

    protected override IDisposable OnAttachedOverride()
    {
        if (AssociatedObject is null)
        {
            return DisposableAction.Empty;
        }

        void Handler(object? sender, PointerPressedEventArgs e)
        {
            if (Target is not null)
            {
                Target.Text = "Pressed";
            }
        }

        AssociatedObject.AddHandler(InputElement.PointerPressedEvent, Handler, RoutingStrategies.Tunnel);

        return DisposableAction.Create(() =>
        {
            AssociatedObject.RemoveHandler(InputElement.PointerPressedEvent, Handler);
        });
    }
}
