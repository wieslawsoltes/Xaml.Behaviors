using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Xunit;

namespace Avalonia.Xaml.Interactions.UnitTests.Core;

public class MultiDataTriggerBehaviorTests
{
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
}
