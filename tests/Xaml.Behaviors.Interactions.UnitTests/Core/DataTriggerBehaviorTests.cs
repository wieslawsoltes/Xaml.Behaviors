using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Xunit;

namespace Avalonia.Xaml.Interactions.UnitTests.Core;

public class DataTriggerBehaviorTests
{
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
}
