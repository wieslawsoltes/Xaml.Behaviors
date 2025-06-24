using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
using Avalonia.Xaml.Interactions.Custom;

namespace BehaviorsTestApplication.Behaviors;

public class LoadedMessageBehavior : LoadedBehavior<Control>
{
    public static readonly StyledProperty<TextBlock?> TargetProperty =
        AvaloniaProperty.Register<LoadedMessageBehavior, TextBlock?>(nameof(Target));

    [ResolveByName]
    public TextBlock? Target
    {
        get => GetValue(TargetProperty);
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
