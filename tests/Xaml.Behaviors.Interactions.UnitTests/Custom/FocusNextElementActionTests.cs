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
        Assert.Equal(
            "CycleFirst",
            (FocusNavigationHelper.FindAdjacent(cycleContainer, cycleSecond, NavigationDirection.Next, wrap: false) as Button)?.Content);

        var action = new FocusNextElementAction();
        action.Execute(cycleSecond, null);

        Dispatcher.UIThread.RunJobs();

        Assert.Equal("CycleFirst", (window.FocusManager?.GetFocusedElement() as Button)?.Content);
    }

    [AvaloniaFact]
    public void Execute_DoesNotRaise_Tab_KeyDown_Handlers()
    {
        var first = CreateButton("First");
        var second = CreateButton("Second");
        var tabKeyDownCount = 0;

        var window = new Window
        {
            Width = 240,
            Height = 120,
            Content = new StackPanel
            {
                Children =
                {
                    first,
                    second
                }
            }
        };

        window.KeyDown += (_, e) =>
        {
            if (e.Key == Key.Tab)
            {
                tabKeyDownCount++;
            }
        };

        window.Show();
        window.CaptureRenderedFrame();

        Assert.True(first.Focus());

        var action = new FocusNextElementAction();
        action.Execute(first, null);

        Dispatcher.UIThread.RunJobs();

        Assert.Equal("Second", (window.FocusManager?.GetFocusedElement() as Button)?.Content);
        Assert.Equal(0, tabKeyDownCount);
    }

    private static Button CreateButton(string content)
    {
        return new Button
        {
            Content = content,
            Name = content,
            Width = 120,
            Height = 32,
            Focusable = true
        };
    }
}
