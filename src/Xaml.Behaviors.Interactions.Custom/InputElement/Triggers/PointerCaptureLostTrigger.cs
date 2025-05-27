// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// 
/// </summary>
public class PointerCaptureLostTrigger : RoutedEventTriggerBase<PointerCaptureLostEventArgs>
{
    /// <inheritdoc />
    protected override RoutedEvent<PointerCaptureLostEventArgs> RoutedEvent 
        => InputElement.PointerCaptureLostEvent;
}
