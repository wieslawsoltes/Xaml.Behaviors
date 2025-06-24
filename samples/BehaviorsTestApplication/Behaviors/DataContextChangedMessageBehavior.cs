using System;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
using Avalonia.Xaml.Interactions.Custom;

namespace BehaviorsTestApplication.Behaviors;

public class DataContextChangedMessageBehavior : DataContextChangedBehavior<StyledElement>
{
    [ResolveByName]
    public TextBlock? Target { get; set; }

    protected override IDisposable OnDataContextChangedEventOverride()
    {
        if (Target is not null)
        {
            Target.Text = "DataContext Changed";
        }

        return DisposableAction.Empty;
    }
}
