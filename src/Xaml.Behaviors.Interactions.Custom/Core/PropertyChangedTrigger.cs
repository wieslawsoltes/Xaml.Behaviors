// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Dispatching;
using Xaml.Interactivity;
#else
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Represents a trigger that performs actions when the bound data have changed.
/// </summary>
public partial class PropertyChangedTrigger : StyledElementTrigger
{

    /// <summary>
    /// Gets or sets a binding object that the trigger will listen to. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial object? Binding { get; set; }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == BindingProperty)
        {
            OnBindingChanged(change);
        }
    }

    private void OnBindingChanged(AvaloniaPropertyChangedEventArgs args)
    {
        // Property changes of this behavior are always raised on this instance.
        Dispatcher.UIThread.Post(() => Execute(args));
    }

    private void Execute(object? parameter)
    {
        if (AssociatedObject is null || !IsEnabled)
        {
            return;
        }

        Interaction.ExecuteActions(AssociatedObject, Actions, parameter);
    }
}
