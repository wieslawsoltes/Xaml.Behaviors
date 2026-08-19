using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Xaml.Interactions.Custom;
using Avalonia.Xaml.Interactions.UnitTests.Core;
using Xunit;

namespace Avalonia.Xaml.Interactions.UnitTests.Custom;

public class InvokeButtonClickActionTests
{
    [AvaloniaFact]
    public void Execute_Invokes_Button_Command_With_Parameter()
    {
        object? executedParameter = null;
        var clickCount = 0;
        var parameter = new object();
        var button = new Button
        {
            Command = new Command(value => executedParameter = value),
            CommandParameter = parameter
        };
        button.Click += (_, _) => clickCount++;
        var window = new Window { Content = button };
        var action = new InvokeButtonClickAction { TargetButton = button };

        window.Show();

        var result = action.Execute(sender: null, parameter: null);

        Assert.True(Assert.IsType<bool>(result));
        Assert.Equal(1, clickCount);
        Assert.Same(parameter, executedParameter);
    }

    [AvaloniaFact]
    public void Execute_Respects_Handled_Click_Event()
    {
        var commandCallCount = 0;
        var button = new Button
        {
            Command = new Command(_ => commandCallCount++)
        };
        button.Click += (_, args) => args.Handled = true;
        var window = new Window { Content = button };
        var action = new InvokeButtonClickAction { TargetButton = button };

        window.Show();

        var result = action.Execute(sender: null, parameter: null);

        Assert.True(Assert.IsType<bool>(result));
        Assert.Equal(0, commandCallCount);
    }

    [AvaloniaFact]
    public void Execute_Does_Not_Invoke_Disabled_Button()
    {
        var clickCount = 0;
        var commandCallCount = 0;
        var button = new Button
        {
            IsEnabled = false,
            Command = new Command(_ => commandCallCount++)
        };
        button.Click += (_, _) => clickCount++;
        var window = new Window { Content = button };
        var action = new InvokeButtonClickAction { TargetButton = button };

        window.Show();

        var result = action.Execute(sender: null, parameter: null);

        Assert.False(Assert.IsType<bool>(result));
        Assert.Equal(0, clickCount);
        Assert.Equal(0, commandCallCount);
    }
}
