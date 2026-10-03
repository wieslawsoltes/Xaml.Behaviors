#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Xaml.Interactions.DragAndDrop;
#else
using Avalonia.Input;
using Avalonia.Xaml.Interactions.DragAndDrop;
#endif

#if UNO
namespace Xaml.Interactions.UnitTests.DragAndDrop;
#else
namespace Avalonia.Xaml.Interactions.UnitTests.DragAndDrop;
#endif

internal class TestContextDragBehavior : ContextDragBehaviorBase
{
    public bool BeforeCalled { get; private set; }
    public bool AfterCalled { get; private set; }
    public int AttachedToVisualTreeCount { get; private set; }
    public int DetachedFromVisualTreeCount { get; private set; }

    protected override void OnAttachedToVisualTree()
    {
        base.OnAttachedToVisualTree();
        AttachedToVisualTreeCount++;
    }

    protected override void OnDetachedFromVisualTree()
    {
        base.OnDetachedFromVisualTree();
        DetachedFromVisualTreeCount++;
    }

    protected override void OnBeforeDragDrop(object? sender, PointerEventArgs e, object? context)
    {
        BeforeCalled = true;
    }

    protected override void OnAfterDragDrop(object? sender, PointerEventArgs e, object? context)
    {
        AfterCalled = true;
    }
}
