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
/// Centers the attached window on the current screen when it is attached to the visual tree.
/// </summary>
public partial class CenterWindowBehavior : StyledElementBehavior<Control>
{

    /// <summary>
    /// Gets or sets the window to center. If not set, the visual root window is used.
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial Window? Window { get; set; }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        base.OnAttachedToVisualTree();
        Center();
    }

    private void Center()
    {
        var window = Window ?? TopLevel.GetTopLevel(AssociatedObject) as Window;
        if (window is null)
        {
            return;
        }

        var screen = window.Screens.ScreenFromWindow(window);
        if (screen is null)
        {
            return;
        }

        var rect = screen.WorkingArea;
        var x = rect.X + (rect.Width - window.Width) / 2;
        var y = rect.Y + (rect.Height - window.Height) / 2;
        window.Position = new PixelPoint((int)x, (int)y);
    }
}
