#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactions.Custom;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.Xaml.Interactions.Custom;
using Avalonia.Xaml.Interactivity;
#endif
using Xunit;

#if UNO
namespace Xaml.Interactions.UnitTests.Custom;
#else
namespace Avalonia.Xaml.Interactions.UnitTests.Custom;
#endif

public class TextInputTriggerTests
{
    [AvaloniaFact]
    public void TextProperty_Is_Owned_By_TextInputTrigger()
    {
#if !UNO
        // WinUI dependency properties expose neither their name nor their owner type.
        Assert.Equal(nameof(TextInputTrigger.Text), TextInputTrigger.TextProperty.Name);
        Assert.Equal(typeof(TextInputTrigger), TextInputTrigger.TextProperty.OwnerType);
        Assert.True(AvaloniaPropertyRegistry.Instance.IsRegistered(typeof(TextInputTrigger), TextInputTrigger.TextProperty));
        Assert.False(AvaloniaPropertyRegistry.Instance.IsRegistered(typeof(KeyDownTrigger), TextInputTrigger.TextProperty));
        Assert.Null(AvaloniaPropertyRegistry.Instance.FindRegistered(typeof(KeyDownTrigger), nameof(TextInputTrigger.Text)));
#endif
        var trigger = new TextInputTrigger();

        Assert.Null(trigger.Text);

        trigger.Text = "a";

        Assert.Equal("a", trigger.Text);
        Assert.Equal("a", trigger.GetValue(TextInputTrigger.TextProperty));
    }

    [AvaloniaFact]
    public void Text_Filters_The_Text_Input()
    {
        var target = new Button { Content = "target" };
        var trigger = new TextInputTrigger { Text = "a" };
        var action = new CountingAction();
        trigger.Actions!.Add(action);
        Interaction.GetBehaviors(target).Add(trigger);
        var window = new Window { Width = 200, Height = 200, Content = target };

        window.Show();
        Assert.True(target.Focus());
        Dispatcher.UIThread.RunJobs();

        window.KeyTextInput("b");
        Dispatcher.UIThread.RunJobs();

        Assert.Equal(0, action.ExecutionCount);

        window.KeyTextInput("a");
        Dispatcher.UIThread.RunJobs();

        Assert.True(action.ExecutionCount > 0);

        // The filter applies while Text is set (a local null value filters as well); clearing it accepts any text.
        var filteredCount = action.ExecutionCount;
        trigger.ClearValue(TextInputTrigger.TextProperty);
        window.KeyTextInput("b");
        Dispatcher.UIThread.RunJobs();

        Assert.True(action.ExecutionCount > filteredCount);

        window.Close();
    }

#if UNO
    private sealed class CountingAction : Xaml.Interactivity.Action
#else
    private sealed class CountingAction : Avalonia.Xaml.Interactivity.Action
#endif
    {
        public int ExecutionCount { get; private set; }

        public override object? Execute(object? sender, object? parameter)
        {
            ExecutionCount++;
            return null;
        }
    }
}
