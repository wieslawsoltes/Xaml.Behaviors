// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Microsoft.UI.Xaml;

namespace Xaml.Interactivity;

/// <content>
/// Uno Platform counterpart of the Avalonia <c>AvaloniaObject.PropertyChanged</c> event for conditions.
/// </content>
public partial class Condition
{
    /// <summary>
    /// Occurs when the value of a dependency property of the condition changes.
    /// </summary>
    public event EventHandler<DependencyPropertyChangedEventArgs>? PropertyChanged;

    private void RaisePropertyChanged(DependencyPropertyChangedEventArgs change) => PropertyChanged?.Invoke(this, change);
}
