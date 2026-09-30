// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
#else
using Avalonia.Input;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// 
/// </summary>
public abstract partial class ExecuteCommandOnKeyBehaviorBase : ExecuteCommandRoutedEventBehaviorBase
{

    /// <summary>
    /// 
    /// </summary>
    [StyledProperty]
    public partial Key? Key { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [StyledProperty]
    public partial KeyGesture? Gesture { get; set; }
}
