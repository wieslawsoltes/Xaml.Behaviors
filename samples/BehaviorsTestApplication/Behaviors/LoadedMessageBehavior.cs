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

public class LoadedMessageBehavior : LoadedBehavior<Control>
{
#if UNO
    public static readonly DependencyProperty TargetProperty =
        DependencyProperty.Register(nameof(Target), typeof(TextBlock), typeof(LoadedMessageBehavior), new PropertyMetadata(null));
#else
    public static readonly StyledProperty<TextBlock?> TargetProperty =
        AvaloniaProperty.Register<LoadedMessageBehavior, TextBlock?>(nameof(Target));
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

    protected override IDisposable OnLoadedOverride()
    {
        if (Target is not null)
        {
            Target.Text = "Loaded";
        }

        return DisposableAction.Empty;
    }
}
