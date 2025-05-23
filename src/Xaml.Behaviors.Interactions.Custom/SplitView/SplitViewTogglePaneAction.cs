using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Toggles the <see cref="SplitView.IsPaneOpen"/> state of a <see cref="SplitView"/> when executed.
/// </summary>
public class SplitViewTogglePaneAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <seealso cref="TargetSplitView"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<SplitView?> TargetSplitViewProperty =
        AvaloniaProperty.Register<SplitViewTogglePaneAction, SplitView?>(nameof(TargetSplitView));

    /// <summary>
    /// Gets or sets the target <see cref="SplitView"/>. If not set, the sender is used.
    /// This is an avalonia property.
    /// </summary>
    [ResolveByName]
    public SplitView? TargetSplitView
    {
        get => GetValue(TargetSplitViewProperty);
        set => SetValue(TargetSplitViewProperty, value);
    }

    /// <inheritdoc />
    public override object? Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        var splitView = GetValue(TargetSplitViewProperty) is not null ? TargetSplitView : sender as SplitView;
        if (splitView is null)
        {
            return false;
        }

        splitView.IsPaneOpen = !splitView.IsPaneOpen;
        return true;
    }
}
