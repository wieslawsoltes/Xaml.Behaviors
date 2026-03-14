using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.Xaml.Interactions.Custom;
using Xunit;

namespace Avalonia.Xaml.Interactions.UnitTests.Custom;

public class FocusNextElementActionTests
{
    [AvaloniaFact]
    public void Execute_Uses_Framework_TabNavigation_Rules()
    {
        var before = CreateButton("Before");
        var cycleFirst = CreateButton("CycleFirst");
        var cycleSecond = CreateButton("CycleSecond");
        var after = CreateButton("After");

        var cycleContainer = new StackPanel
        {
            Children =
            {
                cycleFirst,
                cycleSecond
            }
        };

        KeyboardNavigation.SetTabNavigation(cycleContainer, KeyboardNavigationMode.Cycle);

        var window = new Window
        {
            Width = 240,
            Height = 160,
            Content = new StackPanel
            {
                Children =
                {
                    before,
                    cycleContainer,
                    after
                }
            }
        };

        window.Show();
        window.CaptureRenderedFrame();

        Assert.True(cycleSecond.Focus());

        var action = new FocusNextElementAction();
        action.Execute(cycleSecond, null);

        Dispatcher.UIThread.RunJobs();

        Assert.Same(cycleFirst, window.FocusManager?.GetFocusedElement());
    }

    private static Button CreateButton(string content)
    {
        return new Button
        {
            Content = content,
            Width = 120,
            Height = 32,
            Focusable = true
        };
    }
}
