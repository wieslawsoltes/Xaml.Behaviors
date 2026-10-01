using System;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactivity;
using Xaml.Interactions.Custom;
#else
using Avalonia;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
using Avalonia.Xaml.Interactions.Custom;
#endif

namespace BehaviorsTestApplication.Behaviors;

public class InitializedMessageBehavior : InitializedBehavior<StyledElement>
{
#if UNO
    public static readonly DependencyProperty TargetProperty =
        DependencyProperty.Register(nameof(Target), typeof(TextBlock), typeof(InitializedMessageBehavior), new PropertyMetadata(null));
#else
    public static readonly StyledProperty<TextBlock?> TargetProperty =
        AvaloniaProperty.Register<InitializedMessageBehavior, TextBlock?>(nameof(Target));
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

    protected override IDisposable OnInitializedEventOverride()
    {
        if (Target is not null)
        {
            Target.Text = "Initialized";
        }

        return DisposableAction.Empty;
    }
}
