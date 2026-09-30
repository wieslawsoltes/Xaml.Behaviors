// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Collections;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactivity;
// WinUI's single child decorator is Border.
using Decorator = Microsoft.UI.Xaml.Controls.Border;
#else
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Removes the associated or target element from its parent when executed.
/// </summary>
public partial class RemoveElementAction : StyledElementAction
{

    /// <summary>
    /// Gets or sets the element to remove. This is an avalonia property.
    /// If not set, the sender will be used.
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial Control? TargetObject { get; set; }

    /// <inheritdoc />
    public override object Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        var element = GetValue(TargetObjectProperty) is not null ? TargetObject : sender as Control;
        if (element is null)
        {
            return false;
        }

        var parent = element.Parent;
        if (parent is null)
        {
            return false;
        }

        if (parent is Panel panel)
        {
            panel.Children.Remove(element);
            return true;
        }

        if (parent is Decorator decorator)
        {
            if (decorator.Child == element)
            {
                decorator.Child = null;
                return true;
            }
        }

        if (parent is ContentControl contentControl)
        {
            if (contentControl.Content == element)
            {
                contentControl.Content = null;
                return true;
            }
        }

        if (parent is ContentPresenter presenter)
        {
            if (presenter.Content == element)
            {
                presenter.Content = null;
                return true;
            }
        }

#if UNO
        if (parent is ItemsControl itemsControl && itemsControl.Items.Contains(element))
        {
            itemsControl.Items.Remove(element);
            return true;
        }
#else
        if (parent is ItemsControl itemsControl && itemsControl.Items is IList list && list.Contains(element))
        {
            list.Remove(element);
            return true;
        }
#endif

        return false;
    }
}

