// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Microsoft.UI.Xaml;

namespace Xaml.Interactions.Custom;

/// <summary>
/// Base class of the generic validation rules on Uno Platform.
/// </summary>
/// <remarks>
/// Uno's dependency object source generator does not support generic classes deriving directly from
/// <see cref="DependencyObject"/>; the generic rules (Avalonia: <c>AvaloniaObject</c>) derive from this class.
/// </remarks>
public abstract partial class ValidationRuleBase : DependencyObject
{
}
