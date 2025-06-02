using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Core;

/// <summary>
/// Executes actions when the bound value becomes <c>true</c>.
/// </summary>
public class UploadCompletedTrigger : StyledElementTrigger
{
    /// <summary>
    /// Identifies the <see cref="IsCompleted"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<bool> IsCompletedProperty =
        AvaloniaProperty.Register<UploadCompletedTrigger, bool>(nameof(IsCompleted));

    /// <summary>
    /// Gets or sets a value indicating upload completion. This is an avalonia property.
    /// </summary>
    public bool IsCompleted
    {
        get => GetValue(IsCompletedProperty);
        set => SetValue(IsCompletedProperty, value);
    }

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
        if (args.Sender is not UploadCompletedTrigger trigger)
        {
            return;
        }

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
