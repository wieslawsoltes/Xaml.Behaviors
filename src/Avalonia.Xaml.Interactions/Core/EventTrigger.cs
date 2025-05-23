using System.Diagnostics.CodeAnalysis;

namespace Avalonia.Xaml.Interactions.Core;

/// <summary>
/// A behavior that listens for a specified event on its source and executes its actions when that event is fired.
/// </summary>
[RequiresUnreferencedCode("This functionality is not compatible with trimming.")]
public class EventTrigger : EventTriggerBehavior;
