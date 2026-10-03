#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactivity.UnitTests;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Xaml.Interactivity.UnitTests;
using Avalonia.Xaml.Interactivity;
#endif
using Xunit;

#if UNO
namespace Xaml.Interactivity.UnitTests;
#else
namespace Avalonia.Xaml.Interactivity.UnitTests;
#endif

public class StyledElementActionTests
{
    [AvaloniaFact]
    public void IsEnabled_Defaults_True()
    {
        var action = new StubAction();

        Assert.True(action.IsEnabled);
    }

    [AvaloniaFact]
    public void IsEnabled_CanBeSet()
    {
        var action = new StubAction();

        action.IsEnabled = false;

        Assert.False(action.IsEnabled);
    }

    [AvaloniaFact]
    public void Execute_RecordsParametersAndReturnsValue()
    {
        var action = new StubAction("Result");
        var sender = new Button();
        var parameter = new object();

        var result = action.Execute(sender, parameter);

        Assert.Equal("Result", result);
        Assert.Equal(1, action.ExecuteCount);
        Assert.Equal(sender, action.Sender);
        Assert.Equal(parameter, action.Parameter);
    }

#if !UNO
    // WinUI has no Initialized event: the Uno StyledElementAction has no initialization state.
    [AvaloniaFact]
    public void Initialize_SetsIsInitialized()
    {
        var action = new StubAction();
        var initializedCount = 0;
        action.Initialized += (_, _) => initializedCount++;

        Assert.False(action.IsInitialized);

        action.Initialize();

        Assert.True(action.IsInitialized);
        Assert.Equal(1, initializedCount);

        // Subsequent calls should not raise event again
        action.Initialize();
        Assert.Equal(1, initializedCount);
    }
#endif

    [AvaloniaFact]
    public void AttachActionToLogicalTree_SetsParentAndTemplatedParent()
    {
        var action = new StubAction();
        var parent = new Button();
#if UNO
        // WinUI has no logical tree and no settable templated parent: the action joins the action tree of its
        // parent (Action.Host).
        action.AttachActionToLogicalTree(parent);

        Assert.Equal(parent, action.Host);
        Assert.True(((ILogical)action).IsAttachedToLogicalTree);
#else
        var templatedParent = new ContentControl();
        TemplatedParentHelper.SetTemplatedParent(parent, templatedParent);

        action.AttachActionToLogicalTree(parent);

        Assert.Equal(parent, action.Parent);
        Assert.Equal(templatedParent, action.TemplatedParent);
#endif
    }

    [AvaloniaFact]
    public void DetachActionFromLogicalTree_ClearsParentAndTemplatedParent()
    {
        var action = new StubAction();
        var parent = new Button();
#if UNO
        // WinUI has no logical tree and no settable templated parent: the action leaves the action tree (Action.Host).
        action.AttachActionToLogicalTree(parent);
        action.DetachActionFromLogicalTree(parent);

        Assert.Null(action.Host);
        Assert.False(((ILogical)action).IsAttachedToLogicalTree);
#else
        var templatedParent = new ContentControl();
        TemplatedParentHelper.SetTemplatedParent(parent, templatedParent);

        action.AttachActionToLogicalTree(parent);
        action.DetachActionFromLogicalTree(parent);

        Assert.Null(action.Parent);
        Assert.Null(action.TemplatedParent);
#endif
    }
}
