using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Xaml.Interactions.Core;
using Xunit;

namespace Avalonia.Xaml.Interactions.UnitTests.Core;

public class PickerBehaviorBaseTests
{
    [AvaloniaFact]
    public void CanExecuteCommand_DefaultsToTrueWithoutCommand()
    {
        var behavior = new ButtonOpenFilePickerBehavior();
        var button = new Button();

        behavior.Attach(button);

        Assert.True(behavior.CanExecuteCommand);

        behavior.Detach();
    }

    [AvaloniaFact]
    public void CanExecuteCommand_UpdatesWhenAttached()
    {
        var behavior = new ButtonOpenFilePickerBehavior
        {
            Command = new ObservableCommand { CanExecuteResult = false }
        };
        var button = new Button();

        behavior.Attach(button);

        Assert.False(behavior.CanExecuteCommand);

        behavior.Detach();
    }

    [AvaloniaFact]
    public void CanExecuteCommand_UpdatesWhenCommandRaisesCanExecuteChanged()
    {
        var command = new ObservableCommand { CanExecuteResult = false };
        var behavior = new ButtonOpenFilePickerBehavior
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
        var behavior = new ButtonOpenFolderPickerBehavior
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
        var behavior = new ButtonSaveFilePickerBehavior
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
}
