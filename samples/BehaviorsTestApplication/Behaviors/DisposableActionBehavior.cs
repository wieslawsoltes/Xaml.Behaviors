using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace BehaviorsTestApplication.Behaviors;

public class DisposableActionBehavior : StyledElementBehavior<Control>
{
    public static readonly StyledProperty<TextBlock?> TargetProperty =
        AvaloniaProperty.Register<DisposableActionBehavior, TextBlock?>(nameof(Target));

    [ResolveByName]
    public TextBlock? Target
    {
        get => GetValue(TargetProperty);
        set => SetValue(TargetProperty, value);
    }

    private IDisposable? _disposable;

    protected override void OnAttached()
    {
        base.OnAttached();

        if (AssociatedObject is null)
        {
            return;
        }

        void Handler(object? sender, PointerPressedEventArgs e)
        {
            if (Target is not null)
            {
                Target.Text = "Pressed";
            }
        }

        AssociatedObject.AddHandler(InputElement.PointerPressedEvent, Handler, RoutingStrategies.Tunnel);

        _disposable = DisposableAction.Create(() =>
        {
            AssociatedObject.RemoveHandler(InputElement.PointerPressedEvent, Handler);
        });
    }

    protected override void OnDetaching()
    {
        _disposable?.Dispose();
        _disposable = null;
        base.OnDetaching();
    }
}
