// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Diagnostics.CodeAnalysis;
#if UNO
using Xaml.Interactivity;
#else
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Core;
#else
namespace Avalonia.Xaml.Interactions.Core;
#endif

/// <summary>
/// A behavior that listens for a specified event on its source and executes its actions when that event is fired.
/// </summary>
[RequiresUnreferencedCode("This functionality is not compatible with trimming.")]
public class EventTriggerBehavior : EventTriggerBase;
