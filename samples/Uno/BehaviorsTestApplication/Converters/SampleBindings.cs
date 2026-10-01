// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace BehaviorsTestApplication.Converters;

/// <summary>
/// Binding objects for compiled bindings (<c>{x:Bind converters:SampleBindings.Create(Slider, 'Value')}</c>).
/// </summary>
/// <remarks>
/// Avalonia assigns a binding written in XAML to a <c>BindingBase</c> property marked with <c>[AssignBinding]</c>
/// (<c>BindingBehavior.Binding</c>, <c>BindingTriggerBehavior.Binding</c>). WinUI XAML always applies a binding to
/// the dependency property instead (also in property element syntax), so the Uno Platform views create the binding
/// object with a compiled binding, which assigns the value.
/// </remarks>
public static class SampleBindings
{
    /// <summary>
    /// Creates a one-way binding to a property of an object (Avalonia: <c>{Binding #Source.Path}</c>).
    /// </summary>
    /// <param name="source">The source object, for example a named element.</param>
    /// <param name="path">The property path.</param>
    /// <returns>The binding.</returns>
    public static BindingBase Create(object source, string path)
        => new Binding { Source = source, Path = new PropertyPath(path), Mode = BindingMode.OneWay };
}
