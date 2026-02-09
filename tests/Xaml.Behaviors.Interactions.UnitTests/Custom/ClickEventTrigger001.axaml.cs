using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace Avalonia.Xaml.Interactions.UnitTests.Custom;

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
        HandleEventFalseButtonTarget.AddHandler(Button.ClickEvent, OnHandleEventFalseButtonClickEvent, RoutingStrategies.Bubble);
        HandleEventFalseTextBoxTarget.AddHandler(Button.ClickEvent, OnHandleEventFalseTextBoxClickEvent, RoutingStrategies.Bubble);

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
