// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Xaml.Interactions.Custom;

namespace BehaviorsTestApplication.Behaviors;

// WinUI XAML has no x:TypeArguments: the generic validation rules used by the Avalonia sample with x:TypeArguments are
// closed here so the Uno views can use them (see ClosedGenericBehaviors.cs).

/// <summary>
/// <see cref="RangeValidationRule{T}"/> for <see cref="double"/> values (Avalonia: <c>x:TypeArguments="x:Double"</c>).
/// </summary>
public partial class DoubleRangeValidationRule : RangeValidationRule<double>
{
}
