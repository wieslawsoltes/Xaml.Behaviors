// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;

namespace Xaml.Interactivity;

/// <summary>
/// Source compatibility marker for the Avalonia <c>[ResolveByName]</c> attribute.
/// </summary>
/// <remarks>
/// WinUI XAML does not resolve element names assigned to <see cref="object"/> properties.
/// On Uno Platform use <c>{Binding ElementName=...}</c> or <c>{x:Bind}</c> instead.
/// </remarks>
[AttributeUsage(AttributeTargets.Property)]
internal sealed class ResolveByNameAttribute : Attribute
{
}
