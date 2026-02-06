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
    private IInputElement? _rootInputElement;

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

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        if (AssociatedObject is not null)
        {
            AssociatedObject.AddHandler(InputElement.PointerPressedEvent, OnPointerPressed, RoutingStrategies.Bubble);
            AssociatedObject.AddHandler(InputElement.PointerReleasedEvent, OnPointerReleased, RoutingStrategies.Bubble);
            AssociatedObject.AddHandler(InputElement.PointerCaptureLostEvent, OnPointerCaptureLost, RoutingStrategies.Direct);
            AssociatedObject.AddHandler(InputElement.KeyDownEvent, OnKeyDown, RoutingStrategies.Bubble);
            AssociatedObject.AddHandler(InputElement.KeyUpEvent, OnKeyUp, RoutingStrategies.Bubble);
            AssociatedObject.AddHandler(InputElement.LostFocusEvent, OnLostFocus, RoutingStrategies.Bubble);

            UpdateRootSubscription();
        }
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        DetachRootSubscription();

        if (AssociatedObject is not null)
        {
            AssociatedObject.RemoveHandler(InputElement.PointerPressedEvent, OnPointerPressed);
            AssociatedObject.RemoveHandler(InputElement.PointerReleasedEvent, OnPointerReleased);
            AssociatedObject.RemoveHandler(InputElement.PointerCaptureLostEvent, OnPointerCaptureLost);
            AssociatedObject.RemoveHandler(InputElement.KeyDownEvent, OnKeyDown);
            AssociatedObject.RemoveHandler(InputElement.KeyUpEvent, OnKeyUp);
            AssociatedObject.RemoveHandler(InputElement.LostFocusEvent, OnLostFocus);
        }

        _isPressed = false;
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IsDefaultProperty || change.Property == IsCancelProperty)
        {
            UpdateRootSubscription();
        }
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (AssociatedObject is not { IsEffectivelyEnabled: true } associatedObject
            || !e.GetCurrentPoint(associatedObject).Properties.IsLeftButtonPressed)
        {
            return;
        }

        if (IsFlyoutOpenForAssociatedObject())
        {
            SetHandled(e);
            ExecuteClick(e.KeyModifiers);
            return;
        }

        _isPressed = true;
        e.Pointer?.Capture(associatedObject);
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
            e.Pointer?.Capture(null);
            SetHandled(e);

            if (ClickMode == ClickMode.Release && IsPointerWithinAssociatedObject(e))
            {
                ExecuteClick(e.KeyModifiers);
            }
        }
    }

    private void OnPointerCaptureLost(object? sender, PointerCaptureLostEventArgs e)
    {
        _isPressed = false;
    }

    private void OnLostFocus(object? sender, RoutedEventArgs e)
    {
        _isPressed = false;
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (AssociatedObject is not { IsEffectivelyEnabled: true } associatedObject)
        {
            return;
        }

        switch (e.Key)
        {
            case Key.Enter:
                if (associatedObject.IsFocused && ExecuteClick(e.KeyModifiers))
                {
                    SetHandled(e);
                }
                break;
            case Key.Space:
                if (associatedObject.IsFocused)
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
                if (TryGetResolvedFlyout() is { } flyout && IsFlyoutOpenForAssociatedObject(flyout))
                {
                    flyout.Hide();
                }
                break;
        }
    }

    private void OnKeyUp(object? sender, KeyEventArgs e)
    {
        if (AssociatedObject is not { IsEffectivelyEnabled: true } associatedObject)
        {
            return;
        }

        if (e.Key == Key.Space && associatedObject.IsFocused)
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
        if (AssociatedObject is null
            || (!IsDefault && !IsCancel)
            || !AssociatedObject.IsEffectivelyVisible
            || !AssociatedObject.IsEffectivelyEnabled
            || IsSourceInsideAssociatedObject(e.Source))
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
        if (!IsEnabled || AssociatedObject is not { IsEffectivelyEnabled: true } associatedObject || !MatchesModifiers(currentModifiers))
        {
            return false;
        }

        ToggleFlyout(associatedObject);

        var clickArgs = new RoutedEventArgs(Button.ClickEvent, associatedObject);
        Interaction.ExecuteActions(associatedObject, Actions, clickArgs);
        return true;
    }

    private void ToggleFlyout(Control associatedObject)
    {
        var flyout = TryGetResolvedFlyout();
        if (flyout is null)
        {
            return;
        }

        if (IsFlyoutOpenForAssociatedObject(flyout))
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

        return UseAttachedFlyout && AssociatedObject is not null
            ? FlyoutBase.GetAttachedFlyout(AssociatedObject)
            : null;
    }

    private bool IsFlyoutOpenForAssociatedObject()
    {
        return IsFlyoutOpenForAssociatedObject(TryGetResolvedFlyout());
    }

    private bool IsFlyoutOpenForAssociatedObject(FlyoutBase? flyout)
    {
        return flyout is not null
               && flyout.IsOpen
               && ReferenceEquals(flyout.Target, AssociatedObject);
    }

    private bool IsPointerWithinAssociatedObject(PointerReleasedEventArgs e)
    {
        if (AssociatedObject is null)
        {
            return false;
        }

        var position = e.GetPosition(AssociatedObject);
        foreach (var visual in AssociatedObject.GetVisualsAt(position))
        {
            if (ReferenceEquals(visual, AssociatedObject) || AssociatedObject.IsVisualAncestorOf(visual))
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

    private bool IsSourceInsideAssociatedObject(object? source)
    {
        if (AssociatedObject is null || source is not Visual sourceVisual)
        {
            return false;
        }

        return ReferenceEquals(sourceVisual, AssociatedObject) || AssociatedObject.IsVisualAncestorOf(sourceVisual);
    }

    private void SetHandled(RoutedEventArgs e)
    {
        if (HandleEvent)
        {
            e.Handled = true;
        }
    }

    private void UpdateRootSubscription()
    {
        if (AssociatedObject is null)
        {
            return;
        }

        if (!IsDefault && !IsCancel)
        {
            DetachRootSubscription();
            return;
        }

        var rootInputElement = AssociatedObject.GetVisualRoot() as IInputElement;
        if (ReferenceEquals(_rootInputElement, rootInputElement))
        {
            return;
        }

        DetachRootSubscription();

        if (rootInputElement is null)
        {
            return;
        }

        _rootInputElement = rootInputElement;
        _rootInputElement.AddHandler(InputElement.KeyDownEvent, OnRootKeyDown);
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
