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

        Assert.False(window.TargetBorder.IsVisible);
        Assert.Equal(string.Empty, window.TargetTextBlock.Text);

        window.InputTextBox.Text = "Hello";

        window.CaptureRenderedFrame()?.Save("DataTriggerBehavior_002_1.png");

        Assert.True(window.TargetBorder.IsVisible);
        Assert.Equal("Hello", window.TargetTextBlock.Text);
    }

    [AvaloniaFact]
    public void DataTriggerBehavior_003()
    {
        var window = new DataTriggerBehavior003();

        window.Show();
        window.CaptureRenderedFrame()?.Save("DataTriggerBehavior_003_0.png");

        Assert.Equal("Less than 25", window.TargetTextBlock.Text);
        Assert.Equal(0d, window.TargetSlider.Value);

        window.TargetSlider.Value = 30d;

        window.CaptureRenderedFrame()?.Save("DataTriggerBehavior_003_1.png");

        Assert.Equal("Greater or equal 25", window.TargetTextBlock.Text);
        Assert.Equal(30d, window.TargetSlider.Value);
    }
}
