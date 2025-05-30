using Avalonia.Controls;
using Avalonia.Xaml.Interactions.Core;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Trigger that listens for the <c>Opened</c> event of <see cref="ContextDialogBehavior"/>.
/// </summary>
public class ContextDialogOpenedTrigger : EventTriggerBehavior
{
    static ContextDialogOpenedTrigger()
    {
        EventNameProperty.OverrideMetadata<ContextDialogOpenedTrigger>(
            new StyledPropertyMetadata<string?>("Opened"));
    }
}
