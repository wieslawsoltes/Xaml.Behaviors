#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Xaml.Interactions.DragAndDrop;
#else
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactions.DragAndDrop;
#endif

#if UNO
namespace Xaml.Interactions.UnitTests.DragAndDrop;
#else
namespace Avalonia.Xaml.Interactions.UnitTests.DragAndDrop;
#endif

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
