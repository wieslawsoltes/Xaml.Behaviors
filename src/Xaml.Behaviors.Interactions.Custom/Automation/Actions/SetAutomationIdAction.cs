// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Sets <see cref="AutomationProperties.AutomationIdProperty"/> on the target control when executed.
/// </summary>
public class SetAutomationIdAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <see cref="TargetControl"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Control?> TargetControlProperty =
        AvaloniaProperty.Register<SetAutomationIdAction, Control?>(nameof(TargetControl));

    /// <summary>
    /// Identifies the <see cref="AutomationId"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> AutomationIdProperty =
        AvaloniaProperty.Register<SetAutomationIdAction, string?>(nameof(AutomationId));

    /// <summary>
    /// Gets or sets the target control. This is an avalonia property.
    /// </summary>
    [ResolveByName]
    public Control? TargetControl
    {
        get => GetValue(TargetControlProperty);
        set => SetValue(TargetControlProperty, value);
    }

    /// <summary>
    /// Gets or sets the automation id value. This is an avalonia property.
    /// </summary>
    public string? AutomationId
    {
        get => GetValue(AutomationIdProperty);
        set => SetValue(AutomationIdProperty, value);
    }

    /// <inheritdoc />
    public override object Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        var control = TargetControl ?? sender as Control;
        if (control is null)
        {
            return false;
        }

        control.SetValue(AutomationProperties.AutomationIdProperty, AutomationId);
        return true;
    }
}
