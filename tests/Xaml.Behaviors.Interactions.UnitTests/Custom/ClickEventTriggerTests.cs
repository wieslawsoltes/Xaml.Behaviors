using System;
using System.Linq;
using System.Threading.Tasks;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Dispatching;
using Xaml.Interactions.Core;
using Xaml.Interactions.Custom;
using Xaml.Interactions.UnitTests.Core;
#else
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.Xaml.Interactions.Core;
using Avalonia.Xaml.Interactions.Custom;
using Avalonia.Xaml.Interactions.UnitTests.Core;
#endif
using Xunit;

#if UNO
namespace Xaml.Interactions.UnitTests.Custom;
#else
namespace Avalonia.Xaml.Interactions.UnitTests.Custom;
#endif

public class ClickEventTriggerTests
{
    [AvaloniaFact]
    public async Task ClickEventTrigger_SaveFilePickerAction_Cancel_DoesNotExecuteCommand()
    {
        var commandCalls = 0;

        var window = new Window
        {
            Width = 200,
            Height = 120,
        };

        var target = new Border
        {
            Width = 160,
            Height = 60,
#if UNO
            IsTabStop = true,
#else
            Focusable = true,
#endif
        };

        var trigger = new ClickEventTrigger();
        var action = new SaveFilePickerAction
        {
            Command = new Command(_ => commandCalls++),
        };

        trigger.Actions ??= [];
        trigger.Actions.Add(action);
#if UNO
        Interaction.GetBehaviors(target).Add(trigger);
#else
        Avalonia.Xaml.Interactivity.Interaction.GetBehaviors(target).Add(trigger);
#endif

        window.Content = target;
        window.Show();

        window.Click(target);

        await Dispatcher.UIThread.InvokeAsync(() => { }, DispatcherPriority.Background);
        await Dispatcher.UIThread.InvokeAsync(() => { }, DispatcherPriority.Background);

        Assert.Equal(0, commandCalls);
    }

#if !UNO
    // WinUI has no tunneling pointer route: controls handle a pointer press in their own handlers before any
    // routed handler (the Uno port maps Tunnel to handledEventsToo), so a trigger cannot pre-empt them.
    [AvaloniaFact]
    public async Task ClickEventTrigger_ButtonWithPickerAction_Cancel_DoesNotInvokeNativeButtonCommand()
    {
        var nativeCommandCalls = 0;
        var pickerCommandCalls = 0;

        var window = new Window
        {
            Width = 200,
            Height = 120,
        };

        var button = new Button
        {
            Width = 160,
            Height = 60,
            Command = new Command(_ => nativeCommandCalls++),
        };

        var trigger = new ClickEventTrigger();
        var saveFilePickerAction = new SaveFilePickerAction
        {
            Command = new Command(_ => pickerCommandCalls++),
        };
        trigger.Actions ??= [];
        trigger.Actions.Add(saveFilePickerAction);
#if UNO
        Interaction.GetBehaviors(button).Add(trigger);
#else
        Avalonia.Xaml.Interactivity.Interaction.GetBehaviors(button).Add(trigger);
#endif

        window.Content = button;
        window.Show();

        window.Click(button);

        await Dispatcher.UIThread.InvokeAsync(() => { }, DispatcherPriority.Background);
        await Dispatcher.UIThread.InvokeAsync(() => { }, DispatcherPriority.Background);

        Assert.Equal(0, nativeCommandCalls);
        Assert.Equal(0, pickerCommandCalls);
    }
#endif

    [AvaloniaFact]
    public void ClickEventTrigger_PickerAction_UseCommandCanExecuteForIsEnabled_DisablesAssociatedPanel()
    {
        var command = new ObservableCommand { CanExecuteResult = false };

        var window = new Window
        {
            Width = 200,
            Height = 120,
        };

#if UNO
        // WinUI borders have no IsEnabled (it is a Control property): the panel is a content control.
        var target = new ContentControl
#else
        var target = new Border
#endif
        {
            Width = 160,
            Height = 60,
#if UNO
            IsTabStop = true,
#else
            Focusable = true,
#endif
        };

        var trigger = new ClickEventTrigger();
        var action = new OpenFilePickerAction
        {
            Command = command,
            CanExecuteCommandParameter = "probe",
            UseCommandCanExecuteForIsEnabled = true,
        };

        trigger.Actions ??= [];
        trigger.Actions.Add(action);
#if UNO
        Interaction.GetBehaviors(target).Add(trigger);
#else
        Avalonia.Xaml.Interactivity.Interaction.GetBehaviors(target).Add(trigger);
#endif

        window.Content = target;
        window.Show();

        Assert.False(target.IsEnabled);

        command.CanExecuteResult = true;
        command.RaiseCanExecuteChanged();

        Assert.True(target.IsEnabled);
    }

    [AvaloniaFact]
    public void ClickEventTrigger_AsyncActionGroup_DetachesChildCommandObservers()
    {
        var command = new ObservableCommand();

        var window = new Window
        {
            Width = 200,
            Height = 120,
        };

        var target = new Border
        {
            Width = 160,
            Height = 60,
#if UNO
            IsTabStop = true,
#else
            Focusable = true,
#endif
        };

        var trigger = new ClickEventTrigger();
        var group = new AsyncActionGroup();
        var action = new InvokeCommandAction
        {
            Command = command,
        };

        group.Actions ??= [];
        group.Actions.Add(action);

        trigger.Actions ??= [];
        trigger.Actions.Add(group);
#if UNO
        Interaction.GetBehaviors(target).Add(trigger);
#else
        Avalonia.Xaml.Interactivity.Interaction.GetBehaviors(target).Add(trigger);
#endif

        window.Content = target;
        window.Show();

        Assert.Equal(1, command.SubscriptionCount);

#if UNO
        Interaction.GetBehaviors(target).Remove(trigger);
#else
        Avalonia.Xaml.Interactivity.Interaction.GetBehaviors(target).Remove(trigger);
#endif

        Assert.Equal(0, command.SubscriptionCount);
#if UNO
        // WinUI has no logical tree: the actions are hosted by their parent (Action.Host).
        Assert.Null(group.Host);
        Assert.Null(action.Host);
#else
        Assert.Null(group.Parent);
        Assert.Null(action.Parent);
#endif
    }

    [AvaloniaFact]
    public async Task ClickEventTrigger_RoutingStrategies_PropertyChange_RewiresHandlers()
    {
        var nativeCommandCalls = 0;
        var pickerCommandCalls = 0;

        var window = new Window
        {
            Width = 200,
            Height = 120,
        };

        var button = new Button
        {
            Width = 160,
            Height = 60,
            Command = new Command(_ => nativeCommandCalls++),
        };

        var trigger = new ClickEventTrigger
        {
            RoutingStrategies = RoutingStrategies.Bubble
        };
        var saveFilePickerAction = new SaveFilePickerAction
        {
            Command = new Command(_ => pickerCommandCalls++),
        };

        trigger.Actions ??= [];
        trigger.Actions.Add(saveFilePickerAction);
#if UNO
        Interaction.GetBehaviors(button).Add(trigger);
#else
        Avalonia.Xaml.Interactivity.Interaction.GetBehaviors(button).Add(trigger);
#endif

        window.Content = button;
        window.Show();

        // Bubble route allows native Button command to run before the trigger consumes input.
        window.Click(button);
        await Dispatcher.UIThread.InvokeAsync(() => { }, DispatcherPriority.Background);
        await Dispatcher.UIThread.InvokeAsync(() => { }, DispatcherPriority.Background);

        Assert.Equal(1, nativeCommandCalls);
        Assert.Equal(0, pickerCommandCalls);

        // Switching to Tunnel should rewire handlers and suppress native Button command on cancel.
        trigger.RoutingStrategies = RoutingStrategies.Tunnel;

        window.Click(button);
        await Dispatcher.UIThread.InvokeAsync(() => { }, DispatcherPriority.Background);
        await Dispatcher.UIThread.InvokeAsync(() => { }, DispatcherPriority.Background);

#if UNO
        // WinUI has no tunneling pointer route: controls handle a pointer press in their own handlers before any
        // routed handler (the Uno port maps Tunnel to handledEventsToo), so a trigger cannot pre-empt them.
        // The rewired trigger receives the handled press and the cancelled picker executes no command.
        Assert.Equal(2, nativeCommandCalls);
#else
        Assert.Equal(1, nativeCommandCalls);
#endif
        Assert.Equal(0, pickerCommandCalls);
    }

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
#if UNO
        // The mouse of the Uno headless session outlives the test window: release the button for the next tests.
        window.MouseUp(window.PressTarget, new Point(10, 10), MouseButton.Left);
#endif
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

#if !UNO
    // WinUI has no tunneling pointer route: controls handle a pointer press in their own handlers before any
    // routed handler (the Uno port maps Tunnel to handledEventsToo), so a trigger cannot pre-empt them.
    // The window tunnel handlers of the test page cannot handle the press before the trigger.
    [AvaloniaFact]
    public void ClickEventTrigger_HandledEventsToo_DefaultFalse_AndPropertyChange_RewiresHandlers_ForButtonAndTextBoxSources()
    {
        var window = new ClickEventTrigger001();

        window.Show();

#if UNO
        var buttonTrigger = Interaction.GetBehaviors(window.HandledEventsTooHostTarget)
            .OfType<ClickEventTrigger>()
            .Single();
#else
        var buttonTrigger = Avalonia.Xaml.Interactivity.Interaction.GetBehaviors(window.HandledEventsTooHostTarget)
            .OfType<ClickEventTrigger>()
            .Single();
#endif
#if UNO
        var textBoxTrigger = Interaction.GetBehaviors(window.HandledEventsTooTextBoxHostTarget)
            .OfType<ClickEventTrigger>()
            .Single();
#else
        var textBoxTrigger = Avalonia.Xaml.Interactivity.Interaction.GetBehaviors(window.HandledEventsTooTextBoxHostTarget)
            .OfType<ClickEventTrigger>()
            .Single();
#endif

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
#endif

    [AvaloniaFact]
    public void ClickEventTrigger_HandledEventsToo_AlsoControlsKeyboardHandlers()
    {
        var window = new ClickEventTrigger001();

        window.Show();

#if UNO
        var textBoxTrigger = Interaction.GetBehaviors(window.HandledEventsTooTextBoxHostTarget)
            .OfType<ClickEventTrigger>()
            .Single();
#else
        var textBoxTrigger = Avalonia.Xaml.Interactivity.Interaction.GetBehaviors(window.HandledEventsTooTextBoxHostTarget)
            .OfType<ClickEventTrigger>()
            .Single();
#endif

        window.HandledEventsTooTextBoxSourceTarget.Focus();
#if UNO
        // WinUI cannot raise key events: the focused element receives a real key press and release.
        window.KeyPressQwerty(PhysicalKey.Space, RawInputModifiers.None);
#else
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
#endif
        Assert.Equal(0, window.HandledEventsTooTextBoxClicks);

        textBoxTrigger.HandledEventsToo = true;

        window.HandledEventsTooTextBoxSourceTarget.Focus();
#if UNO
        // WinUI cannot raise key events: the focused element receives a real key press and release.
        window.KeyPressQwerty(PhysicalKey.Space, RawInputModifiers.None);
#else
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
#endif
        Assert.Equal(1, window.HandledEventsTooTextBoxClicks);

        textBoxTrigger.HandledEventsToo = false;

        window.HandledEventsTooTextBoxSourceTarget.Focus();
#if UNO
        // WinUI cannot raise key events: the focused element receives a real key press and release.
        window.KeyPressQwerty(PhysicalKey.Space, RawInputModifiers.None);
#else
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
#endif
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
#if UNO
        // WinUI cannot raise key events: the focused element receives a real key press and release.
        window.KeyPressQwerty(PhysicalKey.Space, RawInputModifiers.None);
#else
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
#endif

        Assert.Equal(1, window.HandleEventFalseTextBoxClicks);
#if UNO
        // WinUI has no routed Button.ClickEvent the trigger could raise on a TextBox.
        Assert.Equal(0, window.HandleEventFalseTextBoxClickEvents);
#else
        Assert.Equal(1, window.HandleEventFalseTextBoxClickEvents);
#endif
        Assert.Equal(1, window.HandleEventFalseTextBoxBubbledKeyUp);
    }

    [AvaloniaFact]
    public void ClickEventTrigger_HandleEventFalse_TextBox_PointerClick_FiresTriggerAndClickEvent()
    {
        var window = new ClickEventTrigger001();

        window.Show();

        window.Click(window.HandleEventFalseTextBoxTarget);

        Assert.Equal(1, window.HandleEventFalseTextBoxClicks);
#if UNO
        // WinUI has no routed Button.ClickEvent the trigger could raise on a TextBox.
        Assert.Equal(0, window.HandleEventFalseTextBoxClickEvents);
#else
        Assert.Equal(1, window.HandleEventFalseTextBoxClickEvents);
#endif
    }

    [AvaloniaFact]
    public void ClickEventTrigger_HandleEventFalse_TextBox_AllowsPointerDragSelection()
    {
        var window = new ClickEventTrigger001();

        window.Show();

        var textBox = window.HandleEventFalseTextBoxTarget;
        textBox.Focus();
#if UNO
        textBox.SelectionStart = 0;
#else
        textBox.CaretIndex = 0;
#endif

        var y = textBox.Bounds.Height / 2;
        window.MouseDown(textBox, new Point(4, y), MouseButton.Left);
        window.MouseMove(textBox, new Point(textBox.Bounds.Width - 6, y), RawInputModifiers.LeftMouseButton);
        window.MouseUp(textBox, new Point(textBox.Bounds.Width - 6, y), MouseButton.Left);

#if UNO
        Assert.True(textBox.SelectionLength > 0);
#else
        Assert.True(Math.Abs(textBox.SelectionEnd - textBox.SelectionStart) > 0);
#endif
    }

    [AvaloniaFact]
    public void ClickEventTrigger_SpaceHandling_WorksForPressAndReleaseModes()
    {
        var window = new ClickEventTrigger001();

        window.Show();

        window.SpaceReleaseTarget.Focus();
#if UNO
        // WinUI cannot raise key events: the focused element receives a real key press and release.
        window.KeyPressQwerty(PhysicalKey.Space, RawInputModifiers.None);
#else
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
#endif
        Assert.Equal(1, window.SpaceReleaseClicks);

        window.SpacePressTarget.Focus();
#if UNO
        // WinUI cannot raise key events: the focused element receives a real key press and release.
        window.KeyPressQwerty(PhysicalKey.Space, RawInputModifiers.None);
#else
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
#endif
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
#if UNO
        // Uno Platform has no light dismiss pass through (FlyoutBase.OverlayInputPassThroughElement is not forwarded to
        // the popup): the light dismiss layer consumes the press and closes the flyout without a click.
        Assert.Equal(1, window.FlyoutClicks);
#else
        Assert.Equal(2, window.FlyoutClicks);
#endif
    }

}
