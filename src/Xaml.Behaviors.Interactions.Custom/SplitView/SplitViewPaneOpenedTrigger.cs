using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Executes actions when the <see cref="SplitView.PaneOpened"/> event is raised.
/// </summary>
public class SplitViewPaneOpenedTrigger : StyledElementTrigger<SplitView>
{
    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        if (AssociatedObject is not null)
        {
            AssociatedObject.PaneOpened += OnPaneOpened;
        }
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        if (AssociatedObject is not null)
        {
            AssociatedObject.PaneOpened -= OnPaneOpened;
        }
    }

    private void OnPaneOpened(object? sender, RoutedEventArgs e)
    {
        Execute(e);
    }

    private void Execute(object? parameter)
    {
        if (!IsEnabled)
        {
            return;
        }

        if (AssociatedObject is not null)
        {
            Interaction.ExecuteActions(AssociatedObject, Actions, parameter);
        }
    }
}
