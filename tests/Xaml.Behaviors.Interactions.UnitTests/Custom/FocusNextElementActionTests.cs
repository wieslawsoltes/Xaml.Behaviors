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

    [AvaloniaFact]
    public void Execute_Skips_None_TabNavigation_Children()
    {
        var before = CreateButton("Before");
        var skippedChild = CreateButton("SkippedChild");
        var after = CreateButton("After");

        var noneContainer = new StackPanel
        {
            Children =
            {
                skippedChild
            }
        };

        KeyboardNavigation.SetTabNavigation(noneContainer, KeyboardNavigationMode.None);

        var window = new Window
        {
            Width = 240,
            Height = 160,
            Content = new StackPanel
            {
                Children =
                {
                    before,
                    noneContainer,
                    after
                }
            }
        };

        window.Show();
        window.CaptureRenderedFrame();

        Assert.True(before.Focus());

        var action = new FocusNextElementAction();
        action.Execute(before, null);

        Dispatcher.UIThread.RunJobs();

        Assert.Equal("After", (window.FocusManager?.GetFocusedElement() as Button)?.Content);
    }

    [AvaloniaFact]
    public void Execute_Uses_TabOnceActiveElement_For_Once_Containers()
    {
        var before = CreateButton("Before");
        var onceFirst = CreateButton("OnceFirst");
        var onceSecond = CreateButton("OnceSecond");
        var after = CreateButton("After");

        var onceContainer = new StackPanel
        {
            Children =
            {
                onceFirst,
                onceSecond
            }
        };

        KeyboardNavigation.SetTabNavigation(onceContainer, KeyboardNavigationMode.Once);
        KeyboardNavigation.SetTabOnceActiveElement(onceContainer, onceSecond);

        var window = new Window
        {
            Width = 240,
            Height = 160,
            Content = new StackPanel
            {
                Children =
                {
                    before,
                    onceContainer,
                    after
                }
            }
        };

        window.Show();
        window.CaptureRenderedFrame();

        var action = new FocusNextElementAction();

        Assert.True(before.Focus());
        action.Execute(before, null);
        Dispatcher.UIThread.RunJobs();
        Assert.Equal("OnceSecond", (window.FocusManager?.GetFocusedElement() as Button)?.Content);

        action.Execute(onceSecond, null);
        Dispatcher.UIThread.RunJobs();
        Assert.Equal("After", (window.FocusManager?.GetFocusedElement() as Button)?.Content);
    }

    [AvaloniaFact]
    public void Execute_Respects_Local_TabNavigation_Subtree_Order()
    {
        var localFirst = CreateButton("LocalFirst");
        localFirst.TabIndex = 10;

        var localSecond = CreateButton("LocalSecond");
        localSecond.TabIndex = 0;

        var after = CreateButton("After");
        after.TabIndex = 1;

        var localContainer = new StackPanel
        {
            Children =
            {
                localFirst,
                localSecond
            }
        };

        KeyboardNavigation.SetTabNavigation(localContainer, KeyboardNavigationMode.Local);

        var window = new Window
        {
            Width = 240,
            Height = 160,
            Content = new StackPanel
            {
                Children =
                {
                    localContainer,
                    after
                }
            }
        };

        window.Show();
        window.CaptureRenderedFrame();

        Assert.True(localSecond.Focus());

        var action = new FocusNextElementAction();
        action.Execute(localSecond, null);

        Dispatcher.UIThread.RunJobs();

        Assert.Equal("LocalFirst", (window.FocusManager?.GetFocusedElement() as Button)?.Content);
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
