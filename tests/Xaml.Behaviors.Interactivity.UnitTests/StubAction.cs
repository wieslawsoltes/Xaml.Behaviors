#if UNO
namespace Xaml.Interactivity.UnitTests;
#else
namespace Avalonia.Xaml.Interactivity.UnitTests;
#endif

#if UNO
public partial class StubAction(object? returnValue) : Xaml.Interactivity.StyledElementAction
#else
public class StubAction(object? returnValue) : Avalonia.Xaml.Interactivity.StyledElementAction
#endif
{
    public StubAction() : this(null)
    {
    }

    public object? Sender
    {
        get;
        private set;
    }

    public object? Parameter
    {
        get;
        private set;
    }

    public int ExecuteCount
    {
        get;
        private set;
    }

    public override object? Execute(object? sender, object? parameter)
    {
        ExecuteCount++;
        Sender = sender;
        Parameter = parameter;
        return returnValue;
    }
}
