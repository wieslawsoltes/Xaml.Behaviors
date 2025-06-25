using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Xunit;

namespace Avalonia.Xaml.Interactivity.UnitTests;

public class TriggerTests
{
    [AvaloniaFact]
    public void Constructor_InitializesActionCollection()
    {
        var trigger = new StubTrigger();

        Assert.NotNull(trigger.Actions);
        Assert.Empty(trigger.Actions!);
    }

    [AvaloniaFact]
    public void Initialize_InitializesActions()
    {
        var trigger = new StubTrigger();
        var action = new StubAction();
        trigger.Actions!.Add(action);

        trigger.Initialize();

        Assert.True(action.IsInitialized);
    }

    [AvaloniaFact]
    public void Attach_ToTopLevel_AttachesActions()
    {
        var window = new Window();
        var trigger = new StubTrigger();
        var action = new StubAction();
        trigger.Actions!.Add(action);

        trigger.Initialize();
        trigger.Attach(window);

        Assert.Equal(window, action.Parent);
        Assert.Equal(1, trigger.AttachCount);
    }

    [AvaloniaFact]
    public void Detach_FromTopLevel_DetachesActions()
    {
        var window = new Window();
        var trigger = new StubTrigger();
        var action = new StubAction();
        trigger.Actions!.Add(action);

        trigger.Initialize();
        trigger.Attach(window);
        trigger.DetachBehaviorFromLogicalTree();

        Assert.Null(action.Parent);
    }

    [AvaloniaFact]
    public void ActionsPropertyChange_DetachesOldAndAttachesNew()
    {
        var window = new Window();
        var trigger = new StubTrigger();
        var oldAction = new StubAction();
        trigger.Actions!.Add(oldAction);

        trigger.Initialize();
        trigger.Attach(window);

        Assert.Equal(window, oldAction.Parent);

        var newAction = new StubAction();
        var collection = new ActionCollection { newAction };

        trigger.Actions = collection;

        Assert.Null(oldAction.Parent);
        Assert.Equal(window, newAction.Parent);
        Assert.True(newAction.IsInitialized);
    }
}
