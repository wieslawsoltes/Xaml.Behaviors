// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Trigger that emulates <see cref="Button"/> click semantics on any <see cref="Control"/>.
/// </summary>
public class ClickEventTrigger : StyledElementTrigger<Control>
{
    private bool _isPressed;
    private bool _ownsPointerCapture;
    private Control? _resolvedSourceControl;
    private IInputElement? _rootInputElement;

    /// <summary>
    /// Identifies the <see cref="SourceControl"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Control?> SourceControlProperty =
        AvaloniaProperty.Register<ClickEventTrigger, Control?>(nameof(SourceControl));

    /// <summary>
    /// Identifies the <see cref="ClickMode"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<ClickMode> ClickModeProperty =
        AvaloniaProperty.Register<ClickEventTrigger, ClickMode>(nameof(ClickMode), ClickMode.Release);

    /// <summary>
    /// Identifies the <see cref="KeyModifiers"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<KeyModifiers?> KeyModifiersProperty =
        AvaloniaProperty.Register<ClickEventTrigger, KeyModifiers?>(nameof(KeyModifiers));

    /// <summary>
    /// Identifies the <see cref="IsDefault"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<bool> IsDefaultProperty =
        AvaloniaProperty.Register<ClickEventTrigger, bool>(nameof(IsDefault));

    /// <summary>
    /// Identifies the <see cref="IsCancel"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<bool> IsCancelProperty =
        AvaloniaProperty.Register<ClickEventTrigger, bool>(nameof(IsCancel));

    /// <summary>
    /// Identifies the <see cref="Flyout"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<FlyoutBase?> FlyoutProperty =
        AvaloniaProperty.Register<ClickEventTrigger, FlyoutBase?>(nameof(Flyout));

    /// <summary>
    /// Identifies the <see cref="UseAttachedFlyout"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<bool> UseAttachedFlyoutProperty =
        AvaloniaProperty.Register<ClickEventTrigger, bool>(nameof(UseAttachedFlyout), true);

    /// <summary>
    /// Identifies the <see cref="HandleEvent"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<bool> HandleEventProperty =
        AvaloniaProperty.Register<ClickEventTrigger, bool>(nameof(HandleEvent), true);

    /// <summary>
    /// Identifies the <see cref="HandledEventsToo"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<bool> HandledEventsTooProperty =
        AvaloniaProperty.Register<ClickEventTrigger, bool>(nameof(HandledEventsToo), false);

    /// <summary>
    /// Gets or sets the source control from which this trigger listens for click semantics.
    /// If not set, it defaults to the associated object.
    /// </summary>
    [ResolveByName]
    public Control? SourceControl
    {
        get => GetValue(SourceControlProperty);
        set => SetValue(SourceControlProperty, value);
    }

    /// <summary>
    /// Gets or sets how this trigger reacts to pointer and keyboard clicks.
    /// </summary>
    public ClickMode ClickMode
    {
        get => GetValue(ClickModeProperty);
        set => SetValue(ClickModeProperty, value);
    }

    /// <summary>
    /// Gets or sets required key modifiers for click execution.
    /// </summary>
    public KeyModifiers? KeyModifiers
    {
        get => GetValue(KeyModifiersProperty);
        set => SetValue(KeyModifiersProperty, value);
    }

    /// <summary>
    /// Gets or sets whether Enter on the visual root should invoke this trigger.
    /// </summary>
    public bool IsDefault
    {
        get => GetValue(IsDefaultProperty);
        set => SetValue(IsDefaultProperty, value);
    }

    /// <summary>
    /// Gets or sets whether Escape on the visual root should invoke this trigger.
    /// </summary>
    public bool IsCancel
    {
        get => GetValue(IsCancelProperty);
        set => SetValue(IsCancelProperty, value);
    }

    /// <summary>
    /// Gets or sets an explicit flyout instance used for toggle-on-click behavior.
    /// </summary>
    [ResolveByName]
    public FlyoutBase? Flyout
    {
        get => GetValue(FlyoutProperty);
        set => SetValue(FlyoutProperty, value);
    }

    /// <summary>
    /// Gets or sets whether to fallback to <see cref="FlyoutBase.AttachedFlyoutProperty"/> when <see cref="Flyout"/> is not set.
    /// </summary>
    public bool UseAttachedFlyout
    {
        get => GetValue(UseAttachedFlyoutProperty);
        set => SetValue(UseAttachedFlyoutProperty, value);
    }

    /// <summary>
    /// Gets or sets whether handled semantics should be applied to routed events.
    /// </summary>
    public bool HandleEvent
    {
        get => GetValue(HandleEventProperty);
        set => SetValue(HandleEventProperty, value);
    }

    /// <summary>
    /// Gets or sets whether this trigger should receive already handled routed events.
    /// </summary>
    public bool HandledEventsToo
    {
        get => GetValue(HandledEventsTooProperty);
        set => SetValue(HandledEventsTooProperty, value);
    }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        SetResolvedSource(ResolveSourceControl());
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        DetachRootSubscription();
        SetResolvedSource(null);

        _isPressed = false;
        _ownsPointerCapture = false;
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == SourceControlProperty)
        {
            SetResolvedSource(ResolveSourceControl());
        }
        else if (change.Property == HandledEventsTooProperty)
        {
            SetResolvedSource(ResolveSourceControl(), forceReattachHandlers: true);
        }
        else if (change.Property == IsDefaultProperty || change.Property == IsCancelProperty)
        {
            UpdateRootSubscription();
        }
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (_resolvedSourceControl is not { IsEffectivelyEnabled: true } sourceControl
            || !e.GetCurrentPoint(sourceControl).Properties.IsLeftButtonPressed)
        {
            return;
        }

        if (IsFlyoutOpenForResolvedSource())
        {
            SetHandled(e);
            ExecuteClick(e.KeyModifiers);
            return;
        }

        _isPressed = true;

        if (ShouldCapturePointer(sourceControl))
        {
            e.Pointer?.Capture(sourceControl);
            _ownsPointerCapture = true;
        }

        SetHandled(e);

        if (ClickMode == ClickMode.Press)
        {
            ExecuteClick(e.KeyModifiers);
        }
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (_isPressed && e.InitialPressMouseButton == MouseButton.Left)
        {
            _isPressed = false;

            if (_ownsPointerCapture)
            {
                e.Pointer?.Capture(null);
                _ownsPointerCapture = false;
            }

            SetHandled(e);

            if (ClickMode == ClickMode.Release && IsPointerWithinResolvedSource(e))
            {
                ExecuteClick(e.KeyModifiers);
            }
        }
    }

    private void OnPointerCaptureLost(object? sender, PointerCaptureLostEventArgs e)
    {
        _isPressed = false;
        _ownsPointerCapture = false;
    }

    private void OnLostFocus(object? sender, RoutedEventArgs e)
    {
        _isPressed = false;
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (_resolvedSourceControl is not { IsEffectivelyEnabled: true } sourceControl)
        {
            return;
        }

        switch (e.Key)
        {
            case Key.Enter:
                if (sourceControl.IsFocused && ExecuteClick(e.KeyModifiers))
                {
                    SetHandled(e);
                }
                break;
            case Key.Space:
                if (sourceControl.IsFocused)
                {
                    if (ClickMode == ClickMode.Press)
                    {
                        ExecuteClick(e.KeyModifiers);
                    }

                    _isPressed = true;
                    SetHandled(e);
                }
                break;
            case Key.Escape:
                if (TryGetResolvedFlyout() is { } flyout && IsFlyoutOpenForResolvedSource(flyout))
                {
                    flyout.Hide();
                }
                break;
        }
    }

    private void OnKeyUp(object? sender, KeyEventArgs e)
    {
        if (_resolvedSourceControl is not { IsEffectivelyEnabled: true } sourceControl)
        {
            return;
        }

        if (e.Key == Key.Space && sourceControl.IsFocused)
        {
            if (ClickMode == ClickMode.Release)
            {
                ExecuteClick(e.KeyModifiers);
            }

            _isPressed = false;
            SetHandled(e);
        }
    }

    private void OnRootKeyDown(object? sender, KeyEventArgs e)
    {
        if (_resolvedSourceControl is null
            || (!IsDefault && !IsCancel)
            || !_resolvedSourceControl.IsEffectivelyVisible
            || !_resolvedSourceControl.IsEffectivelyEnabled
            || IsSourceInsideResolvedSource(e.Source))
        {
            return;
        }

        if (IsDefault && e.Key == Key.Enter && ExecuteClick(e.KeyModifiers))
        {
            SetHandled(e);
        }
        else if (IsCancel && e.Key == Key.Escape && ExecuteClick(e.KeyModifiers))
        {
            SetHandled(e);
        }
    }

    private bool ExecuteClick(KeyModifiers currentModifiers)
    {
        if (!IsEnabled || _resolvedSourceControl is not { IsEffectivelyEnabled: true } sourceControl || !MatchesModifiers(currentModifiers))
        {
            return false;
        }

        ToggleFlyout(sourceControl);

        var clickArgs = new RoutedEventArgs(Button.ClickEvent, sourceControl);
        RaiseClickEventIfNeeded(sourceControl, clickArgs);
        Interaction.ExecuteActions(sourceControl, Actions, clickArgs);
        return true;
    }

    private static void RaiseClickEventIfNeeded(Control sourceControl, RoutedEventArgs clickArgs)
    {
        // Button-derived controls already raise ClickEvent through native control logic.
        if (sourceControl is Button)
        {
            return;
        }

        sourceControl.RaiseEvent(clickArgs);
    }

    private void ToggleFlyout(Control associatedObject)
    {
        var flyout = TryGetResolvedFlyout();
        if (flyout is null)
        {
            return;
        }

        if (IsFlyoutOpenForResolvedSource(flyout))
        {
            flyout.Hide();
        }
        else
        {
            flyout.ShowAt(associatedObject);
        }
    }

    private FlyoutBase? TryGetResolvedFlyout()
    {
        if (Flyout is not null)
        {
            return Flyout;
        }

        return UseAttachedFlyout && _resolvedSourceControl is not null
            ? FlyoutBase.GetAttachedFlyout(_resolvedSourceControl)
            : null;
    }

    private bool IsFlyoutOpenForResolvedSource()
    {
        return IsFlyoutOpenForResolvedSource(TryGetResolvedFlyout());
    }

    private bool IsFlyoutOpenForResolvedSource(FlyoutBase? flyout)
    {
        return flyout is not null
               && flyout.IsOpen
               && ReferenceEquals(flyout.Target, _resolvedSourceControl);
    }

    private bool IsPointerWithinResolvedSource(PointerReleasedEventArgs e)
    {
        if (_resolvedSourceControl is null)
        {
            return false;
        }

        var position = e.GetPosition(_resolvedSourceControl);
        foreach (var visual in _resolvedSourceControl.GetVisualsAt(position))
        {
            if (ReferenceEquals(visual, _resolvedSourceControl) || _resolvedSourceControl.IsVisualAncestorOf(visual))
            {
                return true;
            }
        }

        return false;
    }

    private bool MatchesModifiers(KeyModifiers currentModifiers)
    {
        var requiredModifiers = KeyModifiers;
        return requiredModifiers is null || requiredModifiers.Value == currentModifiers;
    }

    private bool ShouldCapturePointer(Control sourceControl)
    {
        // In non-invasive mode we should not capture the pointer, otherwise controls such as TextBox can lose native drag-selection behavior.
        if (!HandleEvent)
        {
            return false;
        }

        return sourceControl is not TextBox;
    }

    private bool IsSourceInsideResolvedSource(object? source)
    {
        if (_resolvedSourceControl is null || source is not Visual sourceVisual)
        {
            return false;
        }

        return ReferenceEquals(sourceVisual, _resolvedSourceControl) || _resolvedSourceControl.IsVisualAncestorOf(sourceVisual);
    }

    private void SetHandled(RoutedEventArgs e)
    {
        if (HandleEvent)
        {
            e.Handled = true;
        }
    }

    private void UpdateRootSubscription(bool forceReattach = false)
    {
        if (_resolvedSourceControl is null)
        {
            DetachRootSubscription();
            return;
        }

        if (!IsDefault && !IsCancel)
        {
            DetachRootSubscription();
            return;
        }

        var rootInputElement = _resolvedSourceControl.GetVisualRoot() as IInputElement;
        if (!forceReattach && ReferenceEquals(_rootInputElement, rootInputElement))
        {
            return;
        }

        DetachRootSubscription();

        if (rootInputElement is null)
        {
            return;
        }

        _rootInputElement = rootInputElement;
        _rootInputElement.AddHandler(
            InputElement.KeyDownEvent,
            OnRootKeyDown,
            RoutingStrategies.Direct | RoutingStrategies.Bubble,
            HandledEventsToo);
    }

    private void SetResolvedSource(Control? newSource, bool forceReattachHandlers = false)
    {
        if (ReferenceEquals(_resolvedSourceControl, newSource))
        {
            if (forceReattachHandlers)
            {
                _isPressed = false;
                _ownsPointerCapture = false;

                if (_resolvedSourceControl is not null)
                {
                    UnregisterInputHandlers(_resolvedSourceControl);
                    RegisterInputHandlers(_resolvedSourceControl);
                }

                UpdateRootSubscription(forceReattach: true);
            }

            return;
        }

        _isPressed = false;
        _ownsPointerCapture = false;

        if (_resolvedSourceControl is not null)
        {
            UnregisterInputHandlers(_resolvedSourceControl);
        }

        _resolvedSourceControl = newSource;

        if (_resolvedSourceControl is not null)
        {
            RegisterInputHandlers(_resolvedSourceControl);
        }

        UpdateRootSubscription();
    }

    private Control? ResolveSourceControl()
    {
        return SourceControl ?? AssociatedObject;
    }

    private void RegisterInputHandlers(Control sourceControl)
    {
        sourceControl.AddHandler(InputElement.PointerPressedEvent, OnPointerPressed, RoutingStrategies.Bubble, HandledEventsToo);
        sourceControl.AddHandler(InputElement.PointerReleasedEvent, OnPointerReleased, RoutingStrategies.Bubble, HandledEventsToo);
        sourceControl.AddHandler(InputElement.PointerCaptureLostEvent, OnPointerCaptureLost, RoutingStrategies.Direct, HandledEventsToo);
        sourceControl.AddHandler(InputElement.KeyDownEvent, OnKeyDown, RoutingStrategies.Bubble, HandledEventsToo);
        sourceControl.AddHandler(InputElement.KeyUpEvent, OnKeyUp, RoutingStrategies.Bubble, HandledEventsToo);
        sourceControl.AddHandler(InputElement.LostFocusEvent, OnLostFocus, RoutingStrategies.Bubble, HandledEventsToo);
    }

    private void UnregisterInputHandlers(Control sourceControl)
    {
        sourceControl.RemoveHandler(InputElement.PointerPressedEvent, OnPointerPressed);
        sourceControl.RemoveHandler(InputElement.PointerReleasedEvent, OnPointerReleased);
        sourceControl.RemoveHandler(InputElement.PointerCaptureLostEvent, OnPointerCaptureLost);
        sourceControl.RemoveHandler(InputElement.KeyDownEvent, OnKeyDown);
        sourceControl.RemoveHandler(InputElement.KeyUpEvent, OnKeyUp);
        sourceControl.RemoveHandler(InputElement.LostFocusEvent, OnLostFocus);
    }

    private void DetachRootSubscription()
    {
        if (_rootInputElement is null)
        {
            return;
        }

        _rootInputElement.RemoveHandler(InputElement.KeyDownEvent, OnRootKeyDown);
        _rootInputElement = null;
    }
}
