using System.Threading.Tasks;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using VerifyXunit;
using Xunit;

namespace Avalonia.Xaml.Interactions.UnitTests.Core;

public class InteractionTriggerBehaviorTests
{
    [AvaloniaFact]
    public Task InteractionTriggerBehavior_001()
    {
        var window = new InteractionTriggerBehavior001();

        window.Show();
        window.CaptureRenderedFrame()?.Save("InteractionTriggerBehavior_001_0.png");

        window.TargetButton.Focus();
        window.KeyPressQwerty(PhysicalKey.Enter, RawInputModifiers.None);

        window.CaptureRenderedFrame()?.Save("InteractionTriggerBehavior_001_1.png");

        Assert.Equal("Triggered", window.TargetTextBox.Text);
        return Verifier.Verify(window);
    }
}
