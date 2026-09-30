// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if !UNO
using Avalonia.Collections;
#endif

#if UNO
namespace Xaml.Interactivity;
#else
namespace Avalonia.Xaml.Interactivity;
#endif

/// <summary>
/// Represents a collection of <see cref="Condition"/> objects.
/// </summary>
public class ConditionCollection : AvaloniaList<Condition>
{
}
