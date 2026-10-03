// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactivity;

namespace Xaml.Behaviors.Uno.XamlPages;

/// <summary>
/// Writes a message to an <c>x:Bind</c> assigned text block when the data context of the associated object changes.
/// </summary>
public sealed partial class DataContextMessageBehavior : Behavior<FrameworkElement>
{
    public static readonly DependencyProperty TargetProperty =
        DependencyProperty.Register(nameof(Target), typeof(TextBlock), typeof(DataContextMessageBehavior), new PropertyMetadata(null));

    public TextBlock? Target
    {
        get => (TextBlock?)GetValue(TargetProperty);
        set => SetValue(TargetProperty, value);
    }

    public int Notifications { get; private set; }

    protected override void OnDataContextChangedEvent()
    {
        Notifications++;
        if (Target is not null)
        {
            Target.Text = "DataContext Changed";
        }
    }
}
