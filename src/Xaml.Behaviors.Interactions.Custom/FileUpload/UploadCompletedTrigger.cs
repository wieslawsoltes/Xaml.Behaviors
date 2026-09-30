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
/// Executes actions when the bound value becomes <c>true</c>.
/// </summary>
public partial class UploadCompletedTrigger : StyledElementTrigger
{

    /// <summary>
    /// Gets or sets a value indicating upload completion. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial bool IsCompleted { get; set; }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IsCompletedProperty)
        {
            OnIsCompletedChanged(change);
        }
    }

    private void OnIsCompletedChanged(AvaloniaPropertyChangedEventArgs args)
    {
#if UNO
        // WinUI change arguments carry no sender: the change is always raised for this trigger.
        var trigger = this;
#else
        if (args.Sender is not UploadCompletedTrigger trigger)
        {
            return;
        }
#endif

        if (args.NewValue is bool completed && completed)
        {
            Dispatcher.UIThread.Post(() => trigger.Execute());
        }
    }

    private void Execute()
    {
        if (!IsEnabled || AssociatedObject is null)
        {
            return;
        }

        Interaction.ExecuteActions(AssociatedObject, Actions, null);
    }
}
