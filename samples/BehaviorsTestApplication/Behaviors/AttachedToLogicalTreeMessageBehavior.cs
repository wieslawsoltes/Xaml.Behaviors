using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
using Avalonia.Xaml.Interactions.Custom;

namespace BehaviorsTestApplication.Behaviors;

public class AttachedToLogicalTreeMessageBehavior : AttachedToLogicalTreeBehavior<StyledElement>
{
    public static readonly StyledProperty<TextBlock?> TargetProperty =
        AvaloniaProperty.Register<AttachedToLogicalTreeMessageBehavior, TextBlock?>(nameof(Target));

    [ResolveByName]
    public TextBlock? Target
    {
        get => GetValue(TargetProperty);
        set => SetValue(TargetProperty, value);
    }

    protected override IDisposable OnAttachedToLogicalTreeOverride()
    {
        if (Target is not null)
        {
            Target.Text = "Attached";
        }

        return DisposableAction.Empty;
    }
}
