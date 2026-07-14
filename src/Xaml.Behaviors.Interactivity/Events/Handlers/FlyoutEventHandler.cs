// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Avalonia.Controls.Primitives;

namespace Avalonia.Xaml.Interactivity;

internal sealed class FlyoutEventHandler : IAddEventHandler
{
    public bool Matches(object source, string eventName)
    {
        return source is FlyoutBase &&
               eventName is nameof(FlyoutBase.Opened) or nameof(FlyoutBase.Closed);
    }

    public IDisposable? AddHandler(
        object source,
        string eventName,
        Action<object?, object> handler)
    {
        if (source is not FlyoutBase target)
        {
            return null;
        }

        EventHandler eventHandler = (sender, eventArgs) => handler(sender, eventArgs);

        switch (eventName)
        {
            case nameof(FlyoutBase.Opened):
                target.Opened += eventHandler;
                return DisposableAction.Create(() => target.Opened -= eventHandler);

            case nameof(FlyoutBase.Closed):
                target.Closed += eventHandler;
                return DisposableAction.Create(() => target.Closed -= eventHandler);

            default:
                return null;
        }
    }
}
