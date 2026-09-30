// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
#else
using Avalonia.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// 
/// </summary>
public abstract partial class RoutedEventTriggerBase : AttachedToVisualTreeTriggerBase<Visual>
{

    /// <summary>
    /// 
    /// </summary>
    [StyledProperty(DefaultValue = RoutingStrategies.Direct)]
    public partial RoutingStrategies EventRoutingStrategy { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool MarkAsHandled { get; set; }
}
