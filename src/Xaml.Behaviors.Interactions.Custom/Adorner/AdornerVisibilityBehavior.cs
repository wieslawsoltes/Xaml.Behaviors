// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Metadata;
using Avalonia.Styling;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Behavior that manages visibility of an adorner on the associated control.
/// </summary>
public class AdornerVisibilityBehavior : AttachedToVisualTreeBehavior<Control>
{
    /// <summary>
    /// Identifies the <see cref="IsVisible"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<bool> IsVisibleProperty =
        AvaloniaProperty.Register<AdornerVisibilityBehavior, bool>(nameof(IsVisible));

    /// <summary>
    /// Identifies the <see cref="AdornerTemplate"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<ITemplate?> AdornerTemplateProperty =
        AvaloniaProperty.Register<AdornerVisibilityBehavior, ITemplate?>(nameof(AdornerTemplate));

    private Control? _adorner;

    /// <summary>
    /// Gets or sets whether the adorner is visible.
    /// </summary>
    public bool IsVisible
    {
        get => GetValue(IsVisibleProperty);
        set => SetValue(IsVisibleProperty, value);
    }

    /// <summary>
    /// Gets or sets the adorner template.
    /// </summary>
    [Content]
    [TemplateContent(TemplateResultType = typeof(Control))]
    public ITemplate? AdornerTemplate
    {
        get => GetValue(AdornerTemplateProperty);
        set => SetValue(AdornerTemplateProperty, value);
    }

    /// <inheritdoc />
    protected override IDisposable OnAttachedToVisualTreeOverride()
    {
        if (IsVisible)
        {
            ShowAdorner();
        }

        return new DisposableAction(HideAdorner);
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IsVisibleProperty)
        {
            var isVisible = change.GetNewValue<bool>();
            if (isVisible)
            {
                ShowAdorner();
            }
            else
            {
                HideAdorner();
            }
        }
    }

    private void ShowAdorner()
    {
        if (_adorner is not null || AssociatedObject is null)
        {
            return;
        }

        var layer = AdornerLayer.GetAdornerLayer(AssociatedObject);
        if (layer is null)
        {
            return;
        }

        var adorner = CreateAdorner();
        if (adorner is null)
        {
            return;
        }

        _adorner = adorner;
        adorner.SetValue(AdornerLayer.AdornedElementProperty, AssociatedObject);
        ((ISetLogicalParent)adorner).SetParent(AssociatedObject);
        layer.Children.Add(adorner);
    }

    private void HideAdorner()
    {
        if (_adorner is null || AssociatedObject is null)
        {
            return;
        }

        var layer = AdornerLayer.GetAdornerLayer(AssociatedObject);
        if (layer is not null)
        {
            layer.Children.Remove(_adorner);
        }

        ((ISetLogicalParent)_adorner).SetParent(null);
        _adorner = null;
    }

    private Control? CreateAdorner()
    {
        var template = AdornerTemplate;
        if (template is null)
        {
            return null;
        }

        var obj = template.Build();
        return obj as Control;
    }
}
