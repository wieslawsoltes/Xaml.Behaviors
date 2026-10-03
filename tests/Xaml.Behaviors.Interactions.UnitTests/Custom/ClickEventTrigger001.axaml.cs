#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
#else
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
#endif

#if UNO
namespace Xaml.Interactions.UnitTests.Custom;
#else
namespace Avalonia.Xaml.Interactions.UnitTests.Custom;
#endif

public partial class ClickEventTrigger001 : Window
{
    public int ReleaseClicks { get; private set; }
    public int PressClicks { get; private set; }
    public int ModifierClicks { get; private set; }
    public int SourceControlClicks { get; private set; }
    public int HandledEventsTooClicks { get; private set; }
    public int HandledEventsTooTextBoxClicks { get; private set; }
    public int HandleEventFalseButtonClicks { get; private set; }
    public int HandleEventFalseTextBoxClicks { get; private set; }
    public int HandleEventFalseButtonNativeClicks { get; private set; }
    public int HandleEventFalseButtonClickEvents { get; private set; }
    public int HandleEventFalseTextBoxClickEvents { get; private set; }
    public int HandleEventFalseTextBoxBubbledKeyUp { get; private set; }
    public int SpaceReleaseClicks { get; private set; }
    public int SpacePressClicks { get; private set; }
    public int DefaultClicks { get; private set; }
    public int CancelClicks { get; private set; }
    public int FlyoutClicks { get; private set; }

    public ClickEventTrigger001()
    {
        InitializeComponent();
        DataContext = this;
        HandleEventFalseButtonTarget.Click += OnHandleEventFalseButtonNativeClick;
#if UNO
        // WinUI click events are CLR events of ButtonBase (no routed Button.ClickEvent): the trigger cannot raise a
        // click event on other controls, so only the button's own click is observed.
        HandleEventFalseButtonTarget.Click += OnHandleEventFalseButtonClickEvent;
#else
        HandleEventFalseButtonTarget.AddHandler(Button.ClickEvent, OnHandleEventFalseButtonClickEvent, RoutingStrategies.Bubble);
        HandleEventFalseTextBoxTarget.AddHandler(Button.ClickEvent, OnHandleEventFalseTextBoxClickEvent, RoutingStrategies.Bubble);
#endif

#if UNO
        // The Avalonia routed event helpers of the Uno port (the WinUI instance AddHandler takes three arguments).
        this.AddHandler(
            InputElement.PointerPressedEvent,
            OnHandledEventsTooWindowPointerPressed,
            RoutingStrategies.Tunnel,
            handledEventsToo: false);
        this.AddHandler(
            InputElement.PointerReleasedEvent,
            OnHandledEventsTooWindowPointerReleased,
            RoutingStrategies.Tunnel,
            handledEventsToo: false);
        this.AddHandler(
            InputElement.KeyDownEvent,
            OnHandledEventsTooWindowKeyDown,
            RoutingStrategies.Tunnel,
            handledEventsToo: false);
        this.AddHandler(
            InputElement.KeyUpEvent,
            OnHandledEventsTooWindowKeyUp,
            RoutingStrategies.Tunnel,
            handledEventsToo: false);
        this.AddHandler(
            InputElement.KeyUpEvent,
            OnHandleEventFalseWindowKeyUp,
            RoutingStrategies.Bubble,
            handledEventsToo: false);
#else
        AddHandler(
            InputElement.PointerPressedEvent,
            OnHandledEventsTooWindowPointerPressed,
            RoutingStrategies.Tunnel);
        AddHandler(
            InputElement.PointerReleasedEvent,
            OnHandledEventsTooWindowPointerReleased,
            RoutingStrategies.Tunnel);
        AddHandler(
            InputElement.KeyDownEvent,
            OnHandledEventsTooWindowKeyDown,
            RoutingStrategies.Tunnel);
        AddHandler(
            InputElement.KeyUpEvent,
            OnHandledEventsTooWindowKeyUp,
            RoutingStrategies.Tunnel);
        AddHandler(
            InputElement.KeyUpEvent,
            OnHandleEventFalseWindowKeyUp,
            RoutingStrategies.Bubble);
#endif
    }

    public void OnReleaseClicked() => ReleaseClicks++;

    public void OnPressClicked() => PressClicks++;

    public void OnModifierClicked() => ModifierClicks++;

    public void OnSourceControlClicked() => SourceControlClicks++;

    public void OnHandledEventsTooClicked() => HandledEventsTooClicks++;

    public void OnHandledEventsTooTextBoxClicked() => HandledEventsTooTextBoxClicks++;

    public void OnHandleEventFalseButtonClicked() => HandleEventFalseButtonClicks++;

    public void OnHandleEventFalseTextBoxClicked() => HandleEventFalseTextBoxClicks++;

    public void OnSpaceReleaseClicked() => SpaceReleaseClicks++;

    public void OnSpacePressClicked() => SpacePressClicks++;

    public void OnDefaultClicked() => DefaultClicks++;

    public void OnCancelClicked() => CancelClicks++;

    public void OnFlyoutClicked() => FlyoutClicks++;

    private void OnHandledEventsTooWindowPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (IsHandledEventsTooSource(e.Source))
        {
            e.Handled = true;
        }
    }

    private void OnHandledEventsTooWindowPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (IsHandledEventsTooSource(e.Source))
        {
            e.Handled = true;
        }
    }

    private void OnHandledEventsTooWindowKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Space && IsSourceInsideTarget(e.Source, HandledEventsTooTextBoxSourceTarget))
        {
            e.Handled = true;
        }
    }

    private void OnHandledEventsTooWindowKeyUp(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Space && IsSourceInsideTarget(e.Source, HandledEventsTooTextBoxSourceTarget))
        {
            e.Handled = true;
        }
    }

    private void OnHandleEventFalseWindowKeyUp(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Space && IsSourceInsideTarget(e.Source, HandleEventFalseTextBoxTarget))
        {
            HandleEventFalseTextBoxBubbledKeyUp++;
        }
    }

    private void OnHandleEventFalseButtonNativeClick(object? sender, RoutedEventArgs e)
    {
        HandleEventFalseButtonNativeClicks++;
    }

    private void OnHandleEventFalseButtonClickEvent(object? sender, RoutedEventArgs e)
    {
        if (IsSourceInsideTarget(e.Source, HandleEventFalseButtonTarget))
        {
            HandleEventFalseButtonClickEvents++;
        }
    }

    private void OnHandleEventFalseTextBoxClickEvent(object? sender, RoutedEventArgs e)
    {
        if (IsSourceInsideTarget(e.Source, HandleEventFalseTextBoxTarget))
        {
            HandleEventFalseTextBoxClickEvents++;
        }
    }

    private bool IsHandledEventsTooSource(object? source)
    {
        return IsSourceInsideTarget(source, HandledEventsTooSourceTarget)
            || IsSourceInsideTarget(source, HandledEventsTooTextBoxSourceTarget);
    }

    private static bool IsSourceInsideTarget(object? source, Visual target)
    {
        if (source is not Visual sourceVisual)
        {
            return false;
        }

        return ReferenceEquals(sourceVisual, target) || target.IsVisualAncestorOf(sourceVisual);
    }
}
