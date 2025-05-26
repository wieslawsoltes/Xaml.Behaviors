using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Automatically selects the entire content of the associated <see cref="TextBox"/> when it is loaded.
/// </summary>
public sealed class AutoSelectBehavior : StyledElementBehavior<TextBox>
{
    /// <inheritdoc/>
    protected override void OnLoaded()
    {
        base.OnLoaded();

        Dispatcher.UIThread.Post(() => AssociatedObject?.SelectAll());
    }
}
