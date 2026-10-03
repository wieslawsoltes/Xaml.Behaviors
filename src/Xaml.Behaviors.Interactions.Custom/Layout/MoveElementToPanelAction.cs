// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
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
/// Moves the associated or target element to a specified <see cref="Panel"/>.
/// </summary>
public sealed partial class MoveElementToPanelAction : StyledElementAction
{

    /// <summary>
    /// Gets or sets the panel to move the element into. If not set, the sender is used as target.
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial Panel? TargetPanel { get; set; }

    /// <inheritdoc />
    public override object Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        if (sender is not Control element)
        {
            return false;
        }

        var target = TargetPanel ?? sender as Panel;
        if (target is null)
        {
            return false;
        }

        if (element.Parent is Panel source)
        {
            source.Children.Remove(element);
        }

        target.Children.Add(element);
        return true;
    }
}
