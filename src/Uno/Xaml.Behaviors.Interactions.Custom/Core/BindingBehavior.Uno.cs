// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace Xaml.Interactions.Custom;

/// <content>
/// Uno Platform value semantics of <see cref="Binding"/>.
/// </content>
/// <remarks>
/// Avalonia assigns the binding written in XAML to the behavior, which binds the target property to it. WinUI applies
/// the binding to <see cref="Binding"/> instead, so the behavior pushes the bound value to the target property while it
/// is attached to the visual tree, and again whenever the value, the target object or the target property changes
/// (x:Bind values are also applied after the behavior was attached).
/// </remarks>
public partial class BindingBehavior
{
    private bool _isInVisualTree;
    private IDisposable? _application;
    private DependencyObject? _valueTarget;
    private DependencyProperty? _valueProperty;

    /// <inheritdoc />
    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (!_isInVisualTree)
        {
            return;
        }

        if (e.Property == BindingProperty
            && _valueTarget is { } valueTarget
            && _valueProperty is { } valueProperty
            && e.NewValue is not null and not BindingBase)
        {
            // The bound value changed: update the target without clearing it first.
            valueTarget.SetValue(valueProperty, e.NewValue);
            return;
        }

        if (e.Property == BindingProperty || e.Property == TargetObjectProperty || e.Property == TargetPropertyProperty)
        {
            Apply();
        }
    }

    private IDisposable ApplyWhileAttached()
    {
        _isInVisualTree = true;
        Apply();

        return DisposableAction.Create(() =>
        {
            _isInVisualTree = false;
            Release();
        });
    }

    private void Apply()
    {
        Release();

        if (TargetObject is not { } target || TargetProperty is not { } property || Binding is not { } binding)
        {
            return;
        }

        if (binding is BindingBase bindingBase)
        {
            _application = target.Bind(property, bindingBase);
            return;
        }

        target.SetValue(property, binding);
        _valueTarget = target;
        _valueProperty = property;
        _application = DisposableAction.Create(() => target.ClearValue(property));
    }

    private void Release()
    {
        var application = _application;
        _application = null;
        _valueTarget = null;
        _valueProperty = null;
        application?.Dispose();
    }
}
