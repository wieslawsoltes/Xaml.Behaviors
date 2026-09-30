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
/// <remarks>
/// On Uno Platform the collection is a <c>DependencyObjectCollection&lt;Condition&gt;</c> so the conditions inherit the
/// data context of the owning behavior.
/// </remarks>
#if UNO
public class ConditionCollection : Microsoft.UI.Xaml.DependencyObjectCollection<Condition>
#else
public class ConditionCollection : AvaloniaList<Condition>
#endif
{
}
