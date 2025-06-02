// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Sets <see cref="AutomationProperties.NameProperty"/> on the associated control when attached.
/// </summary>
public class AutomationNameBehavior : StyledElementBehavior<Control>
{
    /// <summary>
    /// Identifies the <see cref="AutomationName"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> AutomationNameProperty =
        AvaloniaProperty.Register<AutomationNameBehavior, string?>(nameof(AutomationName));

    /// <summary>
    /// Gets or sets the automation name. This is an avalonia property.
    /// </summary>
    public string? AutomationName
    {
        get => GetValue(NameProperty);
        set => SetValue(NameProperty, value);
    }

    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();
        UpdateName();
    }

    /// <inheritdoc />
    protected override void OnDetaching()
    {
        SetAutomationPropertiesName(null);
        base.OnDetaching();
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == AutomationNameProperty)
        {
            UpdateName();
        }
    }

    private void UpdateName()
    {
        SetAutomationPropertiesName(AutomationName);
    }

    private void SetAutomationPropertiesName(string? value)
    {
        if (AssociatedObject is null)
        {
            return;
        }

        AssociatedObject.SetValue(AutomationProperties.NameProperty, value);
    }
}
