using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Behavior that enables translate, zoom and rotate interactions on a control using multi-touch gestures.
/// </summary>
public sealed class TranslateZoomRotateBehavior : StyledElementBehavior<Control>
{
    /// <summary>
    /// Identifies the <seealso cref="TargetControl"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Control?> TargetControlProperty =
        AvaloniaProperty.Register<TranslateZoomRotateBehavior, Control?>(nameof(TargetControl));

    private Control? _parent;
    private Point _previous;
    private bool _pressed;

    private ScaleTransform? _scale;
    private RotateTransform? _rotate;
    private TranslateTransform? _translate;

    /// <summary>
    /// Gets or sets the control that will receive the transforms.
    /// </summary>
    [ResolveByName]
    public Control? TargetControl
    {
        get => GetValue(TargetControlProperty);
        set => SetValue(TargetControlProperty, value);
    }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        var source = AssociatedObject;
        if (source is not null)
        {
            source.PointerPressed += OnPointerPressed;
            source.PointerReleased += OnPointerReleased;
            source.PointerMoved += OnPointerMoved;
            source.AddHandler(Gestures.PinchEvent, OnPinch, RoutingStrategies.Bubble);
        }
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        var source = AssociatedObject;
        if (source is not null)
        {
            source.PointerPressed -= OnPointerPressed;
            source.PointerReleased -= OnPointerReleased;
            source.PointerMoved -= OnPointerMoved;
            source.RemoveHandler(Gestures.PinchEvent, OnPinch);
        }
    }

    private void EnsureTransforms(Control target)
    {
        if (target.RenderTransform is TransformGroup group &&
            group.Children.Count == 3 &&
            group.Children[0] is ScaleTransform &&
            group.Children[1] is RotateTransform &&
            group.Children[2] is TranslateTransform)
        {
            _scale = (ScaleTransform)group.Children[0];
            _rotate = (RotateTransform)group.Children[1];
            _translate = (TranslateTransform)group.Children[2];
        }
        else
        {
            _scale = new ScaleTransform();
            _rotate = new RotateTransform();
            _translate = new TranslateTransform();

            var tg = new TransformGroup
            {
                Children = { _scale, _rotate, _translate }
            };

            target.RenderTransform = tg;
        }
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var target = TargetControl ?? AssociatedObject;
        if (target is null)
        {
            return;
        }

        EnsureTransforms(target);
        _parent = target.Parent as Control;
        _previous = e.GetPosition(_parent);
        _pressed = true;
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _pressed = false;
        _parent = null;
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (!_pressed)
        {
            return;
        }

        var target = TargetControl ?? AssociatedObject;
        if (target is null || _translate is null)
        {
            return;
        }

        var pos = e.GetPosition(_parent);
        _translate.X += pos.X - _previous.X;
        _translate.Y += pos.Y - _previous.Y;
        _previous = pos;
    }

    private void OnPinch(object? sender, RoutedEventArgs e)
    {
        var target = TargetControl ?? AssociatedObject;
        if (target is null)
        {
            return;
        }

        EnsureTransforms(target);

        if (_scale is null || _rotate is null || _translate is null)
        {
            return;
        }

        if (e is PinchEventArgs pinch)
        {
            var origin = pinch.ScaleOrigin;
            // TODO:
            // _scale.CenterX = origin.X;
            // _scale.CenterY = origin.Y;
            _scale.ScaleX *= pinch.Scale;
            _scale.ScaleY *= pinch.Scale;

            _rotate.CenterX = origin.X;
            _rotate.CenterY = origin.Y;
            _rotate.Angle += pinch.Angle;

            // TODO:
            // var delta = pinch.Translation;
            // _translate.X += delta.X;
            // _translate.Y += delta.Y;
        }
    }
}
