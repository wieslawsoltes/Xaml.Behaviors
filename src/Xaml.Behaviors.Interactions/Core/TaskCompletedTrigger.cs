// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Threading.Tasks;
#if UNO
using Microsoft.UI.Dispatching;
using Xaml.Interactivity;
#else
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Core;
#else
namespace Avalonia.Xaml.Interactions.Core;
#endif

/// <summary>
/// A trigger that invokes its actions when the supplied task completes.
/// </summary>
public partial class TaskCompletedTrigger : StyledElementTrigger
{

    /// <summary>
    /// Gets or sets the task monitored by the trigger.
    /// </summary>
    [StyledProperty]
    public partial Task? Task { get; set; }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == TaskProperty)
        {
            OnTaskChanged(change.GetNewValue<Task?>());
        }
    }

    private void OnTaskChanged(Task? task)
    {
        if (task is null)
        {
            return;
        }

        task.ContinueWith(_ =>
            Dispatcher.UIThread.Post(() => Interaction.ExecuteActions(AssociatedObject, Actions, null)));
    }
}
