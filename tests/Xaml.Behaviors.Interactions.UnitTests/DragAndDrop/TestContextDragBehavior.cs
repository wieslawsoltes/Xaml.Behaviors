using Avalonia.Input;
using Avalonia.Xaml.Interactions.DragAndDrop;

namespace Avalonia.Xaml.Interactions.UnitTests.DragAndDrop;

internal class TestContextDragBehavior : ContextDragBehaviorBase
{
    public bool BeforeCalled { get; private set; }
    public bool AfterCalled { get; private set; }

    protected override void OnBeforeDragDrop(object? sender, PointerEventArgs e, object? context)
    {
        BeforeCalled = true;
    }

    protected override void OnAfterDragDrop(object? sender, PointerEventArgs e, object? context)
    {
        AfterCalled = true;
    }
}
