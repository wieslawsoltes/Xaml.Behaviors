using Avalonia;

namespace Avalonia.Xaml.Interactivity.UnitTests;

public class StubTrigger : Trigger
{
    public int AttachCount { get; private set; }
    public int DetachCount { get; private set; }

    protected override void OnAttached()
    {
        AttachCount++;
        base.OnAttached();
    }

    protected override void OnDetaching()
    {
        DetachCount++;
        base.OnDetaching();
    }
}

public class StubTrigger<T> : Trigger<T> where T : AvaloniaObject
{
    public int AttachCount { get; private set; }
    public int DetachCount { get; private set; }

    protected override void OnAttached()
    {
        AttachCount++;
        base.OnAttached();
    }

    protected override void OnDetaching()
    {
        DetachCount++;
        base.OnDetaching();
    }
}
