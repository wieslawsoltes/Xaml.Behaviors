using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Controls;
using Avalonia.Xaml.Interactions.Core;
using Xunit;

namespace Avalonia.Xaml.Interactions.UnitTests.Core;

public class NewBehaviorsTests
{
    [AvaloniaFact]
    public void EventToCommandBehavior_CanBeCreated()
    {
        var behavior = new EventToCommandBehavior();
        Assert.NotNull(behavior);
    }

    [AvaloniaFact]
    public void GoToStateAction_CanBeCreated()
    {
        var action = new GoToStateAction();
        Assert.NotNull(action);
    }

    [AvaloniaFact]
    public void WindowEventCommandBehavior_CanBeCreated()
    {
        var behavior = new WindowEventCommandBehavior();
        Assert.NotNull(behavior);
    }

    [AvaloniaFact]
    public void NotifyErrorsCommandBehavior_CanBeCreated()
    {
        var behavior = new NotifyErrorsCommandBehavior();
        Assert.NotNull(behavior);
    }

    [AvaloniaFact]
    public void DataGridSelectionChangedCommandBehavior_CanBeCreated()
    {
        var behavior = new DataGridSelectionChangedCommandBehavior();
        Assert.NotNull(behavior);
    }

    [AvaloniaFact]
    public void DelayedCommandBehavior_CanBeCreated()
    {
        var behavior = new DelayedCommandBehavior();
        Assert.NotNull(behavior);
    }
}
