using System.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Removes the associated or target element from its parent when executed.
/// </summary>
public class RemoveElementAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <see cref="TargetObject"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Control?> TargetObjectProperty =
        AvaloniaProperty.Register<RemoveElementAction, Control?>(nameof(TargetObject));

    /// <summary>
    /// Gets or sets the element to remove. This is an avalonia property.
    /// If not set, the sender will be used.
    /// </summary>
    [ResolveByName]
    public Control? TargetObject
    {
        get => GetValue(TargetObjectProperty);
        set => SetValue(TargetObjectProperty, value);
    }

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

        if (parent is ItemsControl itemsControl && itemsControl.Items is IList list && list.Contains(element))
        {
            list.Remove(element);
            return true;
        }

        return false;
    }
}

