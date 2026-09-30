// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Reactive;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Observes the bounds of an associated <see cref="Control"/> and updates its Width and Height properties.
/// </summary>
public partial class BoundsObserverBehavior : DisposingBehavior<Control>
{

    /// <summary>
    /// Gets or sets the bounds of the associated control. This is a styled Avalonia property.
    /// </summary>
    [StyledProperty(DefaultBindingMode = PropertyBindingMode.OneWay)]
    public partial Rect Bounds { get; set; }

    /// <summary>
    /// Gets or sets the width of the associated control. This is a two-way bound Avalonia property.
    /// </summary>
    [StyledProperty(DefaultBindingMode = PropertyBindingMode.TwoWay)]
    public partial double Width { get; set; }

    /// <summary>
    /// Gets or sets the height of the associated control. This is a two-way bound Avalonia property.
    /// </summary>
    [StyledProperty(DefaultBindingMode = PropertyBindingMode.TwoWay)]
    public partial double Height { get; set; }

    /// <summary>
    /// Attaches the behavior to the associated control and starts observing its bounds to update the Width and Height properties accordingly.
    /// </summary>
    /// <returns>A disposable resource to be disposed when the behavior is detached.</returns>
    protected override IDisposable OnAttachedOverride()
    {
        if (AssociatedObject is not null)
        {
            return this.GetObservable<Rect>(BoundsProperty)
                .Subscribe(new AnonymousObserver<Rect>(bounds =>
                {
                    Width = bounds.Width;
                    Height = bounds.Height;
                }));
        }
        
        return DisposableAction.Empty;
    }
}
