// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Xaml.Interactivity;
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
/// An action that displays a <see cref="Popup"/> for the associated control when executed.
/// </summary>
/// <remarks>If the associated control is of type <see cref="Control"/> than popup inherits control <see cref="StyledElement.DataContext"/>.</remarks>
public partial class PopupAction : Avalonia.Xaml.Interactivity.StyledElementAction
{
    private Popup? _popup;

    /// <summary>
    /// Gets or sets the popup Child control. This is an avalonia property.
    /// </summary>
    [StyledProperty(Content = true)]
    public partial Control? Child { get; set; }

    /// <summary>
    /// Executes the action.
    /// </summary>
    /// <param name="sender">The <see cref="object"/> that is passed to the action by the behavior. Generally this is <seealso cref="IBehavior.AssociatedObject"/> or a target object.</param>
    /// <param name="parameter">The value of this parameter is determined by the caller.</param>
    /// <returns>Returns null after executed.</returns>
    public override object? Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        if (_popup is null)
        {
            var parent = sender as Control;

            _popup = new Popup()
            {
                Placement = PlacementMode.Pointer, PlacementTarget = parent, IsLightDismissEnabled = true
            };

            if (sender is Control control)
            {
                BindToDataContext(control, _popup);
            }

            ((ISetLogicalParent)_popup).SetParent(parent);
        }

        _popup.Child = Child;
        _popup.Open();
        return null;
    }

    private static void BindToDataContext(Control source, Control target)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (target is null)
        {
            throw new ArgumentNullException(nameof(target));
        }

        var data = source.GetObservable(StyledElement.DataContextProperty);
        if (data is not null)
        {
            target.Bind(StyledElement.DataContextProperty, data);
        }
    }
}
