using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactions.DragAndDrop;

namespace Avalonia.Xaml.Interactions.UnitTests.DragAndDrop;

internal class TestDropHandler : DropHandlerBase
{
    public object? LastSourceContext { get; set; }
    public object? LastTargetContext { get; set; }

    public override bool Validate(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
    {
        return true;
    }

    public override bool Execute(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
    {
        LastSourceContext = sourceContext;
        LastTargetContext = targetContext;
        return true;
    }
}
