using System;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Xaml.Interactivity;
#else
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;
#endif

namespace BehaviorsTestApplication.Behaviors;

public class DisposingMessageBehavior : DisposingBehavior<Control>
{
#if UNO
    public static readonly DependencyProperty TargetProperty =
        DependencyProperty.Register(nameof(Target), typeof(TextBlock), typeof(DisposingMessageBehavior), new PropertyMetadata(null));
#else
    public static readonly StyledProperty<TextBlock?> TargetProperty =
        AvaloniaProperty.Register<DisposingMessageBehavior, TextBlock?>(nameof(Target));
#endif

    // Uno Platform: WinUI XAML does not resolve names for properties; the views assign the target with {x:Bind}.
#if !UNO
    [ResolveByName]
#endif
    public TextBlock? Target
    {
        get => (TextBlock?)GetValue(TargetProperty);
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
