using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
using Avalonia.Xaml.Interactions.Custom;

namespace BehaviorsTestApplication.Behaviors;

public class InitializedMessageBehavior : InitializedBehavior<StyledElement>
{
    public static readonly StyledProperty<TextBlock?> TargetProperty =
        AvaloniaProperty.Register<InitializedMessageBehavior, TextBlock?>(nameof(Target));

    [ResolveByName]
    public TextBlock? Target
    {
        get => GetValue(TargetProperty);
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
