// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// This behavior automatically sets the focus on the associated <see cref="Control"/> when it is loaded.
/// </summary>
public class AutoFocusBehavior : StyledElementBehavior<Control>
{
    /// <inheritdoc/>
    protected override void OnLoaded()
    {
        base.OnLoaded();

        Dispatcher.UIThread.Post(() => AssociatedObject?.Focus());
    }
}
