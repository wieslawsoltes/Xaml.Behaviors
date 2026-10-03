// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Platform;
using Avalonia.Reactive;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// A behavior that exposes the screen containing the associated <see cref="TopLevel"/>.
/// </summary>
public partial class ActiveScreenBehavior : AttachedToVisualTreeBehavior<TopLevel>
{

    /// <summary>
    /// Gets the active screen of the associated <see cref="TopLevel"/>. This is an avalonia property.
    /// </summary>
    [DirectProperty]
    public partial Screen? ActiveScreen { get; private set; }

    /// <inheritdoc />
    protected override IDisposable OnAttachedToVisualTreeOverride()
    {
        if (AssociatedObject is not { } topLevel)
        {
            return DisposableAction.Empty;
        }

        Update();
        
        if (topLevel is Window window)
        {
            window.PositionChanged += OnChanged; 
        }
        
        topLevel.GetObservable(TopLevel.ClientSizeProperty).Subscribe(new AnonymousObserver<Size>(_ => Update()));
        var screens = topLevel.Screens;
        if (screens is not null)
        {
            screens.Changed += OnScreensChanged;
        }

        return DisposableAction.Create(() =>
        {
            if (topLevel is Window w)
            {
                w.PositionChanged -= OnChanged; 
            }

            if (screens is not null)
            {
                screens.Changed -= OnScreensChanged;
            }
        });
    }

    private void OnChanged(object? sender, EventArgs e) => Update();

    private void OnScreensChanged(object? sender, EventArgs e) => Update();

    private void Update()
    {
        if (AssociatedObject is { Screens: { } screens } top)
        {
            ActiveScreen = screens.ScreenFromTopLevel(top);
        }
        else
        {
            ActiveScreen = null;
        }
    }
}
