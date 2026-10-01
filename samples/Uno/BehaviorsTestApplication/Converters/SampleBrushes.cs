// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;

namespace BehaviorsTestApplication.Converters;

/// <summary>
/// Brush functions for compiled bindings (<c>{x:Bind converters:SampleBrushes.FromName(Color)}</c>).
/// </summary>
/// <remarks>
/// Avalonia converts a color name to a brush when an action assigns it to a brush property; on WinUI only the XAML
/// parser does (the value type of a brush dependency property is not known at run time).
/// </remarks>
public static class SampleBrushes
{
    /// <summary>
    /// Creates a brush from a color name or code (for example <c>Red</c> or <c>#FF0000</c>).
    /// </summary>
    /// <param name="name">The color name or code.</param>
    /// <returns>The brush, or <c>null</c> when <paramref name="name"/> is empty.</returns>
    public static Brush? FromName(string? name)
        => string.IsNullOrEmpty(name) ? null : XamlBindingHelper.ConvertValue(typeof(Brush), name) as Brush;
}

/// <summary>
/// Converts a color name to a brush (<see cref="SampleBrushes.FromName"/>) for bindings in templates, where compiled
/// bindings are not evaluated (behavior templates).
/// </summary>
public sealed partial class BrushFromNameConverter : IValueConverter
{
    /// <inheritdoc />
    public object? Convert(object? value, Type targetType, object? parameter, string language)
        => SampleBrushes.FromName(value as string);

    /// <inheritdoc />
    public object? ConvertBack(object? value, Type targetType, object? parameter, string language)
        => throw new NotSupportedException();
}

/// <summary>
/// Dependency properties for bindings in templates (Avalonia: <c>{x:Static TemplatedControl.BackgroundProperty}</c>);
/// WinUI XAML has no <c>x:Static</c>, and compiled bindings are not evaluated in behavior templates.
/// </summary>
public sealed partial class SampleProperties
{
    /// <summary>
    /// Gets the <see cref="Control.BackgroundProperty"/>.
    /// </summary>
    public DependencyProperty ControlBackground => Control.BackgroundProperty;
}
