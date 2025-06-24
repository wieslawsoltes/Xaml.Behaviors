using System;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
using Avalonia.Xaml.Interactions.Custom;

namespace BehaviorsTestApplication.Behaviors;

public class AttachedToLogicalTreeMessageBehavior : AttachedToLogicalTreeBehavior<StyledElement>
{
    [ResolveByName]
    public TextBlock? Target { get; set; }

    protected override IDisposable OnAttachedToLogicalTreeOverride()
    {
        if (Target is not null)
        {
            Target.Text = "Attached";
        }

        return DisposableAction.Empty;
    }
}
