// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// 
/// </summary>
public class TextBoxSelectAllTextBehavior : AttachedToVisualTreeBehavior<TextBox>
{
    /// <summary>
    /// 
    /// </summary>
    protected override System.IDisposable OnAttachedToVisualTreeOverride()
    {
        AssociatedObject?.SelectAll();

        return DisposableAction.Empty;
    }
}
