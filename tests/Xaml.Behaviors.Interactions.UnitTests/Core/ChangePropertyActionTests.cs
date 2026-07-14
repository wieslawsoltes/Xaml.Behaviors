using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Xaml.Interactions.Core;
using Xunit;

namespace Avalonia.Xaml.Interactions.UnitTests.Core;

public class ChangePropertyActionTests
{
    private sealed class Grid
    {
    }

    private sealed class AttachedPropertyOwner : AvaloniaObject
    {
        public static readonly AttachedProperty<int> ValueProperty =
            AvaloniaProperty.RegisterAttached<AttachedPropertyOwner, Border, int>("Value");
    }

    /// <summary>
    /// Regular property.
    /// </summary>
    [AvaloniaFact]
    public void ChangePropertyAction_001()
    {
        var window = new ChangePropertyAction001();

        window.Show();
        window.CaptureRenderedFrame()?.Save("ChangePropertyAction_001_0.png");

        // Click
        window.TargetButton.Focus();
        window.KeyPressQwerty(PhysicalKey.Enter, RawInputModifiers.None);

        window.CaptureRenderedFrame()?.Save("ChangePropertyAction_001_1.png");

        Assert.Equal("Updated Text", window.TargetTextBox.Text);
    }

    /// <summary>
    /// Attached property.
    /// </summary>
    [AvaloniaFact]
    public void ChangePropertyAction_002()
    {
        var window = new ChangePropertyAction002();

        window.Show();
        window.CaptureRenderedFrame()?.Save("ChangePropertyAction_002_0.png");

        // Click
        window.TargetButton.Focus();
        window.KeyPressQwerty(PhysicalKey.Enter, RawInputModifiers.None);

        window.CaptureRenderedFrame()?.Save("ChangePropertyAction_002_1.png");

        Assert.Equal(12d, window.TargetTextBox.FontSize);
    }

    [AvaloniaFact]
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Validates the reflection-based compatibility action.")]
    public void ChangePropertyAction_UpdatesAttachedPropertyWhenOwnerTypeNameCollides()
    {
        _ = Avalonia.Controls.Grid.ColumnProperty;
        var target = new Border();
        var action = new ChangePropertyAction
        {
            PropertyName = "(Grid.Column)",
            Value = 2,
        };

        var result = action.Execute(target, null);

        Assert.Equal(true, result);
        Assert.Equal(2, Avalonia.Controls.Grid.GetColumn(target));
    }

    [AvaloniaFact]
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Validates the reflection-based compatibility action.")]
    public void ChangePropertyAction_FindsAttachedPropertyRegisteredForTargetType()
    {
        _ = AttachedPropertyOwner.ValueProperty;
        var target = new Border();
        var action = new ChangePropertyAction
        {
            PropertyName = "(AttachedPropertyOwner.Value)",
            Value = 42,
        };

        var result = action.Execute(target, null);

        Assert.Equal(true, result);
        Assert.Equal(42, target.GetValue(AttachedPropertyOwner.ValueProperty));
    }
}
