// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Threading.Tasks;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Core;
#else
namespace Avalonia.Xaml.Interactions.Core;
#endif

/// <summary>
/// An action that waits for a specified duration.
/// </summary>
public partial class DelayAction : StyledElementAction
{

    /// <summary>
    /// Gets or sets the duration to wait. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial TimeSpan Duration { get; set; }

    /// <inheritdoc />
    public override object? Execute(object? sender, object? parameter)
    {
        return Task.Delay(Duration);
    }
}
