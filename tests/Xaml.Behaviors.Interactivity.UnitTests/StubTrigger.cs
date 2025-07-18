using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactivity.UnitTests;

public class StubTrigger : StyledElementTrigger
{
    public int AttachCount { get; private set; }
    public int DetachCount { get; private set; }

    protected override void OnAttached()
    {
        base.OnAttached();
        AttachCount++;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        DetachCount++;
    }
}
