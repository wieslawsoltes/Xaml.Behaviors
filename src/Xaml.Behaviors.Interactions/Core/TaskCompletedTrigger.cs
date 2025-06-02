// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Threading.Tasks;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Core;

/// <summary>
/// A trigger that invokes its actions when the supplied task completes.
/// </summary>
public class TaskCompletedTrigger : StyledElementTrigger
{
    /// <summary>
    /// Identifies the <see cref="Task"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Task?> TaskProperty =
        AvaloniaProperty.Register<TaskCompletedTrigger, Task?>(nameof(Task));

    /// <summary>
    /// Gets or sets the task monitored by the trigger.
    /// </summary>
    public Task? Task
    {
        get => GetValue(TaskProperty);
        set => SetValue(TaskProperty, value);
    }

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
