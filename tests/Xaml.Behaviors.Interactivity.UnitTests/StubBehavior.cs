using System.Collections.Generic;

#if UNO
namespace Xaml.Interactivity.UnitTests;
#else
namespace Avalonia.Xaml.Interactivity.UnitTests;
#endif

public partial class StubBehavior : AvaloniaObject, IBehavior
{
    public int AttachCount
    {
        get;
        private set;
    }

    public int DetachCount
    {
        get;
        private set;
    }

    public ActionCollection Actions { get; private set; } = [];

    public AvaloniaObject? AssociatedObject
    {
        get;
        private set;
    }

    public void Attach(AvaloniaObject? avaloniaObject)
    {
        AssociatedObject = avaloniaObject;
        AttachCount++;
    }

    public void Detach()
    {
        AssociatedObject = null;
        DetachCount++;
    }

    public IEnumerable<object> Execute(object? sender, object parameter)
    {
        return Interaction.ExecuteActions(sender, Actions, parameter);
    }
}
