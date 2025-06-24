using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
using Avalonia.Xaml.Interactions.Custom;

namespace BehaviorsTestApplication.Behaviors;

public class DataContextChangedMessageBehavior : DataContextChangedBehavior<StyledElement>
{
    public static readonly StyledProperty<TextBlock?> TargetProperty =
        AvaloniaProperty.Register<DataContextChangedMessageBehavior, TextBlock?>(nameof(Target));

    [ResolveByName]
    public TextBlock? Target
    {
        get => GetValue(TargetProperty);
        set => SetValue(TargetProperty, value);
    }

    protected override IDisposable OnDataContextChangedEventOverride()
    {
        if (Target is not null)
        {
            Target.Text = "DataContext Changed";
        }

        return DisposableAction.Empty;
    }
}
