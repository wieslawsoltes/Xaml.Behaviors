using Avalonia.Controls;
using Avalonia.Xaml.Interactions.Core;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Trigger that listens for the <c>Closed</c> event of <see cref="ContextDialogBehavior"/>.
/// </summary>
public class ContextDialogClosedTrigger : EventTriggerBehavior
{
    static ContextDialogClosedTrigger()
    {
        EventNameProperty.OverrideMetadata<ContextDialogClosedTrigger>(
            new StyledPropertyMetadata<string?>("Closed"));
    }
}
