using System;
using System.Linq;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactions.Custom;
using Xunit;

namespace Avalonia.Xaml.Interactions.UnitTests.Custom;

public class ClickEventTriggerTests
{
    [AvaloniaFact]
    public void ClickEventTrigger_ReleaseMode_DoesNotFireWhenReleasedOutside()
    {
        var window = new ClickEventTrigger001();

        window.Show();

        window.MouseDown(window.ReleaseTarget, new Point(10, 10), MouseButton.Left);
        window.MouseUp(window.OutsideTarget, new Point(10, 10), MouseButton.Left);

        Assert.Equal(0, window.ReleaseClicks);

        window.Click(window.ReleaseTarget);

        Assert.Equal(1, window.ReleaseClicks);
    }

    [AvaloniaFact]
    public void ClickEventTrigger_ReleaseMode_DoesNotLeakPressedStateAcrossInteractions()
    {
        var window = new ClickEventTrigger001();

        window.Show();

        window.MouseDown(window.ReleaseTarget, new Point(10, 10), MouseButton.Left);
        window.MouseUp(window.OutsideTarget, new Point(10, 10), MouseButton.Left);
        Assert.Equal(0, window.ReleaseClicks);

        window.MouseDown(window.OutsideTarget, new Point(10, 10), MouseButton.Left);
        window.MouseUp(window.ReleaseTarget, new Point(10, 10), MouseButton.Left);
        Assert.Equal(0, window.ReleaseClicks);
    }

    [AvaloniaFact]
    public void ClickEventTrigger_PressMode_FiresOnPointerDown()
    {
        var window = new ClickEventTrigger001();

        window.Show();

        window.MouseDown(window.PressTarget, new Point(10, 10), MouseButton.Left);

        Assert.Equal(1, window.PressClicks);
    }

    [AvaloniaFact]
    public void ClickEventTrigger_KeyModifiers_FilterBlocksAndAllows()
    {
        var window = new ClickEventTrigger001();

        window.Show();

        window.Click(window.ModifierTarget);
        Assert.Equal(0, window.ModifierClicks);

        window.Click(window.ModifierTarget, MouseButton.Left, RawInputModifiers.Control);
        Assert.Equal(1, window.ModifierClicks);
    }

    [AvaloniaFact]
    public void ClickEventTrigger_SourceControl_UsesExternalSourceControl()
    {
        var window = new ClickEventTrigger001();

        window.Show();

        window.Click(window.SourceControlHostTarget);
        Assert.Equal(0, window.SourceControlClicks);

        window.Click(window.SourceControlSourceTarget);
        Assert.Equal(1, window.SourceControlClicks);
    }

    [AvaloniaFact]
    public void ClickEventTrigger_HandledEventsToo_DefaultFalse_AndPropertyChange_RewiresHandlers_ForButtonAndTextBoxSources()
    {
        var window = new ClickEventTrigger001();

        window.Show();

        var buttonTrigger = Avalonia.Xaml.Interactivity.Interaction.GetBehaviors(window.HandledEventsTooHostTarget)
            .OfType<ClickEventTrigger>()
            .Single();
        var textBoxTrigger = Avalonia.Xaml.Interactivity.Interaction.GetBehaviors(window.HandledEventsTooTextBoxHostTarget)
            .OfType<ClickEventTrigger>()
            .Single();

        Assert.False(buttonTrigger.HandledEventsToo);
        Assert.False(textBoxTrigger.HandledEventsToo);

        window.Click(window.HandledEventsTooSourceTarget);
        Assert.Equal(0, window.HandledEventsTooClicks);
        window.Click(window.HandledEventsTooTextBoxSourceTarget);
        Assert.Equal(0, window.HandledEventsTooTextBoxClicks);

        buttonTrigger.HandledEventsToo = true;
        textBoxTrigger.HandledEventsToo = true;

        window.Click(window.HandledEventsTooSourceTarget);
        Assert.Equal(1, window.HandledEventsTooClicks);
        window.Click(window.HandledEventsTooTextBoxSourceTarget);
        Assert.Equal(1, window.HandledEventsTooTextBoxClicks);

        buttonTrigger.HandledEventsToo = false;
        textBoxTrigger.HandledEventsToo = false;

        window.Click(window.HandledEventsTooSourceTarget);
        Assert.Equal(1, window.HandledEventsTooClicks);
        window.Click(window.HandledEventsTooTextBoxSourceTarget);
        Assert.Equal(1, window.HandledEventsTooTextBoxClicks);
    }

    [AvaloniaFact]
    public void ClickEventTrigger_HandledEventsToo_AlsoControlsKeyboardHandlers()
    {
        var window = new ClickEventTrigger001();

        window.Show();

        var textBoxTrigger = Avalonia.Xaml.Interactivity.Interaction.GetBehaviors(window.HandledEventsTooTextBoxHostTarget)
            .OfType<ClickEventTrigger>()
            .Single();

        window.HandledEventsTooTextBoxSourceTarget.Focus();
        window.HandledEventsTooTextBoxSourceTarget.RaiseEvent(new KeyEventArgs
        {
            RoutedEvent = InputElement.KeyDownEvent,
            Key = Key.Space,
            KeyModifiers = KeyModifiers.None,
            Source = window.HandledEventsTooTextBoxSourceTarget
        });
        window.HandledEventsTooTextBoxSourceTarget.RaiseEvent(new KeyEventArgs
        {
            RoutedEvent = InputElement.KeyUpEvent,
            Key = Key.Space,
            KeyModifiers = KeyModifiers.None,
            Source = window.HandledEventsTooTextBoxSourceTarget
        });
        Assert.Equal(0, window.HandledEventsTooTextBoxClicks);

        textBoxTrigger.HandledEventsToo = true;

        window.HandledEventsTooTextBoxSourceTarget.Focus();
        window.HandledEventsTooTextBoxSourceTarget.RaiseEvent(new KeyEventArgs
        {
            RoutedEvent = InputElement.KeyDownEvent,
            Key = Key.Space,
            KeyModifiers = KeyModifiers.None,
            Source = window.HandledEventsTooTextBoxSourceTarget
        });
        window.HandledEventsTooTextBoxSourceTarget.RaiseEvent(new KeyEventArgs
        {
            RoutedEvent = InputElement.KeyUpEvent,
            Key = Key.Space,
            KeyModifiers = KeyModifiers.None,
            Source = window.HandledEventsTooTextBoxSourceTarget
        });
        Assert.Equal(1, window.HandledEventsTooTextBoxClicks);

        textBoxTrigger.HandledEventsToo = false;

        window.HandledEventsTooTextBoxSourceTarget.Focus();
        window.HandledEventsTooTextBoxSourceTarget.RaiseEvent(new KeyEventArgs
        {
            RoutedEvent = InputElement.KeyDownEvent,
            Key = Key.Space,
            KeyModifiers = KeyModifiers.None,
            Source = window.HandledEventsTooTextBoxSourceTarget
        });
        window.HandledEventsTooTextBoxSourceTarget.RaiseEvent(new KeyEventArgs
        {
            RoutedEvent = InputElement.KeyUpEvent,
            Key = Key.Space,
            KeyModifiers = KeyModifiers.None,
            Source = window.HandledEventsTooTextBoxSourceTarget
        });
        Assert.Equal(1, window.HandledEventsTooTextBoxClicks);
    }

    [AvaloniaFact]
    public void ClickEventTrigger_HandleEventFalse_Button_PreservesNativeClick()
    {
        var window = new ClickEventTrigger001();

        window.Show();

        window.Click(window.HandleEventFalseButtonTarget);

        Assert.Equal(1, window.HandleEventFalseButtonClicks);
        Assert.Equal(1, window.HandleEventFalseButtonNativeClicks);
        Assert.Equal(1, window.HandleEventFalseButtonClickEvents);
    }

    [AvaloniaFact]
    public void ClickEventTrigger_HandleEventFalse_TextBox_PreservesKeyBubbling()
    {
        var window = new ClickEventTrigger001();

        window.Show();

        window.HandleEventFalseTextBoxTarget.Focus();
        window.HandleEventFalseTextBoxTarget.RaiseEvent(new KeyEventArgs
        {
            RoutedEvent = InputElement.KeyDownEvent,
            Key = Key.Space,
            KeyModifiers = KeyModifiers.None,
            Source = window.HandleEventFalseTextBoxTarget
        });
        window.HandleEventFalseTextBoxTarget.RaiseEvent(new KeyEventArgs
        {
            RoutedEvent = InputElement.KeyUpEvent,
            Key = Key.Space,
            KeyModifiers = KeyModifiers.None,
            Source = window.HandleEventFalseTextBoxTarget
        });

        Assert.Equal(1, window.HandleEventFalseTextBoxClicks);
        Assert.Equal(1, window.HandleEventFalseTextBoxClickEvents);
        Assert.Equal(1, window.HandleEventFalseTextBoxBubbledKeyUp);
    }

    [AvaloniaFact]
    public void ClickEventTrigger_HandleEventFalse_TextBox_AllowsPointerDragSelection()
    {
        var window = new ClickEventTrigger001();

        window.Show();

        var textBox = window.HandleEventFalseTextBoxTarget;
        textBox.Focus();
        textBox.CaretIndex = 0;

        var y = textBox.Bounds.Height / 2;
        window.MouseDown(textBox, new Point(4, y), MouseButton.Left);
        window.MouseMove(textBox, new Point(textBox.Bounds.Width - 6, y), RawInputModifiers.LeftMouseButton);
        window.MouseUp(textBox, new Point(textBox.Bounds.Width - 6, y), MouseButton.Left);

        Assert.True(Math.Abs(textBox.SelectionEnd - textBox.SelectionStart) > 0);
    }

    [AvaloniaFact]
    public void ClickEventTrigger_SpaceHandling_WorksForPressAndReleaseModes()
    {
        var window = new ClickEventTrigger001();

        window.Show();

        window.SpaceReleaseTarget.Focus();
        window.SpaceReleaseTarget.RaiseEvent(new KeyEventArgs
        {
            RoutedEvent = InputElement.KeyDownEvent,
            Key = Key.Space,
            KeyModifiers = KeyModifiers.None,
            Source = window.SpaceReleaseTarget
        });
        window.SpaceReleaseTarget.RaiseEvent(new KeyEventArgs
        {
            RoutedEvent = InputElement.KeyUpEvent,
            Key = Key.Space,
            KeyModifiers = KeyModifiers.None,
            Source = window.SpaceReleaseTarget
        });
        Assert.Equal(1, window.SpaceReleaseClicks);

        window.SpacePressTarget.Focus();
        window.SpacePressTarget.RaiseEvent(new KeyEventArgs
        {
            RoutedEvent = InputElement.KeyDownEvent,
            Key = Key.Space,
            KeyModifiers = KeyModifiers.None,
            Source = window.SpacePressTarget
        });
        window.SpacePressTarget.RaiseEvent(new KeyEventArgs
        {
            RoutedEvent = InputElement.KeyUpEvent,
            Key = Key.Space,
            KeyModifiers = KeyModifiers.None,
            Source = window.SpacePressTarget
        });
        Assert.Equal(1, window.SpacePressClicks);
    }

    [AvaloniaFact]
    public void ClickEventTrigger_IsDefault_RootEnter_TriggersAndDoesNotDuplicate()
    {
        var window = new ClickEventTrigger001();

        window.Show();

        window.FocusSource.Focus();
        window.KeyPressQwerty(PhysicalKey.Enter, RawInputModifiers.None);
        Assert.Equal(1, window.DefaultClicks);

        window.DefaultTarget.Focus();
        window.KeyPressQwerty(PhysicalKey.Enter, RawInputModifiers.None);
        Assert.Equal(2, window.DefaultClicks);
    }

    [AvaloniaFact]
    public void ClickEventTrigger_IsCancel_RootEscape_TriggersAndDoesNotDuplicate()
    {
        var window = new ClickEventTrigger001();

        window.Show();

        window.FocusSource.Focus();
        window.KeyPressQwerty(PhysicalKey.Escape, RawInputModifiers.None);
        Assert.Equal(1, window.CancelClicks);

        window.CancelTarget.Focus();
        window.KeyPressQwerty(PhysicalKey.Escape, RawInputModifiers.None);
        Assert.Equal(1, window.CancelClicks);
    }

    [AvaloniaFact]
    public void ClickEventTrigger_Flyout_TogglesOpenAndClose()
    {
        var window = new ClickEventTrigger001();

        window.Show();

        var flyout = FlyoutBase.GetAttachedFlyout(window.FlyoutTarget);
        Assert.NotNull(flyout);
        Assert.False(flyout!.IsOpen);

        window.Click(window.FlyoutTarget);

        Assert.True(flyout.IsOpen);
        Assert.Equal(1, window.FlyoutClicks);

        window.Click(window.FlyoutTarget);

        Assert.False(flyout.IsOpen);
        Assert.Equal(2, window.FlyoutClicks);
    }
}
