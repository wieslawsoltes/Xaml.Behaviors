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

public class MultiDataTriggerBehaviorTests
{
    private sealed class ExecutionPathAction : StyledElementAction, IReversibleAction
    {
        public int LegacyExecutionCount { get; private set; }
        public int ReversibleExecutionCount { get; private set; }

        public override object Execute(object? sender, object? parameter)
        {
            LegacyExecutionCount++;
            return true;
        }

        public object? ExecuteReversibly(object? sender, object? parameter)
        {
            ReversibleExecutionCount++;
            return true;
        }

        public object? Revert(object? sender, object? parameter)
        {
            return true;
        }
    }

    [AvaloniaFact]
    public void MultiDataTriggerBehavior_001()
    {
        var window = new MultiDataTriggerBehavior001();

        window.Show();
        window.CaptureRenderedFrame()?.Save("MultiDataTriggerBehavior_001_0.png");

        Assert.Equal("Move slider and check", window.TargetTextBlock.Text);
        Assert.False(window.TargetCheckBox.IsChecked);
        Assert.Equal(0d, window.TargetSlider.Value);

        window.Click(window.TargetCheckBox);

        window.CaptureRenderedFrame()?.Save("MultiDataTriggerBehavior_001_1.png");

        Assert.Equal("Slider too low", window.TargetTextBlock.Text);
        Assert.True(window.TargetCheckBox.IsChecked);

        window.TargetSlider.Focus();
        window.KeyPressQwerty(PhysicalKey.ArrowRight, RawInputModifiers.None);
        window.KeyPressQwerty(PhysicalKey.ArrowRight, RawInputModifiers.None);

        window.CaptureRenderedFrame()?.Save("MultiDataTriggerBehavior_001_2.png");

        Assert.Equal("Ready", window.TargetTextBlock.Text);
        Assert.Equal(50d, window.TargetSlider.Value);

        window.Click(window.TargetCheckBox);

        window.CaptureRenderedFrame()?.Save("MultiDataTriggerBehavior_001_3.png");

        Assert.Equal("Checkbox unchecked", window.TargetTextBlock.Text);
        Assert.False(window.TargetCheckBox.IsChecked);
    }

    [AvaloniaFact]
    public void MultiDataTriggerBehavior_002_Property_Conditions()
    {
        var window = new MultiDataTriggerBehavior002();

        window.Show();
        window.CaptureRenderedFrame()?.Save("MultiDataTriggerBehavior_002_0.png");

        Assert.Equal("Move slider and check", window.TargetTextBlock.Text);
        Assert.False(window.TargetCheckBox.IsChecked);
        Assert.Equal(0d, window.TargetSlider.Value);

        window.Click(window.TargetCheckBox);

        window.CaptureRenderedFrame()?.Save("MultiDataTriggerBehavior_002_1.png");

        Assert.Equal("Slider too low", window.TargetTextBlock.Text);
        Assert.True(window.TargetCheckBox.IsChecked);

        window.TargetSlider.Focus();
        window.KeyPressQwerty(PhysicalKey.ArrowRight, RawInputModifiers.None);
        window.KeyPressQwerty(PhysicalKey.ArrowRight, RawInputModifiers.None);

        window.CaptureRenderedFrame()?.Save("MultiDataTriggerBehavior_002_2.png");

        Assert.Equal("Ready", window.TargetTextBlock.Text);
        Assert.Equal(50d, window.TargetSlider.Value);

        window.Click(window.TargetCheckBox);

        window.CaptureRenderedFrame()?.Save("MultiDataTriggerBehavior_002_3.png");

        Assert.Equal("Checkbox unchecked", window.TargetTextBlock.Text);
        Assert.False(window.TargetCheckBox.IsChecked);
    }

    [AvaloniaFact]
    public void MultiDataTriggerBehavior_003_Reverts_On_False_When_Enabled()
    {
        var window = new MultiDataTriggerBehavior003();

        window.Show();
        window.CaptureRenderedFrame()?.Save("MultiDataTriggerBehavior_003_0.png");

        Assert.Equal("Red", window.TargetTextBlock.Text);
        Assert.False(window.TargetCheckBox.IsChecked);
        Assert.Equal(0d, window.TargetSlider.Value);

        window.Click(window.TargetCheckBox);
        window.TargetSlider.Focus();
        window.KeyPressQwerty(PhysicalKey.ArrowRight, RawInputModifiers.None);
        window.KeyPressQwerty(PhysicalKey.ArrowRight, RawInputModifiers.None);

        window.CaptureRenderedFrame()?.Save("MultiDataTriggerBehavior_003_1.png");

        Assert.Equal("Green", window.TargetTextBlock.Text);
        Assert.True(window.TargetCheckBox.IsChecked);
        Assert.Equal(50d, window.TargetSlider.Value);

        window.Click(window.TargetCheckBox);

        window.CaptureRenderedFrame()?.Save("MultiDataTriggerBehavior_003_2.png");

        Assert.Equal("Red", window.TargetTextBlock.Text);
        Assert.False(window.TargetCheckBox.IsChecked);
    }

    [AvaloniaFact]
    [RequiresUnreferencedCode("Test intentionally exercises reflection-based trigger comparison.")]
    public void MultiDataTriggerBehavior_UsesExecutionPathSelectedByRevertOnFalse()
    {
        var legacyAction = new ExecutionPathAction();
        var reversibleAction = new ExecutionPathAction();
        var button = new Button { IsEnabled = true };
        Interaction.SetBehaviors(button, new BehaviorCollection
        {
            CreateBehavior(revertOnFalse: false, legacyAction),
            CreateBehavior(revertOnFalse: true, reversibleAction),
        });
        var window = new Window { Content = button };

        window.Show();
        Dispatcher.UIThread.RunJobs();

        Assert.True(legacyAction.LegacyExecutionCount > 0);
        Assert.Equal(0, legacyAction.ReversibleExecutionCount);
        Assert.Equal(0, reversibleAction.LegacyExecutionCount);
        Assert.True(reversibleAction.ReversibleExecutionCount > 0);
        window.Close();
    }

    [AvaloniaFact]
    public void MultiDataTriggerBehavior_003_Clears_Reversible_State_When_Disabled()
    {
        var window = new MultiDataTriggerBehavior003();
        window.Show();
        var behavior = Assert.IsType<MultiDataTriggerBehavior>(Assert.Single(
            Interaction.GetBehaviors(window.TargetTextBlock)));
        window.Click(window.TargetCheckBox);
        window.TargetSlider.Focus();
        window.KeyPressQwerty(PhysicalKey.ArrowRight, RawInputModifiers.None);
        window.KeyPressQwerty(PhysicalKey.ArrowRight, RawInputModifiers.None);
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
    public void MultiDataTriggerBehavior_003_Reconciles_Reversible_State_When_IsEnabled_Changes()
    {
        var window = new MultiDataTriggerBehavior003();
        window.Show();
        var behavior = Assert.IsType<MultiDataTriggerBehavior>(Assert.Single(
            Interaction.GetBehaviors(window.TargetTextBlock)));
        window.Click(window.TargetCheckBox);
        window.TargetSlider.Focus();
        window.KeyPressQwerty(PhysicalKey.ArrowRight, RawInputModifiers.None);
        window.KeyPressQwerty(PhysicalKey.ArrowRight, RawInputModifiers.None);
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
    public void MultiDataTriggerBehavior_Does_Not_Execute_Replaced_Actions_In_Legacy_Mode()
    {
        var initialAction = new ExecutionPathAction();
        var replacementAction = new ExecutionPathAction();
        var behavior = CreateBehavior(revertOnFalse: false, initialAction);
        var button = new Button { IsEnabled = true };
        Interaction.SetBehaviors(button, new BehaviorCollection { behavior });
        var window = new Window { Content = button };
        window.Show();
        Dispatcher.UIThread.RunJobs();
        Assert.True(initialAction.LegacyExecutionCount > 0);

        behavior.Actions = new ActionCollection { replacementAction };
        Dispatcher.UIThread.RunJobs();

        Assert.Equal(0, replacementAction.LegacyExecutionCount);
        Assert.Equal(0, replacementAction.ReversibleExecutionCount);
        window.Close();
    }

    [AvaloniaFact]
    [RequiresUnreferencedCode("Test intentionally exercises reflection-based property actions.")]
    public void MultiDataTriggerBehavior_003_Reverts_And_Clears_State_When_Removed()
    {
        var window = new MultiDataTriggerBehavior003();
        window.Show();
        var behaviors = Interaction.GetBehaviors(window.TargetTextBlock);
        var behavior = Assert.IsType<MultiDataTriggerBehavior>(Assert.Single(behaviors));
        window.Click(window.TargetCheckBox);
        window.TargetSlider.Focus();
        window.KeyPressQwerty(PhysicalKey.ArrowRight, RawInputModifiers.None);
        window.KeyPressQwerty(PhysicalKey.ArrowRight, RawInputModifiers.None);
        Assert.Equal("Green", window.TargetTextBlock.Text);

        behaviors.Remove(behavior);

        Assert.Equal("Red", window.TargetTextBlock.Text);
        var laterAction = new ChangePropertyAction
        {
            PropertyName = nameof(TextBlock.Text),
            Value = "Blue",
        };
        Assert.True((bool)((IReversibleAction)laterAction).ExecuteReversibly(window.TargetTextBlock, null)!);
        Assert.True((bool)laterAction.Revert(window.TargetTextBlock, null));
        Assert.Equal("Red", window.TargetTextBlock.Text);
        window.Close();
    }

    [AvaloniaFact]
    [RequiresUnreferencedCode("Test intentionally exercises reflection-based property actions.")]
    public void MultiDataTriggerBehavior_003_Reconciles_Actions_While_Active()
    {
        var window = new MultiDataTriggerBehavior003();
        window.Show();
        var behavior = Assert.IsType<MultiDataTriggerBehavior>(Assert.Single(
            Interaction.GetBehaviors(window.TargetTextBlock)));
        window.Click(window.TargetCheckBox);
        window.TargetSlider.Focus();
        window.KeyPressQwerty(PhysicalKey.ArrowRight, RawInputModifiers.None);
        window.KeyPressQwerty(PhysicalKey.ArrowRight, RawInputModifiers.None);
        Assert.Equal("Green", window.TargetTextBlock.Text);

        behavior.Actions!.Clear();
        Dispatcher.UIThread.RunJobs();
        Assert.Equal("Red", window.TargetTextBlock.Text);

        behavior.Actions.Add(new ChangePropertyAction
        {
            PropertyName = nameof(TextBlock.Text),
            Value = "Blue",
        });
        Dispatcher.UIThread.RunJobs();
        Assert.Equal("Blue", window.TargetTextBlock.Text);

        window.Click(window.TargetCheckBox);
        Assert.Equal("Red", window.TargetTextBlock.Text);
        window.Close();
    }

    [RequiresUnreferencedCode("Test helper intentionally constructs a reflection-based trigger.")]
    private static MultiDataTriggerBehavior CreateBehavior(bool revertOnFalse, ExecutionPathAction action)
    {
        return new MultiDataTriggerBehavior
        {
            RevertOnFalse = revertOnFalse,
            Conditions = new ConditionCollection
            {
                new Condition
                {
                    Property = Button.IsEnabledProperty,
                    Value = true,
                },
            },
            Actions = new ActionCollection { action },
        };
    }
}
