// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Closes the associated or target window when executed.
/// </summary>
public partial class CloseWindowAction : StyledElementAction
{

    /// <summary>
    /// Gets or sets the target window. This is an avalonia property.
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial Window? TargetWindow { get; set; }

    /// <inheritdoc />
    public override object Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        var window = TargetWindow
                     ?? sender as Window
                     ?? TopLevel.GetTopLevel(sender as Visual) as Window;
        if (window is null)
        {
            return false;
        }

        window.Close();
        return true;
    }
}
