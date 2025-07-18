using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Xaml.Interactivity.UnitTests;
using Avalonia.Xaml.Interactivity;
using Xunit;

namespace Avalonia.Xaml.Interactivity.UnitTests;

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

    [AvaloniaFact]
    public void AttachActionToLogicalTree_SetsParentAndTemplatedParent()
    {
        var action = new StubAction();
        var parent = new Button();
        var templatedParent = new ContentControl();
        TemplatedParentHelper.SetTemplatedParent(parent, templatedParent);

        action.AttachActionToLogicalTree(parent);

        Assert.Equal(parent, action.Parent);
        Assert.Equal(templatedParent, action.TemplatedParent);
    }
}
