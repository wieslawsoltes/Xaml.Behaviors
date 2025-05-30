using System;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Behavior that manages a context dialog implemented using a <see cref="Popup"/>.
/// </summary>
public class ContextDialogBehavior : AttachedToVisualTreeBehavior<Control>
{
    /// <summary>
    /// Identifies the <see cref="DialogContent"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Control?> DialogContentProperty =
        AvaloniaProperty.Register<ContextDialogBehavior, Control?>(nameof(DialogContent));

    /// <summary>
    /// Identifies the <see cref="IsOpen"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<bool> IsOpenProperty =
        AvaloniaProperty.Register<ContextDialogBehavior, bool>(nameof(IsOpen));

    /// <summary>
    /// Identifies the <see cref="Placement"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<PlacementMode> PlacementProperty =
        AvaloniaProperty.Register<ContextDialogBehavior, PlacementMode>(nameof(Placement), PlacementMode.Pointer);

    private Popup? _popup;

    /// <summary>
    /// Occurs when the dialog is opened.
    /// </summary>
    public event EventHandler? Opened;

    /// <summary>
    /// Occurs when the dialog is closed.
    /// </summary>
    public event EventHandler? Closed;

    /// <summary>
    /// Gets or sets the dialog content. This is an avalonia property.
    /// </summary>
    [Content]
    public Control? DialogContent
    {
        get => GetValue(DialogContentProperty);
        set => SetValue(DialogContentProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the dialog is open. This is an avalonia property.
    /// </summary>
    public bool IsOpen
    {
        get => GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    /// <summary>
    /// Gets or sets the popup placement mode. This is an avalonia property.
    /// </summary>
    public PlacementMode Placement
    {
        get => GetValue(PlacementProperty);
        set => SetValue(PlacementProperty, value);
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (_popup is null)
        {
            return;
        }

        if (change.Property == DialogContentProperty)
        {
            _popup.Child = DialogContent;
        }
        else if (change.Property == IsOpenProperty)
        {
            UpdatePopup();
        }
        else if (change.Property == PlacementProperty)
        {
            _popup.PlacementMode = Placement;
        }
    }

    /// <inheritdoc />
    protected override IDisposable OnAttachedToVisualTreeOverride()
    {
        _popup = new Popup
        {
            PlacementMode = Placement,
            PlacementTarget = AssociatedObject,
            StaysOpen = true,
            IsLightDismissEnabled = true,
            Child = DialogContent
        };

        UpdatePopup();

        return DisposableAction.Create(() =>
        {
            if (_popup is not null)
            {
                _popup.Close();
                Closed?.Invoke(this, EventArgs.Empty);
                _popup = null;
            }
        });
    }

    private void UpdatePopup()
    {
        if (_popup is null)
        {
            return;
        }

        if (IsOpen)
        {
            _popup.Open(AssociatedObject);
            Opened?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            _popup.Close();
            Closed?.Invoke(this, EventArgs.Empty);
        }
    }
}
