// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace Xaml.Interactivity;

/// <summary>
/// Uno Platform counterpart of the Avalonia property registration API used by the shared sources.
/// </summary>
/// <remarks>
/// The shared (Avalonia-first) sources register their properties with
/// <c>AvaloniaProperty.Register&lt;TOwner, TValue&gt;(...)</c>. On Uno the same expression resolves to this
/// type and produces a WinUI <see cref="DependencyProperty"/>. Every property registered here routes its
/// change notifications to <see cref="IDependencyPropertyChangedHandler"/> so the shared
/// <c>OnPropertyChanged</c> overrides keep working.
/// </remarks>
internal static class AvaloniaProperty
{
    private static readonly PropertyChangedCallback s_changedCallback = OnChanged;

    /// <summary>
    /// Registers a dependency property (Avalonia <c>StyledProperty</c> counterpart).
    /// </summary>
    /// <remarks>
    /// WinUI has no per-property default binding mode or value inheritance, so
    /// <paramref name="inherits"/> and <paramref name="defaultBindingMode"/> are accepted for source
    /// compatibility and ignored.
    /// </remarks>
    public static DependencyProperty Register<TOwner, TValue>(
        string name,
        TValue defaultValue = default!,
        bool inherits = false,
        BindingMode defaultBindingMode = BindingMode.OneWay)
    {
        _ = inherits;
        _ = defaultBindingMode;
        return DependencyProperty.Register(
            name,
            typeof(TValue),
            typeof(TOwner),
            new PropertyMetadata(defaultValue, s_changedCallback));
    }

    /// <summary>
    /// Registers an attached dependency property (Avalonia <c>AttachedProperty</c> counterpart).
    /// </summary>
    public static DependencyProperty RegisterAttached<TOwner, THost, TValue>(
        string name,
        TValue defaultValue = default!,
        bool inherits = false,
        BindingMode defaultBindingMode = BindingMode.OneWay)
    {
        _ = inherits;
        _ = defaultBindingMode;
        return DependencyProperty.RegisterAttached(
            name,
            typeof(TValue),
            typeof(TOwner),
            new PropertyMetadata(defaultValue, s_changedCallback));
    }

    /// <summary>
    /// Registers a dependency property backed by a field (Avalonia <c>DirectProperty</c> counterpart).
    /// </summary>
    /// <remarks>
    /// Values assigned through XAML or bindings are pushed to the field through <paramref name="setter"/>;
    /// values assigned through the CLR property are pushed to the dependency property with
    /// <see cref="AvaloniaObjectCompatExtensions.SetAndRaise{T}(DependencyObject, DependencyProperty, ref T, T)"/>.
    /// </remarks>
    public static DependencyProperty RegisterDirect<TOwner, TValue>(
        string name,
        Func<TOwner, TValue> getter,
        Action<TOwner, TValue>? setter = null,
        TValue unsetValue = default!,
        BindingMode defaultBindingMode = BindingMode.OneWay,
        bool enableDataValidation = false)
        where TOwner : class
    {
        _ = getter;
        _ = defaultBindingMode;
        _ = enableDataValidation;

        PropertyChangedCallback callback = setter is null
            ? s_changedCallback
            : (d, e) =>
            {
                if (d is TOwner owner)
                {
                    setter(owner, e.NewValue is TValue value ? value : unsetValue);
                }

                OnChanged(d, e);
            };

        return DependencyProperty.Register(
            name,
            typeof(TValue),
            typeof(TOwner),
            new PropertyMetadata(unsetValue, callback));
    }

    private static void OnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is IDependencyPropertyChangedHandler handler)
        {
            handler.OnDependencyPropertyChanged(e);
        }
    }
}
