// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Xaml.Interactivity;
using PlacementMode = Microsoft.UI.Xaml.Controls.Primitives.PopupPlacementMode;
#else
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Metadata;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Behavior that manages a context dialog implemented using a <see cref="Popup"/>.
/// </summary>
/// <remarks>
/// On Uno Platform the placement is a WinUI <c>PopupPlacementMode</c> (<c>Popup.DesiredPlacement</c>).
/// </remarks>
public partial class ContextDialogBehavior : AttachedToVisualTreeBehavior<Control>
{

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
    [StyledProperty(Content = true)]
    public partial Control? DialogContent { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the dialog is open. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial bool IsOpen { get; set; }

    /// <summary>
    /// Gets or sets the popup placement mode. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial PlacementMode Placement { get; set; }

    /// <summary>
    /// Gets or sets a value that determines how the dialog can be dismissed. This is an avalonia property.
    /// </summary>
    [StyledProperty(DefaultValue = true)]
    public partial bool IsLightDismissEnabled { get; set; }

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
#if UNO
            _popup.DesiredPlacement = Placement;
#else
            _popup.Placement = Placement;
#endif
        }
    }

    /// <inheritdoc />
    protected override IDisposable OnAttachedToVisualTreeOverride()
    {
#if UNO
        _popup = new Popup
        {
            DesiredPlacement = Placement,
            PlacementTarget = AssociatedObject,
            IsLightDismissEnabled = IsLightDismissEnabled,
            Child = DialogContent,
            XamlRoot = AssociatedObject?.XamlRoot,
        };
#else
        _popup = new Popup
        {
            Placement = Placement,
            PlacementTarget = AssociatedObject,
            IsLightDismissEnabled = IsLightDismissEnabled,
            Child = DialogContent
        };
#endif

        UpdatePopup();
        
        _popup.Closed += PopupOnClosed;

        return DisposableAction.Create(() =>
        {
            if (_popup is not null)
            {
                _popup.Closed -= PopupOnClosed;
#if UNO
                _popup.IsOpen = false;
#else
                _popup.Close();
#endif
                Closed?.Invoke(this, EventArgs.Empty);
                _popup = null;
            }
        });
    }

#if UNO
    private void PopupOnClosed(object? sender, object e)
#else
    private void PopupOnClosed(object? sender, EventArgs e)
#endif
    {
        if (_popup is not null)
        {
            SetCurrentValue(IsOpenProperty, false);
            Closed?.Invoke(this, EventArgs.Empty);
        }
    }

    private void UpdatePopup()
    {
        if (_popup is null)
        {
            return;
        }

        if (IsOpen)
        {
#if UNO
            _popup.IsOpen = true;
#else
            _popup.Open();
#endif
            Opened?.Invoke(this, EventArgs.Empty);
        }
        else
        {
#if UNO
            _popup.IsOpen = false;
#else
            _popup.Close();
#endif
            Closed?.Invoke(this, EventArgs.Empty);
        }
    }
}
