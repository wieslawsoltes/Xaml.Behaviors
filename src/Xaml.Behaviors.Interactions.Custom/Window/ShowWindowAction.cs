// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Metadata;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Shows the specified window when executed.
/// </summary>
public partial class ShowWindowAction : StyledElementAction
{

    /// <summary>
    /// Gets or sets the window instance to show. This is an avalonia property.
    /// </summary>
    [StyledProperty(Content = true)]
    public partial Window? Window { get; set; }

    /// <inheritdoc />
    public override object Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        var window = Window;
        if (window is null)
        {
            return false;
        }

        if (sender is Control control)
        {
            window.DataContext = control.DataContext;
        }

        window.Show();
        return true;
    }
}
