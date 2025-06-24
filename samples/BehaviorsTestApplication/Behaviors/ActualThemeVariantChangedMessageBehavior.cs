using System;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
using Avalonia.Xaml.Interactions.Custom;

namespace BehaviorsTestApplication.Behaviors;

public class ActualThemeVariantChangedMessageBehavior : ActualThemeVariantChangedBehavior<ThemeVariantScope>
{
    [ResolveByName]
    public TextBlock? Target { get; set; }

    protected override IDisposable OnActualThemeVariantChangedEventOverride()
    {
        if (Target is not null)
        {
            Target.Text = "Theme Changed";
        }

        return DisposableAction.Empty;
    }
}
