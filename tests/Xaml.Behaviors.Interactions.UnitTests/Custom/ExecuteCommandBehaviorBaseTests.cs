#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactions.Custom;
using Xaml.Interactions.UnitTests.Core;
#else
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Xaml.Interactions.Custom;
using Avalonia.Xaml.Interactions.UnitTests.Core;
#endif
using Xunit;

#if UNO
namespace Xaml.Interactions.UnitTests.Custom;
#else
namespace Avalonia.Xaml.Interactions.UnitTests.Custom;
#endif

public class ExecuteCommandBehaviorBaseTests
{
    [AvaloniaFact]
    public void CanExecuteCommand_UpdatesWhenCommandRaisesCanExecuteChanged()
    {
        var command = new ObservableCommand { CanExecuteResult = false };
        var behavior = new ExecuteCommandOnTappedBehavior
        {
            Command = command
        };
        var button = new Button();

        behavior.Attach(button);
        Assert.False(behavior.CanExecuteCommand);

        command.CanExecuteResult = true;
        command.RaiseCanExecuteChanged();

        Assert.True(behavior.CanExecuteCommand);

        behavior.Detach();
    }

    [AvaloniaFact]
    public void CanExecuteCommand_UsesCurrentCommandParameter()
    {
        var command = new ObservableCommand
        {
            CanExecuteCallback = parameter => Equals(parameter, "enabled")
        };
        var behavior = new ExecuteCommandOnTappedBehavior
        {
            Command = command,
            CommandParameter = "disabled"
        };
        var button = new Button();

        behavior.Attach(button);
        Assert.False(behavior.CanExecuteCommand);

        behavior.CommandParameter = "enabled";

        Assert.True(behavior.CanExecuteCommand);
        Assert.Equal("enabled", command.LastCanExecuteParameter);

        behavior.Detach();
    }

    [AvaloniaFact]
    public void CanExecuteCommand_RewiresSubscriptionsWhenCommandChangesAndDetaches()
    {
        var firstCommand = new ObservableCommand { CanExecuteResult = false };
        var secondCommand = new ObservableCommand { CanExecuteResult = true };
        var behavior = new ExecuteCommandOnTappedBehavior
        {
            Command = firstCommand
        };
        var button = new Button();

        behavior.Attach(button);

        Assert.Equal(1, firstCommand.SubscriptionCount);
        Assert.False(behavior.CanExecuteCommand);

        behavior.Command = secondCommand;

        Assert.Equal(0, firstCommand.SubscriptionCount);
        Assert.Equal(1, secondCommand.SubscriptionCount);
        Assert.True(behavior.CanExecuteCommand);

        behavior.Detach();

        Assert.Equal(0, secondCommand.SubscriptionCount);
    }

    [AvaloniaFact]
    public void FocusControlProperty_Is_Registered_As_FocusControl()
    {
#if !UNO
        // WinUI dependency properties expose neither their name nor their owner type.
        Assert.Equal(nameof(ExecuteCommandBehaviorBase.FocusControl), ExecuteCommandBehaviorBase.FocusControlProperty.Name);
        Assert.Equal(typeof(ExecuteCommandBehaviorBase), ExecuteCommandBehaviorBase.FocusControlProperty.OwnerType);
        Assert.Same(
            ExecuteCommandBehaviorBase.FocusControlProperty,
            AvaloniaPropertyRegistry.Instance.FindRegistered(typeof(ExecuteCommandOnTappedBehavior), nameof(ExecuteCommandBehaviorBase.FocusControl)));
        Assert.Same(
            ExecuteCommandBehaviorBase.CommandParameterProperty,
            AvaloniaPropertyRegistry.Instance.FindRegistered(typeof(ExecuteCommandOnTappedBehavior), nameof(ExecuteCommandBehaviorBase.CommandParameter)));
#endif
        Assert.NotSame(ExecuteCommandBehaviorBase.CommandParameterProperty, ExecuteCommandBehaviorBase.FocusControlProperty);
    }

    [AvaloniaFact]
    public void FocusControl_And_CommandParameter_Are_Independent()
    {
        var focusControl = new Button();
        var behavior = new ExecuteCommandOnTappedBehavior
        {
            FocusControl = focusControl
        };

        Assert.Same(focusControl, behavior.FocusControl);
        Assert.Same(focusControl, behavior.GetValue(ExecuteCommandBehaviorBase.FocusControlProperty));
        Assert.Null(behavior.CommandParameter);

        behavior.CommandParameter = "parameter";

        Assert.Same(focusControl, behavior.FocusControl);
        Assert.Equal("parameter", behavior.CommandParameter);
    }
}
