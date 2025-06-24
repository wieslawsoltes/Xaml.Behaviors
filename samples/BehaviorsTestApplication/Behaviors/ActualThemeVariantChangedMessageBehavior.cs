using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
using Avalonia.Xaml.Interactions.Custom;

namespace BehaviorsTestApplication.Behaviors;

public class ActualThemeVariantChangedMessageBehavior : ActualThemeVariantChangedBehavior<ThemeVariantScope>
{
    public static readonly StyledProperty<TextBlock?> TargetProperty =
        AvaloniaProperty.Register<ActualThemeVariantChangedMessageBehavior, TextBlock?>(nameof(Target));

    [ResolveByName]
    public TextBlock? Target
    {
        get => GetValue(TargetProperty);
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
