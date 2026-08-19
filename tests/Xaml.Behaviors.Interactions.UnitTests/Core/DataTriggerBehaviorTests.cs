using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.Xaml.Interactions.Core;
using Avalonia.Xaml.Interactivity;
using Xunit;

namespace Avalonia.Xaml.Interactions.UnitTests.Core;

public class DataTriggerBehaviorTests
{
    private sealed class CountingAction : StyledElementAction
    {
        public int ExecutionCount { get; private set; }

        public override object Execute(object? sender, object? parameter)
        {
            ExecutionCount++;
            return true;
        }
    }

    [AvaloniaFact]
    public void DataTriggerBehavior_001()
    {
        var window = new DataTriggerBehavior001();

        window.Show();
        window.CaptureRenderedFrame()?.Save("DataTriggerBehavior_001_0.png");

        Assert.Equal("Less than or equal 50", window.TargetTextBlock.Text);
        Assert.Equal("0", window.TargetTextBox.Text);
        Assert.Equal(0d, window.TargetSlider.Value);

        window.TargetSlider.Focus();
        window.KeyPressQwerty(PhysicalKey.ArrowRight, RawInputModifiers.None);
        window.KeyPressQwerty(PhysicalKey.ArrowRight, RawInputModifiers.None);
        window.KeyPressQwerty(PhysicalKey.ArrowRight, RawInputModifiers.None);

        window.CaptureRenderedFrame()?.Save("DataTriggerBehavior_001_1.png");

        Assert.Equal("More than 50", window.TargetTextBlock.Text);
        Assert.Equal("75", window.TargetTextBox.Text);
        Assert.Equal(75d, window.TargetSlider.Value);
    }

    [AvaloniaFact]
    public void DataTriggerBehavior_002()
    {
        var window = new DataTriggerBehavior002();

        window.Show();
        window.CaptureRenderedFrame()?.Save("DataTriggerBehavior_002_0.png");

        Assert.Equal("Unchecked", window.TargetTextBlock.Text);
        Assert.False(window.TargetCheckBox.IsChecked);

        window.Click(window.TargetCheckBox);

        window.CaptureRenderedFrame()?.Save("DataTriggerBehavior_002_1.png");

        Assert.Equal("Checked", window.TargetTextBlock.Text);
        Assert.True(window.TargetCheckBox.IsChecked);
    }

    [AvaloniaFact]
    public void DataTriggerBehavior_003()
    {
        var window = new DataTriggerBehavior003();

        window.Show();
        window.CaptureRenderedFrame()?.Save("DataTriggerBehavior_003_0.png");

        Assert.Equal("Less than 50", window.TargetTextBlock.Text);
        Assert.Equal(0d, window.TargetSlider.Value);

        window.TargetSlider.Focus();
        window.KeyPressQwerty(PhysicalKey.ArrowRight, RawInputModifiers.None);
        window.KeyPressQwerty(PhysicalKey.ArrowRight, RawInputModifiers.None);

        window.CaptureRenderedFrame()?.Save("DataTriggerBehavior_003_1.png");

        Assert.Equal("50 or more", window.TargetTextBlock.Text);
        Assert.Equal(50d, window.TargetSlider.Value);
    }

    [AvaloniaFact]
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Validates the reflection-based compatibility trigger.")]
    public void DataTriggerBehavior_NonConvertibleValue_DoesNotThrowOrExecuteActions()
    {
        var commandCalls = 0;
        var target = new Border();
        var trigger = new DataTriggerBehavior
        {
            Binding = 42,
            ComparisonCondition = ComparisonConditionType.Equal,
            Value = "not-an-integer",
        };
        var action = new InvokeCommandAction
        {
            Command = new Command(_ => commandCalls++),
        };
        trigger.Actions ??= [];
        trigger.Actions.Add(action);
        Interaction.GetBehaviors(target).Add(trigger);
        var window = new Window { Content = target };
        window.Show();

        var exception = Record.Exception(() => Dispatcher.UIThread.RunJobs());

        Assert.Null(exception);
        Assert.Equal(0, commandCalls);
    }

    [AvaloniaFact]
    public void DataTriggerBehavior_004_Reverts_On_False()
    {
        var window = new DataTriggerBehavior004();

        window.Show();
        window.CaptureRenderedFrame()?.Save("DataTriggerBehavior_004_0.png");

        Assert.Equal("Red", window.TargetTextBlock.Text);
        Assert.False(window.TargetCheckBox.IsChecked);

        window.Click(window.TargetCheckBox);

        window.CaptureRenderedFrame()?.Save("DataTriggerBehavior_004_1.png");

        Assert.Equal("Green", window.TargetTextBlock.Text);
        Assert.True(window.TargetCheckBox.IsChecked);

        window.Click(window.TargetCheckBox);

        window.CaptureRenderedFrame()?.Save("DataTriggerBehavior_004_2.png");

        Assert.Equal("Red", window.TargetTextBlock.Text);
        Assert.False(window.TargetCheckBox.IsChecked);
    }

    [AvaloniaFact]
    public void DataTriggerBehavior_004_Clears_Reversible_State_When_Disabled()
    {
        var window = new DataTriggerBehavior004();
        window.Show();
        var behavior = Assert.IsType<DataTriggerBehavior>(Assert.Single(
            Interaction.GetBehaviors(window.TargetTextBlock)));

        window.Click(window.TargetCheckBox);
        Assert.Equal("Green", window.TargetTextBlock.Text);

        behavior.RevertOnFalse = false;
        Dispatcher.UIThread.RunJobs();
        window.TargetTextBlock.Text = "External";
        behavior.RevertOnFalse = true;
        Dispatcher.UIThread.RunJobs();

        Assert.Equal("Green", window.TargetTextBlock.Text);
        window.Click(window.TargetCheckBox);
        Assert.Equal("External", window.TargetTextBlock.Text);
        window.Close();
    }

    [AvaloniaFact]
    public void DataTriggerBehavior_004_Reconciles_Reversible_State_When_IsEnabled_Changes()
    {
        var window = new DataTriggerBehavior004();
        window.Show();
        var behavior = Assert.IsType<DataTriggerBehavior>(Assert.Single(
            Interaction.GetBehaviors(window.TargetTextBlock)));
        window.Click(window.TargetCheckBox);
        Assert.Equal("Green", window.TargetTextBlock.Text);

        behavior.IsEnabled = false;
        Assert.Equal("Red", window.TargetTextBlock.Text);

        behavior.IsEnabled = true;
        Dispatcher.UIThread.RunJobs();
        Assert.Equal("Green", window.TargetTextBlock.Text);

        behavior.IsEnabled = false;
        window.Click(window.TargetCheckBox);
        behavior.IsEnabled = true;
        Dispatcher.UIThread.RunJobs();

        Assert.Equal("Red", window.TargetTextBlock.Text);
        window.Close();
    }

    [AvaloniaFact]
    [RequiresUnreferencedCode("Test intentionally constructs a reflection-based trigger.")]
    public void DataTriggerBehavior_Does_Not_Execute_Replaced_Actions_In_Legacy_Mode()
    {
        var initialAction = new CountingAction();
        var replacementAction = new CountingAction();
        var button = new Avalonia.Controls.Button();
        var behavior = new DataTriggerBehavior
        {
            Binding = true,
            Value = true,
            Actions = new ActionCollection { initialAction },
        };
        Interaction.SetBehaviors(button, new BehaviorCollection { behavior });
        var window = new Avalonia.Controls.Window { Content = button };
        window.Show();
        Dispatcher.UIThread.RunJobs();
        Assert.True(initialAction.ExecutionCount > 0);

        behavior.Actions = new ActionCollection { replacementAction };
        Dispatcher.UIThread.RunJobs();

        Assert.Equal(0, replacementAction.ExecutionCount);
        window.Close();
    }

    [AvaloniaFact]
    [RequiresUnreferencedCode("Test intentionally exercises reflection-based property actions.")]
    public void DataTriggerBehavior_004_Reverts_And_Clears_State_When_Removed()
    {
        var window = new DataTriggerBehavior004();
        window.Show();
        var behaviors = Interaction.GetBehaviors(window.TargetTextBlock);
        var behavior = Assert.IsType<DataTriggerBehavior>(Assert.Single(behaviors));
        window.Click(window.TargetCheckBox);
        Assert.Equal("Green", window.TargetTextBlock.Text);

        behaviors.Remove(behavior);

        Assert.Equal("Red", window.TargetTextBlock.Text);
        var laterAction = new ChangePropertyAction
        {
            PropertyName = nameof(Avalonia.Controls.TextBlock.Text),
            Value = "Blue",
        };
        Assert.True((bool)((IReversibleAction)laterAction).ExecuteReversibly(window.TargetTextBlock, null)!);
        Assert.True((bool)laterAction.Revert(window.TargetTextBlock, null));
        Assert.Equal("Red", window.TargetTextBlock.Text);
        window.Close();
    }

    [AvaloniaFact]
    [RequiresUnreferencedCode("Test intentionally exercises reflection-based property actions.")]
    public void DataTriggerBehavior_004_Reconciles_Actions_While_Active()
    {
        var window = new DataTriggerBehavior004();
        window.Show();
        var behavior = Assert.IsType<DataTriggerBehavior>(Assert.Single(
            Interaction.GetBehaviors(window.TargetTextBlock)));
        window.Click(window.TargetCheckBox);
        Assert.Equal("Green", window.TargetTextBlock.Text);

        behavior.Actions!.Clear();
        Dispatcher.UIThread.RunJobs();
        Assert.Equal("Red", window.TargetTextBlock.Text);

        behavior.Actions.Add(new ChangePropertyAction
        {
            PropertyName = nameof(Avalonia.Controls.TextBlock.Text),
            Value = "Blue",
        });
        Dispatcher.UIThread.RunJobs();
        Assert.Equal("Blue", window.TargetTextBlock.Text);

        window.Click(window.TargetCheckBox);
        Assert.Equal("Red", window.TargetTextBlock.Text);
        window.Close();
    }

    [AvaloniaFact]
    public void DataTriggerBehavior_ExecutesWhenBoundValueBecomesNull()
    {
        var window = new DataTriggerBehavior004();
        var source = Assert.IsType<DataTriggerBehavior004BindingSource>(window.DataContext);

        window.Show();
        Dispatcher.UIThread.RunJobs();

        Assert.Equal("Not empty", window.NullTargetTextBlock.Text);
        Assert.Equal("Unchanged", window.UnconfiguredTextBlock.Text);
        Assert.Equal("Unchanged", window.RelationalTextBlock.Text);

        source.TestProperty = null;
        Dispatcher.UIThread.RunJobs();

        Assert.Equal("Empty", window.NullTargetTextBlock.Text);
        Assert.Equal("Unchanged", window.UnconfiguredTextBlock.Text);
        Assert.Equal("Unchanged", window.RelationalTextBlock.Text);
    }

    [AvaloniaFact]
    public void DataTriggerBehavior_ExecutesWhenInitialBoundValueIsNull()
    {
        var window = new DataTriggerBehavior004();
        var source = Assert.IsType<DataTriggerBehavior004BindingSource>(window.DataContext);
        source.TestProperty = null;

        window.Show();
        Dispatcher.UIThread.RunJobs();

        Assert.Equal("Empty", window.NullTargetTextBlock.Text);
        Assert.Equal("Unchanged", window.UnconfiguredTextBlock.Text);
        Assert.Equal("Unchanged", window.RelationalTextBlock.Text);
    }
}
