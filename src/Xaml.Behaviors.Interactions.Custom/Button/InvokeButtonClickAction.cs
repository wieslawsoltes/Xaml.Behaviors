// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Automation.Peers;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Invokes the target button when executed.
/// </summary>
public class InvokeButtonClickAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <see cref="TargetButton"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Button?> TargetButtonProperty =
        AvaloniaProperty.Register<InvokeButtonClickAction, Button?>(nameof(TargetButton));

    /// <summary>
    /// Gets or sets the target button. This is an avalonia property.
    /// </summary>
    [ResolveByName]
    public Button? TargetButton
    {
        get => GetValue(TargetButtonProperty);
        set => SetValue(TargetButtonProperty, value);
    }

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
