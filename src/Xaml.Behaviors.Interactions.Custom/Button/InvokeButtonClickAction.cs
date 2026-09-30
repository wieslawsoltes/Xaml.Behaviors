// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactivity;
#else
using Avalonia.Automation.Peers;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Invokes the target button when executed.
/// </summary>
public partial class InvokeButtonClickAction : StyledElementAction
{

    /// <summary>
    /// Gets or sets the target button. This is an avalonia property.
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial Button? TargetButton { get; set; }

    /// <inheritdoc />
    public override object Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        var button = TargetButton ?? sender as Button;
        if (button is null || !button.IsEffectivelyEnabled)
        {
            return false;
        }

        var automationPeer = new ButtonAutomationPeer(button);
        automationPeer.Invoke();
        return true;
    }
}
