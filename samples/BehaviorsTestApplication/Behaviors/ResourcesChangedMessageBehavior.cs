using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
using Avalonia.Xaml.Interactions.Custom;

namespace BehaviorsTestApplication.Behaviors;

public class ResourcesChangedMessageBehavior : ResourcesChangedBehavior<StyledElement>
{
    public static readonly StyledProperty<TextBlock?> TargetProperty =
        AvaloniaProperty.Register<ResourcesChangedMessageBehavior, TextBlock?>(nameof(Target));

    [ResolveByName]
    public TextBlock? Target
    {
        get => GetValue(TargetProperty);
        set => SetValue(TargetProperty, value);
    }

    protected override IDisposable OnResourcesChangedEventOverride()
    {
        if (Target is not null)
        {
            Target.Text = "Resources changed";
        }

        return DisposableAction.Empty;
    }
}
