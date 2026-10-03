using System;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactivity;
using Xaml.Interactions.Custom;
// Uno Platform: every WinUI element is a theme scope (FrameworkElement.RequestedTheme).
using ThemeVariantScope = Microsoft.UI.Xaml.FrameworkElement;
#else
using Avalonia;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
using Avalonia.Xaml.Interactions.Custom;
#endif

namespace BehaviorsTestApplication.Behaviors;

public class ActualThemeVariantChangedMessageBehavior : ActualThemeVariantChangedBehavior<ThemeVariantScope>
{
#if UNO
    public static readonly DependencyProperty TargetProperty =
        DependencyProperty.Register(nameof(Target), typeof(TextBlock), typeof(ActualThemeVariantChangedMessageBehavior), new PropertyMetadata(null));
#else
    public static readonly StyledProperty<TextBlock?> TargetProperty =
        AvaloniaProperty.Register<ActualThemeVariantChangedMessageBehavior, TextBlock?>(nameof(Target));
#endif

    // Uno Platform: WinUI XAML does not resolve names for properties; the views assign the target with {x:Bind}.
#if !UNO
    [ResolveByName]
#endif
    public TextBlock? Target
    {
#if UNO
        get => (TextBlock?)GetValue(TargetProperty);
#else
        get => GetValue(TargetProperty);
#endif
        set => SetValue(TargetProperty, value);
    }

    protected override IDisposable OnActualThemeVariantChangedEventOverride()
    {
        if (Target is not null)
        {
            Target.Text = "Theme Changed";
        }

        return DisposableAction.Empty;
    }
}
